using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using Microsoft.AspNetCore.Mvc;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public partial class SupplierController : RpcController
    {
        [Route(SupplierRoute.FilterListDistrict), HttpPost]
        public async Task<List<Supplier_DistrictDTO>> FilterListDistrict([FromBody] Supplier_DistrictFilterDTO Supplier_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Supplier_DistrictFilterDTO.Id;
            DistrictFilter.Code = Supplier_DistrictFilterDTO.Code;
            DistrictFilter.Name = Supplier_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Supplier_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Supplier_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Supplier_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Supplier_DistrictDTO> Supplier_DistrictDTOs = Districts
                .Select(x => new Supplier_DistrictDTO(x)).ToList();
            return Supplier_DistrictDTOs;
        }
        [Route(SupplierRoute.FilterListNation), HttpPost]
        public async Task<List<Supplier_NationDTO>> FilterListNation([FromBody] Supplier_NationFilterDTO Supplier_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = new NationFilter();
            NationFilter.Skip = 0;
            NationFilter.Take = 20;
            NationFilter.OrderBy = NationOrder.Id;
            NationFilter.OrderType = OrderType.ASC;
            NationFilter.Selects = NationSelect.ALL;
            NationFilter.Id = Supplier_NationFilterDTO.Id;
            NationFilter.Code = Supplier_NationFilterDTO.Code;
            NationFilter.Name = Supplier_NationFilterDTO.Name;
            NationFilter.Priority = Supplier_NationFilterDTO.Priority;
            NationFilter.StatusId = Supplier_NationFilterDTO.StatusId;
            NationFilter.RowId = Supplier_NationFilterDTO.RowId;

            List<Nation> Nations = await NationService.List(NationFilter);
            List<Supplier_NationDTO> Supplier_NationDTOs = Nations
                .Select(x => new Supplier_NationDTO(x)).ToList();
            return Supplier_NationDTOs;
        }
        [Route(SupplierRoute.FilterListAppUser), HttpPost]
        public async Task<List<Supplier_AppUserDTO>> FilterListAppUser([FromBody] Supplier_AppUserFilterDTO Supplier_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Supplier_AppUserFilterDTO.Id;
            AppUserFilter.Username = Supplier_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Supplier_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Supplier_AppUserFilterDTO.Address;
            AppUserFilter.Email = Supplier_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Supplier_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = Supplier_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = Supplier_AppUserFilterDTO.Birthday;
            AppUserFilter.Department = Supplier_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Supplier_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = Supplier_AppUserFilterDTO.StatusId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Supplier_AppUserDTO> Supplier_AppUserDTOs = AppUsers
                .Select(x => new Supplier_AppUserDTO(x)).ToList();
            return Supplier_AppUserDTOs;
        }
        [Route(SupplierRoute.FilterListProvince), HttpPost]
        public async Task<List<Supplier_ProvinceDTO>> FilterListProvince([FromBody] Supplier_ProvinceFilterDTO Supplier_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Supplier_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = Supplier_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = Supplier_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Supplier_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Supplier_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Supplier_ProvinceDTO> Supplier_ProvinceDTOs = Provinces
                .Select(x => new Supplier_ProvinceDTO(x)).ToList();
            return Supplier_ProvinceDTOs;
        }
        [Route(SupplierRoute.FilterListStatus), HttpPost]
        public async Task<List<Supplier_StatusDTO>> FilterListStatus([FromBody] Supplier_StatusFilterDTO Supplier_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Supplier_StatusDTO> Supplier_StatusDTOs = Statuses
                .Select(x => new Supplier_StatusDTO(x)).ToList();
            return Supplier_StatusDTOs;
        }
        [Route(SupplierRoute.FilterListWard), HttpPost]
        public async Task<List<Supplier_WardDTO>> FilterListWard([FromBody] Supplier_WardFilterDTO Supplier_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Supplier_WardFilterDTO.Id;
            WardFilter.Code = Supplier_WardFilterDTO.Code;
            WardFilter.Name = Supplier_WardFilterDTO.Name;
            WardFilter.Priority = Supplier_WardFilterDTO.Priority;
            WardFilter.DistrictId = Supplier_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Supplier_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<Supplier_WardDTO> Supplier_WardDTOs = Wards
                .Select(x => new Supplier_WardDTO(x)).ToList();
            return Supplier_WardDTOs;
        }
        [Route(SupplierRoute.FilterListCategory), HttpPost]
        public async Task<List<Supplier_CategoryDTO>> FilterListCategory([FromBody] Supplier_CategoryFilterDTO Supplier_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = Supplier_CategoryFilterDTO.Id;
            CategoryFilter.Code = Supplier_CategoryFilterDTO.Code;
            CategoryFilter.Name = Supplier_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Supplier_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Supplier_CategoryFilterDTO.Path;
            CategoryFilter.Level = Supplier_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Supplier_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Supplier_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Supplier_CategoryFilterDTO.RowId;

            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Supplier_CategoryDTO> Supplier_CategoryDTOs = Categories
                .Select(x => new Supplier_CategoryDTO(x)).ToList();
            return Supplier_CategoryDTOs;
        }

    }
}

