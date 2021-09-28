using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Services.MProductType;
using ELODIE.Services.MStatus;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.product_type
{


    public class ProductTypeController : RpcController
    {
        private IStatusService StatusService;
        private IProductTypeService ProductTypeService;
        private ICurrentContext CurrentContext;
        public ProductTypeController(
            IStatusService StatusService,
            IProductTypeService ProductTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.ProductTypeService = ProductTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO);
            int count = await ProductTypeService.Count(ProductTypeFilter);
            return count;
        }

        [Route(ProductTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<ProductType_ProductTypeDTO>>> List([FromBody] ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO);
            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ProductType_ProductTypeDTO> ProductType_ProductTypeDTOs = ProductTypes
                .Select(c => new ProductType_ProductTypeDTO(c)).ToList();
            return ProductType_ProductTypeDTOs;
        }

        [Route(ProductTypeRoute.Get), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Get([FromBody]ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = await ProductTypeService.Get(ProductType_ProductTypeDTO.Id);
            return new ProductType_ProductTypeDTO(ProductType);
        }

        [Route(ProductTypeRoute.Create), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Create([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Create(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.Update), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Update([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Update(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.Delete), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Delete([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Delete(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Id = new IdFilter { In = Ids };
            ProductTypeFilter.Selects = ProductTypeSelect.Id;
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = int.MaxValue;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            ProductTypes = await ProductTypeService.BulkDelete(ProductTypes);
            if (ProductTypes.Any(x => !x.IsValidated))
                return BadRequest(ProductTypes.Where(x => !x.IsValidated));
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            if (Id == 0)
            {

            }
            else
            {
                ProductTypeFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductTypeService.Count(ProductTypeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ProductType ConvertDTOToEntity(ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            ProductType ProductType = new ProductType();
            ProductType.Id = ProductType_ProductTypeDTO.Id;
            ProductType.Code = ProductType_ProductTypeDTO.Code;
            ProductType.Name = ProductType_ProductTypeDTO.Name;
            ProductType.Description = ProductType_ProductTypeDTO.Description;
            ProductType.StatusId = ProductType_ProductTypeDTO.StatusId;
            ProductType.UpdatedAt = ProductType_ProductTypeDTO.UpdateTime;
            ProductType.Status = ProductType_ProductTypeDTO.Status == null ? null : new Status
            {
                Id = ProductType_ProductTypeDTO.Status.Id,
                Code = ProductType_ProductTypeDTO.Status.Code,
                Name = ProductType_ProductTypeDTO.Status.Name,
            };
            ProductType.BaseLanguage = CurrentContext.Language;
            return ProductType;
        }

        private ProductTypeFilter ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Skip = ProductType_ProductTypeFilterDTO.Skip;
            ProductTypeFilter.Take = ProductType_ProductTypeFilterDTO.Take;
            ProductTypeFilter.OrderBy = ProductType_ProductTypeFilterDTO.OrderBy;
            ProductTypeFilter.OrderType = ProductType_ProductTypeFilterDTO.OrderType;

            ProductTypeFilter.Id = ProductType_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ProductType_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ProductType_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = ProductType_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = ProductType_ProductTypeFilterDTO.StatusId;
            ProductTypeFilter.UpdatedTime = ProductType_ProductTypeFilterDTO.UpdatedTime;

            ProductTypeFilter.Search = ProductType_ProductTypeFilterDTO.Search;
            
            return ProductTypeFilter;
        }

        [Route(ProductTypeRoute.SingleListStatus), HttpPost]
        public async Task<List<ProductType_StatusDTO>> SingleListStatus([FromBody] ProductType_StatusFilterDTO ProductType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ProductType_StatusDTO> ProductType_StatusDTOs = Statuses
                .Select(x => new ProductType_StatusDTO(x)).ToList();
            return ProductType_StatusDTOs;
        }

        [Route(ProductTypeRoute.FilterListStatus), HttpPost]
        public async Task<List<ProductType_StatusDTO>> FilterListStatus([FromBody] ProductType_StatusFilterDTO ProductType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ProductType_StatusDTO> ProductType_StatusDTOs = Statuses
                .Select(x => new ProductType_StatusDTO(x)).ToList();
            return ProductType_StatusDTOs;
        }

    }
}

