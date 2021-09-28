using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ELODIE.Rpc.product
{
    public class Product_VariationGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long ProductId { get; set; }
        public List<Product_VariationDTO> Variations { get; set; }
        public Product_VariationGroupingDTO() { }
        public Product_VariationGroupingDTO(VariationGrouping VariationGrouping)
        {
            this.Id = VariationGrouping.Id;
            this.Code = VariationGrouping.Code;
            this.Name = VariationGrouping.Name;
            this.ProductId = VariationGrouping.ProductId;
            this.Variations = VariationGrouping.Variations?.Select(x => new Product_VariationDTO(x)).ToList();

            this.Errors = VariationGrouping.Errors;
        }
    }

    public class Product_VariationGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }

        public IdFilter ProductId { get; set; }

        public VariationGroupingOrder OrderBy { get; set; }
    }
}