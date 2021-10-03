using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using ELODIE.Entities;
using ELODIE.Services.MCustomer;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MCustomerSource;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MProfession;
using ELODIE.Services.MProvince;
using ELODIE.Services.MWard;

namespace ELODIE.Rpc.customer
{
    public partial class CustomerController : RpcController
    {
        [Route(CustomerRoute.SingleListAppUser), HttpPost]
        public async Task<List<Customer_AppUserDTO>> SingleListAppUser([FromBody] Customer_AppUserFilterDTO Customer_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Customer_AppUserFilterDTO.Id;
            AppUserFilter.Username = Customer_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Customer_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Customer_AppUserFilterDTO.Address;
            AppUserFilter.Email = Customer_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Customer_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = Customer_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Customer_AppUserFilterDTO.Birthday;
            AppUserFilter.Department = Customer_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Customer_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = Customer_AppUserFilterDTO.StatusId;
            AppUserFilter.Password = Customer_AppUserFilterDTO.Password;
            AppUserFilter.StatusId = new IdFilter{ Equal = 1 };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Customer_AppUserDTO> Customer_AppUserDTOs = AppUsers
                .Select(x => new Customer_AppUserDTO(x)).ToList();
            return Customer_AppUserDTOs;
        }
        [Route(CustomerRoute.SingleListCodeGeneratorRule), HttpPost]
        public async Task<List<Customer_CodeGeneratorRuleDTO>> SingleListCodeGeneratorRule([FromBody] Customer_CodeGeneratorRuleFilterDTO Customer_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
            CodeGeneratorRuleFilter.Skip = 0;
            CodeGeneratorRuleFilter.Take = 20;
            CodeGeneratorRuleFilter.OrderBy = CodeGeneratorRuleOrder.Id;
            CodeGeneratorRuleFilter.OrderType = OrderType.ASC;
            CodeGeneratorRuleFilter.Selects = CodeGeneratorRuleSelect.ALL;
            CodeGeneratorRuleFilter.Id = Customer_CodeGeneratorRuleFilterDTO.Id;
            CodeGeneratorRuleFilter.EntityTypeId = Customer_CodeGeneratorRuleFilterDTO.EntityTypeId;
            CodeGeneratorRuleFilter.AutoNumberLenth = Customer_CodeGeneratorRuleFilterDTO.AutoNumberLenth;
            CodeGeneratorRuleFilter.StatusId = Customer_CodeGeneratorRuleFilterDTO.StatusId;
            CodeGeneratorRuleFilter.RowId = Customer_CodeGeneratorRuleFilterDTO.RowId;
            CodeGeneratorRuleFilter.StatusId = new IdFilter{ Equal = 1 };
            List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);
            List<Customer_CodeGeneratorRuleDTO> Customer_CodeGeneratorRuleDTOs = CodeGeneratorRules
                .Select(x => new Customer_CodeGeneratorRuleDTO(x)).ToList();
            return Customer_CodeGeneratorRuleDTOs;
        }
        [Route(CustomerRoute.SingleListCustomerSource), HttpPost]
        public async Task<List<Customer_CustomerSourceDTO>> SingleListCustomerSource([FromBody] Customer_CustomerSourceFilterDTO Customer_CustomerSourceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSourceFilter CustomerSourceFilter = new CustomerSourceFilter();
            CustomerSourceFilter.Skip = 0;
            CustomerSourceFilter.Take = 20;
            CustomerSourceFilter.OrderBy = CustomerSourceOrder.Id;
            CustomerSourceFilter.OrderType = OrderType.ASC;
            CustomerSourceFilter.Selects = CustomerSourceSelect.ALL;
            CustomerSourceFilter.Id = Customer_CustomerSourceFilterDTO.Id;
            CustomerSourceFilter.Code = Customer_CustomerSourceFilterDTO.Code;
            CustomerSourceFilter.Name = Customer_CustomerSourceFilterDTO.Name;
            CustomerSourceFilter.StatusId = Customer_CustomerSourceFilterDTO.StatusId;
            CustomerSourceFilter.Description = Customer_CustomerSourceFilterDTO.Description;
            CustomerSourceFilter.RowId = Customer_CustomerSourceFilterDTO.RowId;
            CustomerSourceFilter.StatusId = new IdFilter{ Equal = 1 };
            List<CustomerSource> CustomerSources = await CustomerSourceService.List(CustomerSourceFilter);
            List<Customer_CustomerSourceDTO> Customer_CustomerSourceDTOs = CustomerSources
                .Select(x => new Customer_CustomerSourceDTO(x)).ToList();
            return Customer_CustomerSourceDTOs;
        }
        [Route(CustomerRoute.SingleListDistrict), HttpPost]
        public async Task<List<Customer_DistrictDTO>> SingleListDistrict([FromBody] Customer_DistrictFilterDTO Customer_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Customer_DistrictFilterDTO.Id;
            DistrictFilter.Code = Customer_DistrictFilterDTO.Code;
            DistrictFilter.Name = Customer_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Customer_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Customer_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Customer_DistrictFilterDTO.StatusId;
            DistrictFilter.StatusId = new IdFilter{ Equal = 1 };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Customer_DistrictDTO> Customer_DistrictDTOs = Districts
                .Select(x => new Customer_DistrictDTO(x)).ToList();
            return Customer_DistrictDTOs;
        }
        [Route(CustomerRoute.SingleListNation), HttpPost]
        public async Task<List<Customer_NationDTO>> SingleListNation([FromBody] Customer_NationFilterDTO Customer_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = new NationFilter();
            NationFilter.Skip = 0;
            NationFilter.Take = 20;
            NationFilter.OrderBy = NationOrder.Id;
            NationFilter.OrderType = OrderType.ASC;
            NationFilter.Selects = NationSelect.ALL;
            NationFilter.Id = Customer_NationFilterDTO.Id;
            NationFilter.Code = Customer_NationFilterDTO.Code;
            NationFilter.Name = Customer_NationFilterDTO.Name;
            NationFilter.Priority = Customer_NationFilterDTO.Priority;
            NationFilter.StatusId = Customer_NationFilterDTO.StatusId;
            NationFilter.RowId = Customer_NationFilterDTO.RowId;
            NationFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Nation> Nations = await NationService.List(NationFilter);
            List<Customer_NationDTO> Customer_NationDTOs = Nations
                .Select(x => new Customer_NationDTO(x)).ToList();
            return Customer_NationDTOs;
        }
        [Route(CustomerRoute.SingleListOrganization), HttpPost]
        public async Task<List<Customer_OrganizationDTO>> SingleListOrganization([FromBody] Customer_OrganizationFilterDTO Customer_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Customer_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Customer_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Customer_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Customer_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Customer_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Customer_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Customer_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Customer_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Customer_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Customer_OrganizationFilterDTO.Address;
            OrganizationFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Customer_OrganizationDTO> Customer_OrganizationDTOs = Organizations
                .Select(x => new Customer_OrganizationDTO(x)).ToList();
            return Customer_OrganizationDTOs;
        }
        [Route(CustomerRoute.SingleListProfession), HttpPost]
        public async Task<List<Customer_ProfessionDTO>> SingleListProfession([FromBody] Customer_ProfessionFilterDTO Customer_ProfessionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProfessionFilter ProfessionFilter = new ProfessionFilter();
            ProfessionFilter.Skip = 0;
            ProfessionFilter.Take = 20;
            ProfessionFilter.OrderBy = ProfessionOrder.Id;
            ProfessionFilter.OrderType = OrderType.ASC;
            ProfessionFilter.Selects = ProfessionSelect.ALL;
            ProfessionFilter.Id = Customer_ProfessionFilterDTO.Id;
            ProfessionFilter.Code = Customer_ProfessionFilterDTO.Code;
            ProfessionFilter.Name = Customer_ProfessionFilterDTO.Name;
            ProfessionFilter.StatusId = Customer_ProfessionFilterDTO.StatusId;
            ProfessionFilter.RowId = Customer_ProfessionFilterDTO.RowId;
            ProfessionFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Profession> Professions = await ProfessionService.List(ProfessionFilter);
            List<Customer_ProfessionDTO> Customer_ProfessionDTOs = Professions
                .Select(x => new Customer_ProfessionDTO(x)).ToList();
            return Customer_ProfessionDTOs;
        }
        [Route(CustomerRoute.SingleListProvince), HttpPost]
        public async Task<List<Customer_ProvinceDTO>> SingleListProvince([FromBody] Customer_ProvinceFilterDTO Customer_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Customer_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = Customer_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = Customer_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Customer_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Customer_ProvinceFilterDTO.StatusId;
            ProvinceFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Customer_ProvinceDTO> Customer_ProvinceDTOs = Provinces
                .Select(x => new Customer_ProvinceDTO(x)).ToList();
            return Customer_ProvinceDTOs;
        }
        [Route(CustomerRoute.SingleListWard), HttpPost]
        public async Task<List<Customer_WardDTO>> SingleListWard([FromBody] Customer_WardFilterDTO Customer_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Customer_WardFilterDTO.Id;
            WardFilter.Code = Customer_WardFilterDTO.Code;
            WardFilter.Name = Customer_WardFilterDTO.Name;
            WardFilter.Priority = Customer_WardFilterDTO.Priority;
            WardFilter.DistrictId = Customer_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Customer_WardFilterDTO.StatusId;
            WardFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Customer_WardDTO> Customer_WardDTOs = Wards
                .Select(x => new Customer_WardDTO(x)).ToList();
            return Customer_WardDTOs;
        }
    }
}

