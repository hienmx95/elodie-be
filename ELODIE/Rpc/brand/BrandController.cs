using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Services.MBrand;
using ELODIE.Services.MStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.brand
{
    public class BrandController : RpcController
    {
        private IStatusService StatusService;
        private IBrandService BrandService;
        private ICurrentContext CurrentContext;
        public BrandController(
            IStatusService StatusService,
            IBrandService BrandService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.BrandService = BrandService;
            this.CurrentContext = CurrentContext;
        }

        [Route(BrandRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO);
            int count = await BrandService.Count(BrandFilter);
            return count;
        }

        [Route(BrandRoute.List), HttpPost]
        public async Task<ActionResult<List<Brand_BrandDTO>>> List([FromBody] Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO);
            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Brand_BrandDTO> Brand_BrandDTOs = Brands
                .Select(c => new Brand_BrandDTO(c)).ToList();
            return Brand_BrandDTOs;
        }

        [Route(BrandRoute.Get), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Get([FromBody]Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = await BrandService.Get(Brand_BrandDTO.Id);
            return new Brand_BrandDTO(Brand);
        }

        [Route(BrandRoute.Create), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Create([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Create(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.Update), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Update([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Update(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.Delete), HttpPost]
        public async Task<ActionResult<Brand_BrandDTO>> Delete([FromBody] Brand_BrandDTO Brand_BrandDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Brand_BrandDTO.Id))
                return Forbid();

            Brand Brand = ConvertDTOToEntity(Brand_BrandDTO);
            Brand = await BrandService.Delete(Brand);
            Brand_BrandDTO = new Brand_BrandDTO(Brand);
            if (Brand.IsValidated)
                return Brand_BrandDTO;
            else
                return BadRequest(Brand_BrandDTO);
        }

        [Route(BrandRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Id = new IdFilter { In = Ids };
            BrandFilter.Selects = BrandSelect.Id;
            BrandFilter.Skip = 0;
            BrandFilter.Take = int.MaxValue;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            Brands = await BrandService.BulkDelete(Brands);
            if (Brands.Any(x => !x.IsValidated))
                return BadRequest(Brands.Where(x => !x.IsValidated));
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            BrandFilter BrandFilter = new BrandFilter();
            if (Id == 0)
            {

            }
            else
            {
                BrandFilter.Id = new IdFilter { Equal = Id };
                int count = await BrandService.Count(BrandFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Brand ConvertDTOToEntity(Brand_BrandDTO Brand_BrandDTO)
        {
            Brand Brand = new Brand();
            Brand.Id = Brand_BrandDTO.Id;
            Brand.Code = Brand_BrandDTO.Code;
            Brand.Name = Brand_BrandDTO.Name;
            Brand.Description = Brand_BrandDTO.Description;
            Brand.StatusId = Brand_BrandDTO.StatusId;
            Brand.UpdatedAt = Brand_BrandDTO.UpdatedAt;
            Brand.Status = Brand_BrandDTO.Status == null ? null : new Status
            {
                Id = Brand_BrandDTO.Status.Id,
                Code = Brand_BrandDTO.Status.Code,
                Name = Brand_BrandDTO.Status.Name,
            };
            Brand.BaseLanguage = CurrentContext.Language;
            return Brand;
        }

        private BrandFilter ConvertFilterDTOToFilterEntity(Brand_BrandFilterDTO Brand_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Skip = Brand_BrandFilterDTO.Skip;
            BrandFilter.Take = Brand_BrandFilterDTO.Take;
            BrandFilter.OrderBy = Brand_BrandFilterDTO.OrderBy;
            BrandFilter.OrderType = Brand_BrandFilterDTO.OrderType;

            BrandFilter.Id = Brand_BrandFilterDTO.Id;
            BrandFilter.Code = Brand_BrandFilterDTO.Code;
            BrandFilter.Name = Brand_BrandFilterDTO.Name;
            BrandFilter.Description = Brand_BrandFilterDTO.Description;
            BrandFilter.StatusId = Brand_BrandFilterDTO.StatusId;
            BrandFilter.UpdateTime = Brand_BrandFilterDTO.UpdateTime;

            BrandFilter.Search = Brand_BrandFilterDTO.Search;
            
            return BrandFilter;
        }

        [Route(BrandRoute.SingleListStatus), HttpPost]
        public async Task<List<Brand_StatusDTO>> SingleListStatus([FromBody] Brand_StatusFilterDTO Brand_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Brand_StatusFilterDTO.Id;
            StatusFilter.Code = Brand_StatusFilterDTO.Code;
            StatusFilter.Name = Brand_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Brand_StatusDTO> Brand_StatusDTOs = Statuses
                .Select(x => new Brand_StatusDTO(x)).ToList();
            return Brand_StatusDTOs;
        }
        [Route(BrandRoute.FilterListStatus), HttpPost]
        public async Task<List<Brand_StatusDTO>> FilterListStatus([FromBody] Brand_StatusFilterDTO Brand_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Brand_StatusFilterDTO.Id;
            StatusFilter.Code = Brand_StatusFilterDTO.Code;
            StatusFilter.Name = Brand_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Brand_StatusDTO> Brand_StatusDTOs = Statuses
                .Select(x => new Brand_StatusDTO(x)).ToList();
            return Brand_StatusDTOs;
        }

    }
}

