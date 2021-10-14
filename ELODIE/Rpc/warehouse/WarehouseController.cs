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
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private IWardService WardService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IItemService ItemService;
        private IProductService ProductService;
        private IWarehouseService WarehouseService;
        private ICurrentContext CurrentContext;
        public WarehouseController(
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            IWardService WardService,
            IUnitOfMeasureService UnitOfMeasureService,
            IItemService ItemService,
            IProductService ProductService,
            IWarehouseService WarehouseService,
            ICurrentContext CurrentContext
        )
        {
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.WardService = WardService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.WarehouseService = WarehouseService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WarehouseRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO);
            WarehouseFilter = await WarehouseService.ToFilter(WarehouseFilter);
            int count = await WarehouseService.Count(WarehouseFilter);
            return count;
        }

        [Route(WarehouseRoute.List), HttpPost]
        public async Task<ActionResult<List<Warehouse_WarehouseDTO>>> List([FromBody] Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO);
            WarehouseFilter = await WarehouseService.ToFilter(WarehouseFilter);
            List<Warehouse> Warehouses = await WarehouseService.List(WarehouseFilter);
            List<Warehouse_WarehouseDTO> Warehouse_WarehouseDTOs = Warehouses
                .Select(c => new Warehouse_WarehouseDTO(c)).ToList();
            return Warehouse_WarehouseDTOs;
        }

        [Route(WarehouseRoute.Get), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Get([FromBody]Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = await WarehouseService.Get(Warehouse_WarehouseDTO.Id);
            return new Warehouse_WarehouseDTO(Warehouse);
        }

        [Route(WarehouseRoute.Create), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Create([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Create(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }

        [Route(WarehouseRoute.Update), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Update([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Update(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }

        [Route(WarehouseRoute.Delete), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Delete([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Delete(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }
        
        [Route(WarehouseRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter = await WarehouseService.ToFilter(WarehouseFilter);
            WarehouseFilter.Id = new IdFilter { In = Ids };
            WarehouseFilter.Selects = WarehouseSelect.Id;
            WarehouseFilter.Skip = 0;
            WarehouseFilter.Take = int.MaxValue;

            List<Warehouse> Warehouses = await WarehouseService.List(WarehouseFilter);
            Warehouses = await WarehouseService.BulkDelete(Warehouses);
            if (Warehouses.Any(x => !x.IsValidated))
                return BadRequest(Warehouses.Where(x => !x.IsValidated));
            return true;
        }
        private async Task<bool> HasPermission(long Id)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter = await WarehouseService.ToFilter(WarehouseFilter);
            if (Id == 0)
            {

            }
            else
            {
                WarehouseFilter.Id = new IdFilter { Equal = Id };
                int count = await WarehouseService.Count(WarehouseFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Warehouse ConvertDTOToEntity(Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            Warehouse Warehouse = new Warehouse();
            Warehouse.Id = Warehouse_WarehouseDTO.Id;
            Warehouse.Code = Warehouse_WarehouseDTO.Code;
            Warehouse.Name = Warehouse_WarehouseDTO.Name;
            Warehouse.Address = Warehouse_WarehouseDTO.Address;
            Warehouse.OrganizationId = Warehouse_WarehouseDTO.OrganizationId;
            Warehouse.ProvinceId = Warehouse_WarehouseDTO.ProvinceId;
            Warehouse.DistrictId = Warehouse_WarehouseDTO.DistrictId;
            Warehouse.WardId = Warehouse_WarehouseDTO.WardId;
            Warehouse.StatusId = Warehouse_WarehouseDTO.StatusId;
            Warehouse.RowId = Warehouse_WarehouseDTO.RowId;
            Warehouse.District = Warehouse_WarehouseDTO.District == null ? null : new District
            {
                Id = Warehouse_WarehouseDTO.District.Id,
                Code = Warehouse_WarehouseDTO.District.Code,
                Name = Warehouse_WarehouseDTO.District.Name,
                Priority = Warehouse_WarehouseDTO.District.Priority,
                ProvinceId = Warehouse_WarehouseDTO.District.ProvinceId,
                StatusId = Warehouse_WarehouseDTO.District.StatusId,
                RowId = Warehouse_WarehouseDTO.District.RowId,
                Used = Warehouse_WarehouseDTO.District.Used,
            };
            Warehouse.Organization = Warehouse_WarehouseDTO.Organization == null ? null : new Organization
            {
                Id = Warehouse_WarehouseDTO.Organization.Id,
                Code = Warehouse_WarehouseDTO.Organization.Code,
                Name = Warehouse_WarehouseDTO.Organization.Name,
                ParentId = Warehouse_WarehouseDTO.Organization.ParentId,
                Path = Warehouse_WarehouseDTO.Organization.Path,
                Level = Warehouse_WarehouseDTO.Organization.Level,
                StatusId = Warehouse_WarehouseDTO.Organization.StatusId,
                Phone = Warehouse_WarehouseDTO.Organization.Phone,
                Email = Warehouse_WarehouseDTO.Organization.Email,
                Address = Warehouse_WarehouseDTO.Organization.Address,
                RowId = Warehouse_WarehouseDTO.Organization.RowId,
                Used = Warehouse_WarehouseDTO.Organization.Used,
                IsDisplay = Warehouse_WarehouseDTO.Organization.IsDisplay,
            };
            Warehouse.Province = Warehouse_WarehouseDTO.Province == null ? null : new Province
            {
                Id = Warehouse_WarehouseDTO.Province.Id,
                Code = Warehouse_WarehouseDTO.Province.Code,
                Name = Warehouse_WarehouseDTO.Province.Name,
                Priority = Warehouse_WarehouseDTO.Province.Priority,
                StatusId = Warehouse_WarehouseDTO.Province.StatusId,
                RowId = Warehouse_WarehouseDTO.Province.RowId,
                Used = Warehouse_WarehouseDTO.Province.Used,
            };
            Warehouse.Status = Warehouse_WarehouseDTO.Status == null ? null : new Status
            {
                Id = Warehouse_WarehouseDTO.Status.Id,
                Code = Warehouse_WarehouseDTO.Status.Code,
                Name = Warehouse_WarehouseDTO.Status.Name,
            };
            Warehouse.Ward = Warehouse_WarehouseDTO.Ward == null ? null : new Ward
            {
                Id = Warehouse_WarehouseDTO.Ward.Id,
                Code = Warehouse_WarehouseDTO.Ward.Code,
                Name = Warehouse_WarehouseDTO.Ward.Name,
                Priority = Warehouse_WarehouseDTO.Ward.Priority,
                DistrictId = Warehouse_WarehouseDTO.Ward.DistrictId,
                StatusId = Warehouse_WarehouseDTO.Ward.StatusId,
                RowId = Warehouse_WarehouseDTO.Ward.RowId,
                Used = Warehouse_WarehouseDTO.Ward.Used,
            };
            Warehouse.Inventories = Warehouse_WarehouseDTO.Inventories?
                .Select(x => new Inventory
                {
                    ItemId = x.ItemId,
                    AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                    AlternateQuantity = x.AlternateQuantity,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    WarehouseId = x.WarehouseId,
                    AlternateUnitOfMeasure = x.AlternateUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.AlternateUnitOfMeasure.Id,
                        Code = x.AlternateUnitOfMeasure.Code,
                        Name = x.AlternateUnitOfMeasure.Name,
                        Description = x.AlternateUnitOfMeasure.Description,
                        StatusId = x.AlternateUnitOfMeasure.StatusId,
                        Used = x.AlternateUnitOfMeasure.Used,
                        RowId = x.AlternateUnitOfMeasure.RowId,
                    },
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        ERPCode = x.Item.ERPCode,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                        Used = x.Item.Used,
                        RowId = x.Item.RowId,
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
            Warehouse.BaseLanguage = CurrentContext.Language;
            return Warehouse;
        }

        private WarehouseFilter ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter.Selects = WarehouseSelect.ALL;
            WarehouseFilter.Skip = Warehouse_WarehouseFilterDTO.Skip;
            WarehouseFilter.Take = Warehouse_WarehouseFilterDTO.Take;
            WarehouseFilter.OrderBy = Warehouse_WarehouseFilterDTO.OrderBy;
            WarehouseFilter.OrderType = Warehouse_WarehouseFilterDTO.OrderType;

            WarehouseFilter.Id = Warehouse_WarehouseFilterDTO.Id;
            WarehouseFilter.Code = Warehouse_WarehouseFilterDTO.Code;
            WarehouseFilter.Name = Warehouse_WarehouseFilterDTO.Name;
            WarehouseFilter.Address = Warehouse_WarehouseFilterDTO.Address;
            WarehouseFilter.OrganizationId = Warehouse_WarehouseFilterDTO.OrganizationId;
            WarehouseFilter.ProvinceId = Warehouse_WarehouseFilterDTO.ProvinceId;
            WarehouseFilter.DistrictId = Warehouse_WarehouseFilterDTO.DistrictId;
            WarehouseFilter.WardId = Warehouse_WarehouseFilterDTO.WardId;
            WarehouseFilter.StatusId = Warehouse_WarehouseFilterDTO.StatusId;
            WarehouseFilter.RowId = Warehouse_WarehouseFilterDTO.RowId;
            WarehouseFilter.CreatedAt = Warehouse_WarehouseFilterDTO.CreatedAt;
            WarehouseFilter.UpdatedAt = Warehouse_WarehouseFilterDTO.UpdatedAt;
            return WarehouseFilter;
        }
    }
}

