using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.ward
{
    public class Ward_WardDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long DistrictId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Ward_DistrictDTO District { get; set; }
        public Ward_StatusDTO Status { get; set; }
        public Ward_WardDTO() { }
        public Ward_WardDTO(Ward Ward)
        {
            this.Id = Ward.Id;
            this.Code = Ward.Code;
            this.Name = Ward.Name;
            this.Priority = Ward.Priority;
            this.DistrictId = Ward.DistrictId;
            this.StatusId = Ward.StatusId;
            this.Used = Ward.Used;
            this.District = Ward.District == null ? null : new Ward_DistrictDTO(Ward.District);
            this.Status = Ward.Status == null ? null : new Ward_StatusDTO(Ward.Status);
            this.Errors = Ward.Errors;
        }
    }

    public class Ward_WardFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter StatusId { get; set; }
        public WardOrder OrderBy { get; set; }
    }
}
