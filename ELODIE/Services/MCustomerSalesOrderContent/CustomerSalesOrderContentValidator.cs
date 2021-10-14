using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MCustomerSalesOrderContent
{
    public interface ICustomerSalesOrderContentValidator : IServiceScoped
    {
        Task Get(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> Create(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> Update(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> Delete(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
        Task<bool> Import(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
    }

    public class CustomerSalesOrderContentValidator : ICustomerSalesOrderContentValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private CustomerSalesOrderContentMessage CustomerSalesOrderContentMessage;

        public CustomerSalesOrderContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderContentMessage = new CustomerSalesOrderContentMessage();
        }

        public async Task Get(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
        }

        public async Task<bool> Create(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            return CustomerSalesOrderContent.IsValidated;
        }

        public async Task<bool> Update(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            if (await ValidateId(CustomerSalesOrderContent))
            {
            }
            return CustomerSalesOrderContent.IsValidated;
        }

        public async Task<bool> Delete(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            if (await ValidateId(CustomerSalesOrderContent))
            {
            }
            return CustomerSalesOrderContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            foreach (CustomerSalesOrderContent CustomerSalesOrderContent in CustomerSalesOrderContents)
            {
                await Delete(CustomerSalesOrderContent);
            }
            return CustomerSalesOrderContents.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter = new CustomerSalesOrderContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CustomerSalesOrderContent.Id },
                Selects = CustomerSalesOrderContentSelect.Id
            };

            int count = await UOW.CustomerSalesOrderContentRepository.CountAll(CustomerSalesOrderContentFilter);
            if (count == 0)
                CustomerSalesOrderContent.AddError(nameof(CustomerSalesOrderContentValidator), nameof(CustomerSalesOrderContent.Id), CustomerSalesOrderContentMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
