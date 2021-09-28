using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_UnitOfMeasureGroupingDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public long UnitOfMeasureId { get; set; }

        public long StatusId { get; set; }


        public ProductGrouping_UnitOfMeasureGroupingDTO() { }
        public ProductGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {

            this.Id = UnitOfMeasureGrouping.Id;

            this.Name = UnitOfMeasureGrouping.Name;

            this.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;

            this.StatusId = UnitOfMeasureGrouping.StatusId;

        }
    }

    public class ProductGrouping_UnitOfMeasureGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter UnitOfMeasureId { get; set; }

        public IdFilter StatusId { get; set; }

        public UnitOfMeasureGroupingOrder OrderBy { get; set; }
    }
}
