using ELODIE.Common;
using ELODIE.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.product
{
    public partial class ProductController : RpcController
    {
        [Route(ProductRoute.FilterListStatus), HttpPost]
        public async Task<List<Product_StatusDTO>> FilterListStatus([FromBody] Product_StatusFilterDTO Product_StatusFilterDTO)
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
        [Route(ProductRoute.FilterListSupplier), HttpPost]
        public async Task<List<Product_SupplierDTO>> FilterListSupplier([FromBody] Product_SupplierFilterDTO Product_SupplierFilterDTO)
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

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Product_SupplierDTO> Product_SupplierDTOs = Suppliers
                .Select(x => new Product_SupplierDTO(x)).ToList();
            return Product_SupplierDTOs;
        }

        [Route(ProductRoute.FilterListCategory), HttpPost]
        public async Task<List<Product_CategoryDTO>> FilterListCategory([FromBody] Product_CategoryFilterDTO Product_CategoryFilterDTO)
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

            List<Category> Categorys = await CategoryService.List(CategoryFilter);
            List<Product_CategoryDTO> Product_CategoryDTOs = Categorys
                .Select(x => new Product_CategoryDTO(x)).ToList();
            return Product_CategoryDTOs;
        }

        [Route(ProductRoute.FilterListProductType), HttpPost]
        public async Task<List<Product_ProductTypeDTO>> FilterListProductType([FromBody] Product_ProductTypeFilterDTO Product_ProductTypeFilterDTO)
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

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<Product_ProductTypeDTO> Product_ProductTypeDTOs = ProductTypes
                .Select(x => new Product_ProductTypeDTO(x)).ToList();
            return Product_ProductTypeDTOs;
        }
        [Route(ProductRoute.FilterListUsedVariation), HttpPost]
        public async Task<List<Product_UsedVariationDTO>> FilterListUsedVariation([FromBody] Product_UsedVariationFilterDTO Product_UsedVariationFilterDTO)
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
        [Route(ProductRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> FilterListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
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
    }
}