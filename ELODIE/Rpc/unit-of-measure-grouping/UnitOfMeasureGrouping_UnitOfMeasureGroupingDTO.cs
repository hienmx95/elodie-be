using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ELODIE.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public UnitOfMeasureGrouping_StatusDTO Status { get; set; }
        public UnitOfMeasureGrouping_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public List<UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO> UnitOfMeasureGroupingContents { get; set; }
        public UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO() { }
        public UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            this.Id = UnitOfMeasureGrouping.Id;
            this.Code = UnitOfMeasureGrouping.Code;
            this.Name = UnitOfMeasureGrouping.Name;
            this.Description = UnitOfMeasureGrouping.Description;
            this.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
            this.StatusId = UnitOfMeasureGrouping.StatusId;
            this.Used = UnitOfMeasureGrouping.Used;
            this.Status = UnitOfMeasureGrouping.Status == null ? null : new UnitOfMeasureGrouping_StatusDTO(UnitOfMeasureGrouping.Status);
            this.UnitOfMeasure = UnitOfMeasureGrouping.UnitOfMeasure == null ? null : new UnitOfMeasureGrouping_UnitOfMeasureDTO(UnitOfMeasureGrouping.UnitOfMeasure);
            this.UnitOfMeasureGroupingContents = UnitOfMeasureGrouping.UnitOfMeasureGroupingContents?.Select(x => new UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO(x)).ToList();
            this.Errors = UnitOfMeasureGrouping.Errors;
        }
    }

    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter StatusId { get; set; }
        public UnitOfMeasureGroupingOrder OrderBy { get; set; }
        public string Search { get; set; }
    }
}
