using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCategory;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MProvince;
using ELODIE.Services.MStatus;
using ELODIE.Services.MSupplier;
using ELODIE.Services.MWard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.supplier
{
    public partial class SupplierController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private INationService NationService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private ICategoryService CategoryService;
        private ISupplierService SupplierService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public SupplierController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            INationService NationService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            IWardService WardService,
            ICategoryService CategoryService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.NationService = NationService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.WardService = WardService;
            this.CategoryService = CategoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SupplierRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = await SupplierService.ToFilter(SupplierFilter);
            int count = await SupplierService.Count(SupplierFilter);
            return count;
        }

        [Route(SupplierRoute.List), HttpPost]
        public async Task<ActionResult<List<Supplier_SupplierDTO>>> List([FromBody] Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO);
            SupplierFilter = await SupplierService.ToFilter(SupplierFilter);
            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Supplier_SupplierDTO> Supplier_SupplierDTOs = Suppliers
                .Select(c => new Supplier_SupplierDTO(c)).ToList();
            return Supplier_SupplierDTOs;
        }

        [Route(SupplierRoute.Get), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Get([FromBody]Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = await SupplierService.Get(Supplier_SupplierDTO.Id);
            return new Supplier_SupplierDTO(Supplier);
        }

        [Route(SupplierRoute.Create), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Create([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Create(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.QuickCreate), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> QuickCreate([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.QuickCreate(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }


        [Route(SupplierRoute.Update), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Update([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Update(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.Delete), HttpPost]
        public async Task<ActionResult<Supplier_SupplierDTO>> Delete([FromBody] Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Supplier_SupplierDTO.Id))
                return Forbid();

            Supplier Supplier = ConvertDTOToEntity(Supplier_SupplierDTO);
            Supplier = await SupplierService.Delete(Supplier);
            Supplier_SupplierDTO = new Supplier_SupplierDTO(Supplier);
            if (Supplier.IsValidated)
                return Supplier_SupplierDTO;
            else
                return BadRequest(Supplier_SupplierDTO);
        }

        [Route(SupplierRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Id = new IdFilter { In = Ids };
            SupplierFilter.Selects = SupplierSelect.Id;
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = int.MaxValue;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            Suppliers = await SupplierService.BulkDelete(Suppliers);
            if (Suppliers.Any(x => !x.IsValidated))
                return BadRequest(Suppliers.Where(x => !x.IsValidated));
            return true;
        }


        [Route(SupplierRoute.SaveImage), HttpPost]
        public async Task<ActionResult<string>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            return await SupplierService.SaveImage(Image);
        }



        private async Task<bool> HasPermission(long Id)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter = await SupplierService.ToFilter(SupplierFilter);
            if (Id == 0)
            {

            }
            else
            {
                SupplierFilter.Id = new IdFilter { Equal = Id };
                int count = await SupplierService.Count(SupplierFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Supplier ConvertDTOToEntity(Supplier_SupplierDTO Supplier_SupplierDTO)
        {
            Supplier Supplier = new Supplier();
            Supplier.Id = Supplier_SupplierDTO.Id;
            Supplier.Code = Supplier_SupplierDTO.Code;
            Supplier.Name = Supplier_SupplierDTO.Name;
            Supplier.TaxCode = Supplier_SupplierDTO.TaxCode;
            Supplier.Phone = Supplier_SupplierDTO.Phone;
            Supplier.Email = Supplier_SupplierDTO.Email;
            Supplier.Avatar = Supplier_SupplierDTO.Avatar;
            Supplier.Address = Supplier_SupplierDTO.Address;
            Supplier.NationId = Supplier_SupplierDTO.NationId;
            Supplier.ProvinceId = Supplier_SupplierDTO.ProvinceId;
            Supplier.DistrictId = Supplier_SupplierDTO.DistrictId;
            Supplier.WardId = Supplier_SupplierDTO.WardId;
            Supplier.OwnerName = Supplier_SupplierDTO.OwnerName;
            Supplier.PersonInChargeId = Supplier_SupplierDTO.PersonInChargeId;
            Supplier.StatusId = Supplier_SupplierDTO.StatusId;
            Supplier.Description = Supplier_SupplierDTO.Description;
            Supplier.Used = Supplier_SupplierDTO.Used;
            Supplier.RowId = Supplier_SupplierDTO.RowId;
            Supplier.District = Supplier_SupplierDTO.District == null ? null : new District
            {
                Id = Supplier_SupplierDTO.District.Id,
                Code = Supplier_SupplierDTO.District.Code,
                Name = Supplier_SupplierDTO.District.Name,
                Priority = Supplier_SupplierDTO.District.Priority,
                ProvinceId = Supplier_SupplierDTO.District.ProvinceId,
                StatusId = Supplier_SupplierDTO.District.StatusId,
                RowId = Supplier_SupplierDTO.District.RowId,
                Used = Supplier_SupplierDTO.District.Used,
            };
            Supplier.Nation = Supplier_SupplierDTO.Nation == null ? null : new Nation
            {
                Id = Supplier_SupplierDTO.Nation.Id,
                Code = Supplier_SupplierDTO.Nation.Code,
                Name = Supplier_SupplierDTO.Nation.Name,
                Priority = Supplier_SupplierDTO.Nation.Priority,
                StatusId = Supplier_SupplierDTO.Nation.StatusId,
                Used = Supplier_SupplierDTO.Nation.Used,
                RowId = Supplier_SupplierDTO.Nation.RowId,
            };
            Supplier.PersonInCharge = Supplier_SupplierDTO.PersonInCharge == null ? null : new AppUser
            {
                Id = Supplier_SupplierDTO.PersonInCharge.Id,
                Username = Supplier_SupplierDTO.PersonInCharge.Username,
                DisplayName = Supplier_SupplierDTO.PersonInCharge.DisplayName,
                Address = Supplier_SupplierDTO.PersonInCharge.Address,
                Email = Supplier_SupplierDTO.PersonInCharge.Email,
                Phone = Supplier_SupplierDTO.PersonInCharge.Phone,
                SexId = Supplier_SupplierDTO.PersonInCharge.SexId,
                Birthday = Supplier_SupplierDTO.PersonInCharge.Birthday,
                Avatar = Supplier_SupplierDTO.PersonInCharge.Avatar,
                Department = Supplier_SupplierDTO.PersonInCharge.Department,
                OrganizationId = Supplier_SupplierDTO.PersonInCharge.OrganizationId,
                StatusId = Supplier_SupplierDTO.PersonInCharge.StatusId,
                RowId = Supplier_SupplierDTO.PersonInCharge.RowId,
                Used = Supplier_SupplierDTO.PersonInCharge.Used,
            };
            Supplier.Province = Supplier_SupplierDTO.Province == null ? null : new Province
            {
                Id = Supplier_SupplierDTO.Province.Id,
                Code = Supplier_SupplierDTO.Province.Code,
                Name = Supplier_SupplierDTO.Province.Name,
                Priority = Supplier_SupplierDTO.Province.Priority,
                StatusId = Supplier_SupplierDTO.Province.StatusId,
                RowId = Supplier_SupplierDTO.Province.RowId,
                Used = Supplier_SupplierDTO.Province.Used,
            };
            Supplier.Status = Supplier_SupplierDTO.Status == null ? null : new Status
            {
                Id = Supplier_SupplierDTO.Status.Id,
                Code = Supplier_SupplierDTO.Status.Code,
                Name = Supplier_SupplierDTO.Status.Name,
            };
            Supplier.Ward = Supplier_SupplierDTO.Ward == null ? null : new Ward
            {
                Id = Supplier_SupplierDTO.Ward.Id,
                Code = Supplier_SupplierDTO.Ward.Code,
                Name = Supplier_SupplierDTO.Ward.Name,
                Priority = Supplier_SupplierDTO.Ward.Priority,
                DistrictId = Supplier_SupplierDTO.Ward.DistrictId,
                StatusId = Supplier_SupplierDTO.Ward.StatusId,
                RowId = Supplier_SupplierDTO.Ward.RowId,
                Used = Supplier_SupplierDTO.Ward.Used,
            };
            Supplier.SupplierBankAccounts = Supplier_SupplierDTO.SupplierBankAccounts?
                .Select(x => new SupplierBankAccount
                {
                    Id = x.Id,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                    BankAccountOwnerName = x.BankAccountOwnerName,
                    Used = x.Used,
                    RowId = x.RowId,
                }).ToList();
            Supplier.SupplierCategoryMappings = Supplier_SupplierDTO.SupplierCategoryMappings?
                .Select(x => new SupplierCategoryMapping
                {
                    CategoryId = x.CategoryId,
                    Category = x.Category == null ? null : new Category
                    {
                        Id = x.Category.Id,
                        Code = x.Category.Code,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        Path = x.Category.Path,
                        Level = x.Category.Level,
                        StatusId = x.Category.StatusId,
                        ImageId = x.Category.ImageId,
                        RowId = x.Category.RowId,
                        Used = x.Category.Used,
                    },
                }).ToList();
            Supplier.SupplierContactors = Supplier_SupplierDTO.SupplierContactors?
                .Select(x => new SupplierContactor
                {
                    Id = x.Id,
                    Name = x.Name,
                    Phone = x.Phone,
                    Email = x.Email,
                    Used = x.Used,
                    RowId = x.RowId,
                }).ToList();
            Supplier.BaseLanguage = CurrentContext.Language;
            return Supplier;
        }

        private SupplierFilter ConvertFilterDTOToFilterEntity(Supplier_SupplierFilterDTO Supplier_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Skip = Supplier_SupplierFilterDTO.Skip;
            SupplierFilter.Take = Supplier_SupplierFilterDTO.Take;
            SupplierFilter.OrderBy = Supplier_SupplierFilterDTO.OrderBy;
            SupplierFilter.OrderType = Supplier_SupplierFilterDTO.OrderType;

            SupplierFilter.Id = Supplier_SupplierFilterDTO.Id;
            SupplierFilter.Code = Supplier_SupplierFilterDTO.Code;
            SupplierFilter.Name = Supplier_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Supplier_SupplierFilterDTO.TaxCode;
            SupplierFilter.Phone = Supplier_SupplierFilterDTO.Phone;
            SupplierFilter.Email = Supplier_SupplierFilterDTO.Email;
            SupplierFilter.Avatar = Supplier_SupplierFilterDTO.Avatar;
            SupplierFilter.Address = Supplier_SupplierFilterDTO.Address;
            SupplierFilter.NationId = Supplier_SupplierFilterDTO.NationId;
            SupplierFilter.ProvinceId = Supplier_SupplierFilterDTO.ProvinceId;
            SupplierFilter.DistrictId = Supplier_SupplierFilterDTO.DistrictId;
            SupplierFilter.WardId = Supplier_SupplierFilterDTO.WardId;
            SupplierFilter.OwnerName = Supplier_SupplierFilterDTO.OwnerName;
            SupplierFilter.PersonInChargeId = Supplier_SupplierFilterDTO.PersonInChargeId;
            SupplierFilter.StatusId = Supplier_SupplierFilterDTO.StatusId;
            SupplierFilter.Description = Supplier_SupplierFilterDTO.Description;
            SupplierFilter.RowId = Supplier_SupplierFilterDTO.RowId;
            SupplierFilter.CreatedAt = Supplier_SupplierFilterDTO.CreatedAt;
            SupplierFilter.UpdatedAt = Supplier_SupplierFilterDTO.UpdatedAt;

            SupplierFilter.Search = Supplier_SupplierFilterDTO.Search;
            
            return SupplierFilter;
        }

    }
}

