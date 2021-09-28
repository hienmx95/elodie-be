using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_ProductImageMappingDTO : DataDTO
    {
        public long ProductId { get; set; }
        public long ImageId { get; set; }
        public ProductGrouping_ImageDTO Image { get; set; }

        public ProductGrouping_ProductImageMappingDTO() { }
        public ProductGrouping_ProductImageMappingDTO(ProductImageMapping ProductImageMapping)
        {
            this.ProductId = ProductImageMapping.ProductId;
            this.ImageId = ProductImageMapping.ImageId;
            this.Image = ProductImageMapping.Image == null ? null : new ProductGrouping_ImageDTO(ProductImageMapping.Image);
            this.Errors = ProductImageMapping.Errors;
        }
    }
}