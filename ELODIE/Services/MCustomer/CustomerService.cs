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

namespace ELODIE.Services.MCustomer
{
    public interface ICustomerService :  IServiceScoped
    {
        Task<int> Count(CustomerFilter CustomerFilter);
        Task<List<Customer>> List(CustomerFilter CustomerFilter);
        Task<Customer> Get(long Id);
        Task<Customer> Create(Customer Customer);
        Task<Customer> Update(Customer Customer);
        Task<Customer> Delete(Customer Customer);
        Task<List<Customer>> BulkDelete(List<Customer> Customers);
        Task<List<Customer>> Import(List<Customer> Customers);
        Task<CustomerFilter> ToFilter(CustomerFilter CustomerFilter);
    }

    public class CustomerService : BaseService, ICustomerService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerValidator CustomerValidator;

        public CustomerService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerValidator CustomerValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerValidator = CustomerValidator;
        }
        public async Task<int> Count(CustomerFilter CustomerFilter)
        {
            try
            {
                int result = await UOW.CustomerRepository.Count(CustomerFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return 0;
        }

        public async Task<List<Customer>> List(CustomerFilter CustomerFilter)
        {
            try
            {
                List<Customer> Customers = await UOW.CustomerRepository.List(CustomerFilter);
                return Customers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;
        }

        public async Task<Customer> Get(long Id)
        {
            Customer Customer = await UOW.CustomerRepository.Get(Id);
            await CustomerValidator.Get(Customer);
            if (Customer == null)
                return null;
            return Customer;
        }
        
        public async Task<Customer> Create(Customer Customer)
        {
            if (!await CustomerValidator.Create(Customer))
                return Customer;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var AppUser = await UOW.AppUserRepository.Get(Customer.AppUserId);
                Customer.CreatorId = CurrentContext.UserId;
                Customer.OrganizationId = AppUser.OrganizationId;
                Customer.Phone = Customer.Phone.Replace("+84", "0");
                await UOW.CustomerRepository.Create(Customer);
                Customer = await UOW.CustomerRepository.Get(Customer.Id);
                CodeGeneratorRule CodeGeneratorRule = await GetCodeGeneratorRule();
                if (CodeGeneratorRule == null)
                {
                    Customer.Code = $"KH{Customer.Id}";
                }
                else
                {
                    await CodeGenerator(new List<Customer> { Customer }, CodeGeneratorRule);
                }
                await UOW.CustomerRepository.Update(Customer);
                await Logging.CreateAuditLog(Customer, new { }, nameof(CustomerService));
                return Customer;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;
        }

        public async Task<Customer> Update(Customer Customer)
        {
            if (!await CustomerValidator.Update(Customer))
                return Customer;
            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var oldData = await UOW.CustomerRepository.Get(Customer.Id);
                var AppUser = await UOW.AppUserRepository.Get(Customer.AppUserId);
                Customer.OrganizationId = AppUser.OrganizationId;
                Customer.Phone = Customer.Phone.Replace("+84", "0");
                if (oldData.CodeGeneratorRuleId.HasValue)
                {
                    CodeGeneratorRule CodeGeneratorRule = await UOW.CodeGeneratorRuleRepository.Get(oldData.CodeGeneratorRuleId.Value);
                    Customer.CreatedAt = oldData.CreatedAt;
                    Customer.Organization = oldData.Organization;
                    if (CodeGeneratorRule != null)
                    {
                        await CodeGenerator(new List<Customer> { Customer }, CodeGeneratorRule);
                    }
                }
                else
                {
                    Customer.Code = oldData.Code;
                }
                await UOW.CustomerRepository.Update(Customer);
                Customer = await UOW.CustomerRepository.Get(Customer.Id);
                await Logging.CreateAuditLog(Customer, oldData, nameof(CustomerService));
                return Customer;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;
        }

        public async Task<Customer> Delete(Customer Customer)
        {
            if (!await CustomerValidator.Delete(Customer))
                return Customer;

            try
            {
                await UOW.CustomerRepository.Delete(Customer);
                await Logging.CreateAuditLog(new { }, Customer, nameof(CustomerService));
                return Customer;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;
        }

        public async Task<List<Customer>> BulkDelete(List<Customer> Customers)
        {
            if (!await CustomerValidator.BulkDelete(Customers))
                return Customers;

            try
            {
                await UOW.CustomerRepository.BulkDelete(Customers);
                await Logging.CreateAuditLog(new { }, Customers, nameof(CustomerService));
                return Customers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;

        }
        
        public async Task<List<Customer>> Import(List<Customer> Customers)
        {
            if (!await CustomerValidator.Import(Customers))
                return Customers;
            try
            {
                await UOW.CustomerRepository.BulkMerge(Customers);

                await Logging.CreateAuditLog(Customers, new { }, nameof(CustomerService));
                return Customers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerService));
            }
            return null;
        }

        private async Task<CodeGeneratorRule> GetCodeGeneratorRule()
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                Skip = 0,
                Take = 1,
                Selects = CodeGeneratorRuleSelect.ALL,
                OrderBy = CodeGeneratorRuleOrder.CreatedAt,
                OrderType = OrderType.DESC,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                EntityTypeId = new IdFilter { Equal = EntityTypeEnum.CUSTOMER.Id },
            };
            List<CodeGeneratorRule> CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(CodeGeneratorRuleFilter);
            return CodeGeneratorRules.FirstOrDefault();
        }

        private async Task CodeGenerator(List<Customer> Customers, CodeGeneratorRule CodeGeneratorRule)
        {
            CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings.OrderBy(x => x.Sequence).ToList();
            foreach (Customer Customer in Customers)
            {
                string Code = "";
                foreach (var CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
                {
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.AUTO_NUMBER.Id && CodeGeneratorRule.AutoNumberLenth > 0)
                    {
                        // Customer.Id + 10^n -> cắt lấy n số cuối
                        string AutoNumber = (Math.Pow(10, CodeGeneratorRule.AutoNumberLenth) + Customer.Id).ToString();
                        AutoNumber = AutoNumber.Substring(AutoNumber.Length - (int)CodeGeneratorRule.AutoNumberLenth);
                        Code += $"{AutoNumber}.";
                    }
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.CUSTOMER_YEAR.Id)
                    {
                        Code += $"{Customer.CreatedAt.AddHours(CurrentContext.TimeZone).Year}.";
                    }
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.ORGANIZATION.Id)
                    {
                        Code += $"{Customer.Organization.Code}.";
                    }
                }
                Code = Code.Remove(Code.Length - 1);
                Customer.Code = Code;
                Customer.CodeGeneratorRuleId = CodeGeneratorRule.Id;
                Customer.CodeGeneratorRule = CodeGeneratorRule;
            }
        }

        public async Task<CustomerFilter> ToFilter(CustomerFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerFilter subFilter = new CustomerFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CodeDraft))
                        subFilter.CodeDraft = FilterBuilder.Merge(subFilter.CodeDraft, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = FilterBuilder.Merge(subFilter.Phone, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = FilterBuilder.Merge(subFilter.Address, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NationId))
                        subFilter.NationId = FilterBuilder.Merge(subFilter.NationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProvinceId))
                        subFilter.ProvinceId = FilterBuilder.Merge(subFilter.ProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DistrictId))
                        subFilter.DistrictId = FilterBuilder.Merge(subFilter.DistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WardId))
                        subFilter.WardId = FilterBuilder.Merge(subFilter.WardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Birthday))
                        subFilter.Birthday = FilterBuilder.Merge(subFilter.Birthday, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = FilterBuilder.Merge(subFilter.Email, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProfessionId))
                        subFilter.ProfessionId = FilterBuilder.Merge(subFilter.ProfessionId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CustomerSourceId))
                        subFilter.CustomerSourceId = FilterBuilder.Merge(subFilter.CustomerSourceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SexId))
                        subFilter.SexId = FilterBuilder.Merge(subFilter.SexId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CodeGeneratorRuleId))
                        subFilter.CodeGeneratorRuleId = FilterBuilder.Merge(subFilter.CodeGeneratorRuleId, FilterPermissionDefinition.IdFilter);
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
