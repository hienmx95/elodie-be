using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomerSalesOrderPaymentHistory
{
    public interface ICustomerSalesOrderPaymentHistoryValidator : IServiceScoped
    {
        Task Get(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
        Task<bool> Import(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
    }

    public class CustomerSalesOrderPaymentHistoryValidator : ICustomerSalesOrderPaymentHistoryValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerSalesOrderPaymentHistoryMessage CustomerSalesOrderPaymentHistoryMessage;

        public CustomerSalesOrderPaymentHistoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderPaymentHistoryMessage = new CustomerSalesOrderPaymentHistoryMessage();
        }

        public async Task Get(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
        }

        public async Task<bool> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            return CustomerSalesOrderPaymentHistory.IsValidated;
        }

        public async Task<bool> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            if (await ValidateId(CustomerSalesOrderPaymentHistory))
            {
            }
            return CustomerSalesOrderPaymentHistory.IsValidated;
        }

        public async Task<bool> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            if (await ValidateId(CustomerSalesOrderPaymentHistory))
            {
            }
            return CustomerSalesOrderPaymentHistory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            foreach (CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory in CustomerSalesOrderPaymentHistories)
            {
                await Delete(CustomerSalesOrderPaymentHistory);
            }
            return CustomerSalesOrderPaymentHistories.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter = new CustomerSalesOrderPaymentHistoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CustomerSalesOrderPaymentHistory.Id },
                Selects = CustomerSalesOrderPaymentHistorySelect.Id
            };

            int count = await UOW.CustomerSalesOrderPaymentHistoryRepository.CountAll(CustomerSalesOrderPaymentHistoryFilter);
            if (count == 0)
                CustomerSalesOrderPaymentHistory.AddError(nameof(CustomerSalesOrderPaymentHistoryValidator), nameof(CustomerSalesOrderPaymentHistory.Id), CustomerSalesOrderPaymentHistoryMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
