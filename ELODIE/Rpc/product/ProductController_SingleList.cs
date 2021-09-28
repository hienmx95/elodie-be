using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.product
{
    public partial class ProductController : RpcController
    {
        [Route(ProductRoute.SingleListBrand), HttpPost]
        public async Task<List<Product_BrandDTO>> SingleListBrand([FromBody] Product_BrandFilterDTO Product_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = Product_BrandFilterDTO.Id;
            BrandFilter.Code = Product_BrandFilterDTO.Code;
            BrandFilter.Name = Product_BrandFilterDTO.Name;
            BrandFilter.Description = Product_BrandFilterDTO.Description;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            BrandFilter.UpdateTime = Product_BrandFilterDTO.UpdateTime;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Product_BrandDTO> Product_BrandDTOs = Brands
                .Select(x => new Product_BrandDTO(x)).ToList();
            return Product_BrandDTOs;
        }

        [Route(ProductRoute.SingleListCategory), HttpPost]
        public async Task<List<Product_CategoryDTO>> SingleListCategory([FromBody] Product_CategoryFilterDTO Product_CategoryFilterDTO)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = Product_CategoryFilterDTO.Id;
            CategoryFilter.Code = Product_CategoryFilterDTO.Code;
            CategoryFilter.Name = Product_CategoryFilterDTO.Name;
            CategoryFilter.Level = Product_CategoryFilterDTO.Level;
            CategoryFilter.ParentId = Product_CategoryFilterDTO.ParentId;
            CategoryFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Category> Categorys = await CategoryService.List(CategoryFilter);
            List<Product_CategoryDTO> Product_CategoryDTOs = Categorys
                .Select(x => new Product_CategoryDTO(x)).ToList();
            return Product_CategoryDTOs;
        }

        [Route(ProductRoute.SingleListProductType), HttpPost]
        public async Task<List<Product_ProductTypeDTO>> SingleListProductType([FromBody] Product_ProductTypeFilterDTO Product_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = Product_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = Product_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = Product_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = Product_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<Product_ProductTypeDTO> Product_ProductTypeDTOs = ProductTypes
                .Select(x => new Product_ProductTypeDTO(x)).ToList();
            return Product_ProductTypeDTOs;
        }
        [Route(ProductRoute.SingleListStatus), HttpPost]
        public async Task<List<Product_StatusDTO>> SingleListStatus([FromBody] Product_StatusFilterDTO Product_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Product_StatusDTO> Product_StatusDTOs = Statuses
                .Select(x => new Product_StatusDTO(x)).ToList();
            return Product_StatusDTOs;
        }
        [Route(ProductRoute.SingleListSupplier), HttpPost]
        public async Task<List<Product_SupplierDTO>> SingleListSupplier([FromBody] Product_SupplierFilterDTO Product_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Product_SupplierFilterDTO.Id;
            SupplierFilter.Code = Product_SupplierFilterDTO.Code;
            SupplierFilter.Name = Product_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Product_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Product_SupplierDTO> Product_SupplierDTOs = Suppliers
                .Select(x => new Product_SupplierDTO(x)).ToList();
            return Product_SupplierDTOs;
        }
        [Route(ProductRoute.SingleListTaxType), HttpPost]
        public async Task<List<Product_TaxTypeDTO>> SingleListTaxType([FromBody] Product_TaxTypeFilterDTO Product_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = Product_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = Product_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = Product_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<Product_TaxTypeDTO> Product_TaxTypeDTOs = TaxTypes
                .Select(x => new Product_TaxTypeDTO(x)).ToList();
            return Product_TaxTypeDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<Product_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] Product_UnitOfMeasureFilterDTO Product_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = Product_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = Product_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = Product_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<Product_UnitOfMeasureDTO> Product_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new Product_UnitOfMeasureDTO(x)).ToList();
            return Product_UnitOfMeasureDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasureGrouping), HttpPost]
        public async Task<List<Product_UnitOfMeasureGroupingDTO>> SingleListUnitOfMeasureGrouping([FromBody] Product_UnitOfMeasureGroupingFilterDTO Product_UnitOfMeasureGroupingFilterDTO)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = 20;
            UnitOfMeasureGroupingFilter.OrderBy = UnitOfMeasureGroupingOrder.Id;
            UnitOfMeasureGroupingFilter.OrderType = OrderType.ASC;
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.ALL;
            UnitOfMeasureGroupingFilter.Id = Product_UnitOfMeasureGroupingFilterDTO.Id;
            UnitOfMeasureGroupingFilter.Name = Product_UnitOfMeasureGroupingFilterDTO.Name;
            UnitOfMeasureGroupingFilter.UnitOfMeasureId = Product_UnitOfMeasureGroupingFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            List<Product_UnitOfMeasureGroupingDTO> Product_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(x => new Product_UnitOfMeasureGroupingDTO(x)).ToList();
            return Product_UnitOfMeasureGroupingDTOs;
        }

        [Route(ProductRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> SingleListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }

        [Route(ProductRoute.SingleListUsedVariation), HttpPost]
        public async Task<List<Product_UsedVariationDTO>> SingleListUsedVariation([FromBody] Product_UsedVariationFilterDTO Product_UsedVariationFilterDTO)
        {
            UsedVariationFilter UsedVariationFilter = new UsedVariationFilter();
            UsedVariationFilter.Skip = 0;
            UsedVariationFilter.Take = 20;
            UsedVariationFilter.OrderBy = UsedVariationOrder.Id;
            UsedVariationFilter.OrderType = OrderType.ASC;
            UsedVariationFilter.Selects = UsedVariationSelect.ALL;

            List<UsedVariation> UsedVariationes = await UsedVariationService.List(UsedVariationFilter);
            List<Product_UsedVariationDTO> Product_UsedVariationDTOs = UsedVariationes
                .Select(x => new Product_UsedVariationDTO(x)).ToList();
            return Product_UsedVariationDTOs;
        }
    }
}

