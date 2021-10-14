using ELODIE.Common;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ELODIE.Repositories;
using ELODIE.Entities;
using ELODIE.Enums;

namespace ELODIE.Services.MWarehouse
{
    public interface IWarehouseService :  IServiceScoped
    {
        Task<int> Count(WarehouseFilter WarehouseFilter);
        Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter);
        Task<Warehouse> Get(long Id);
        Task<Warehouse> Create(Warehouse Warehouse);
        Task<Warehouse> Update(Warehouse Warehouse);
        Task<Warehouse> Delete(Warehouse Warehouse);
        Task<List<Warehouse>> BulkDelete(List<Warehouse> Warehouses);
        Task<List<Warehouse>> Import(List<Warehouse> Warehouses);
        Task<WarehouseFilter> ToFilter(WarehouseFilter WarehouseFilter);
    }

    public class WarehouseService : BaseService, IWarehouseService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWarehouseValidator WarehouseValidator;

        public WarehouseService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IWarehouseValidator WarehouseValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WarehouseValidator = WarehouseValidator;
        }
        public async Task<int> Count(WarehouseFilter WarehouseFilter)
        {
            try
            {
                int result = await UOW.WarehouseRepository.Count(WarehouseFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return 0;
        }

        public async Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter)
        {
            try
            {
                List<Warehouse> Warehouses = await UOW.WarehouseRepository.List(WarehouseFilter);
                return Warehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;
        }

        public async Task<Warehouse> Get(long Id)
        {
            Warehouse Warehouse = await UOW.WarehouseRepository.Get(Id);
            await WarehouseValidator.Get(Warehouse);
            if (Warehouse == null)
                return null;
            return Warehouse;
        }
        
        public async Task<Warehouse> Create(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Create(Warehouse))
                return Warehouse;

            try
            {
                Warehouse.Code = "";
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                Warehouse.OrganizationId = CurrentUser.OrganizationId;
                await UOW.WarehouseRepository.Create(Warehouse);
                Warehouse = await UOW.WarehouseRepository.Get(Warehouse.Id);
                Warehouse.Code = $"WH{Warehouse.Id}";
                await UOW.WarehouseRepository.Update(Warehouse);
                await Logging.CreateAuditLog(Warehouse, new { }, nameof(WarehouseService));
                return Warehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;
        }

        public async Task<Warehouse> Update(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Update(Warehouse))
                return Warehouse;
            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                Warehouse.OrganizationId = CurrentUser.OrganizationId;
                var oldData = await UOW.WarehouseRepository.Get(Warehouse.Id);

                await UOW.WarehouseRepository.Update(Warehouse);

                Warehouse = await UOW.WarehouseRepository.Get(Warehouse.Id);
                await Logging.CreateAuditLog(Warehouse, oldData, nameof(WarehouseService));
                return Warehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;
        }

        public async Task<Warehouse> Delete(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Delete(Warehouse))
                return Warehouse;

            try
            {
                await UOW.WarehouseRepository.Delete(Warehouse);
                await Logging.CreateAuditLog(new { }, Warehouse, nameof(WarehouseService));
                return Warehouse;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;
        }

        public async Task<List<Warehouse>> BulkDelete(List<Warehouse> Warehouses)
        {
            if (!await WarehouseValidator.BulkDelete(Warehouses))
                return Warehouses;

            try
            {
                await UOW.WarehouseRepository.BulkDelete(Warehouses);
                await Logging.CreateAuditLog(new { }, Warehouses, nameof(WarehouseService));
                return Warehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;

        }
        
        public async Task<List<Warehouse>> Import(List<Warehouse> Warehouses)
        {
            if (!await WarehouseValidator.Import(Warehouses))
                return Warehouses;
            try
            {
                await UOW.WarehouseRepository.BulkMerge(Warehouses);

                await Logging.CreateAuditLog(Warehouses, new { }, nameof(WarehouseService));
                return Warehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(WarehouseService));
            }
            return null;
        }     
        
        public async Task<WarehouseFilter> ToFilter(WarehouseFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<WarehouseFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                WarehouseFilter subFilter = new WarehouseFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = FilterBuilder.Merge(subFilter.Address, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProvinceId))
                        subFilter.ProvinceId = FilterBuilder.Merge(subFilter.ProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DistrictId))
                        subFilter.DistrictId = FilterBuilder.Merge(subFilter.DistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WardId))
                        subFilter.WardId = FilterBuilder.Merge(subFilter.WardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter); 
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
