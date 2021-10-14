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
using System.Dynamic;
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
using ELODIE.Services.MProduct;

namespace ELODIE.Rpc.customer_sales_order
{
    public partial class CustomerSalesOrderController : RpcController
    {
        private ICodeGeneratorRuleService CodeGeneratorRuleService;
        private IAppUserService AppUserService;
        private ICustomerService CustomerService;
        private IDistrictService DistrictService;
        private INationService NationService;
        private IProvinceService ProvinceService;
        private IWardService WardService;
        private IEditedPriceStatusService EditedPriceStatusService;
        private IOrderPaymentStatusService OrderPaymentStatusService;
        private IOrderSourceService OrderSourceService;
        private IOrganizationService OrganizationService;
        private IRequestStateService RequestStateService;
        private ICustomerSalesOrderContentService CustomerSalesOrderContentService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private ITaxTypeService TaxTypeService;
        private ICustomerSalesOrderPaymentHistoryService CustomerSalesOrderPaymentHistoryService;
        private IPaymentTypeService PaymentTypeService;
        private ICustomerSalesOrderService CustomerSalesOrderService;
        private IItemService ItemService;
        private IProductService ProductService;
        private ICurrentContext CurrentContext;
        public CustomerSalesOrderController(
            ICodeGeneratorRuleService CodeGeneratorRuleService,
            IAppUserService AppUserService,
            ICustomerService CustomerService,
            IDistrictService DistrictService,
            INationService NationService,
            IProvinceService ProvinceService,
            IWardService WardService,
            IEditedPriceStatusService EditedPriceStatusService,
            IOrderPaymentStatusService OrderPaymentStatusService,
            IOrderSourceService OrderSourceService,
            IOrganizationService OrganizationService,
            IRequestStateService RequestStateService,
            ICustomerSalesOrderContentService CustomerSalesOrderContentService,
            IUnitOfMeasureService UnitOfMeasureService,
            ITaxTypeService TaxTypeService,
            ICustomerSalesOrderPaymentHistoryService CustomerSalesOrderPaymentHistoryService,
            IPaymentTypeService PaymentTypeService,
            ICustomerSalesOrderService CustomerSalesOrderService,
            IItemService ItemService,
            IProductService ProductService,
            ICurrentContext CurrentContext
        )
        {
            this.CodeGeneratorRuleService = CodeGeneratorRuleService;
            this.AppUserService = AppUserService;
            this.CustomerService = CustomerService;
            this.DistrictService = DistrictService;
            this.NationService = NationService;
            this.ProvinceService = ProvinceService;
            this.WardService = WardService;
            this.EditedPriceStatusService = EditedPriceStatusService;
            this.OrderPaymentStatusService = OrderPaymentStatusService;
            this.OrderSourceService = OrderSourceService;
            this.OrganizationService = OrganizationService;
            this.RequestStateService = RequestStateService;
            this.CustomerSalesOrderContentService = CustomerSalesOrderContentService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.TaxTypeService = TaxTypeService;
            this.CustomerSalesOrderPaymentHistoryService = CustomerSalesOrderPaymentHistoryService;
            this.PaymentTypeService = PaymentTypeService;
            this.CustomerSalesOrderService = CustomerSalesOrderService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CustomerSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] CustomerSalesOrder_CustomerSalesOrderFilterDTO CustomerSalesOrder_CustomerSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSalesOrderFilter CustomerSalesOrderFilter = ConvertFilterDTOToFilterEntity(CustomerSalesOrder_CustomerSalesOrderFilterDTO);
            CustomerSalesOrderFilter = await CustomerSalesOrderService.ToFilter(CustomerSalesOrderFilter);
            int count = await CustomerSalesOrderService.Count(CustomerSalesOrderFilter);
            return count;
        }

        [Route(CustomerSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<CustomerSalesOrder_CustomerSalesOrderDTO>>> List([FromBody] CustomerSalesOrder_CustomerSalesOrderFilterDTO CustomerSalesOrder_CustomerSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSalesOrderFilter CustomerSalesOrderFilter = ConvertFilterDTOToFilterEntity(CustomerSalesOrder_CustomerSalesOrderFilterDTO);
            CustomerSalesOrderFilter = await CustomerSalesOrderService.ToFilter(CustomerSalesOrderFilter);
            List<CustomerSalesOrder> CustomerSalesOrders = await CustomerSalesOrderService.List(CustomerSalesOrderFilter);
            List<CustomerSalesOrder_CustomerSalesOrderDTO> CustomerSalesOrder_CustomerSalesOrderDTOs = CustomerSalesOrders
                .Select(c => new CustomerSalesOrder_CustomerSalesOrderDTO(c)).ToList();
            return CustomerSalesOrder_CustomerSalesOrderDTOs;
        }

        [Route(CustomerSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<CustomerSalesOrder_CustomerSalesOrderDTO>> Get([FromBody]CustomerSalesOrder_CustomerSalesOrderDTO CustomerSalesOrder_CustomerSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CustomerSalesOrder_CustomerSalesOrderDTO.Id))
                return Forbid();

            CustomerSalesOrder CustomerSalesOrder = await CustomerSalesOrderService.Get(CustomerSalesOrder_CustomerSalesOrderDTO.Id);
            return new CustomerSalesOrder_CustomerSalesOrderDTO(CustomerSalesOrder);
        }

        [Route(CustomerSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<CustomerSalesOrder_CustomerSalesOrderDTO>> Create([FromBody] CustomerSalesOrder_CustomerSalesOrderDTO CustomerSalesOrder_CustomerSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(CustomerSalesOrder_CustomerSalesOrderDTO.Id))
                return Forbid();

            CustomerSalesOrder CustomerSalesOrder = ConvertDTOToEntity(CustomerSalesOrder_CustomerSalesOrderDTO);
            CustomerSalesOrder = await CustomerSalesOrderService.Create(CustomerSalesOrder);
            CustomerSalesOrder_CustomerSalesOrderDTO = new CustomerSalesOrder_CustomerSalesOrderDTO(CustomerSalesOrder);
            if (CustomerSalesOrder.IsValidated)
                return CustomerSalesOrder_CustomerSalesOrderDTO;
            else
                return BadRequest(CustomerSalesOrder_CustomerSalesOrderDTO);
        }

        [Route(CustomerSalesOrderRoute.Update), HttpPost]
        public async Task<ActionResult<CustomerSalesOrder_CustomerSalesOrderDTO>> Update([FromBody] CustomerSalesOrder_CustomerSalesOrderDTO CustomerSalesOrder_CustomerSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(CustomerSalesOrder_CustomerSalesOrderDTO.Id))
                return Forbid();

            CustomerSalesOrder CustomerSalesOrder = ConvertDTOToEntity(CustomerSalesOrder_CustomerSalesOrderDTO);
            CustomerSalesOrder = await CustomerSalesOrderService.Update(CustomerSalesOrder);
            CustomerSalesOrder_CustomerSalesOrderDTO = new CustomerSalesOrder_CustomerSalesOrderDTO(CustomerSalesOrder);
            if (CustomerSalesOrder.IsValidated)
                return CustomerSalesOrder_CustomerSalesOrderDTO;
            else
                return BadRequest(CustomerSalesOrder_CustomerSalesOrderDTO);
        }

        [Route(CustomerSalesOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<CustomerSalesOrder_CustomerSalesOrderDTO>> Delete([FromBody] CustomerSalesOrder_CustomerSalesOrderDTO CustomerSalesOrder_CustomerSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CustomerSalesOrder_CustomerSalesOrderDTO.Id))
                return Forbid();

            CustomerSalesOrder CustomerSalesOrder = ConvertDTOToEntity(CustomerSalesOrder_CustomerSalesOrderDTO);
            CustomerSalesOrder = await CustomerSalesOrderService.Delete(CustomerSalesOrder);
            CustomerSalesOrder_CustomerSalesOrderDTO = new CustomerSalesOrder_CustomerSalesOrderDTO(CustomerSalesOrder);
            if (CustomerSalesOrder.IsValidated)
                return CustomerSalesOrder_CustomerSalesOrderDTO;
            else
                return BadRequest(CustomerSalesOrder_CustomerSalesOrderDTO);
        }
        
        [Route(CustomerSalesOrderRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerSalesOrderFilter CustomerSalesOrderFilter = new CustomerSalesOrderFilter();
            CustomerSalesOrderFilter = await CustomerSalesOrderService.ToFilter(CustomerSalesOrderFilter);
            CustomerSalesOrderFilter.Id = new IdFilter { In = Ids };
            CustomerSalesOrderFilter.Selects = CustomerSalesOrderSelect.Id;
            CustomerSalesOrderFilter.Skip = 0;
            CustomerSalesOrderFilter.Take = int.MaxValue;

            List<CustomerSalesOrder> CustomerSalesOrders = await CustomerSalesOrderService.List(CustomerSalesOrderFilter);
            CustomerSalesOrders = await CustomerSalesOrderService.BulkDelete(CustomerSalesOrders);
            if (CustomerSalesOrders.Any(x => !x.IsValidated))
                return BadRequest(CustomerSalesOrders.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(CustomerSalesOrderRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CodeGeneratorRuleSelect.ALL
            };
            List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            CustomerFilter CustomerFilter = new CustomerFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CustomerSelect.ALL
            };
            List<Customer> Customers = await CustomerService.List(CustomerFilter);
            DistrictFilter DeliveryDistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.ALL
            };
            List<District> DeliveryDistricts = await DistrictService.List(DeliveryDistrictFilter);
            NationFilter DeliveryNationFilter = new NationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NationSelect.ALL
            };
            List<Nation> DeliveryNations = await NationService.List(DeliveryNationFilter);
            ProvinceFilter DeliveryProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.ALL
            };
            List<Province> DeliveryProvinces = await ProvinceService.List(DeliveryProvinceFilter);
            WardFilter DeliveryWardFilter = new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.ALL
            };
            List<Ward> DeliveryWards = await WardService.List(DeliveryWardFilter);
            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = EditedPriceStatusSelect.ALL
            };
            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            DistrictFilter InvoiceDistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.ALL
            };
            List<District> InvoiceDistricts = await DistrictService.List(InvoiceDistrictFilter);
            NationFilter InvoiceNationFilter = new NationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NationSelect.ALL
            };
            List<Nation> InvoiceNations = await NationService.List(InvoiceNationFilter);
            ProvinceFilter InvoiceProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.ALL
            };
            List<Province> InvoiceProvinces = await ProvinceService.List(InvoiceProvinceFilter);
            WardFilter InvoiceWardFilter = new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.ALL
            };
            List<Ward> InvoiceWards = await WardService.List(InvoiceWardFilter);
            OrderPaymentStatusFilter OrderPaymentStatusFilter = new OrderPaymentStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrderPaymentStatusSelect.ALL
            };
            List<OrderPaymentStatus> OrderPaymentStatuses = await OrderPaymentStatusService.List(OrderPaymentStatusFilter);
            OrderSourceFilter OrderSourceFilter = new OrderSourceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrderSourceSelect.ALL
            };
            List<OrderSource> OrderSources = await OrderSourceService.List(OrderSourceFilter);
            RequestStateFilter RequestStateFilter = new RequestStateFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RequestStateSelect.ALL
            };
            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            AppUserFilter SalesEmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> SalesEmployees = await AppUserService.List(SalesEmployeeFilter);
            List<CustomerSalesOrder> CustomerSalesOrders = new List<CustomerSalesOrder>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(CustomerSalesOrders);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int CustomerIdColumn = 2 + StartColumn;
                int OrderSourceIdColumn = 3 + StartColumn;
                int RequestStateIdColumn = 4 + StartColumn;
                int OrderPaymentStatusIdColumn = 5 + StartColumn;
                int EditedPriceStatusIdColumn = 6 + StartColumn;
                int ShippingNameColumn = 7 + StartColumn;
                int OrderDateColumn = 8 + StartColumn;
                int DeliveryDateColumn = 9 + StartColumn;
                int SalesEmployeeIdColumn = 10 + StartColumn;
                int NoteColumn = 11 + StartColumn;
                int InvoiceAddressColumn = 12 + StartColumn;
                int InvoiceNationIdColumn = 13 + StartColumn;
                int InvoiceProvinceIdColumn = 14 + StartColumn;
                int InvoiceDistrictIdColumn = 15 + StartColumn;
                int InvoiceWardIdColumn = 16 + StartColumn;
                int InvoiceZIPCodeColumn = 17 + StartColumn;
                int DeliveryAddressColumn = 18 + StartColumn;
                int DeliveryNationIdColumn = 19 + StartColumn;
                int DeliveryProvinceIdColumn = 20 + StartColumn;
                int DeliveryDistrictIdColumn = 21 + StartColumn;
                int DeliveryWardIdColumn = 22 + StartColumn;
                int DeliveryZIPCodeColumn = 23 + StartColumn;
                int SubTotalColumn = 24 + StartColumn;
                int GeneralDiscountPercentageColumn = 25 + StartColumn;
                int GeneralDiscountAmountColumn = 26 + StartColumn;
                int TotalTaxOtherColumn = 27 + StartColumn;
                int TotalTaxColumn = 28 + StartColumn;
                int TotalColumn = 29 + StartColumn;
                int CreatorIdColumn = 30 + StartColumn;
                int OrganizationIdColumn = 31 + StartColumn;
                int RowIdColumn = 35 + StartColumn;
                int CodeGeneratorRuleIdColumn = 36 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string CustomerIdValue = worksheet.Cells[i + StartRow, CustomerIdColumn].Value?.ToString();
                    string OrderSourceIdValue = worksheet.Cells[i + StartRow, OrderSourceIdColumn].Value?.ToString();
                    string RequestStateIdValue = worksheet.Cells[i + StartRow, RequestStateIdColumn].Value?.ToString();
                    string OrderPaymentStatusIdValue = worksheet.Cells[i + StartRow, OrderPaymentStatusIdColumn].Value?.ToString();
                    string EditedPriceStatusIdValue = worksheet.Cells[i + StartRow, EditedPriceStatusIdColumn].Value?.ToString();
                    string ShippingNameValue = worksheet.Cells[i + StartRow, ShippingNameColumn].Value?.ToString();
                    string OrderDateValue = worksheet.Cells[i + StartRow, OrderDateColumn].Value?.ToString();
                    string DeliveryDateValue = worksheet.Cells[i + StartRow, DeliveryDateColumn].Value?.ToString();
                    string SalesEmployeeIdValue = worksheet.Cells[i + StartRow, SalesEmployeeIdColumn].Value?.ToString();
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    string InvoiceAddressValue = worksheet.Cells[i + StartRow, InvoiceAddressColumn].Value?.ToString();
                    string InvoiceNationIdValue = worksheet.Cells[i + StartRow, InvoiceNationIdColumn].Value?.ToString();
                    string InvoiceProvinceIdValue = worksheet.Cells[i + StartRow, InvoiceProvinceIdColumn].Value?.ToString();
                    string InvoiceDistrictIdValue = worksheet.Cells[i + StartRow, InvoiceDistrictIdColumn].Value?.ToString();
                    string InvoiceWardIdValue = worksheet.Cells[i + StartRow, InvoiceWardIdColumn].Value?.ToString();
                    string InvoiceZIPCodeValue = worksheet.Cells[i + StartRow, InvoiceZIPCodeColumn].Value?.ToString();
                    string DeliveryAddressValue = worksheet.Cells[i + StartRow, DeliveryAddressColumn].Value?.ToString();
                    string DeliveryNationIdValue = worksheet.Cells[i + StartRow, DeliveryNationIdColumn].Value?.ToString();
                    string DeliveryProvinceIdValue = worksheet.Cells[i + StartRow, DeliveryProvinceIdColumn].Value?.ToString();
                    string DeliveryDistrictIdValue = worksheet.Cells[i + StartRow, DeliveryDistrictIdColumn].Value?.ToString();
                    string DeliveryWardIdValue = worksheet.Cells[i + StartRow, DeliveryWardIdColumn].Value?.ToString();
                    string DeliveryZIPCodeValue = worksheet.Cells[i + StartRow, DeliveryZIPCodeColumn].Value?.ToString();
                    string SubTotalValue = worksheet.Cells[i + StartRow, SubTotalColumn].Value?.ToString();
                    string GeneralDiscountPercentageValue = worksheet.Cells[i + StartRow, GeneralDiscountPercentageColumn].Value?.ToString();
                    string GeneralDiscountAmountValue = worksheet.Cells[i + StartRow, GeneralDiscountAmountColumn].Value?.ToString();
                    string TotalTaxOtherValue = worksheet.Cells[i + StartRow, TotalTaxOtherColumn].Value?.ToString();
                    string TotalTaxValue = worksheet.Cells[i + StartRow, TotalTaxColumn].Value?.ToString();
                    string TotalValue = worksheet.Cells[i + StartRow, TotalColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    string CodeGeneratorRuleIdValue = worksheet.Cells[i + StartRow, CodeGeneratorRuleIdColumn].Value?.ToString();
                    
                    CustomerSalesOrder CustomerSalesOrder = new CustomerSalesOrder();
                    CustomerSalesOrder.Code = CodeValue;
                    CustomerSalesOrder.ShippingName = ShippingNameValue;
                    CustomerSalesOrder.OrderDate = DateTime.TryParse(OrderDateValue, out DateTime OrderDate) ? OrderDate : DateTime.Now;
                    CustomerSalesOrder.DeliveryDate = DateTime.TryParse(DeliveryDateValue, out DateTime DeliveryDate) ? DeliveryDate : DateTime.Now;
                    CustomerSalesOrder.Note = NoteValue;
                    CustomerSalesOrder.InvoiceAddress = InvoiceAddressValue;
                    CustomerSalesOrder.InvoiceZIPCode = InvoiceZIPCodeValue;
                    CustomerSalesOrder.DeliveryAddress = DeliveryAddressValue;
                    CustomerSalesOrder.DeliveryZIPCode = DeliveryZIPCodeValue;
                    CustomerSalesOrder.SubTotal = decimal.TryParse(SubTotalValue, out decimal SubTotal) ? SubTotal : 0;
                    CustomerSalesOrder.GeneralDiscountPercentage = decimal.TryParse(GeneralDiscountPercentageValue, out decimal GeneralDiscountPercentage) ? GeneralDiscountPercentage : 0;
                    CustomerSalesOrder.GeneralDiscountAmount = decimal.TryParse(GeneralDiscountAmountValue, out decimal GeneralDiscountAmount) ? GeneralDiscountAmount : 0;
                    CustomerSalesOrder.TotalTaxOther = decimal.TryParse(TotalTaxOtherValue, out decimal TotalTaxOther) ? TotalTaxOther : 0;
                    CustomerSalesOrder.TotalTax = decimal.TryParse(TotalTaxValue, out decimal TotalTax) ? TotalTax : 0;
                    CustomerSalesOrder.Total = decimal.TryParse(TotalValue, out decimal Total) ? Total : 0;
                    CodeGeneratorRule CodeGeneratorRule = CodeGeneratorRules.Where(x => x.Id.ToString() == CodeGeneratorRuleIdValue).FirstOrDefault();
                    CustomerSalesOrder.CodeGeneratorRuleId = CodeGeneratorRule == null ? 0 : CodeGeneratorRule.Id;
                    CustomerSalesOrder.CodeGeneratorRule = CodeGeneratorRule;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    CustomerSalesOrder.CreatorId = Creator == null ? 0 : Creator.Id;
                    CustomerSalesOrder.Creator = Creator;
                    Customer Customer = Customers.Where(x => x.Id.ToString() == CustomerIdValue).FirstOrDefault();
                    CustomerSalesOrder.CustomerId = Customer == null ? 0 : Customer.Id;
                    CustomerSalesOrder.Customer = Customer;
                    District DeliveryDistrict = DeliveryDistricts.Where(x => x.Id.ToString() == DeliveryDistrictIdValue).FirstOrDefault();
                    CustomerSalesOrder.DeliveryDistrictId = DeliveryDistrict == null ? 0 : DeliveryDistrict.Id;
                    CustomerSalesOrder.DeliveryDistrict = DeliveryDistrict;
                    Nation DeliveryNation = DeliveryNations.Where(x => x.Id.ToString() == DeliveryNationIdValue).FirstOrDefault();
                    CustomerSalesOrder.DeliveryNationId = DeliveryNation == null ? 0 : DeliveryNation.Id;
                    CustomerSalesOrder.DeliveryNation = DeliveryNation;
                    Province DeliveryProvince = DeliveryProvinces.Where(x => x.Id.ToString() == DeliveryProvinceIdValue).FirstOrDefault();
                    CustomerSalesOrder.DeliveryProvinceId = DeliveryProvince == null ? 0 : DeliveryProvince.Id;
                    CustomerSalesOrder.DeliveryProvince = DeliveryProvince;
                    Ward DeliveryWard = DeliveryWards.Where(x => x.Id.ToString() == DeliveryWardIdValue).FirstOrDefault();
                    CustomerSalesOrder.DeliveryWardId = DeliveryWard == null ? 0 : DeliveryWard.Id;
                    CustomerSalesOrder.DeliveryWard = DeliveryWard;
                    EditedPriceStatus EditedPriceStatus = EditedPriceStatuses.Where(x => x.Id.ToString() == EditedPriceStatusIdValue).FirstOrDefault();
                    CustomerSalesOrder.EditedPriceStatusId = EditedPriceStatus == null ? 0 : EditedPriceStatus.Id;
                    CustomerSalesOrder.EditedPriceStatus = EditedPriceStatus;
                    District InvoiceDistrict = InvoiceDistricts.Where(x => x.Id.ToString() == InvoiceDistrictIdValue).FirstOrDefault();
                    CustomerSalesOrder.InvoiceDistrictId = InvoiceDistrict == null ? 0 : InvoiceDistrict.Id;
                    CustomerSalesOrder.InvoiceDistrict = InvoiceDistrict;
                    Nation InvoiceNation = InvoiceNations.Where(x => x.Id.ToString() == InvoiceNationIdValue).FirstOrDefault();
                    CustomerSalesOrder.InvoiceNationId = InvoiceNation == null ? 0 : InvoiceNation.Id;
                    CustomerSalesOrder.InvoiceNation = InvoiceNation;
                    Province InvoiceProvince = InvoiceProvinces.Where(x => x.Id.ToString() == InvoiceProvinceIdValue).FirstOrDefault();
                    CustomerSalesOrder.InvoiceProvinceId = InvoiceProvince == null ? 0 : InvoiceProvince.Id;
                    CustomerSalesOrder.InvoiceProvince = InvoiceProvince;
                    Ward InvoiceWard = InvoiceWards.Where(x => x.Id.ToString() == InvoiceWardIdValue).FirstOrDefault();
                    CustomerSalesOrder.InvoiceWardId = InvoiceWard == null ? 0 : InvoiceWard.Id;
                    CustomerSalesOrder.InvoiceWard = InvoiceWard;
                    OrderPaymentStatus OrderPaymentStatus = OrderPaymentStatuses.Where(x => x.Id.ToString() == OrderPaymentStatusIdValue).FirstOrDefault();
                    CustomerSalesOrder.OrderPaymentStatusId = OrderPaymentStatus == null ? 0 : OrderPaymentStatus.Id;
                    CustomerSalesOrder.OrderPaymentStatus = OrderPaymentStatus;
                    OrderSource OrderSource = OrderSources.Where(x => x.Id.ToString() == OrderSourceIdValue).FirstOrDefault();
                    CustomerSalesOrder.OrderSourceId = OrderSource == null ? 0 : OrderSource.Id;
                    CustomerSalesOrder.OrderSource = OrderSource;
                    RequestState RequestState = RequestStates.Where(x => x.Id.ToString() == RequestStateIdValue).FirstOrDefault();
                    CustomerSalesOrder.RequestStateId = RequestState == null ? 0 : RequestState.Id;
                    CustomerSalesOrder.RequestState = RequestState;
                    AppUser SalesEmployee = SalesEmployees.Where(x => x.Id.ToString() == SalesEmployeeIdValue).FirstOrDefault();
                    CustomerSalesOrder.SalesEmployeeId = SalesEmployee == null ? 0 : SalesEmployee.Id;
                    CustomerSalesOrder.SalesEmployee = SalesEmployee;
                    
                    CustomerSalesOrders.Add(CustomerSalesOrder);
                }
            }
            CustomerSalesOrders = await CustomerSalesOrderService.Import(CustomerSalesOrders);
            if (CustomerSalesOrders.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < CustomerSalesOrders.Count; i++)
                {
                    CustomerSalesOrder CustomerSalesOrder = CustomerSalesOrders[i];
                    if (!CustomerSalesOrder.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.Id)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.Id)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.Code)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.Code)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.CustomerId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.CustomerId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.OrderSourceId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.OrderSourceId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.RequestStateId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.RequestStateId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.OrderPaymentStatusId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.OrderPaymentStatusId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.EditedPriceStatusId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.EditedPriceStatusId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.ShippingName)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.ShippingName)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.OrderDate)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.OrderDate)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryDate)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryDate)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.SalesEmployeeId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.SalesEmployeeId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.Note)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.Note)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceAddress)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceAddress)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceNationId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceNationId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceProvinceId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceProvinceId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceDistrictId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceDistrictId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceWardId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceWardId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.InvoiceZIPCode)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.InvoiceZIPCode)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryAddress)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryAddress)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryNationId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryNationId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryProvinceId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryProvinceId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryDistrictId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryDistrictId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryWardId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryWardId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.DeliveryZIPCode)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.DeliveryZIPCode)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.SubTotal)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.SubTotal)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.GeneralDiscountPercentage)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.GeneralDiscountPercentage)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.GeneralDiscountAmount)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.GeneralDiscountAmount)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.TotalTaxOther)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.TotalTaxOther)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.TotalTax)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.TotalTax)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.Total)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.Total)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.CreatorId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.CreatorId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.OrganizationId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.OrganizationId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.RowId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.RowId)];
                        if (CustomerSalesOrder.Errors.ContainsKey(nameof(CustomerSalesOrder.CodeGeneratorRuleId)))
                            Error += CustomerSalesOrder.Errors[nameof(CustomerSalesOrder.CodeGeneratorRuleId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CustomerSalesOrderRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] CustomerSalesOrder_CustomerSalesOrderFilterDTO CustomerSalesOrder_CustomerSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region CustomerSalesOrder
                var CustomerSalesOrderFilter = ConvertFilterDTOToFilterEntity(CustomerSalesOrder_CustomerSalesOrderFilterDTO);
                CustomerSalesOrderFilter.Skip = 0;
                CustomerSalesOrderFilter.Take = int.MaxValue;
                CustomerSalesOrderFilter = await CustomerSalesOrderService.ToFilter(CustomerSalesOrderFilter);
                List<CustomerSalesOrder> CustomerSalesOrders = await CustomerSalesOrderService.List(CustomerSalesOrderFilter);

                var CustomerSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "CustomerId",
                        "OrderSourceId",
                        "RequestStateId",
                        "OrderPaymentStatusId",
                        "EditedPriceStatusId",
                        "ShippingName",
                        "OrderDate",
                        "DeliveryDate",
                        "SalesEmployeeId",
                        "Note",
                        "InvoiceAddress",
                        "InvoiceNationId",
                        "InvoiceProvinceId",
                        "InvoiceDistrictId",
                        "InvoiceWardId",
                        "InvoiceZIPCode",
                        "DeliveryAddress",
                        "DeliveryNationId",
                        "DeliveryProvinceId",
                        "DeliveryDistrictId",
                        "DeliveryWardId",
                        "DeliveryZIPCode",
                        "SubTotal",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TotalTaxOther",
                        "TotalTax",
                        "Total",
                        "CreatorId",
                        "OrganizationId",
                        "RowId",
                        "CodeGeneratorRuleId",
                    }
                };
                List<object[]> CustomerSalesOrderData = new List<object[]>();
                for (int i = 0; i < CustomerSalesOrders.Count; i++)
                {
                    var CustomerSalesOrder = CustomerSalesOrders[i];
                    CustomerSalesOrderData.Add(new Object[]
                    {
                        CustomerSalesOrder.Id,
                        CustomerSalesOrder.Code,
                        CustomerSalesOrder.CustomerId,
                        CustomerSalesOrder.OrderSourceId,
                        CustomerSalesOrder.RequestStateId,
                        CustomerSalesOrder.OrderPaymentStatusId,
                        CustomerSalesOrder.EditedPriceStatusId,
                        CustomerSalesOrder.ShippingName,
                        CustomerSalesOrder.OrderDate,
                        CustomerSalesOrder.DeliveryDate,
                        CustomerSalesOrder.SalesEmployeeId,
                        CustomerSalesOrder.Note,
                        CustomerSalesOrder.InvoiceAddress,
                        CustomerSalesOrder.InvoiceNationId,
                        CustomerSalesOrder.InvoiceProvinceId,
                        CustomerSalesOrder.InvoiceDistrictId,
                        CustomerSalesOrder.InvoiceWardId,
                        CustomerSalesOrder.InvoiceZIPCode,
                        CustomerSalesOrder.DeliveryAddress,
                        CustomerSalesOrder.DeliveryNationId,
                        CustomerSalesOrder.DeliveryProvinceId,
                        CustomerSalesOrder.DeliveryDistrictId,
                        CustomerSalesOrder.DeliveryWardId,
                        CustomerSalesOrder.DeliveryZIPCode,
                        CustomerSalesOrder.SubTotal,
                        CustomerSalesOrder.GeneralDiscountPercentage,
                        CustomerSalesOrder.GeneralDiscountAmount,
                        CustomerSalesOrder.TotalTaxOther,
                        CustomerSalesOrder.TotalTax,
                        CustomerSalesOrder.Total,
                        CustomerSalesOrder.CreatorId,
                        CustomerSalesOrder.OrganizationId,
                        CustomerSalesOrder.RowId,
                        CustomerSalesOrder.CodeGeneratorRuleId,
                    });
                }
                excel.GenerateWorksheet("CustomerSalesOrder", CustomerSalesOrderHeaders, CustomerSalesOrderData);
                #endregion
                
                #region CodeGeneratorRule
                var CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
                CodeGeneratorRuleFilter.Selects = CodeGeneratorRuleSelect.ALL;
                CodeGeneratorRuleFilter.OrderBy = CodeGeneratorRuleOrder.Id;
                CodeGeneratorRuleFilter.OrderType = OrderType.ASC;
                CodeGeneratorRuleFilter.Skip = 0;
                CodeGeneratorRuleFilter.Take = int.MaxValue;
                List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);

                var CodeGeneratorRuleHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "EntityTypeId",
                        "AutoNumberLenth",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CodeGeneratorRuleData = new List<object[]>();
                for (int i = 0; i < CodeGeneratorRules.Count; i++)
                {
                    var CodeGeneratorRule = CodeGeneratorRules[i];
                    CodeGeneratorRuleData.Add(new Object[]
                    {
                        CodeGeneratorRule.Id,
                        CodeGeneratorRule.EntityTypeId,
                        CodeGeneratorRule.AutoNumberLenth,
                        CodeGeneratorRule.StatusId,
                        CodeGeneratorRule.RowId,
                        CodeGeneratorRule.Used,
                    });
                }
                excel.GenerateWorksheet("CodeGeneratorRule", CodeGeneratorRuleHeaders, CodeGeneratorRuleData);
                #endregion
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "SexId",
                        "Birthday",
                        "Avatar",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "RowId",
                        "Used",
                        "Password",
                        "OtpCode",
                        "OtpExpired",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.SexId,
                        AppUser.Birthday,
                        AppUser.Avatar,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.RowId,
                        AppUser.Used,
                        AppUser.Password,
                        AppUser.OtpCode,
                        AppUser.OtpExpired,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region Customer
                var CustomerFilter = new CustomerFilter();
                CustomerFilter.Selects = CustomerSelect.ALL;
                CustomerFilter.OrderBy = CustomerOrder.Id;
                CustomerFilter.OrderType = OrderType.ASC;
                CustomerFilter.Skip = 0;
                CustomerFilter.Take = int.MaxValue;
                List<Customer> Customers = await CustomerService.List(CustomerFilter);

                var CustomerHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "CodeDraft",
                        "Name",
                        "Phone",
                        "Address",
                        "NationId",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Birthday",
                        "Email",
                        "ProfessionId",
                        "CustomerSourceId",
                        "SexId",
                        "StatusId",
                        "AppUserId",
                        "CreatorId",
                        "OrganizationId",
                        "Used",
                        "RowId",
                        "CodeGeneratorRuleId",
                    }
                };
                List<object[]> CustomerData = new List<object[]>();
                for (int i = 0; i < Customers.Count; i++)
                {
                    var Customer = Customers[i];
                    CustomerData.Add(new Object[]
                    {
                        Customer.Id,
                        Customer.Code,
                        Customer.CodeDraft,
                        Customer.Name,
                        Customer.Phone,
                        Customer.Address,
                        Customer.NationId,
                        Customer.ProvinceId,
                        Customer.DistrictId,
                        Customer.WardId,
                        Customer.Birthday,
                        Customer.Email,
                        Customer.ProfessionId,
                        Customer.CustomerSourceId,
                        Customer.SexId,
                        Customer.StatusId,
                        Customer.AppUserId,
                        Customer.CreatorId,
                        Customer.OrganizationId,
                        Customer.Used,
                        Customer.RowId,
                        Customer.CodeGeneratorRuleId,
                    });
                }
                excel.GenerateWorksheet("Customer", CustomerHeaders, CustomerData);
                #endregion
                #region District
                var DistrictFilter = new DistrictFilter();
                DistrictFilter.Selects = DistrictSelect.ALL;
                DistrictFilter.OrderBy = DistrictOrder.Id;
                DistrictFilter.OrderType = OrderType.ASC;
                DistrictFilter.Skip = 0;
                DistrictFilter.Take = int.MaxValue;
                List<District> Districts = await DistrictService.List(DistrictFilter);

                var DistrictHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "ProvinceId",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> DistrictData = new List<object[]>();
                for (int i = 0; i < Districts.Count; i++)
                {
                    var District = Districts[i];
                    DistrictData.Add(new Object[]
                    {
                        District.Id,
                        District.Code,
                        District.Name,
                        District.Priority,
                        District.ProvinceId,
                        District.StatusId,
                        District.RowId,
                        District.Used,
                    });
                }
                excel.GenerateWorksheet("District", DistrictHeaders, DistrictData);
                #endregion
                #region Nation
                var NationFilter = new NationFilter();
                NationFilter.Selects = NationSelect.ALL;
                NationFilter.OrderBy = NationOrder.Id;
                NationFilter.OrderType = OrderType.ASC;
                NationFilter.Skip = 0;
                NationFilter.Take = int.MaxValue;
                List<Nation> Nations = await NationService.List(NationFilter);

                var NationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> NationData = new List<object[]>();
                for (int i = 0; i < Nations.Count; i++)
                {
                    var Nation = Nations[i];
                    NationData.Add(new Object[]
                    {
                        Nation.Id,
                        Nation.Code,
                        Nation.Name,
                        Nation.Priority,
                        Nation.StatusId,
                        Nation.Used,
                        Nation.RowId,
                    });
                }
                excel.GenerateWorksheet("Nation", NationHeaders, NationData);
                #endregion
                #region Province
                var ProvinceFilter = new ProvinceFilter();
                ProvinceFilter.Selects = ProvinceSelect.ALL;
                ProvinceFilter.OrderBy = ProvinceOrder.Id;
                ProvinceFilter.OrderType = OrderType.ASC;
                ProvinceFilter.Skip = 0;
                ProvinceFilter.Take = int.MaxValue;
                List<Province> Provinces = await ProvinceService.List(ProvinceFilter);

                var ProvinceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> ProvinceData = new List<object[]>();
                for (int i = 0; i < Provinces.Count; i++)
                {
                    var Province = Provinces[i];
                    ProvinceData.Add(new Object[]
                    {
                        Province.Id,
                        Province.Code,
                        Province.Name,
                        Province.Priority,
                        Province.StatusId,
                        Province.RowId,
                        Province.Used,
                    });
                }
                excel.GenerateWorksheet("Province", ProvinceHeaders, ProvinceData);
                #endregion
                #region Ward
                var WardFilter = new WardFilter();
                WardFilter.Selects = WardSelect.ALL;
                WardFilter.OrderBy = WardOrder.Id;
                WardFilter.OrderType = OrderType.ASC;
                WardFilter.Skip = 0;
                WardFilter.Take = int.MaxValue;
                List<Ward> Wards = await WardService.List(WardFilter);

                var WardHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "DistrictId",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> WardData = new List<object[]>();
                for (int i = 0; i < Wards.Count; i++)
                {
                    var Ward = Wards[i];
                    WardData.Add(new Object[]
                    {
                        Ward.Id,
                        Ward.Code,
                        Ward.Name,
                        Ward.Priority,
                        Ward.DistrictId,
                        Ward.StatusId,
                        Ward.RowId,
                        Ward.Used,
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, WardData);
                #endregion
                #region EditedPriceStatus
                var EditedPriceStatusFilter = new EditedPriceStatusFilter();
                EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
                EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
                EditedPriceStatusFilter.OrderType = OrderType.ASC;
                EditedPriceStatusFilter.Skip = 0;
                EditedPriceStatusFilter.Take = int.MaxValue;
                List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);

                var EditedPriceStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EditedPriceStatusData = new List<object[]>();
                for (int i = 0; i < EditedPriceStatuses.Count; i++)
                {
                    var EditedPriceStatus = EditedPriceStatuses[i];
                    EditedPriceStatusData.Add(new Object[]
                    {
                        EditedPriceStatus.Id,
                        EditedPriceStatus.Code,
                        EditedPriceStatus.Name,
                    });
                }
                excel.GenerateWorksheet("EditedPriceStatus", EditedPriceStatusHeaders, EditedPriceStatusData);
                #endregion
                #region OrderPaymentStatus
                var OrderPaymentStatusFilter = new OrderPaymentStatusFilter();
                OrderPaymentStatusFilter.Selects = OrderPaymentStatusSelect.ALL;
                OrderPaymentStatusFilter.OrderBy = OrderPaymentStatusOrder.Id;
                OrderPaymentStatusFilter.OrderType = OrderType.ASC;
                OrderPaymentStatusFilter.Skip = 0;
                OrderPaymentStatusFilter.Take = int.MaxValue;
                List<OrderPaymentStatus> OrderPaymentStatuses = await OrderPaymentStatusService.List(OrderPaymentStatusFilter);

                var OrderPaymentStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> OrderPaymentStatusData = new List<object[]>();
                for (int i = 0; i < OrderPaymentStatuses.Count; i++)
                {
                    var OrderPaymentStatus = OrderPaymentStatuses[i];
                    OrderPaymentStatusData.Add(new Object[]
                    {
                        OrderPaymentStatus.Id,
                        OrderPaymentStatus.Code,
                        OrderPaymentStatus.Name,
                    });
                }
                excel.GenerateWorksheet("OrderPaymentStatus", OrderPaymentStatusHeaders, OrderPaymentStatusData);
                #endregion
                #region OrderSource
                var OrderSourceFilter = new OrderSourceFilter();
                OrderSourceFilter.Selects = OrderSourceSelect.ALL;
                OrderSourceFilter.OrderBy = OrderSourceOrder.Id;
                OrderSourceFilter.OrderType = OrderType.ASC;
                OrderSourceFilter.Skip = 0;
                OrderSourceFilter.Take = int.MaxValue;
                List<OrderSource> OrderSources = await OrderSourceService.List(OrderSourceFilter);

                var OrderSourceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "Description",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> OrderSourceData = new List<object[]>();
                for (int i = 0; i < OrderSources.Count; i++)
                {
                    var OrderSource = OrderSources[i];
                    OrderSourceData.Add(new Object[]
                    {
                        OrderSource.Id,
                        OrderSource.Code,
                        OrderSource.Name,
                        OrderSource.Priority,
                        OrderSource.Description,
                        OrderSource.StatusId,
                        OrderSource.Used,
                        OrderSource.RowId,
                    });
                }
                excel.GenerateWorksheet("OrderSource", OrderSourceHeaders, OrderSourceData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                        "RowId",
                        "Used",
                        "IsDisplay",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                        Organization.RowId,
                        Organization.Used,
                        Organization.IsDisplay,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region RequestState
                var RequestStateFilter = new RequestStateFilter();
                RequestStateFilter.Selects = RequestStateSelect.ALL;
                RequestStateFilter.OrderBy = RequestStateOrder.Id;
                RequestStateFilter.OrderType = OrderType.ASC;
                RequestStateFilter.Skip = 0;
                RequestStateFilter.Take = int.MaxValue;
                List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);

                var RequestStateHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> RequestStateData = new List<object[]>();
                for (int i = 0; i < RequestStates.Count; i++)
                {
                    var RequestState = RequestStates[i];
                    RequestStateData.Add(new Object[]
                    {
                        RequestState.Id,
                        RequestState.Code,
                        RequestState.Name,
                    });
                }
                excel.GenerateWorksheet("RequestState", RequestStateHeaders, RequestStateData);
                #endregion
                #region CustomerSalesOrderContent
                var CustomerSalesOrderContentFilter = new CustomerSalesOrderContentFilter();
                CustomerSalesOrderContentFilter.Selects = CustomerSalesOrderContentSelect.ALL;
                CustomerSalesOrderContentFilter.OrderBy = CustomerSalesOrderContentOrder.Id;
                CustomerSalesOrderContentFilter.OrderType = OrderType.ASC;
                CustomerSalesOrderContentFilter.Skip = 0;
                CustomerSalesOrderContentFilter.Take = int.MaxValue;
                List<CustomerSalesOrderContent> CustomerSalesOrderContents = await CustomerSalesOrderContentService.List(CustomerSalesOrderContentFilter);

                var CustomerSalesOrderContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "CustomerSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "RequestedQuantity",
                        "PrimaryUnitOfMeasureId",
                        "SalePrice",
                        "PrimaryPrice",
                        "DiscountPercentage",
                        "DiscountAmount",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TaxPercentage",
                        "TaxAmount",
                        "TaxPercentageOther",
                        "TaxAmountOther",
                        "Amount",
                        "Factor",
                        "EditedPriceStatusId",
                        "TaxTypeId",
                    }
                };
                List<object[]> CustomerSalesOrderContentData = new List<object[]>();
                for (int i = 0; i < CustomerSalesOrderContents.Count; i++)
                {
                    var CustomerSalesOrderContent = CustomerSalesOrderContents[i];
                    CustomerSalesOrderContentData.Add(new Object[]
                    {
                        CustomerSalesOrderContent.Id,
                        CustomerSalesOrderContent.CustomerSalesOrderId,
                        CustomerSalesOrderContent.ItemId,
                        CustomerSalesOrderContent.UnitOfMeasureId,
                        CustomerSalesOrderContent.Quantity,
                        CustomerSalesOrderContent.RequestedQuantity,
                        CustomerSalesOrderContent.PrimaryUnitOfMeasureId,
                        CustomerSalesOrderContent.SalePrice,
                        CustomerSalesOrderContent.PrimaryPrice,
                        CustomerSalesOrderContent.DiscountPercentage,
                        CustomerSalesOrderContent.DiscountAmount,
                        CustomerSalesOrderContent.GeneralDiscountPercentage,
                        CustomerSalesOrderContent.GeneralDiscountAmount,
                        CustomerSalesOrderContent.TaxPercentage,
                        CustomerSalesOrderContent.TaxAmount,
                        CustomerSalesOrderContent.TaxPercentageOther,
                        CustomerSalesOrderContent.TaxAmountOther,
                        CustomerSalesOrderContent.Amount,
                        CustomerSalesOrderContent.Factor,
                        CustomerSalesOrderContent.EditedPriceStatusId,
                        CustomerSalesOrderContent.TaxTypeId,
                    });
                }
                excel.GenerateWorksheet("CustomerSalesOrderContent", CustomerSalesOrderContentHeaders, CustomerSalesOrderContentData);
                #endregion
                #region UnitOfMeasure
                var UnitOfMeasureFilter = new UnitOfMeasureFilter();
                UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
                UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
                UnitOfMeasureFilter.OrderType = OrderType.ASC;
                UnitOfMeasureFilter.Skip = 0;
                UnitOfMeasureFilter.Take = int.MaxValue;
                List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);

                var UnitOfMeasureHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Description",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> UnitOfMeasureData = new List<object[]>();
                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    var UnitOfMeasure = UnitOfMeasures[i];
                    UnitOfMeasureData.Add(new Object[]
                    {
                        UnitOfMeasure.Id,
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name,
                        UnitOfMeasure.Description,
                        UnitOfMeasure.StatusId,
                        UnitOfMeasure.Used,
                        UnitOfMeasure.RowId,
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UnitOfMeasureHeaders, UnitOfMeasureData);
                #endregion
                #region TaxType
                var TaxTypeFilter = new TaxTypeFilter();
                TaxTypeFilter.Selects = TaxTypeSelect.ALL;
                TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
                TaxTypeFilter.OrderType = OrderType.ASC;
                TaxTypeFilter.Skip = 0;
                TaxTypeFilter.Take = int.MaxValue;
                List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);

                var TaxTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Percentage",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> TaxTypeData = new List<object[]>();
                for (int i = 0; i < TaxTypes.Count; i++)
                {
                    var TaxType = TaxTypes[i];
                    TaxTypeData.Add(new Object[]
                    {
                        TaxType.Id,
                        TaxType.Code,
                        TaxType.Name,
                        TaxType.Percentage,
                        TaxType.StatusId,
                        TaxType.Used,
                        TaxType.RowId,
                    });
                }
                excel.GenerateWorksheet("TaxType", TaxTypeHeaders, TaxTypeData);
                #endregion
                #region CustomerSalesOrderPaymentHistory
                var CustomerSalesOrderPaymentHistoryFilter = new CustomerSalesOrderPaymentHistoryFilter();
                CustomerSalesOrderPaymentHistoryFilter.Selects = CustomerSalesOrderPaymentHistorySelect.ALL;
                CustomerSalesOrderPaymentHistoryFilter.OrderBy = CustomerSalesOrderPaymentHistoryOrder.Id;
                CustomerSalesOrderPaymentHistoryFilter.OrderType = OrderType.ASC;
                CustomerSalesOrderPaymentHistoryFilter.Skip = 0;
                CustomerSalesOrderPaymentHistoryFilter.Take = int.MaxValue;
                List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await CustomerSalesOrderPaymentHistoryService.List(CustomerSalesOrderPaymentHistoryFilter);

                var CustomerSalesOrderPaymentHistoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "CustomerSalesOrderId",
                        "PaymentMilestone",
                        "PaymentPercentage",
                        "PaymentAmount",
                        "PaymentTypeId",
                        "Description",
                        "IsPaid",
                    }
                };
                List<object[]> CustomerSalesOrderPaymentHistoryData = new List<object[]>();
                for (int i = 0; i < CustomerSalesOrderPaymentHistories.Count; i++)
                {
                    var CustomerSalesOrderPaymentHistory = CustomerSalesOrderPaymentHistories[i];
                    CustomerSalesOrderPaymentHistoryData.Add(new Object[]
                    {
                        CustomerSalesOrderPaymentHistory.Id,
                        CustomerSalesOrderPaymentHistory.CustomerSalesOrderId,
                        CustomerSalesOrderPaymentHistory.PaymentMilestone,
                        CustomerSalesOrderPaymentHistory.PaymentPercentage,
                        CustomerSalesOrderPaymentHistory.PaymentAmount,
                        CustomerSalesOrderPaymentHistory.PaymentTypeId,
                        CustomerSalesOrderPaymentHistory.Description,
                        CustomerSalesOrderPaymentHistory.IsPaid,
                    });
                }
                excel.GenerateWorksheet("CustomerSalesOrderPaymentHistory", CustomerSalesOrderPaymentHistoryHeaders, CustomerSalesOrderPaymentHistoryData);
                #endregion
                #region PaymentType
                var PaymentTypeFilter = new PaymentTypeFilter();
                PaymentTypeFilter.Selects = PaymentTypeSelect.ALL;
                PaymentTypeFilter.OrderBy = PaymentTypeOrder.Id;
                PaymentTypeFilter.OrderType = OrderType.ASC;
                PaymentTypeFilter.Skip = 0;
                PaymentTypeFilter.Take = int.MaxValue;
                List<PaymentType> PaymentTypes = await PaymentTypeService.List(PaymentTypeFilter);

                var PaymentTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> PaymentTypeData = new List<object[]>();
                for (int i = 0; i < PaymentTypes.Count; i++)
                {
                    var PaymentType = PaymentTypes[i];
                    PaymentTypeData.Add(new Object[]
                    {
                        PaymentType.Id,
                        PaymentType.Code,
                        PaymentType.Name,
                        PaymentType.StatusId,
                        PaymentType.Used,
                        PaymentType.RowId,
                    });
                }
                excel.GenerateWorksheet("PaymentType", PaymentTypeHeaders, PaymentTypeData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "CustomerSalesOrder.xlsx");
        }

        [Route(CustomerSalesOrderRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] CustomerSalesOrder_CustomerSalesOrderFilterDTO CustomerSalesOrder_CustomerSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/CustomerSalesOrder_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "CustomerSalesOrder.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CustomerSalesOrderFilter CustomerSalesOrderFilter = new CustomerSalesOrderFilter();
            CustomerSalesOrderFilter = await CustomerSalesOrderService.ToFilter(CustomerSalesOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                CustomerSalesOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await CustomerSalesOrderService.Count(CustomerSalesOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private CustomerSalesOrder ConvertDTOToEntity(CustomerSalesOrder_CustomerSalesOrderDTO CustomerSalesOrder_CustomerSalesOrderDTO)
        {
            CustomerSalesOrder CustomerSalesOrder = new CustomerSalesOrder();
            CustomerSalesOrder.Id = CustomerSalesOrder_CustomerSalesOrderDTO.Id;
            CustomerSalesOrder.Code = CustomerSalesOrder_CustomerSalesOrderDTO.Code;
            CustomerSalesOrder.CustomerId = CustomerSalesOrder_CustomerSalesOrderDTO.CustomerId;
            CustomerSalesOrder.OrderSourceId = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSourceId;
            CustomerSalesOrder.RequestStateId = CustomerSalesOrder_CustomerSalesOrderDTO.RequestStateId;
            CustomerSalesOrder.OrderPaymentStatusId = CustomerSalesOrder_CustomerSalesOrderDTO.OrderPaymentStatusId;
            CustomerSalesOrder.EditedPriceStatusId = CustomerSalesOrder_CustomerSalesOrderDTO.EditedPriceStatusId;
            CustomerSalesOrder.ShippingName = CustomerSalesOrder_CustomerSalesOrderDTO.ShippingName;
            CustomerSalesOrder.OrderDate = CustomerSalesOrder_CustomerSalesOrderDTO.OrderDate;
            CustomerSalesOrder.DeliveryDate = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDate;
            CustomerSalesOrder.SalesEmployeeId = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployeeId;
            CustomerSalesOrder.Note = CustomerSalesOrder_CustomerSalesOrderDTO.Note;
            CustomerSalesOrder.InvoiceAddress = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceAddress;
            CustomerSalesOrder.InvoiceNationId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNationId;
            CustomerSalesOrder.InvoiceProvinceId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvinceId;
            CustomerSalesOrder.InvoiceDistrictId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrictId;
            CustomerSalesOrder.InvoiceWardId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWardId;
            CustomerSalesOrder.InvoiceZIPCode = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceZIPCode;
            CustomerSalesOrder.DeliveryAddress = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryAddress;
            CustomerSalesOrder.DeliveryNationId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNationId;
            CustomerSalesOrder.DeliveryProvinceId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvinceId;
            CustomerSalesOrder.DeliveryDistrictId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrictId;
            CustomerSalesOrder.DeliveryWardId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWardId;
            CustomerSalesOrder.DeliveryZIPCode = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryZIPCode;
            CustomerSalesOrder.SubTotal = CustomerSalesOrder_CustomerSalesOrderDTO.SubTotal;
            CustomerSalesOrder.GeneralDiscountPercentage = CustomerSalesOrder_CustomerSalesOrderDTO.GeneralDiscountPercentage;
            CustomerSalesOrder.GeneralDiscountAmount = CustomerSalesOrder_CustomerSalesOrderDTO.GeneralDiscountAmount;
            CustomerSalesOrder.TotalTaxOther = CustomerSalesOrder_CustomerSalesOrderDTO.TotalTaxOther;
            CustomerSalesOrder.TotalTax = CustomerSalesOrder_CustomerSalesOrderDTO.TotalTax;
            CustomerSalesOrder.Total = CustomerSalesOrder_CustomerSalesOrderDTO.Total;
            CustomerSalesOrder.CreatorId = CustomerSalesOrder_CustomerSalesOrderDTO.CreatorId;
            CustomerSalesOrder.OrganizationId = CustomerSalesOrder_CustomerSalesOrderDTO.OrganizationId;
            CustomerSalesOrder.RowId = CustomerSalesOrder_CustomerSalesOrderDTO.RowId;
            CustomerSalesOrder.CodeGeneratorRuleId = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRuleId;
            CustomerSalesOrder.CodeGeneratorRule = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule == null ? null : new CodeGeneratorRule
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.Id,
                EntityTypeId = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.EntityTypeId,
                AutoNumberLenth = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.AutoNumberLenth,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.CodeGeneratorRule.Used,
            };
            CustomerSalesOrder.Creator = CustomerSalesOrder_CustomerSalesOrderDTO.Creator == null ? null : new AppUser
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Id,
                Username = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Username,
                DisplayName = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.DisplayName,
                Address = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Address,
                Email = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Email,
                Phone = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Phone,
                SexId = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.SexId,
                Birthday = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Birthday,
                Avatar = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Avatar,
                Department = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Department,
                OrganizationId = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.OrganizationId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Used,
                Password = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.Password,
                OtpCode = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.OtpCode,
                OtpExpired = CustomerSalesOrder_CustomerSalesOrderDTO.Creator.OtpExpired,
            };
            CustomerSalesOrder.Customer = CustomerSalesOrder_CustomerSalesOrderDTO.Customer == null ? null : new Customer
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Code,
                CodeDraft = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.CodeDraft,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Name,
                Phone = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Phone,
                Address = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Address,
                NationId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.NationId,
                ProvinceId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.ProvinceId,
                DistrictId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.DistrictId,
                WardId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.WardId,
                Birthday = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Birthday,
                Email = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Email,
                ProfessionId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.ProfessionId,
                CustomerSourceId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.CustomerSourceId,
                SexId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.SexId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.StatusId,
                AppUserId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.AppUserId,
                CreatorId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.CreatorId,
                OrganizationId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.OrganizationId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.Used,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.RowId,
                CodeGeneratorRuleId = CustomerSalesOrder_CustomerSalesOrderDTO.Customer.CodeGeneratorRuleId,
            };
            CustomerSalesOrder.DeliveryDistrict = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict == null ? null : new District
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.Priority,
                ProvinceId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.ProvinceId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryDistrict.Used,
            };
            CustomerSalesOrder.DeliveryNation = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation == null ? null : new Nation
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.Priority,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.StatusId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.Used,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryNation.RowId,
            };
            CustomerSalesOrder.DeliveryProvince = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince == null ? null : new Province
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.Priority,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryProvince.Used,
            };
            CustomerSalesOrder.DeliveryWard = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard == null ? null : new Ward
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.Priority,
                DistrictId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.DistrictId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.DeliveryWard.Used,
            };
            CustomerSalesOrder.EditedPriceStatus = CustomerSalesOrder_CustomerSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.EditedPriceStatus.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.EditedPriceStatus.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.EditedPriceStatus.Name,
            };
            CustomerSalesOrder.InvoiceDistrict = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict == null ? null : new District
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.Priority,
                ProvinceId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.ProvinceId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceDistrict.Used,
            };
            CustomerSalesOrder.InvoiceNation = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation == null ? null : new Nation
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.Priority,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.StatusId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.Used,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceNation.RowId,
            };
            CustomerSalesOrder.InvoiceProvince = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince == null ? null : new Province
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.Priority,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceProvince.Used,
            };
            CustomerSalesOrder.InvoiceWard = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard == null ? null : new Ward
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.Priority,
                DistrictId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.DistrictId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.InvoiceWard.Used,
            };
            CustomerSalesOrder.OrderPaymentStatus = CustomerSalesOrder_CustomerSalesOrderDTO.OrderPaymentStatus == null ? null : new OrderPaymentStatus
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.OrderPaymentStatus.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.OrderPaymentStatus.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.OrderPaymentStatus.Name,
            };
            CustomerSalesOrder.OrderSource = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource == null ? null : new OrderSource
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Name,
                Priority = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Priority,
                Description = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Description,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.StatusId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.Used,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.OrderSource.RowId,
            };
            CustomerSalesOrder.Organization = CustomerSalesOrder_CustomerSalesOrderDTO.Organization == null ? null : new Organization
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Name,
                ParentId = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.ParentId,
                Path = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Path,
                Level = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Level,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.StatusId,
                Phone = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Phone,
                Email = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Email,
                Address = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Address,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.Used,
                IsDisplay = CustomerSalesOrder_CustomerSalesOrderDTO.Organization.IsDisplay,
            };
            CustomerSalesOrder.RequestState = CustomerSalesOrder_CustomerSalesOrderDTO.RequestState == null ? null : new RequestState
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.RequestState.Id,
                Code = CustomerSalesOrder_CustomerSalesOrderDTO.RequestState.Code,
                Name = CustomerSalesOrder_CustomerSalesOrderDTO.RequestState.Name,
            };
            CustomerSalesOrder.SalesEmployee = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee == null ? null : new AppUser
            {
                Id = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Id,
                Username = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Username,
                DisplayName = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.DisplayName,
                Address = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Address,
                Email = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Email,
                Phone = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Phone,
                SexId = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.SexId,
                Birthday = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Birthday,
                Avatar = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Avatar,
                Department = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Department,
                OrganizationId = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.OrganizationId,
                StatusId = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.StatusId,
                RowId = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.RowId,
                Used = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Used,
                Password = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.Password,
                OtpCode = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.OtpCode,
                OtpExpired = CustomerSalesOrder_CustomerSalesOrderDTO.SalesEmployee.OtpExpired,
            };
            CustomerSalesOrder.CustomerSalesOrderContents = CustomerSalesOrder_CustomerSalesOrderDTO.CustomerSalesOrderContents?
                .Select(x => new CustomerSalesOrderContent
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    RequestedQuantity = x.RequestedQuantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    SalePrice = x.SalePrice,
                    PrimaryPrice = x.PrimaryPrice,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    TaxPercentageOther = x.TaxPercentageOther,
                    TaxAmountOther = x.TaxAmountOther,
                    Amount = x.Amount,
                    Factor = x.Factor,
                    EditedPriceStatusId = x.EditedPriceStatusId,
                    TaxTypeId = x.TaxTypeId,
                    EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                    {
                        Id = x.EditedPriceStatus.Id,
                        Code = x.EditedPriceStatus.Code,
                        Name = x.EditedPriceStatus.Name,
                    },
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                        Used = x.PrimaryUnitOfMeasure.Used,
                        RowId = x.PrimaryUnitOfMeasure.RowId,
                    },
                    TaxType = x.TaxType == null ? null : new TaxType
                    {
                        Id = x.TaxType.Id,
                        Code = x.TaxType.Code,
                        Name = x.TaxType.Name,
                        Percentage = x.TaxType.Percentage,
                        StatusId = x.TaxType.StatusId,
                        Used = x.TaxType.Used,
                        RowId = x.TaxType.RowId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Used = x.UnitOfMeasure.Used,
                        RowId = x.UnitOfMeasure.RowId,
                    },
                }).ToList();
            CustomerSalesOrder.CustomerSalesOrderPaymentHistories = CustomerSalesOrder_CustomerSalesOrderDTO.CustomerSalesOrderPaymentHistories?
                .Select(x => new CustomerSalesOrderPaymentHistory
                {
                    Id = x.Id,
                    PaymentMilestone = x.PaymentMilestone,
                    PaymentPercentage = x.PaymentPercentage,
                    PaymentAmount = x.PaymentAmount,
                    PaymentTypeId = x.PaymentTypeId,
                    Description = x.Description,
                    IsPaid = x.IsPaid,
                    PaymentType = x.PaymentType == null ? null : new PaymentType
                    {
                        Id = x.PaymentType.Id,
                        Code = x.PaymentType.Code,
                        Name = x.PaymentType.Name,
                        StatusId = x.PaymentType.StatusId,
                        Used = x.PaymentType.Used,
                        RowId = x.PaymentType.RowId,
                    },
                }).ToList();
            CustomerSalesOrder.BaseLanguage = CurrentContext.Language;
            return CustomerSalesOrder;
        }

        private CustomerSalesOrderFilter ConvertFilterDTOToFilterEntity(CustomerSalesOrder_CustomerSalesOrderFilterDTO CustomerSalesOrder_CustomerSalesOrderFilterDTO)
        {
            CustomerSalesOrderFilter CustomerSalesOrderFilter = new CustomerSalesOrderFilter();
            CustomerSalesOrderFilter.Selects = CustomerSalesOrderSelect.ALL;
            CustomerSalesOrderFilter.Skip = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Skip;
            CustomerSalesOrderFilter.Take = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Take;
            CustomerSalesOrderFilter.OrderBy = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrderBy;
            CustomerSalesOrderFilter.OrderType = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrderType;

            CustomerSalesOrderFilter.Id = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Id;
            CustomerSalesOrderFilter.Code = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Code;
            CustomerSalesOrderFilter.CustomerId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.CustomerId;
            CustomerSalesOrderFilter.OrderSourceId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrderSourceId;
            CustomerSalesOrderFilter.RequestStateId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.RequestStateId;
            CustomerSalesOrderFilter.OrderPaymentStatusId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrderPaymentStatusId;
            CustomerSalesOrderFilter.EditedPriceStatusId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.EditedPriceStatusId;
            CustomerSalesOrderFilter.ShippingName = CustomerSalesOrder_CustomerSalesOrderFilterDTO.ShippingName;
            CustomerSalesOrderFilter.OrderDate = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrderDate;
            CustomerSalesOrderFilter.DeliveryDate = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryDate;
            CustomerSalesOrderFilter.SalesEmployeeId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.SalesEmployeeId;
            CustomerSalesOrderFilter.Note = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Note;
            CustomerSalesOrderFilter.InvoiceAddress = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceAddress;
            CustomerSalesOrderFilter.InvoiceNationId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceNationId;
            CustomerSalesOrderFilter.InvoiceProvinceId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceProvinceId;
            CustomerSalesOrderFilter.InvoiceDistrictId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceDistrictId;
            CustomerSalesOrderFilter.InvoiceWardId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceWardId;
            CustomerSalesOrderFilter.InvoiceZIPCode = CustomerSalesOrder_CustomerSalesOrderFilterDTO.InvoiceZIPCode;
            CustomerSalesOrderFilter.DeliveryAddress = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryAddress;
            CustomerSalesOrderFilter.DeliveryNationId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryNationId;
            CustomerSalesOrderFilter.DeliveryProvinceId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryProvinceId;
            CustomerSalesOrderFilter.DeliveryDistrictId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryDistrictId;
            CustomerSalesOrderFilter.DeliveryWardId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryWardId;
            CustomerSalesOrderFilter.DeliveryZIPCode = CustomerSalesOrder_CustomerSalesOrderFilterDTO.DeliveryZIPCode;
            CustomerSalesOrderFilter.SubTotal = CustomerSalesOrder_CustomerSalesOrderFilterDTO.SubTotal;
            CustomerSalesOrderFilter.GeneralDiscountPercentage = CustomerSalesOrder_CustomerSalesOrderFilterDTO.GeneralDiscountPercentage;
            CustomerSalesOrderFilter.GeneralDiscountAmount = CustomerSalesOrder_CustomerSalesOrderFilterDTO.GeneralDiscountAmount;
            CustomerSalesOrderFilter.TotalTaxOther = CustomerSalesOrder_CustomerSalesOrderFilterDTO.TotalTaxOther;
            CustomerSalesOrderFilter.TotalTax = CustomerSalesOrder_CustomerSalesOrderFilterDTO.TotalTax;
            CustomerSalesOrderFilter.Total = CustomerSalesOrder_CustomerSalesOrderFilterDTO.Total;
            CustomerSalesOrderFilter.CreatorId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.CreatorId;
            CustomerSalesOrderFilter.OrganizationId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.OrganizationId;
            CustomerSalesOrderFilter.RowId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.RowId;
            CustomerSalesOrderFilter.CodeGeneratorRuleId = CustomerSalesOrder_CustomerSalesOrderFilterDTO.CodeGeneratorRuleId;
            CustomerSalesOrderFilter.CreatedAt = CustomerSalesOrder_CustomerSalesOrderFilterDTO.CreatedAt;
            CustomerSalesOrderFilter.UpdatedAt = CustomerSalesOrder_CustomerSalesOrderFilterDTO.UpdatedAt;
            return CustomerSalesOrderFilter;
        }
    }
}

