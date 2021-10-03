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
        private IAppUserService AppUserService;
        private ICodeGeneratorRuleService CodeGeneratorRuleService;
        private ICustomerSourceService CustomerSourceService;
        private IDistrictService DistrictService;
        private INationService NationService;
        private IOrganizationService OrganizationService;
        private IProfessionService ProfessionService;
        private IProvinceService ProvinceService;
        private IWardService WardService;
        private ICustomerService CustomerService;
        private ICurrentContext CurrentContext;
        public CustomerController(
            IAppUserService AppUserService,
            ICodeGeneratorRuleService CodeGeneratorRuleService,
            ICustomerSourceService CustomerSourceService,
            IDistrictService DistrictService,
            INationService NationService,
            IOrganizationService OrganizationService,
            IProfessionService ProfessionService,
            IProvinceService ProvinceService,
            IWardService WardService,
            ICustomerService CustomerService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.CodeGeneratorRuleService = CodeGeneratorRuleService;
            this.CustomerSourceService = CustomerSourceService;
            this.DistrictService = DistrictService;
            this.NationService = NationService;
            this.OrganizationService = OrganizationService;
            this.ProfessionService = ProfessionService;
            this.ProvinceService = ProvinceService;
            this.WardService = WardService;
            this.CustomerService = CustomerService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CustomerRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Customer_CustomerFilterDTO Customer_CustomerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerFilter CustomerFilter = ConvertFilterDTOToFilterEntity(Customer_CustomerFilterDTO);
            CustomerFilter = await CustomerService.ToFilter(CustomerFilter);
            int count = await CustomerService.Count(CustomerFilter);
            return count;
        }

        [Route(CustomerRoute.List), HttpPost]
        public async Task<ActionResult<List<Customer_CustomerDTO>>> List([FromBody] Customer_CustomerFilterDTO Customer_CustomerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerFilter CustomerFilter = ConvertFilterDTOToFilterEntity(Customer_CustomerFilterDTO);
            CustomerFilter = await CustomerService.ToFilter(CustomerFilter);
            List<Customer> Customers = await CustomerService.List(CustomerFilter);
            List<Customer_CustomerDTO> Customer_CustomerDTOs = Customers
                .Select(c => new Customer_CustomerDTO(c)).ToList();
            return Customer_CustomerDTOs;
        }

        [Route(CustomerRoute.Get), HttpPost]
        public async Task<ActionResult<Customer_CustomerDTO>> Get([FromBody]Customer_CustomerDTO Customer_CustomerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Customer_CustomerDTO.Id))
                return Forbid();

            Customer Customer = await CustomerService.Get(Customer_CustomerDTO.Id);
            return new Customer_CustomerDTO(Customer);
        }

        [Route(CustomerRoute.Create), HttpPost]
        public async Task<ActionResult<Customer_CustomerDTO>> Create([FromBody] Customer_CustomerDTO Customer_CustomerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Customer_CustomerDTO.Id))
                return Forbid();

            Customer Customer = ConvertDTOToEntity(Customer_CustomerDTO);
            Customer = await CustomerService.Create(Customer);
            Customer_CustomerDTO = new Customer_CustomerDTO(Customer);
            if (Customer.IsValidated)
                return Customer_CustomerDTO;
            else
                return BadRequest(Customer_CustomerDTO);
        }

        [Route(CustomerRoute.Update), HttpPost]
        public async Task<ActionResult<Customer_CustomerDTO>> Update([FromBody] Customer_CustomerDTO Customer_CustomerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Customer_CustomerDTO.Id))
                return Forbid();

            Customer Customer = ConvertDTOToEntity(Customer_CustomerDTO);
            Customer = await CustomerService.Update(Customer);
            Customer_CustomerDTO = new Customer_CustomerDTO(Customer);
            if (Customer.IsValidated)
                return Customer_CustomerDTO;
            else
                return BadRequest(Customer_CustomerDTO);
        }

        [Route(CustomerRoute.Delete), HttpPost]
        public async Task<ActionResult<Customer_CustomerDTO>> Delete([FromBody] Customer_CustomerDTO Customer_CustomerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Customer_CustomerDTO.Id))
                return Forbid();

            Customer Customer = ConvertDTOToEntity(Customer_CustomerDTO);
            Customer = await CustomerService.Delete(Customer);
            Customer_CustomerDTO = new Customer_CustomerDTO(Customer);
            if (Customer.IsValidated)
                return Customer_CustomerDTO;
            else
                return BadRequest(Customer_CustomerDTO);
        }
        
        [Route(CustomerRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CustomerFilter CustomerFilter = new CustomerFilter();
            CustomerFilter = await CustomerService.ToFilter(CustomerFilter);
            CustomerFilter.Id = new IdFilter { In = Ids };
            CustomerFilter.Selects = CustomerSelect.Id;
            CustomerFilter.Skip = 0;
            CustomerFilter.Take = int.MaxValue;

            List<Customer> Customers = await CustomerService.List(CustomerFilter);
            Customers = await CustomerService.BulkDelete(Customers);
            if (Customers.Any(x => !x.IsValidated))
                return BadRequest(Customers.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(CustomerRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
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
            CustomerSourceFilter CustomerSourceFilter = new CustomerSourceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CustomerSourceSelect.ALL
            };
            List<CustomerSource> CustomerSources = await CustomerSourceService.List(CustomerSourceFilter);
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.ALL
            };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            NationFilter NationFilter = new NationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NationSelect.ALL
            };
            List<Nation> Nations = await NationService.List(NationFilter);
            ProfessionFilter ProfessionFilter = new ProfessionFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProfessionSelect.ALL
            };
            List<Profession> Professions = await ProfessionService.List(ProfessionFilter);
            ProvinceFilter ProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.ALL
            };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.ALL
            };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Customer> Customers = new List<Customer>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Customers);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int CodeDraftColumn = 2 + StartColumn;
                int NameColumn = 3 + StartColumn;
                int PhoneColumn = 4 + StartColumn;
                int AddressColumn = 5 + StartColumn;
                int NationIdColumn = 6 + StartColumn;
                int ProvinceIdColumn = 7 + StartColumn;
                int DistrictIdColumn = 8 + StartColumn;
                int WardIdColumn = 9 + StartColumn;
                int BirthdayColumn = 10 + StartColumn;
                int EmailColumn = 11 + StartColumn;
                int ProfessionIdColumn = 12 + StartColumn;
                int CustomerSourceIdColumn = 13 + StartColumn;
                int SexIdColumn = 14 + StartColumn;
                int StatusIdColumn = 15 + StartColumn;
                int AppUserIdColumn = 16 + StartColumn;
                int CreatorIdColumn = 17 + StartColumn;
                int OrganizationIdColumn = 18 + StartColumn;
                int UsedColumn = 22 + StartColumn;
                int RowIdColumn = 23 + StartColumn;
                int CodeGeneratorRuleIdColumn = 24 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string CodeDraftValue = worksheet.Cells[i + StartRow, CodeDraftColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string NationIdValue = worksheet.Cells[i + StartRow, NationIdColumn].Value?.ToString();
                    string ProvinceIdValue = worksheet.Cells[i + StartRow, ProvinceIdColumn].Value?.ToString();
                    string DistrictIdValue = worksheet.Cells[i + StartRow, DistrictIdColumn].Value?.ToString();
                    string WardIdValue = worksheet.Cells[i + StartRow, WardIdColumn].Value?.ToString();
                    string BirthdayValue = worksheet.Cells[i + StartRow, BirthdayColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string ProfessionIdValue = worksheet.Cells[i + StartRow, ProfessionIdColumn].Value?.ToString();
                    string CustomerSourceIdValue = worksheet.Cells[i + StartRow, CustomerSourceIdColumn].Value?.ToString();
                    string SexIdValue = worksheet.Cells[i + StartRow, SexIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string AppUserIdValue = worksheet.Cells[i + StartRow, AppUserIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    string CodeGeneratorRuleIdValue = worksheet.Cells[i + StartRow, CodeGeneratorRuleIdColumn].Value?.ToString();
                    
                    Customer Customer = new Customer();
                    Customer.Code = CodeValue;
                    Customer.CodeDraft = CodeDraftValue;
                    Customer.Name = NameValue;
                    Customer.Phone = PhoneValue;
                    Customer.Address = AddressValue;
                    Customer.Birthday = DateTime.TryParse(BirthdayValue, out DateTime Birthday) ? Birthday : DateTime.Now;
                    Customer.Email = EmailValue;
                    AppUser AppUser = AppUsers.Where(x => x.Id.ToString() == AppUserIdValue).FirstOrDefault();
                    Customer.AppUserId = AppUser == null ? 0 : AppUser.Id;
                    Customer.AppUser = AppUser;
                    CodeGeneratorRule CodeGeneratorRule = CodeGeneratorRules.Where(x => x.Id.ToString() == CodeGeneratorRuleIdValue).FirstOrDefault();
                    Customer.CodeGeneratorRuleId = CodeGeneratorRule == null ? 0 : CodeGeneratorRule.Id;
                    Customer.CodeGeneratorRule = CodeGeneratorRule;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    Customer.CreatorId = Creator == null ? 0 : Creator.Id;
                    Customer.Creator = Creator;
                    CustomerSource CustomerSource = CustomerSources.Where(x => x.Id.ToString() == CustomerSourceIdValue).FirstOrDefault();
                    Customer.CustomerSourceId = CustomerSource == null ? 0 : CustomerSource.Id;
                    Customer.CustomerSource = CustomerSource;
                    District District = Districts.Where(x => x.Id.ToString() == DistrictIdValue).FirstOrDefault();
                    Customer.DistrictId = District == null ? 0 : District.Id;
                    Customer.District = District;
                    Nation Nation = Nations.Where(x => x.Id.ToString() == NationIdValue).FirstOrDefault();
                    Customer.NationId = Nation == null ? 0 : Nation.Id;
                    Customer.Nation = Nation;
                    Profession Profession = Professions.Where(x => x.Id.ToString() == ProfessionIdValue).FirstOrDefault();
                    Customer.ProfessionId = Profession == null ? 0 : Profession.Id;
                    Customer.Profession = Profession;
                    Province Province = Provinces.Where(x => x.Id.ToString() == ProvinceIdValue).FirstOrDefault();
                    Customer.ProvinceId = Province == null ? 0 : Province.Id;
                    Customer.Province = Province;
                    Ward Ward = Wards.Where(x => x.Id.ToString() == WardIdValue).FirstOrDefault();
                    Customer.WardId = Ward == null ? 0 : Ward.Id;
                    Customer.Ward = Ward;
                    
                    Customers.Add(Customer);
                }
            }
            Customers = await CustomerService.Import(Customers);
            if (Customers.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Customers.Count; i++)
                {
                    Customer Customer = Customers[i];
                    if (!Customer.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Customer.Errors.ContainsKey(nameof(Customer.Id)))
                            Error += Customer.Errors[nameof(Customer.Id)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Code)))
                            Error += Customer.Errors[nameof(Customer.Code)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.CodeDraft)))
                            Error += Customer.Errors[nameof(Customer.CodeDraft)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Name)))
                            Error += Customer.Errors[nameof(Customer.Name)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Phone)))
                            Error += Customer.Errors[nameof(Customer.Phone)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Address)))
                            Error += Customer.Errors[nameof(Customer.Address)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.NationId)))
                            Error += Customer.Errors[nameof(Customer.NationId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.ProvinceId)))
                            Error += Customer.Errors[nameof(Customer.ProvinceId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.DistrictId)))
                            Error += Customer.Errors[nameof(Customer.DistrictId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.WardId)))
                            Error += Customer.Errors[nameof(Customer.WardId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Birthday)))
                            Error += Customer.Errors[nameof(Customer.Birthday)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Email)))
                            Error += Customer.Errors[nameof(Customer.Email)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.ProfessionId)))
                            Error += Customer.Errors[nameof(Customer.ProfessionId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.CustomerSourceId)))
                            Error += Customer.Errors[nameof(Customer.CustomerSourceId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.SexId)))
                            Error += Customer.Errors[nameof(Customer.SexId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.StatusId)))
                            Error += Customer.Errors[nameof(Customer.StatusId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.AppUserId)))
                            Error += Customer.Errors[nameof(Customer.AppUserId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.CreatorId)))
                            Error += Customer.Errors[nameof(Customer.CreatorId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.OrganizationId)))
                            Error += Customer.Errors[nameof(Customer.OrganizationId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.Used)))
                            Error += Customer.Errors[nameof(Customer.Used)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.RowId)))
                            Error += Customer.Errors[nameof(Customer.RowId)];
                        if (Customer.Errors.ContainsKey(nameof(Customer.CodeGeneratorRuleId)))
                            Error += Customer.Errors[nameof(Customer.CodeGeneratorRuleId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CustomerRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Customer_CustomerFilterDTO Customer_CustomerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Customer
                var CustomerFilter = ConvertFilterDTOToFilterEntity(Customer_CustomerFilterDTO);
                CustomerFilter.Skip = 0;
                CustomerFilter.Take = int.MaxValue;
                CustomerFilter = await CustomerService.ToFilter(CustomerFilter);
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
                #region CustomerSource
                var CustomerSourceFilter = new CustomerSourceFilter();
                CustomerSourceFilter.Selects = CustomerSourceSelect.ALL;
                CustomerSourceFilter.OrderBy = CustomerSourceOrder.Id;
                CustomerSourceFilter.OrderType = OrderType.ASC;
                CustomerSourceFilter.Skip = 0;
                CustomerSourceFilter.Take = int.MaxValue;
                List<CustomerSource> CustomerSources = await CustomerSourceService.List(CustomerSourceFilter);

                var CustomerSourceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "Description",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> CustomerSourceData = new List<object[]>();
                for (int i = 0; i < CustomerSources.Count; i++)
                {
                    var CustomerSource = CustomerSources[i];
                    CustomerSourceData.Add(new Object[]
                    {
                        CustomerSource.Id,
                        CustomerSource.Code,
                        CustomerSource.Name,
                        CustomerSource.StatusId,
                        CustomerSource.Description,
                        CustomerSource.Used,
                        CustomerSource.RowId,
                    });
                }
                excel.GenerateWorksheet("CustomerSource", CustomerSourceHeaders, CustomerSourceData);
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
                #region Profession
                var ProfessionFilter = new ProfessionFilter();
                ProfessionFilter.Selects = ProfessionSelect.ALL;
                ProfessionFilter.OrderBy = ProfessionOrder.Id;
                ProfessionFilter.OrderType = OrderType.ASC;
                ProfessionFilter.Skip = 0;
                ProfessionFilter.Take = int.MaxValue;
                List<Profession> Professions = await ProfessionService.List(ProfessionFilter);

                var ProfessionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> ProfessionData = new List<object[]>();
                for (int i = 0; i < Professions.Count; i++)
                {
                    var Profession = Professions[i];
                    ProfessionData.Add(new Object[]
                    {
                        Profession.Id,
                        Profession.Code,
                        Profession.Name,
                        Profession.StatusId,
                        Profession.RowId,
                        Profession.Used,
                    });
                }
                excel.GenerateWorksheet("Profession", ProfessionHeaders, ProfessionData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Customer.xlsx");
        }

        [Route(CustomerRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Customer_CustomerFilterDTO Customer_CustomerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Customer_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Customer.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CustomerFilter CustomerFilter = new CustomerFilter();
            CustomerFilter = await CustomerService.ToFilter(CustomerFilter);
            if (Id == 0)
            {

            }
            else
            {
                CustomerFilter.Id = new IdFilter { Equal = Id };
                int count = await CustomerService.Count(CustomerFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Customer ConvertDTOToEntity(Customer_CustomerDTO Customer_CustomerDTO)
        {
            Customer Customer = new Customer();
            Customer.Id = Customer_CustomerDTO.Id;
            Customer.Code = Customer_CustomerDTO.Code;
            Customer.CodeDraft = Customer_CustomerDTO.CodeDraft;
            Customer.Name = Customer_CustomerDTO.Name;
            Customer.Phone = Customer_CustomerDTO.Phone;
            Customer.Address = Customer_CustomerDTO.Address;
            Customer.NationId = Customer_CustomerDTO.NationId;
            Customer.ProvinceId = Customer_CustomerDTO.ProvinceId;
            Customer.DistrictId = Customer_CustomerDTO.DistrictId;
            Customer.WardId = Customer_CustomerDTO.WardId;
            Customer.Birthday = Customer_CustomerDTO.Birthday;
            Customer.Email = Customer_CustomerDTO.Email;
            Customer.ProfessionId = Customer_CustomerDTO.ProfessionId;
            Customer.CustomerSourceId = Customer_CustomerDTO.CustomerSourceId;
            Customer.SexId = Customer_CustomerDTO.SexId;
            Customer.StatusId = Customer_CustomerDTO.StatusId;
            Customer.AppUserId = Customer_CustomerDTO.AppUserId;
            Customer.CreatorId = Customer_CustomerDTO.CreatorId;
            Customer.OrganizationId = Customer_CustomerDTO.OrganizationId;
            Customer.Used = Customer_CustomerDTO.Used;
            Customer.RowId = Customer_CustomerDTO.RowId;
            Customer.CodeGeneratorRuleId = Customer_CustomerDTO.CodeGeneratorRuleId;
            Customer.AppUser = Customer_CustomerDTO.AppUser == null ? null : new AppUser
            {
                Id = Customer_CustomerDTO.AppUser.Id,
                Username = Customer_CustomerDTO.AppUser.Username,
                DisplayName = Customer_CustomerDTO.AppUser.DisplayName,
                Address = Customer_CustomerDTO.AppUser.Address,
                Email = Customer_CustomerDTO.AppUser.Email,
                Phone = Customer_CustomerDTO.AppUser.Phone,
                SexId = Customer_CustomerDTO.AppUser.SexId,
                Birthday = Customer_CustomerDTO.AppUser.Birthday,
                Avatar = Customer_CustomerDTO.AppUser.Avatar,
                Department = Customer_CustomerDTO.AppUser.Department,
                OrganizationId = Customer_CustomerDTO.AppUser.OrganizationId,
                StatusId = Customer_CustomerDTO.AppUser.StatusId,
                RowId = Customer_CustomerDTO.AppUser.RowId,
                Used = Customer_CustomerDTO.AppUser.Used,
                Password = Customer_CustomerDTO.AppUser.Password,
                OtpCode = Customer_CustomerDTO.AppUser.OtpCode,
                OtpExpired = Customer_CustomerDTO.AppUser.OtpExpired,
            };
            Customer.CodeGeneratorRule = Customer_CustomerDTO.CodeGeneratorRule == null ? null : new CodeGeneratorRule
            {
                Id = Customer_CustomerDTO.CodeGeneratorRule.Id,
                EntityTypeId = Customer_CustomerDTO.CodeGeneratorRule.EntityTypeId,
                AutoNumberLenth = Customer_CustomerDTO.CodeGeneratorRule.AutoNumberLenth,
                StatusId = Customer_CustomerDTO.CodeGeneratorRule.StatusId,
                RowId = Customer_CustomerDTO.CodeGeneratorRule.RowId,
                Used = Customer_CustomerDTO.CodeGeneratorRule.Used,
            };
            Customer.Creator = Customer_CustomerDTO.Creator == null ? null : new AppUser
            {
                Id = Customer_CustomerDTO.Creator.Id,
                Username = Customer_CustomerDTO.Creator.Username,
                DisplayName = Customer_CustomerDTO.Creator.DisplayName,
                Address = Customer_CustomerDTO.Creator.Address,
                Email = Customer_CustomerDTO.Creator.Email,
                Phone = Customer_CustomerDTO.Creator.Phone,
                SexId = Customer_CustomerDTO.Creator.SexId,
                Birthday = Customer_CustomerDTO.Creator.Birthday,
                Avatar = Customer_CustomerDTO.Creator.Avatar,
                Department = Customer_CustomerDTO.Creator.Department,
                OrganizationId = Customer_CustomerDTO.Creator.OrganizationId,
                StatusId = Customer_CustomerDTO.Creator.StatusId,
                RowId = Customer_CustomerDTO.Creator.RowId,
                Used = Customer_CustomerDTO.Creator.Used,
                Password = Customer_CustomerDTO.Creator.Password,
                OtpCode = Customer_CustomerDTO.Creator.OtpCode,
                OtpExpired = Customer_CustomerDTO.Creator.OtpExpired,
            };
            Customer.CustomerSource = Customer_CustomerDTO.CustomerSource == null ? null : new CustomerSource
            {
                Id = Customer_CustomerDTO.CustomerSource.Id,
                Code = Customer_CustomerDTO.CustomerSource.Code,
                Name = Customer_CustomerDTO.CustomerSource.Name,
                StatusId = Customer_CustomerDTO.CustomerSource.StatusId,
                Description = Customer_CustomerDTO.CustomerSource.Description,
                Used = Customer_CustomerDTO.CustomerSource.Used,
                RowId = Customer_CustomerDTO.CustomerSource.RowId,
            };
            Customer.District = Customer_CustomerDTO.District == null ? null : new District
            {
                Id = Customer_CustomerDTO.District.Id,
                Code = Customer_CustomerDTO.District.Code,
                Name = Customer_CustomerDTO.District.Name,
                Priority = Customer_CustomerDTO.District.Priority,
                ProvinceId = Customer_CustomerDTO.District.ProvinceId,
                StatusId = Customer_CustomerDTO.District.StatusId,
                RowId = Customer_CustomerDTO.District.RowId,
                Used = Customer_CustomerDTO.District.Used,
            };
            Customer.Nation = Customer_CustomerDTO.Nation == null ? null : new Nation
            {
                Id = Customer_CustomerDTO.Nation.Id,
                Code = Customer_CustomerDTO.Nation.Code,
                Name = Customer_CustomerDTO.Nation.Name,
                Priority = Customer_CustomerDTO.Nation.Priority,
                StatusId = Customer_CustomerDTO.Nation.StatusId,
                Used = Customer_CustomerDTO.Nation.Used,
                RowId = Customer_CustomerDTO.Nation.RowId,
            };
            Customer.Organization = Customer_CustomerDTO.Organization == null ? null : new Organization
            {
                Id = Customer_CustomerDTO.Organization.Id,
                Code = Customer_CustomerDTO.Organization.Code,
                Name = Customer_CustomerDTO.Organization.Name,
                ParentId = Customer_CustomerDTO.Organization.ParentId,
                Path = Customer_CustomerDTO.Organization.Path,
                Level = Customer_CustomerDTO.Organization.Level,
                StatusId = Customer_CustomerDTO.Organization.StatusId,
                Phone = Customer_CustomerDTO.Organization.Phone,
                Email = Customer_CustomerDTO.Organization.Email,
                Address = Customer_CustomerDTO.Organization.Address,
                RowId = Customer_CustomerDTO.Organization.RowId,
                Used = Customer_CustomerDTO.Organization.Used,
                IsDisplay = Customer_CustomerDTO.Organization.IsDisplay,
            };
            Customer.Profession = Customer_CustomerDTO.Profession == null ? null : new Profession
            {
                Id = Customer_CustomerDTO.Profession.Id,
                Code = Customer_CustomerDTO.Profession.Code,
                Name = Customer_CustomerDTO.Profession.Name,
                StatusId = Customer_CustomerDTO.Profession.StatusId,
                RowId = Customer_CustomerDTO.Profession.RowId,
                Used = Customer_CustomerDTO.Profession.Used,
            };
            Customer.Province = Customer_CustomerDTO.Province == null ? null : new Province
            {
                Id = Customer_CustomerDTO.Province.Id,
                Code = Customer_CustomerDTO.Province.Code,
                Name = Customer_CustomerDTO.Province.Name,
                Priority = Customer_CustomerDTO.Province.Priority,
                StatusId = Customer_CustomerDTO.Province.StatusId,
                RowId = Customer_CustomerDTO.Province.RowId,
                Used = Customer_CustomerDTO.Province.Used,
            };
            Customer.Ward = Customer_CustomerDTO.Ward == null ? null : new Ward
            {
                Id = Customer_CustomerDTO.Ward.Id,
                Code = Customer_CustomerDTO.Ward.Code,
                Name = Customer_CustomerDTO.Ward.Name,
                Priority = Customer_CustomerDTO.Ward.Priority,
                DistrictId = Customer_CustomerDTO.Ward.DistrictId,
                StatusId = Customer_CustomerDTO.Ward.StatusId,
                RowId = Customer_CustomerDTO.Ward.RowId,
                Used = Customer_CustomerDTO.Ward.Used,
            };
            Customer.BaseLanguage = CurrentContext.Language;
            return Customer;
        }

        private CustomerFilter ConvertFilterDTOToFilterEntity(Customer_CustomerFilterDTO Customer_CustomerFilterDTO)
        {
            CustomerFilter CustomerFilter = new CustomerFilter();
            CustomerFilter.Selects = CustomerSelect.ALL;
            CustomerFilter.Skip = Customer_CustomerFilterDTO.Skip;
            CustomerFilter.Take = Customer_CustomerFilterDTO.Take;
            CustomerFilter.OrderBy = Customer_CustomerFilterDTO.OrderBy;
            CustomerFilter.OrderType = Customer_CustomerFilterDTO.OrderType;

            CustomerFilter.Id = Customer_CustomerFilterDTO.Id;
            CustomerFilter.Code = Customer_CustomerFilterDTO.Code;
            CustomerFilter.CodeDraft = Customer_CustomerFilterDTO.CodeDraft;
            CustomerFilter.Name = Customer_CustomerFilterDTO.Name;
            CustomerFilter.Phone = Customer_CustomerFilterDTO.Phone;
            CustomerFilter.Address = Customer_CustomerFilterDTO.Address;
            CustomerFilter.NationId = Customer_CustomerFilterDTO.NationId;
            CustomerFilter.ProvinceId = Customer_CustomerFilterDTO.ProvinceId;
            CustomerFilter.DistrictId = Customer_CustomerFilterDTO.DistrictId;
            CustomerFilter.WardId = Customer_CustomerFilterDTO.WardId;
            CustomerFilter.Birthday = Customer_CustomerFilterDTO.Birthday;
            CustomerFilter.Email = Customer_CustomerFilterDTO.Email;
            CustomerFilter.ProfessionId = Customer_CustomerFilterDTO.ProfessionId;
            CustomerFilter.CustomerSourceId = Customer_CustomerFilterDTO.CustomerSourceId;
            CustomerFilter.SexId = Customer_CustomerFilterDTO.SexId;
            CustomerFilter.StatusId = Customer_CustomerFilterDTO.StatusId;
            CustomerFilter.AppUserId = Customer_CustomerFilterDTO.AppUserId;
            CustomerFilter.CreatorId = Customer_CustomerFilterDTO.CreatorId;
            CustomerFilter.OrganizationId = Customer_CustomerFilterDTO.OrganizationId;
            CustomerFilter.RowId = Customer_CustomerFilterDTO.RowId;
            CustomerFilter.CodeGeneratorRuleId = Customer_CustomerFilterDTO.CodeGeneratorRuleId;
            CustomerFilter.CreatedAt = Customer_CustomerFilterDTO.CreatedAt;
            CustomerFilter.UpdatedAt = Customer_CustomerFilterDTO.UpdatedAt;
            return CustomerFilter;
        }
    }
}

