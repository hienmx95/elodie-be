using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Services.MProduct;
using ELODIE.Services.MProductGrouping;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Services.MBrand;
using ELODIE.Services.MCategory;
using ELODIE.Services.MStatus;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGroupingController : RpcController
    {
        private IProductService ProductService;
        private IProductGroupingService ProductGroupingService;
        private IBrandService BrandService;
        private ICategoryService CategoryService;
        private ICurrentContext CurrentContext;
        private IStatusService StatusService;
        public ProductGroupingController(
            IProductService ProductService,
            IBrandService BrandService,
            ICategoryService CategoryService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext,
            IStatusService StatusService
        )
        {
            this.ProductService = ProductService;
            this.BrandService = BrandService;
            this.CategoryService = CategoryService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
            this.StatusService = StatusService;
        }

        [Route(ProductGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO);
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            int count = await ProductGroupingService.Count(ProductGroupingFilter);
            return count;
        }

        [Route(ProductGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<ProductGrouping_ProductGroupingDTO>>> List([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO);
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs = ProductGroupings
                .Select(c => new ProductGrouping_ProductGroupingDTO(c)).ToList();
            return ProductGrouping_ProductGroupingDTOs;
        }

        [Route(ProductGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Get([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = await ProductGroupingService.Get(ProductGrouping_ProductGroupingDTO.Id);
            return new ProductGrouping_ProductGroupingDTO(ProductGrouping);
        }

        [Route(ProductGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Create([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Create(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        [Route(ProductGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Update([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Update(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        [Route(ProductGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Delete([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Delete(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProductGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductGroupingService.Count(ProductGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ProductGrouping ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            ProductGrouping ProductGrouping = new ProductGrouping();
            ProductGrouping.Id = ProductGrouping_ProductGroupingDTO.Id;
            ProductGrouping.Code = ProductGrouping_ProductGroupingDTO.Code;
            ProductGrouping.Name = ProductGrouping_ProductGroupingDTO.Name;
            ProductGrouping.Description = ProductGrouping_ProductGroupingDTO.Description;
            ProductGrouping.StatusId = ProductGrouping_ProductGroupingDTO.StatusId;
            ProductGrouping.ParentId = ProductGrouping_ProductGroupingDTO.ParentId;
            ProductGrouping.HasChildren = ProductGrouping_ProductGroupingDTO.HasChildren;
            ProductGrouping.Path = ProductGrouping_ProductGroupingDTO.Path;
            ProductGrouping.Level = ProductGrouping_ProductGroupingDTO.Level;
            ProductGrouping.RowId = ProductGrouping_ProductGroupingDTO.RowId;
            ProductGrouping.Parent = ProductGrouping_ProductGroupingDTO.Parent == null ? null : new ProductGrouping
            {
                Id = ProductGrouping_ProductGroupingDTO.Parent.Id,
                Code = ProductGrouping_ProductGroupingDTO.Parent.Code,
                Name = ProductGrouping_ProductGroupingDTO.Parent.Name,
                Description = ProductGrouping_ProductGroupingDTO.Parent.Description,
                StatusId = ProductGrouping_ProductGroupingDTO.Parent.StatusId,
                ParentId = ProductGrouping_ProductGroupingDTO.Parent.ParentId,
                HasChildren = ProductGrouping_ProductGroupingDTO.Parent.HasChildren,
                Path = ProductGrouping_ProductGroupingDTO.Parent.Path,
                Level = ProductGrouping_ProductGroupingDTO.Parent.Level,
                RowId = ProductGrouping_ProductGroupingDTO.Parent.RowId,
            };
            ProductGrouping.Status = ProductGrouping_ProductGroupingDTO.Status == null ? null : new Status
            {
                Id = ProductGrouping_ProductGroupingDTO.Status.Id,
                Code = ProductGrouping_ProductGroupingDTO.Status.Code,
                Name = ProductGrouping_ProductGroupingDTO.Status.Name,
            };
            ProductGrouping.ProductProductGroupingMappings = ProductGrouping_ProductGroupingDTO.ProductProductGroupingMappings?
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    Product = x.Product == null ? null : new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ProductTypeId = x.Product.ProductTypeId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                        OtherName = x.Product.OtherName,
                        TechnicalName = x.Product.TechnicalName,
                        Note = x.Product.Note,
                    },
                }).ToList();
            ProductGrouping.BaseLanguage = CurrentContext.Language;
            return ProductGrouping;
        }

        private ProductGroupingFilter ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = 99999;
            ProductGroupingFilter.OrderBy = ProductGrouping_ProductGroupingFilterDTO.OrderBy;
            ProductGroupingFilter.OrderType = ProductGrouping_ProductGroupingFilterDTO.OrderType;

            ProductGroupingFilter.Id = ProductGrouping_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = ProductGrouping_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ProductGrouping_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.Description = ProductGrouping_ProductGroupingFilterDTO.Description;
            ProductGroupingFilter.StatusId = ProductGrouping_ProductGroupingFilterDTO.StatusId;
            ProductGroupingFilter.ParentId = ProductGrouping_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = ProductGrouping_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Level = ProductGrouping_ProductGroupingFilterDTO.Level;
            ProductGroupingFilter.RowId = ProductGrouping_ProductGroupingFilterDTO.RowId;
            ProductGroupingFilter.CreatedAt = ProductGrouping_ProductGroupingFilterDTO.CreatedAt;
            ProductGroupingFilter.UpdatedAt = ProductGrouping_ProductGroupingFilterDTO.UpdatedAt;
            return ProductGroupingFilter;
        }


        #region filterList, singleList

        [Route(ProductGroupingRoute.SingleListStatus), HttpPost]
        public async Task<List<ProductGrouping_StatusDTO>> SingleListStatus([FromBody] ProductGrouping_StatusFilterDTO ProductGrouping_StatusFilterDTO)
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
            List<ProductGrouping_StatusDTO> ProductGrouping_StatusDTOs = Statuses
                .Select(x => new ProductGrouping_StatusDTO(x)).ToList();
            return ProductGrouping_StatusDTOs;
        }

        [Route(ProductGroupingRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<ProductGrouping_ProductGroupingDTO>> SingleListProductGrouping([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = 99999;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs = ProductGroupings
                .Select(x => new ProductGrouping_ProductGroupingDTO(x)).ToList();
            return ProductGrouping_ProductGroupingDTOs;
        }
        [Route(ProductGroupingRoute.SingleListProduct), HttpPost]
        public async Task<List<ProductGrouping_ProductDTO>> SingleListProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = 0;
            ProductFilter.Take = 20;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Product> Products = await ProductService.List(ProductFilter);
            List<ProductGrouping_ProductDTO> ProductGrouping_ProductDTOs = Products
                .Select(x => new ProductGrouping_ProductDTO(x)).ToList();
            return ProductGrouping_ProductDTOs;
        }

        [Route(ProductGroupingRoute.FilterListBrand), HttpPost]
        public async Task<List<ProductGrouping_BrandDTO>> SingleListBrand([FromBody] ProductGrouping_BrandFilterDTO ProductGrouping_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.Id | BrandSelect.Code | BrandSelect.Name;
            BrandFilter.Id = ProductGrouping_BrandFilterDTO.Id;
            BrandFilter.Code = ProductGrouping_BrandFilterDTO.Code;
            BrandFilter.Name = ProductGrouping_BrandFilterDTO.Name;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Brand> Brandes = await BrandService.List(BrandFilter);
            List<ProductGrouping_BrandDTO> ProductGrouping_BrandDTOs = Brandes
                .Select(x => new ProductGrouping_BrandDTO(x)).ToList();
            return ProductGrouping_BrandDTOs;
        }

        [Route(ProductGroupingRoute.FilterListCategory), HttpPost]
        public async Task<List<ProductGrouping_CategoryDTO>> FilterListCategory([FromBody] ProductGrouping_CategoryFilterDTO ProductGrouping_CategoryFilterDTO)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = ProductGrouping_CategoryFilterDTO.Id;
            CategoryFilter.Code = ProductGrouping_CategoryFilterDTO.Code;
            CategoryFilter.Name = ProductGrouping_CategoryFilterDTO.Name;

            List<Category> Categorys = await CategoryService.List(CategoryFilter);
            List<ProductGrouping_CategoryDTO> ProductGrouping_CategoryDTOs = Categorys
                .Select(x => new ProductGrouping_CategoryDTO(x)).ToList();
            return ProductGrouping_CategoryDTOs;
        }
        #endregion


        [Route(ProductGroupingRoute.CountProduct), HttpPost]
        public async Task<long> CountProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = ProductGrouping_ProductFilterDTO.ProductTypeId;
            ProductFilter.BrandId = ProductGrouping_ProductFilterDTO.BrandId;
            ProductFilter.CategoryId = ProductGrouping_ProductFilterDTO.CategoryId;
            ProductFilter.UnitOfMeasureId = ProductGrouping_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = ProductGrouping_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = ProductGrouping_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = ProductGrouping_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = ProductGrouping_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = ProductGrouping_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = ProductGrouping_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = ProductGrouping_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = ProductGrouping_ProductFilterDTO.Note;

            ProductFilter.ProductGroupingId = ProductGrouping_ProductFilterDTO.ProductGroupingId;
            ProductFilter.Search = ProductGrouping_ProductFilterDTO.Search;

            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await ProductService.Count(ProductFilter);
        }

        [Route(ProductGroupingRoute.ListProduct), HttpPost]
        public async Task<List<ProductGrouping_ProductDTO>> ListProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = ProductGrouping_ProductFilterDTO.Skip;
            ProductFilter.Take = ProductGrouping_ProductFilterDTO.Take;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = ProductGrouping_ProductFilterDTO.ProductTypeId;
            ProductFilter.BrandId = ProductGrouping_ProductFilterDTO.BrandId;
            ProductFilter.CategoryId = ProductGrouping_ProductFilterDTO.CategoryId;
            ProductFilter.BrandId = ProductGrouping_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = ProductGrouping_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = ProductGrouping_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = ProductGrouping_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = ProductGrouping_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = ProductGrouping_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = ProductGrouping_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = ProductGrouping_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = ProductGrouping_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = ProductGrouping_ProductFilterDTO.Note;

            ProductFilter.ProductGroupingId = ProductGrouping_ProductFilterDTO.ProductGroupingId;
            ProductFilter.Search = ProductGrouping_ProductFilterDTO.Search;

            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Product> Products = await ProductService.List(ProductFilter);
            List<ProductGrouping_ProductDTO> ProductGrouping_ProductDTOs = Products
                .Select(x => new ProductGrouping_ProductDTO(x)).ToList();
            return ProductGrouping_ProductDTOs;
        }

        [Route(ProductGroupingRoute.BulkCreate), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> BulkCreate([FromBody] List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs)
        {
            if (!ModelState.IsValid)
            {
                throw new BindException(ModelState);
            }
            List<ProductGrouping> ProductGroupings = ProductGrouping_ProductGroupingDTOs
                .Select(q => ConvertDTOToEntity(q))
                .ToList();
            ProductGroupings = await ProductGroupingService.Import(ProductGroupings);
            if (ProductGroupings.Any(q => q.IsValidated == false))
                return BadRequest(ProductGroupings);
            return Ok();
        }

        [Route(ProductGroupingRoute.BulkDeleteProduct), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> ProductIds)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = ProductIds };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);
            Products = await ProductService.BulkDelete(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ProductGroupingRoute.DeleteProduct), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductDTO>> DeleteProduct([FromBody] ProductGrouping_ProductDTO ProductGrouping_ProductDTO)
        {
            if (!ModelState.IsValid)
            {
                throw new BindException(ModelState);
            }
            Product Product = new Product
            {
                Id = ProductGrouping_ProductDTO.Id,
            };
            Product = await ProductService.Delete(Product);
            if (!Product.IsValidated)
                return BadRequest(Product);
            return Ok();
        }

    }
}

