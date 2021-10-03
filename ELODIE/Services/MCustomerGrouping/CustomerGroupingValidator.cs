using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomerGrouping
{
    public interface ICustomerGroupingValidator : IServiceScoped
    {
        Task Get(CustomerGrouping CustomerGrouping);
        Task<bool> Create(CustomerGrouping CustomerGrouping);
        Task<bool> Update(CustomerGrouping CustomerGrouping);
        Task<bool> Delete(CustomerGrouping CustomerGrouping);
        Task<bool> BulkDelete(List<CustomerGrouping> CustomerGroupings);
        Task<bool> Import(List<CustomerGrouping> CustomerGroupings);
    }

    public class CustomerGroupingValidator : ICustomerGroupingValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerGroupingMessage CustomerGroupingMessage;

        public CustomerGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerGroupingMessage = new CustomerGroupingMessage();
        }

        public async Task Get(CustomerGrouping CustomerGrouping)
        {
        }

        public async Task<bool> Create(CustomerGrouping CustomerGrouping)
        {
            return CustomerGrouping.IsValidated;
        }

        public async Task<bool> Update(CustomerGrouping CustomerGrouping)
        {
            if (await ValidateId(CustomerGrouping))
            {
            }
            return CustomerGrouping.IsValidated;
        }

        public async Task<bool> Delete(CustomerGrouping CustomerGrouping)
        {
            if (await ValidateId(CustomerGrouping))
            {
            }
            return CustomerGrouping.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CustomerGrouping> CustomerGroupings)
        {
            foreach (CustomerGrouping CustomerGrouping in CustomerGroupings)
            {
                await Delete(CustomerGrouping);
            }
            return CustomerGroupings.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CustomerGrouping> CustomerGroupings)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(CustomerGrouping CustomerGrouping)
        {
            CustomerGroupingFilter CustomerGroupingFilter = new CustomerGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CustomerGrouping.Id },
                Selects = CustomerGroupingSelect.Id
            };

            int count = await UOW.CustomerGroupingRepository.CountAll(CustomerGroupingFilter);
            if (count == 0)
                CustomerGrouping.AddError(nameof(CustomerGroupingValidator), nameof(CustomerGrouping.Id), CustomerGroupingMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
