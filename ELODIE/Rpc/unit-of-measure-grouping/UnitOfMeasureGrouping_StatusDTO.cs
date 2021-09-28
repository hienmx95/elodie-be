using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGrouping_StatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public UnitOfMeasureGrouping_StatusDTO() { }
        public UnitOfMeasureGrouping_StatusDTO(Status Status)
        {

            this.Id = Status.Id;

            this.Code = Status.Code;

            this.Name = Status.Name;

        }
    }

    public class UnitOfMeasureGrouping_StatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}