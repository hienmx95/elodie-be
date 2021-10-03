using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomerSource
{
    public interface ICustomerSourceValidator : IServiceScoped
    {
        Task Get(CustomerSource CustomerSource);
        Task<bool> Create(CustomerSource CustomerSource);
        Task<bool> Update(CustomerSource CustomerSource);
        Task<bool> Delete(CustomerSource CustomerSource);
        Task<bool> BulkDelete(List<CustomerSource> CustomerSources);
        Task<bool> Import(List<CustomerSource> CustomerSources);
    }

    public class CustomerSourceValidator : ICustomerSourceValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerSourceMessage CustomerSourceMessage;

        public CustomerSourceValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerSourceMessage = new CustomerSourceMessage();
        }

        public async Task Get(CustomerSource CustomerSource)
        {
        }

        public async Task<bool> Create(CustomerSource CustomerSource)
        {
            return CustomerSource.IsValidated;
        }

        public async Task<bool> Update(CustomerSource CustomerSource)
        {
            if (await ValidateId(CustomerSource))
            {
            }
            return CustomerSource.IsValidated;
        }

        public async Task<bool> Delete(CustomerSource CustomerSource)
        {
            if (await ValidateId(CustomerSource))
            {
            }
            return CustomerSource.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CustomerSource> CustomerSources)
        {
            foreach (CustomerSource CustomerSource in CustomerSources)
            {
                await Delete(CustomerSource);
            }
            return CustomerSources.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CustomerSource> CustomerSources)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(CustomerSource CustomerSource)
        {
            CustomerSourceFilter CustomerSourceFilter = new CustomerSourceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CustomerSource.Id },
                Selects = CustomerSourceSelect.Id
            };

            int count = await UOW.CustomerSourceRepository.CountAll(CustomerSourceFilter);
            if (count == 0)
                CustomerSource.AddError(nameof(CustomerSourceValidator), nameof(CustomerSource.Id), CustomerSourceMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
