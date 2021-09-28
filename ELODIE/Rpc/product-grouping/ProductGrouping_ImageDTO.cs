using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_ImageDTO : DataDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public ProductGrouping_ImageDTO() { }
        public ProductGrouping_ImageDTO(Image Image)
        {
            this.Id = Image.Id;

            this.Name = Image.Name;

            this.Url = Image.Url;

            this.ThumbnailUrl = Image.ThumbnailUrl;

            this.Errors = Image.Errors;
        }
    }
}