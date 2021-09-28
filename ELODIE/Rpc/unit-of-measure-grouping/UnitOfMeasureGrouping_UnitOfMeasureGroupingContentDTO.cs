using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long UnitOfMeasureGroupingId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? Factor { get; set; }
        public UnitOfMeasureGrouping_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO() { }
        public UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            this.Id = UnitOfMeasureGroupingContent.Id;
            this.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
            this.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
            this.Factor = UnitOfMeasureGroupingContent.Factor;
            this.UnitOfMeasure = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? null : new UnitOfMeasureGrouping_UnitOfMeasureDTO(UnitOfMeasureGroupingContent.UnitOfMeasure);
            this.Errors = UnitOfMeasureGroupingContent.Errors;
        }
    }

    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingContentFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter UnitOfMeasureGroupingId { get; set; }

        public IdFilter UnitOfMeasureId { get; set; }

        public LongFilter Factor { get; set; }

        public UnitOfMeasureGroupingContentOrder OrderBy { get; set; }
    }
}