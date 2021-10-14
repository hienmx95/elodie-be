using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomerSalesOrder
{
    public interface ICustomerSalesOrderValidator : IServiceScoped
    {
        Task Get(CustomerSalesOrder CustomerSalesOrder);
        Task<bool> Create(CustomerSalesOrder CustomerSalesOrder);
        Task<bool> Update(CustomerSalesOrder CustomerSalesOrder);
        Task<bool> Delete(CustomerSalesOrder CustomerSalesOrder);
        Task<bool> BulkDelete(List<CustomerSalesOrder> CustomerSalesOrders);
        Task<bool> Import(List<CustomerSalesOrder> CustomerSalesOrders);
    }

    public class CustomerSalesOrderValidator : ICustomerSalesOrderValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerSalesOrderMessage CustomerSalesOrderMessage;

        public CustomerSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderMessage = new CustomerSalesOrderMessage();
        }

        public async Task Get(CustomerSalesOrder CustomerSalesOrder)
        {
        }

        public async Task<bool> Create(CustomerSalesOrder CustomerSalesOrder)
        {
            return CustomerSalesOrder.IsValidated;
        }

        public async Task<bool> Update(CustomerSalesOrder CustomerSalesOrder)
        {
            if (await ValidateId(CustomerSalesOrder))
            {
            }
            return CustomerSalesOrder.IsValidated;
        }

        public async Task<bool> Delete(CustomerSalesOrder CustomerSalesOrder)
        {
            if (await ValidateId(CustomerSalesOrder))
            {
            }
            return CustomerSalesOrder.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CustomerSalesOrder> CustomerSalesOrders)
        {
            foreach (CustomerSalesOrder CustomerSalesOrder in CustomerSalesOrders)
            {
                await Delete(CustomerSalesOrder);
            }
            return CustomerSalesOrders.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CustomerSalesOrder> CustomerSalesOrders)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(CustomerSalesOrder CustomerSalesOrder)
        {
            CustomerSalesOrderFilter CustomerSalesOrderFilter = new CustomerSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CustomerSalesOrder.Id },
                Selects = CustomerSalesOrderSelect.Id
            };

            int count = await UOW.CustomerSalesOrderRepository.CountAll(CustomerSalesOrderFilter);
            if (count == 0)
                CustomerSalesOrder.AddError(nameof(CustomerSalesOrderValidator), nameof(CustomerSalesOrder.Id), CustomerSalesOrderMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
