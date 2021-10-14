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
using ELODIE.Services.MWarehouse;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MProvince;
using ELODIE.Services.MStatus;
using ELODIE.Services.MWard;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MProduct;

namespace ELODIE.Rpc.warehouse
{
    public partial class WarehouseController : RpcController
    {
        [Route(WarehouseRoute.FilterListDistrict), HttpPost]
        public async Task<List<Warehouse_DistrictDTO>> FilterListDistrict([FromBody] Warehouse_DistrictFilterDTO Warehouse_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Warehouse_DistrictFilterDTO.Id;
            DistrictFilter.Code = Warehouse_DistrictFilterDTO.Code;
            DistrictFilter.Name = Warehouse_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Warehouse_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Warehouse_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Warehouse_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Warehouse_DistrictDTO> Warehouse_DistrictDTOs = Districts
                .Select(x => new Warehouse_DistrictDTO(x)).ToList();
            return Warehouse_DistrictDTOs;
        }
        [Route(WarehouseRoute.FilterListOrganization), HttpPost]
        public async Task<List<Warehouse_OrganizationDTO>> FilterListOrganization([FromBody] Warehouse_OrganizationFilterDTO Warehouse_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = Warehouse_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Warehouse_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Warehouse_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Warehouse_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Warehouse_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Warehouse_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Warehouse_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Warehouse_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = Warehouse_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = Warehouse_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Warehouse_OrganizationDTO> Warehouse_OrganizationDTOs = Organizations
                .Select(x => new Warehouse_OrganizationDTO(x)).ToList();
            return Warehouse_OrganizationDTOs;
        }
        [Route(WarehouseRoute.FilterListProvince), HttpPost]
        public async Task<List<Warehouse_ProvinceDTO>> FilterListProvince([FromBody] Warehouse_ProvinceFilterDTO Warehouse_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Warehouse_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = Warehouse_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = Warehouse_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Warehouse_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Warehouse_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Warehouse_ProvinceDTO> Warehouse_ProvinceDTOs = Provinces
                .Select(x => new Warehouse_ProvinceDTO(x)).ToList();
            return Warehouse_ProvinceDTOs;
        }
        [Route(WarehouseRoute.FilterListStatus), HttpPost]
        public async Task<List<Warehouse_StatusDTO>> FilterListStatus([FromBody] Warehouse_StatusFilterDTO Warehouse_StatusFilterDTO)
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
            List<Warehouse_StatusDTO> Warehouse_StatusDTOs = Statuses
                .Select(x => new Warehouse_StatusDTO(x)).ToList();
            return Warehouse_StatusDTOs;
        }
        [Route(WarehouseRoute.FilterListWard), HttpPost]
        public async Task<List<Warehouse_WardDTO>> FilterListWard([FromBody] Warehouse_WardFilterDTO Warehouse_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Warehouse_WardFilterDTO.Id;
            WardFilter.Code = Warehouse_WardFilterDTO.Code;
            WardFilter.Name = Warehouse_WardFilterDTO.Name;
            WardFilter.Priority = Warehouse_WardFilterDTO.Priority;
            WardFilter.DistrictId = Warehouse_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Warehouse_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<Warehouse_WardDTO> Warehouse_WardDTOs = Wards
                .Select(x => new Warehouse_WardDTO(x)).ToList();
            return Warehouse_WardDTOs;
        }
        [Route(WarehouseRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<Warehouse_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] Warehouse_UnitOfMeasureFilterDTO Warehouse_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = Warehouse_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = Warehouse_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = Warehouse_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = Warehouse_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = Warehouse_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<Warehouse_UnitOfMeasureDTO> Warehouse_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new Warehouse_UnitOfMeasureDTO(x)).ToList();
            return Warehouse_UnitOfMeasureDTOs;
        }
        [Route(WarehouseRoute.FilterListItem), HttpPost]
        public async Task<List<Warehouse_ItemDTO>> FilterListItem([FromBody] Warehouse_ItemFilterDTO Warehouse_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Warehouse_ItemFilterDTO.Id;
            ItemFilter.ProductId = Warehouse_ItemFilterDTO.ProductId;
            ItemFilter.Code = Warehouse_ItemFilterDTO.Code;
            ItemFilter.Name = Warehouse_ItemFilterDTO.Name;
            ItemFilter.ScanCode = Warehouse_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = Warehouse_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = Warehouse_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = Warehouse_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Warehouse_ItemDTO> Warehouse_ItemDTOs = Items
                .Select(x => new Warehouse_ItemDTO(x)).ToList();
            return Warehouse_ItemDTOs;
        }
    }
}

