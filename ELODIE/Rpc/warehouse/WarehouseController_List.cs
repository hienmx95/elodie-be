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
        [Route(WarehouseRoute.ListItem), HttpPost]
        public async Task<List<Warehouse_ItemDTO>> ListItem([FromBody] Warehouse_ItemFilterDTO Warehouse_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = Warehouse_ItemFilterDTO.Skip;
            ItemFilter.Take = Warehouse_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Warehouse_ItemFilterDTO.Id;
            ItemFilter.Code = Warehouse_ItemFilterDTO.Code;
            ItemFilter.Name = Warehouse_ItemFilterDTO.Name;
            ItemFilter.OtherName = Warehouse_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Warehouse_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Warehouse_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Warehouse_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Warehouse_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Warehouse_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Warehouse_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter = ItemService.ToFilter(ItemFilter);

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Warehouse_ItemDTO> Warehouse_ItemDTOs = Items
                .Select(x => new Warehouse_ItemDTO(x)).ToList();
            return Warehouse_ItemDTOs;
        }
        [Route(WarehouseRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] Warehouse_ItemFilterDTO Warehouse_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = Warehouse_ItemFilterDTO.Id;
            ItemFilter.Code = Warehouse_ItemFilterDTO.Code;
            ItemFilter.Name = Warehouse_ItemFilterDTO.Name;
            ItemFilter.OtherName = Warehouse_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Warehouse_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Warehouse_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Warehouse_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Warehouse_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Warehouse_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Warehouse_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            ItemFilter = ItemService.ToFilter(ItemFilter);

            return await ItemService.Count(ItemFilter);
        }
    }
}

