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

namespace ELODIE.Services.MCustomerGrouping
{
    public interface ICustomerGroupingService :  IServiceScoped
    {
        Task<int> Count(CustomerGroupingFilter CustomerGroupingFilter);
        Task<List<CustomerGrouping>> List(CustomerGroupingFilter CustomerGroupingFilter);
        Task<CustomerGrouping> Get(long Id);
        Task<CustomerGrouping> Create(CustomerGrouping CustomerGrouping);
        Task<CustomerGrouping> Update(CustomerGrouping CustomerGrouping);
        Task<CustomerGrouping> Delete(CustomerGrouping CustomerGrouping);
        Task<List<CustomerGrouping>> BulkDelete(List<CustomerGrouping> CustomerGroupings);
        Task<List<CustomerGrouping>> Import(List<CustomerGrouping> CustomerGroupings);
        Task<CustomerGroupingFilter> ToFilter(CustomerGroupingFilter CustomerGroupingFilter);
    }

    public class CustomerGroupingService : BaseService, ICustomerGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerGroupingValidator CustomerGroupingValidator;

        public CustomerGroupingService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerGroupingValidator CustomerGroupingValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerGroupingValidator = CustomerGroupingValidator;
        }
        public async Task<int> Count(CustomerGroupingFilter CustomerGroupingFilter)
        {
            try
            {
                int result = await UOW.CustomerGroupingRepository.Count(CustomerGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return 0;
        }

        public async Task<List<CustomerGrouping>> List(CustomerGroupingFilter CustomerGroupingFilter)
        {
            try
            {
                List<CustomerGrouping> CustomerGroupings = await UOW.CustomerGroupingRepository.List(CustomerGroupingFilter);
                return CustomerGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;
        }

        public async Task<CustomerGrouping> Get(long Id)
        {
            CustomerGrouping CustomerGrouping = await UOW.CustomerGroupingRepository.Get(Id);
            await CustomerGroupingValidator.Get(CustomerGrouping);
            if (CustomerGrouping == null)
                return null;
            return CustomerGrouping;
        }
        
        public async Task<CustomerGrouping> Create(CustomerGrouping CustomerGrouping)
        {
            if (!await CustomerGroupingValidator.Create(CustomerGrouping))
                return CustomerGrouping;

            try
            {
                await UOW.CustomerGroupingRepository.Create(CustomerGrouping);
                CustomerGrouping = await UOW.CustomerGroupingRepository.Get(CustomerGrouping.Id);
                await Logging.CreateAuditLog(CustomerGrouping, new { }, nameof(CustomerGroupingService));
                return CustomerGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;
        }

        public async Task<CustomerGrouping> Update(CustomerGrouping CustomerGrouping)
        {
            if (!await CustomerGroupingValidator.Update(CustomerGrouping))
                return CustomerGrouping;
            try
            {
                var oldData = await UOW.CustomerGroupingRepository.Get(CustomerGrouping.Id);

                await UOW.CustomerGroupingRepository.Update(CustomerGrouping);

                CustomerGrouping = await UOW.CustomerGroupingRepository.Get(CustomerGrouping.Id);
                await Logging.CreateAuditLog(CustomerGrouping, oldData, nameof(CustomerGroupingService));
                return CustomerGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;
        }

        public async Task<CustomerGrouping> Delete(CustomerGrouping CustomerGrouping)
        {
            if (!await CustomerGroupingValidator.Delete(CustomerGrouping))
                return CustomerGrouping;

            try
            {
                await UOW.CustomerGroupingRepository.Delete(CustomerGrouping);
                await Logging.CreateAuditLog(new { }, CustomerGrouping, nameof(CustomerGroupingService));
                return CustomerGrouping;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;
        }

        public async Task<List<CustomerGrouping>> BulkDelete(List<CustomerGrouping> CustomerGroupings)
        {
            if (!await CustomerGroupingValidator.BulkDelete(CustomerGroupings))
                return CustomerGroupings;

            try
            {
                await UOW.CustomerGroupingRepository.BulkDelete(CustomerGroupings);
                await Logging.CreateAuditLog(new { }, CustomerGroupings, nameof(CustomerGroupingService));
                return CustomerGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;

        }
        
        public async Task<List<CustomerGrouping>> Import(List<CustomerGrouping> CustomerGroupings)
        {
            if (!await CustomerGroupingValidator.Import(CustomerGroupings))
                return CustomerGroupings;
            try
            {
                await UOW.CustomerGroupingRepository.BulkMerge(CustomerGroupings);

                await Logging.CreateAuditLog(CustomerGroupings, new { }, nameof(CustomerGroupingService));
                return CustomerGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerGroupingService));
            }
            return null;
        }     
        
        public async Task<CustomerGroupingFilter> ToFilter(CustomerGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerGroupingFilter subFilter = new CustomerGroupingFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = FilterBuilder.Merge(subFilter.ParentId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Path))
                        subFilter.Path = FilterBuilder.Merge(subFilter.Path, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Level))
                        subFilter.Level = FilterBuilder.Merge(subFilter.Level, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
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
