using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_ProductProductGroupingMappingDTO : DataDTO
    {
        public long ProductId { get; set; }
        public long ProductGroupingId { get; set; }
        public ProductGrouping_ProductDTO Product { get; set; }
        public ProductGrouping_ProductGroupingDTO ProductGrouping { get; set; }

        public ProductGrouping_ProductProductGroupingMappingDTO() { }
        public ProductGrouping_ProductProductGroupingMappingDTO(ProductProductGroupingMapping ProductProductGroupingMapping)
        {
            this.ProductId = ProductProductGroupingMapping.ProductId;
            this.ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId;
            this.Product = ProductProductGroupingMapping.Product == null ? null : new ProductGrouping_ProductDTO(ProductProductGroupingMapping.Product);
            this.ProductGrouping = ProductProductGroupingMapping.ProductGrouping == null ? null : new ProductGrouping_ProductGroupingDTO(ProductProductGroupingMapping.ProductGrouping);
        }
    }

    public class ProductGrouping_ProductProductGroupingMappingFilterDTO : FilterDTO
    {

        public IdFilter ProductId { get; set; }

        public IdFilter ProductGroupingId { get; set; }

        public ProductProductGroupingMappingOrder OrderBy { get; set; }
    }
}