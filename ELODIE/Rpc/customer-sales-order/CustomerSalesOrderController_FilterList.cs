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
using ELODIE.Services.MCustomerSalesOrder;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCustomer;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MProvince;
using ELODIE.Services.MWard;
using ELODIE.Services.MEditedPriceStatus;
using ELODIE.Services.MOrderPaymentStatus;
using ELODIE.Services.MOrderSource;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MRequestState;
using ELODIE.Services.MCustomerSalesOrderContent;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MTaxType;
using ELODIE.Services.MCustomerSalesOrderPaymentHistory;
using ELODIE.Services.MPaymentType;

namespace ELODIE.Rpc.customer_sales_order
{
    public partial class CustomerSalesOrderController : RpcController
    {
        [Route(CustomerSalesOrderRoute.FilterListCodeGeneratorRule), HttpPost]
        public async Task<List<CustomerSalesOrder_CodeGeneratorRuleDTO>> FilterListCodeGeneratorRule([FromBody] CustomerSalesOrder_CodeGeneratorRuleFilterDTO CustomerSalesOrder_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
            CodeGeneratorRuleFilter.Skip = 0;
            CodeGeneratorRuleFilter.Take = 20;
            CodeGeneratorRuleFilter.OrderBy = CodeGeneratorRuleOrder.Id;
            CodeGeneratorRuleFilter.OrderType = OrderType.ASC;
            CodeGeneratorRuleFilter.Selects = CodeGeneratorRuleSelect.ALL;
            CodeGeneratorRuleFilter.Id = CustomerSalesOrder_CodeGeneratorRuleFilterDTO.Id;
            CodeGeneratorRuleFilter.EntityTypeId = CustomerSalesOrder_CodeGeneratorRuleFilterDTO.EntityTypeId;
            CodeGeneratorRuleFilter.AutoNumberLenth = CustomerSalesOrder_CodeGeneratorRuleFilterDTO.AutoNumberLenth;
            CodeGeneratorRuleFilter.StatusId = CustomerSalesOrder_CodeGeneratorRuleFilterDTO.StatusId;
            CodeGeneratorRuleFilter.RowId = CustomerSalesOrder_CodeGeneratorRuleFilterDTO.RowId;

            List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);
            List<CustomerSalesOrder_CodeGeneratorRuleDTO> CustomerSalesOrder_CodeGeneratorRuleDTOs = CodeGeneratorRules
                .Select(x => new CustomerSalesOrder_CodeGeneratorRuleDTO(x)).ToList();
            return CustomerSalesOrder_CodeGeneratorRuleDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<CustomerSalesOrder_AppUserDTO>> FilterListAppUser([FromBody] CustomerSalesOrder_AppUserFilterDTO CustomerSalesOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = CustomerSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = CustomerSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = CustomerSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = CustomerSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = CustomerSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = CustomerSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = CustomerSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = CustomerSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.Avatar = CustomerSalesOrder_AppUserFilterDTO.Avatar;
            AppUserFilter.Department = CustomerSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = CustomerSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = CustomerSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.RowId = CustomerSalesOrder_AppUserFilterDTO.RowId;
            AppUserFilter.Password = CustomerSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.OtpCode = CustomerSalesOrder_AppUserFilterDTO.OtpCode;
            AppUserFilter.OtpExpired = CustomerSalesOrder_AppUserFilterDTO.OtpExpired;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<CustomerSalesOrder_AppUserDTO> CustomerSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new CustomerSalesOrder_AppUserDTO(x)).ToList();
            return CustomerSalesOrder_AppUserDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListCustomer), HttpPost]
        public async Task<List<CustomerSalesOrder_CustomerDTO>> FilterListCustomer([FromBody] CustomerSalesOrder_CustomerFilterDTO CustomerSalesOrder_CustomerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerFilter CustomerFilter = new CustomerFilter();
            CustomerFilter.Skip = 0;
            CustomerFilter.Take = 20;
            CustomerFilter.OrderBy = CustomerOrder.Id;
            CustomerFilter.OrderType = OrderType.ASC;
            CustomerFilter.Selects = CustomerSelect.ALL;
            CustomerFilter.Id = CustomerSalesOrder_CustomerFilterDTO.Id;
            CustomerFilter.Code = CustomerSalesOrder_CustomerFilterDTO.Code;
            CustomerFilter.CodeDraft = CustomerSalesOrder_CustomerFilterDTO.CodeDraft;
            CustomerFilter.Name = CustomerSalesOrder_CustomerFilterDTO.Name;
            CustomerFilter.Phone = CustomerSalesOrder_CustomerFilterDTO.Phone;
            CustomerFilter.Address = CustomerSalesOrder_CustomerFilterDTO.Address;
            CustomerFilter.NationId = CustomerSalesOrder_CustomerFilterDTO.NationId;
            CustomerFilter.ProvinceId = CustomerSalesOrder_CustomerFilterDTO.ProvinceId;
            CustomerFilter.DistrictId = CustomerSalesOrder_CustomerFilterDTO.DistrictId;
            CustomerFilter.WardId = CustomerSalesOrder_CustomerFilterDTO.WardId;
            CustomerFilter.Birthday = CustomerSalesOrder_CustomerFilterDTO.Birthday;
            CustomerFilter.Email = CustomerSalesOrder_CustomerFilterDTO.Email;
            CustomerFilter.ProfessionId = CustomerSalesOrder_CustomerFilterDTO.ProfessionId;
            CustomerFilter.CustomerSourceId = CustomerSalesOrder_CustomerFilterDTO.CustomerSourceId;
            CustomerFilter.SexId = CustomerSalesOrder_CustomerFilterDTO.SexId;
            CustomerFilter.StatusId = CustomerSalesOrder_CustomerFilterDTO.StatusId;
            CustomerFilter.AppUserId = CustomerSalesOrder_CustomerFilterDTO.AppUserId;
            CustomerFilter.CreatorId = CustomerSalesOrder_CustomerFilterDTO.CreatorId;
            CustomerFilter.OrganizationId = CustomerSalesOrder_CustomerFilterDTO.OrganizationId;
            CustomerFilter.RowId = CustomerSalesOrder_CustomerFilterDTO.RowId;
            CustomerFilter.CodeGeneratorRuleId = CustomerSalesOrder_CustomerFilterDTO.CodeGeneratorRuleId;

            List<Customer> Customers = await CustomerService.List(CustomerFilter);
            List<CustomerSalesOrder_CustomerDTO> CustomerSalesOrder_CustomerDTOs = Customers
                .Select(x => new CustomerSalesOrder_CustomerDTO(x)).ToList();
            return CustomerSalesOrder_CustomerDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListDistrict), HttpPost]
        public async Task<List<CustomerSalesOrder_DistrictDTO>> FilterListDistrict([FromBody] CustomerSalesOrder_DistrictFilterDTO CustomerSalesOrder_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = CustomerSalesOrder_DistrictFilterDTO.Id;
            DistrictFilter.Code = CustomerSalesOrder_DistrictFilterDTO.Code;
            DistrictFilter.Name = CustomerSalesOrder_DistrictFilterDTO.Name;
            DistrictFilter.Priority = CustomerSalesOrder_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = CustomerSalesOrder_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = CustomerSalesOrder_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<CustomerSalesOrder_DistrictDTO> CustomerSalesOrder_DistrictDTOs = Districts
                .Select(x => new CustomerSalesOrder_DistrictDTO(x)).ToList();
            return CustomerSalesOrder_DistrictDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListNation), HttpPost]
        public async Task<List<CustomerSalesOrder_NationDTO>> FilterListNation([FromBody] CustomerSalesOrder_NationFilterDTO CustomerSalesOrder_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = new NationFilter();
            NationFilter.Skip = 0;
            NationFilter.Take = 20;
            NationFilter.OrderBy = NationOrder.Id;
            NationFilter.OrderType = OrderType.ASC;
            NationFilter.Selects = NationSelect.ALL;
            NationFilter.Id = CustomerSalesOrder_NationFilterDTO.Id;
            NationFilter.Code = CustomerSalesOrder_NationFilterDTO.Code;
            NationFilter.Name = CustomerSalesOrder_NationFilterDTO.Name;
            NationFilter.Priority = CustomerSalesOrder_NationFilterDTO.Priority;
            NationFilter.StatusId = CustomerSalesOrder_NationFilterDTO.StatusId;
            NationFilter.RowId = CustomerSalesOrder_NationFilterDTO.RowId;

            List<Nation> Nations = await NationService.List(NationFilter);
            List<CustomerSalesOrder_NationDTO> CustomerSalesOrder_NationDTOs = Nations
                .Select(x => new CustomerSalesOrder_NationDTO(x)).ToList();
            return CustomerSalesOrder_NationDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListProvince), HttpPost]
        public async Task<List<CustomerSalesOrder_ProvinceDTO>> FilterListProvince([FromBody] CustomerSalesOrder_ProvinceFilterDTO CustomerSalesOrder_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = CustomerSalesOrder_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = CustomerSalesOrder_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = CustomerSalesOrder_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = CustomerSalesOrder_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = CustomerSalesOrder_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<CustomerSalesOrder_ProvinceDTO> CustomerSalesOrder_ProvinceDTOs = Provinces
                .Select(x => new CustomerSalesOrder_ProvinceDTO(x)).ToList();
            return CustomerSalesOrder_ProvinceDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListWard), HttpPost]
        public async Task<List<CustomerSalesOrder_WardDTO>> FilterListWard([FromBody] CustomerSalesOrder_WardFilterDTO CustomerSalesOrder_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = CustomerSalesOrder_WardFilterDTO.Id;
            WardFilter.Code = CustomerSalesOrder_WardFilterDTO.Code;
            WardFilter.Name = CustomerSalesOrder_WardFilterDTO.Name;
            WardFilter.Priority = CustomerSalesOrder_WardFilterDTO.Priority;
            WardFilter.DistrictId = CustomerSalesOrder_WardFilterDTO.DistrictId;
            WardFilter.StatusId = CustomerSalesOrder_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<CustomerSalesOrder_WardDTO> CustomerSalesOrder_WardDTOs = Wards
                .Select(x => new CustomerSalesOrder_WardDTO(x)).ToList();
            return CustomerSalesOrder_WardDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListEditedPriceStatus), HttpPost]
        public async Task<List<CustomerSalesOrder_EditedPriceStatusDTO>> FilterListEditedPriceStatus([FromBody] CustomerSalesOrder_EditedPriceStatusFilterDTO CustomerSalesOrder_EditedPriceStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = int.MaxValue;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<CustomerSalesOrder_EditedPriceStatusDTO> CustomerSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new CustomerSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return CustomerSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListOrderPaymentStatus), HttpPost]
        public async Task<List<CustomerSalesOrder_OrderPaymentStatusDTO>> FilterListOrderPaymentStatus([FromBody] CustomerSalesOrder_OrderPaymentStatusFilterDTO CustomerSalesOrder_OrderPaymentStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderPaymentStatusFilter OrderPaymentStatusFilter = new OrderPaymentStatusFilter();
            OrderPaymentStatusFilter.Skip = 0;
            OrderPaymentStatusFilter.Take = int.MaxValue;
            OrderPaymentStatusFilter.Take = 20;
            OrderPaymentStatusFilter.OrderBy = OrderPaymentStatusOrder.Id;
            OrderPaymentStatusFilter.OrderType = OrderType.ASC;
            OrderPaymentStatusFilter.Selects = OrderPaymentStatusSelect.ALL;

            List<OrderPaymentStatus> OrderPaymentStatuses = await OrderPaymentStatusService.List(OrderPaymentStatusFilter);
            List<CustomerSalesOrder_OrderPaymentStatusDTO> CustomerSalesOrder_OrderPaymentStatusDTOs = OrderPaymentStatuses
                .Select(x => new CustomerSalesOrder_OrderPaymentStatusDTO(x)).ToList();
            return CustomerSalesOrder_OrderPaymentStatusDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListOrderSource), HttpPost]
        public async Task<List<CustomerSalesOrder_OrderSourceDTO>> FilterListOrderSource([FromBody] CustomerSalesOrder_OrderSourceFilterDTO CustomerSalesOrder_OrderSourceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderSourceFilter OrderSourceFilter = new OrderSourceFilter();
            OrderSourceFilter.Skip = 0;
            OrderSourceFilter.Take = 20;
            OrderSourceFilter.OrderBy = OrderSourceOrder.Id;
            OrderSourceFilter.OrderType = OrderType.ASC;
            OrderSourceFilter.Selects = OrderSourceSelect.ALL;
            OrderSourceFilter.Id = CustomerSalesOrder_OrderSourceFilterDTO.Id;
            OrderSourceFilter.Code = CustomerSalesOrder_OrderSourceFilterDTO.Code;
            OrderSourceFilter.Name = CustomerSalesOrder_OrderSourceFilterDTO.Name;
            OrderSourceFilter.Priority = CustomerSalesOrder_OrderSourceFilterDTO.Priority;
            OrderSourceFilter.Description = CustomerSalesOrder_OrderSourceFilterDTO.Description;
            OrderSourceFilter.StatusId = CustomerSalesOrder_OrderSourceFilterDTO.StatusId;
            OrderSourceFilter.RowId = CustomerSalesOrder_OrderSourceFilterDTO.RowId;

            List<OrderSource> OrderSources = await OrderSourceService.List(OrderSourceFilter);
            List<CustomerSalesOrder_OrderSourceDTO> CustomerSalesOrder_OrderSourceDTOs = OrderSources
                .Select(x => new CustomerSalesOrder_OrderSourceDTO(x)).ToList();
            return CustomerSalesOrder_OrderSourceDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListOrganization), HttpPost]
        public async Task<List<CustomerSalesOrder_OrganizationDTO>> FilterListOrganization([FromBody] CustomerSalesOrder_OrganizationFilterDTO CustomerSalesOrder_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = CustomerSalesOrder_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = CustomerSalesOrder_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = CustomerSalesOrder_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = CustomerSalesOrder_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = CustomerSalesOrder_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = CustomerSalesOrder_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = CustomerSalesOrder_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = CustomerSalesOrder_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = CustomerSalesOrder_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = CustomerSalesOrder_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<CustomerSalesOrder_OrganizationDTO> CustomerSalesOrder_OrganizationDTOs = Organizations
                .Select(x => new CustomerSalesOrder_OrganizationDTO(x)).ToList();
            return CustomerSalesOrder_OrganizationDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListRequestState), HttpPost]
        public async Task<List<CustomerSalesOrder_RequestStateDTO>> FilterListRequestState([FromBody] CustomerSalesOrder_RequestStateFilterDTO CustomerSalesOrder_RequestStateFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = int.MaxValue;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;

            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            List<CustomerSalesOrder_RequestStateDTO> CustomerSalesOrder_RequestStateDTOs = RequestStates
                .Select(x => new CustomerSalesOrder_RequestStateDTO(x)).ToList();
            return CustomerSalesOrder_RequestStateDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListCustomerSalesOrderContent), HttpPost]
        public async Task<List<CustomerSalesOrder_CustomerSalesOrderContentDTO>> FilterListCustomerSalesOrderContent([FromBody] CustomerSalesOrder_CustomerSalesOrderContentFilterDTO CustomerSalesOrder_CustomerSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter = new CustomerSalesOrderContentFilter();
            CustomerSalesOrderContentFilter.Skip = 0;
            CustomerSalesOrderContentFilter.Take = 20;
            CustomerSalesOrderContentFilter.OrderBy = CustomerSalesOrderContentOrder.Id;
            CustomerSalesOrderContentFilter.OrderType = OrderType.ASC;
            CustomerSalesOrderContentFilter.Selects = CustomerSalesOrderContentSelect.ALL;
            CustomerSalesOrderContentFilter.Id = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.Id;
            CustomerSalesOrderContentFilter.CustomerSalesOrderId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.CustomerSalesOrderId;
            CustomerSalesOrderContentFilter.ItemId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.ItemId;
            CustomerSalesOrderContentFilter.UnitOfMeasureId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.UnitOfMeasureId;
            CustomerSalesOrderContentFilter.Quantity = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.Quantity;
            CustomerSalesOrderContentFilter.RequestedQuantity = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.RequestedQuantity;
            CustomerSalesOrderContentFilter.PrimaryUnitOfMeasureId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.PrimaryUnitOfMeasureId;
            CustomerSalesOrderContentFilter.SalePrice = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.SalePrice;
            CustomerSalesOrderContentFilter.PrimaryPrice = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.PrimaryPrice;
            CustomerSalesOrderContentFilter.DiscountPercentage = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.DiscountPercentage;
            CustomerSalesOrderContentFilter.DiscountAmount = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.DiscountAmount;
            CustomerSalesOrderContentFilter.GeneralDiscountPercentage = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.GeneralDiscountPercentage;
            CustomerSalesOrderContentFilter.GeneralDiscountAmount = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.GeneralDiscountAmount;
            CustomerSalesOrderContentFilter.TaxPercentage = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.TaxPercentage;
            CustomerSalesOrderContentFilter.TaxAmount = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.TaxAmount;
            CustomerSalesOrderContentFilter.TaxPercentageOther = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.TaxPercentageOther;
            CustomerSalesOrderContentFilter.TaxAmountOther = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.TaxAmountOther;
            CustomerSalesOrderContentFilter.Amount = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.Amount;
            CustomerSalesOrderContentFilter.Factor = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.Factor;
            CustomerSalesOrderContentFilter.EditedPriceStatusId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.EditedPriceStatusId;
            CustomerSalesOrderContentFilter.TaxTypeId = CustomerSalesOrder_CustomerSalesOrderContentFilterDTO.TaxTypeId;

            List<CustomerSalesOrderContent> CustomerSalesOrderContents = await CustomerSalesOrderContentService.List(CustomerSalesOrderContentFilter);
            List<CustomerSalesOrder_CustomerSalesOrderContentDTO> CustomerSalesOrder_CustomerSalesOrderContentDTOs = CustomerSalesOrderContents
                .Select(x => new CustomerSalesOrder_CustomerSalesOrderContentDTO(x)).ToList();
            return CustomerSalesOrder_CustomerSalesOrderContentDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<CustomerSalesOrder_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] CustomerSalesOrder_UnitOfMeasureFilterDTO CustomerSalesOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = CustomerSalesOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = CustomerSalesOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = CustomerSalesOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = CustomerSalesOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = CustomerSalesOrder_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<CustomerSalesOrder_UnitOfMeasureDTO> CustomerSalesOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new CustomerSalesOrder_UnitOfMeasureDTO(x)).ToList();
            return CustomerSalesOrder_UnitOfMeasureDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListTaxType), HttpPost]
        public async Task<List<CustomerSalesOrder_TaxTypeDTO>> FilterListTaxType([FromBody] CustomerSalesOrder_TaxTypeFilterDTO CustomerSalesOrder_TaxTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = CustomerSalesOrder_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = CustomerSalesOrder_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = CustomerSalesOrder_TaxTypeFilterDTO.Name;
            TaxTypeFilter.Percentage = CustomerSalesOrder_TaxTypeFilterDTO.Percentage;
            TaxTypeFilter.StatusId = CustomerSalesOrder_TaxTypeFilterDTO.StatusId;

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<CustomerSalesOrder_TaxTypeDTO> CustomerSalesOrder_TaxTypeDTOs = TaxTypes
                .Select(x => new CustomerSalesOrder_TaxTypeDTO(x)).ToList();
            return CustomerSalesOrder_TaxTypeDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListCustomerSalesOrderPaymentHistory), HttpPost]
        public async Task<List<CustomerSalesOrder_CustomerSalesOrderPaymentHistoryDTO>> FilterListCustomerSalesOrderPaymentHistory([FromBody] CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter = new CustomerSalesOrderPaymentHistoryFilter();
            CustomerSalesOrderPaymentHistoryFilter.Skip = 0;
            CustomerSalesOrderPaymentHistoryFilter.Take = 20;
            CustomerSalesOrderPaymentHistoryFilter.OrderBy = CustomerSalesOrderPaymentHistoryOrder.Id;
            CustomerSalesOrderPaymentHistoryFilter.OrderType = OrderType.ASC;
            CustomerSalesOrderPaymentHistoryFilter.Selects = CustomerSalesOrderPaymentHistorySelect.ALL;
            CustomerSalesOrderPaymentHistoryFilter.Id = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.Id;
            CustomerSalesOrderPaymentHistoryFilter.CustomerSalesOrderId = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.CustomerSalesOrderId;
            CustomerSalesOrderPaymentHistoryFilter.PaymentMilestone = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.PaymentMilestone;
            CustomerSalesOrderPaymentHistoryFilter.PaymentPercentage = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.PaymentPercentage;
            CustomerSalesOrderPaymentHistoryFilter.PaymentAmount = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.PaymentAmount;
            CustomerSalesOrderPaymentHistoryFilter.PaymentTypeId = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.PaymentTypeId;
            CustomerSalesOrderPaymentHistoryFilter.Description = CustomerSalesOrder_CustomerSalesOrderPaymentHistoryFilterDTO.Description;

            List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await CustomerSalesOrderPaymentHistoryService.List(CustomerSalesOrderPaymentHistoryFilter);
            List<CustomerSalesOrder_CustomerSalesOrderPaymentHistoryDTO> CustomerSalesOrder_CustomerSalesOrderPaymentHistoryDTOs = CustomerSalesOrderPaymentHistories
                .Select(x => new CustomerSalesOrder_CustomerSalesOrderPaymentHistoryDTO(x)).ToList();
            return CustomerSalesOrder_CustomerSalesOrderPaymentHistoryDTOs;
        }
        [Route(CustomerSalesOrderRoute.FilterListPaymentType), HttpPost]
        public async Task<List<CustomerSalesOrder_PaymentTypeDTO>> FilterListPaymentType([FromBody] CustomerSalesOrder_PaymentTypeFilterDTO CustomerSalesOrder_PaymentTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PaymentTypeFilter PaymentTypeFilter = new PaymentTypeFilter();
            PaymentTypeFilter.Skip = 0;
            PaymentTypeFilter.Take = 20;
            PaymentTypeFilter.OrderBy = PaymentTypeOrder.Id;
            PaymentTypeFilter.OrderType = OrderType.ASC;
            PaymentTypeFilter.Selects = PaymentTypeSelect.ALL;
            PaymentTypeFilter.Id = CustomerSalesOrder_PaymentTypeFilterDTO.Id;
            PaymentTypeFilter.Code = CustomerSalesOrder_PaymentTypeFilterDTO.Code;
            PaymentTypeFilter.Name = CustomerSalesOrder_PaymentTypeFilterDTO.Name;
            PaymentTypeFilter.StatusId = CustomerSalesOrder_PaymentTypeFilterDTO.StatusId;
            PaymentTypeFilter.RowId = CustomerSalesOrder_PaymentTypeFilterDTO.RowId;

            List<PaymentType> PaymentTypes = await PaymentTypeService.List(PaymentTypeFilter);
            List<CustomerSalesOrder_PaymentTypeDTO> CustomerSalesOrder_PaymentTypeDTOs = PaymentTypes
                .Select(x => new CustomerSalesOrder_PaymentTypeDTO(x)).ToList();
            return CustomerSalesOrder_PaymentTypeDTOs;
        }
    }
}

