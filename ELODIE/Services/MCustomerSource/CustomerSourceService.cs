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

namespace ELODIE.Services.MCustomerSource
{
    public interface ICustomerSourceService :  IServiceScoped
    {
        Task<int> Count(CustomerSourceFilter CustomerSourceFilter);
        Task<List<CustomerSource>> List(CustomerSourceFilter CustomerSourceFilter);
        Task<CustomerSource> Get(long Id);
        Task<CustomerSource> Create(CustomerSource CustomerSource);
        Task<CustomerSource> Update(CustomerSource CustomerSource);
        Task<CustomerSource> Delete(CustomerSource CustomerSource);
        Task<List<CustomerSource>> BulkDelete(List<CustomerSource> CustomerSources);
        Task<List<CustomerSource>> Import(List<CustomerSource> CustomerSources);
        Task<CustomerSourceFilter> ToFilter(CustomerSourceFilter CustomerSourceFilter);
    }

    public class CustomerSourceService : BaseService, ICustomerSourceService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerSourceValidator CustomerSourceValidator;

        public CustomerSourceService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerSourceValidator CustomerSourceValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerSourceValidator = CustomerSourceValidator;
        }
        public async Task<int> Count(CustomerSourceFilter CustomerSourceFilter)
        {
            try
            {
                int result = await UOW.CustomerSourceRepository.Count(CustomerSourceFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return 0;
        }

        public async Task<List<CustomerSource>> List(CustomerSourceFilter CustomerSourceFilter)
        {
            try
            {
                List<CustomerSource> CustomerSources = await UOW.CustomerSourceRepository.List(CustomerSourceFilter);
                return CustomerSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;
        }

        public async Task<CustomerSource> Get(long Id)
        {
            CustomerSource CustomerSource = await UOW.CustomerSourceRepository.Get(Id);
            await CustomerSourceValidator.Get(CustomerSource);
            if (CustomerSource == null)
                return null;
            return CustomerSource;
        }
        
        public async Task<CustomerSource> Create(CustomerSource CustomerSource)
        {
            if (!await CustomerSourceValidator.Create(CustomerSource))
                return CustomerSource;

            try
            {
                await UOW.CustomerSourceRepository.Create(CustomerSource);
                CustomerSource = await UOW.CustomerSourceRepository.Get(CustomerSource.Id);
                await Logging.CreateAuditLog(CustomerSource, new { }, nameof(CustomerSourceService));
                return CustomerSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;
        }

        public async Task<CustomerSource> Update(CustomerSource CustomerSource)
        {
            if (!await CustomerSourceValidator.Update(CustomerSource))
                return CustomerSource;
            try
            {
                var oldData = await UOW.CustomerSourceRepository.Get(CustomerSource.Id);

                await UOW.CustomerSourceRepository.Update(CustomerSource);

                CustomerSource = await UOW.CustomerSourceRepository.Get(CustomerSource.Id);
                await Logging.CreateAuditLog(CustomerSource, oldData, nameof(CustomerSourceService));
                return CustomerSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;
        }

        public async Task<CustomerSource> Delete(CustomerSource CustomerSource)
        {
            if (!await CustomerSourceValidator.Delete(CustomerSource))
                return CustomerSource;

            try
            {
                await UOW.CustomerSourceRepository.Delete(CustomerSource);
                await Logging.CreateAuditLog(new { }, CustomerSource, nameof(CustomerSourceService));
                return CustomerSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;
        }

        public async Task<List<CustomerSource>> BulkDelete(List<CustomerSource> CustomerSources)
        {
            if (!await CustomerSourceValidator.BulkDelete(CustomerSources))
                return CustomerSources;

            try
            {
                await UOW.CustomerSourceRepository.BulkDelete(CustomerSources);
                await Logging.CreateAuditLog(new { }, CustomerSources, nameof(CustomerSourceService));
                return CustomerSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;

        }
        
        public async Task<List<CustomerSource>> Import(List<CustomerSource> CustomerSources)
        {
            if (!await CustomerSourceValidator.Import(CustomerSources))
                return CustomerSources;
            try
            {
                await UOW.CustomerSourceRepository.BulkMerge(CustomerSources);

                await Logging.CreateAuditLog(CustomerSources, new { }, nameof(CustomerSourceService));
                return CustomerSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSourceService));
            }
            return null;
        }     
        
        public async Task<CustomerSourceFilter> ToFilter(CustomerSourceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerSourceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerSourceFilter subFilter = new CustomerSourceFilter();
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
