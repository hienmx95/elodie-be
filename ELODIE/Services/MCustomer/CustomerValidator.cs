using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomer
{
    public interface ICustomerValidator : IServiceScoped
    {
        Task Get(Customer Customer);
        Task<bool> Create(Customer Customer);
        Task<bool> Update(Customer Customer);
        Task<bool> Delete(Customer Customer);
        Task<bool> BulkDelete(List<Customer> Customers);
        Task<bool> Import(List<Customer> Customers);
    }

    public class CustomerValidator : ICustomerValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerMessage CustomerMessage;

        public CustomerValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerMessage = new CustomerMessage();
        }

        public async Task Get(Customer Customer)
        {
        }

        public async Task<bool> Create(Customer Customer)
        {
            return Customer.IsValidated;
        }

        public async Task<bool> Update(Customer Customer)
        {
            if (await ValidateId(Customer))
            {
            }
            return Customer.IsValidated;
        }

        public async Task<bool> Delete(Customer Customer)
        {
            if (await ValidateId(Customer))
            {
            }
            return Customer.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Customer> Customers)
        {
            foreach (Customer Customer in Customers)
            {
                await Delete(Customer);
            }
            return Customers.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Customer> Customers)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(Customer Customer)
        {
            CustomerFilter CustomerFilter = new CustomerFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Customer.Id },
                Selects = CustomerSelect.Id
            };

            int count = await UOW.CustomerRepository.CountAll(CustomerFilter);
            if (count == 0)
                Customer.AddError(nameof(CustomerValidator), nameof(Customer.Id), CustomerMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
