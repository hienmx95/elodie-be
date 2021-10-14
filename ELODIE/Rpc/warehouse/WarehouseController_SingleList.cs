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
using ELODIE.Enums;

namespace ELODIE.Rpc.warehouse
{
    public partial class WarehouseController : RpcController
    {
        [Route(WarehouseRoute.SingleListDistrict), HttpPost]
        public async Task<List<Warehouse_DistrictDTO>> SingleListDistrict([FromBody] Warehouse_DistrictFilterDTO Warehouse_DistrictFilterDTO)
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
            DistrictFilter.StatusId = new IdFilter{ Equal = 1 };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Warehouse_DistrictDTO> Warehouse_DistrictDTOs = Districts
                .Select(x => new Warehouse_DistrictDTO(x)).ToList();
            return Warehouse_DistrictDTOs;
        }
        [Route(WarehouseRoute.SingleListOrganization), HttpPost]
        public async Task<List<Warehouse_OrganizationDTO>> SingleListOrganization([FromBody] Warehouse_OrganizationFilterDTO Warehouse_OrganizationFilterDTO)
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
            OrganizationFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Warehouse_OrganizationDTO> Warehouse_OrganizationDTOs = Organizations
                .Select(x => new Warehouse_OrganizationDTO(x)).ToList();
            return Warehouse_OrganizationDTOs;
        }
        [Route(WarehouseRoute.SingleListProvince), HttpPost]
        public async Task<List<Warehouse_ProvinceDTO>> SingleListProvince([FromBody] Warehouse_ProvinceFilterDTO Warehouse_ProvinceFilterDTO)
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
            ProvinceFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Warehouse_ProvinceDTO> Warehouse_ProvinceDTOs = Provinces
                .Select(x => new Warehouse_ProvinceDTO(x)).ToList();
            return Warehouse_ProvinceDTOs;
        }
        [Route(WarehouseRoute.SingleListStatus), HttpPost]
        public async Task<List<Warehouse_StatusDTO>> SingleListStatus([FromBody] Warehouse_StatusFilterDTO Warehouse_StatusFilterDTO)
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
        [Route(WarehouseRoute.SingleListWard), HttpPost]
        public async Task<List<Warehouse_WardDTO>> SingleListWard([FromBody] Warehouse_WardFilterDTO Warehouse_WardFilterDTO)
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
            WardFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Warehouse_WardDTO> Warehouse_WardDTOs = Wards
                .Select(x => new Warehouse_WardDTO(x)).ToList();
            return Warehouse_WardDTOs;
        }
        [Route(WarehouseRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<Warehouse_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] Warehouse_UnitOfMeasureFilterDTO Warehouse_UnitOfMeasureFilterDTO)
        {
            List<Product> Products = await ProductService.List(new ProductFilter
            {
                Id = Warehouse_UnitOfMeasureFilterDTO.ProductId,
                Selects = ProductSelect.Id,
            });
            long ProductId = Products.Select(p => p.Id).FirstOrDefault();
            Product Product = await ProductService.Get(ProductId);

            List<Warehouse_UnitOfMeasureDTO> Warehouse_UnitOfMeasureDTOs = new List<Warehouse_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null)
            {
                Warehouse_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new Warehouse_UnitOfMeasureDTO(x)).ToList();
            }
            Warehouse_UnitOfMeasureDTO Warehouse_UnitOfMeasureDTO = new Warehouse_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = StatusEnum.ACTIVE.Id,
                Factor = 1,
            };
            
            Warehouse_UnitOfMeasureDTOs.Add(Warehouse_UnitOfMeasureDTO);
            foreach (var item in Warehouse_UnitOfMeasureDTOs)
            {
                item.MainUnitOfMeasure = new Warehouse_UnitOfMeasureDTO(Product.UnitOfMeasure);
            }
            Warehouse_UnitOfMeasureDTOs = Warehouse_UnitOfMeasureDTOs.Distinct().ToList();
            return Warehouse_UnitOfMeasureDTOs;
        }
        [Route(WarehouseRoute.SingleListItem), HttpPost]
        public async Task<List<Warehouse_ItemDTO>> SingleListItem([FromBody] Warehouse_ItemFilterDTO Warehouse_ItemFilterDTO)
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
            ItemFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Item> Items = await ItemService.List(ItemFilter);
            List<Warehouse_ItemDTO> Warehouse_ItemDTOs = Items
                .Select(x => new Warehouse_ItemDTO(x)).ToList();
            return Warehouse_ItemDTOs;
        }
    }
}

