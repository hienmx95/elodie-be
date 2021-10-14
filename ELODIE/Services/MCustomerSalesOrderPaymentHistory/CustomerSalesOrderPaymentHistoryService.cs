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

namespace ELODIE.Services.MCustomerSalesOrderPaymentHistory
{
    public interface ICustomerSalesOrderPaymentHistoryService :  IServiceScoped
    {
        Task<int> Count(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
        Task<List<CustomerSalesOrderPaymentHistory>> List(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
        Task<CustomerSalesOrderPaymentHistory> Get(long Id);
        Task<CustomerSalesOrderPaymentHistory> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<CustomerSalesOrderPaymentHistory> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<CustomerSalesOrderPaymentHistory> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<List<CustomerSalesOrderPaymentHistory>> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
        Task<List<CustomerSalesOrderPaymentHistory>> Import(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
        Task<CustomerSalesOrderPaymentHistoryFilter> ToFilter(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
    }

    public class CustomerSalesOrderPaymentHistoryService : BaseService, ICustomerSalesOrderPaymentHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerSalesOrderPaymentHistoryValidator CustomerSalesOrderPaymentHistoryValidator;

        public CustomerSalesOrderPaymentHistoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerSalesOrderPaymentHistoryValidator CustomerSalesOrderPaymentHistoryValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderPaymentHistoryValidator = CustomerSalesOrderPaymentHistoryValidator;
        }
        public async Task<int> Count(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter)
        {
            try
            {
                int result = await UOW.CustomerSalesOrderPaymentHistoryRepository.Count(CustomerSalesOrderPaymentHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return 0;
        }

        public async Task<List<CustomerSalesOrderPaymentHistory>> List(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter)
        {
            try
            {
                List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await UOW.CustomerSalesOrderPaymentHistoryRepository.List(CustomerSalesOrderPaymentHistoryFilter);
                return CustomerSalesOrderPaymentHistories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderPaymentHistory> Get(long Id)
        {
            CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory = await UOW.CustomerSalesOrderPaymentHistoryRepository.Get(Id);
            await CustomerSalesOrderPaymentHistoryValidator.Get(CustomerSalesOrderPaymentHistory);
            if (CustomerSalesOrderPaymentHistory == null)
                return null;
            return CustomerSalesOrderPaymentHistory;
        }
        
        public async Task<CustomerSalesOrderPaymentHistory> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            if (!await CustomerSalesOrderPaymentHistoryValidator.Create(CustomerSalesOrderPaymentHistory))
                return CustomerSalesOrderPaymentHistory;

            try
            {
                await UOW.CustomerSalesOrderPaymentHistoryRepository.Create(CustomerSalesOrderPaymentHistory);
                CustomerSalesOrderPaymentHistory = await UOW.CustomerSalesOrderPaymentHistoryRepository.Get(CustomerSalesOrderPaymentHistory.Id);
                await Logging.CreateAuditLog(CustomerSalesOrderPaymentHistory, new { }, nameof(CustomerSalesOrderPaymentHistoryService));
                return CustomerSalesOrderPaymentHistory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderPaymentHistory> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            if (!await CustomerSalesOrderPaymentHistoryValidator.Update(CustomerSalesOrderPaymentHistory))
                return CustomerSalesOrderPaymentHistory;
            try
            {
                var oldData = await UOW.CustomerSalesOrderPaymentHistoryRepository.Get(CustomerSalesOrderPaymentHistory.Id);

                await UOW.CustomerSalesOrderPaymentHistoryRepository.Update(CustomerSalesOrderPaymentHistory);

                CustomerSalesOrderPaymentHistory = await UOW.CustomerSalesOrderPaymentHistoryRepository.Get(CustomerSalesOrderPaymentHistory.Id);
                await Logging.CreateAuditLog(CustomerSalesOrderPaymentHistory, oldData, nameof(CustomerSalesOrderPaymentHistoryService));
                return CustomerSalesOrderPaymentHistory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderPaymentHistory> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            if (!await CustomerSalesOrderPaymentHistoryValidator.Delete(CustomerSalesOrderPaymentHistory))
                return CustomerSalesOrderPaymentHistory;

            try
            {
                await UOW.CustomerSalesOrderPaymentHistoryRepository.Delete(CustomerSalesOrderPaymentHistory);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrderPaymentHistory, nameof(CustomerSalesOrderPaymentHistoryService));
                return CustomerSalesOrderPaymentHistory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;
        }

        public async Task<List<CustomerSalesOrderPaymentHistory>> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            if (!await CustomerSalesOrderPaymentHistoryValidator.BulkDelete(CustomerSalesOrderPaymentHistories))
                return CustomerSalesOrderPaymentHistories;

            try
            {
                await UOW.CustomerSalesOrderPaymentHistoryRepository.BulkDelete(CustomerSalesOrderPaymentHistories);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrderPaymentHistories, nameof(CustomerSalesOrderPaymentHistoryService));
                return CustomerSalesOrderPaymentHistories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;

        }
        
        public async Task<List<CustomerSalesOrderPaymentHistory>> Import(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            if (!await CustomerSalesOrderPaymentHistoryValidator.Import(CustomerSalesOrderPaymentHistories))
                return CustomerSalesOrderPaymentHistories;
            try
            {
                await UOW.CustomerSalesOrderPaymentHistoryRepository.BulkMerge(CustomerSalesOrderPaymentHistories);

                await Logging.CreateAuditLog(CustomerSalesOrderPaymentHistories, new { }, nameof(CustomerSalesOrderPaymentHistoryService));
                return CustomerSalesOrderPaymentHistories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderPaymentHistoryService));
            }
            return null;
        }     
        
        public async Task<CustomerSalesOrderPaymentHistoryFilter> ToFilter(CustomerSalesOrderPaymentHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerSalesOrderPaymentHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerSalesOrderPaymentHistoryFilter subFilter = new CustomerSalesOrderPaymentHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CustomerSalesOrderId))
                        subFilter.CustomerSalesOrderId = FilterBuilder.Merge(subFilter.CustomerSalesOrderId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PaymentMilestone))
                        subFilter.PaymentMilestone = FilterBuilder.Merge(subFilter.PaymentMilestone, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PaymentPercentage))
                        subFilter.PaymentPercentage = FilterBuilder.Merge(subFilter.PaymentPercentage, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PaymentAmount))
                        subFilter.PaymentAmount = FilterBuilder.Merge(subFilter.PaymentAmount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PaymentTypeId))
                        subFilter.PaymentTypeId = FilterBuilder.Merge(subFilter.PaymentTypeId, FilterPermissionDefinition.IdFilter);
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
