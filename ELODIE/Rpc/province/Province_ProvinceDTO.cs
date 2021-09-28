using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.province
{
    public class Province_ProvinceDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Province_StatusDTO Status { get; set; }
        public Province_ProvinceDTO() { }
        public Province_ProvinceDTO(Province Province)
        {
            this.Id = Province.Id;
            this.Code = Province.Code;
            this.Name = Province.Name;
            this.Priority = Province.Priority;
            this.StatusId = Province.StatusId;
            this.Used = Province.Used;
            this.Status = Province.Status == null ? null : new Province_StatusDTO(Province.Status);
            this.Errors = Province.Errors;
        }
    }

    public class Province_ProvinceFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public IdFilter StatusId { get; set; }
        public ProvinceOrder OrderBy { get; set; }
    }
}
