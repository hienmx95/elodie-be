using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.nation
{
    public class Nation_NationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Nation_StatusDTO Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Nation_NationDTO() {}
        public Nation_NationDTO(Nation Nation)
        {
            this.Id = Nation.Id;
            this.Code = Nation.Code;
            this.Name = Nation.Name;
            this.Priority = Nation.Priority;
            this.StatusId = Nation.StatusId;
            this.Used = Nation.Used;
            this.RowId = Nation.RowId;
            this.Status = Nation.Status == null ? null : new Nation_StatusDTO(Nation.Status);
            this.CreatedAt = Nation.CreatedAt;
            this.UpdatedAt = Nation.UpdatedAt;
            this.Errors = Nation.Errors;
        }
    }

    public class Nation_NationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public NationOrder OrderBy { get; set; }
    }
}
