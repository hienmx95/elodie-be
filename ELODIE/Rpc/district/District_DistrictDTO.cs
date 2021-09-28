using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.district
{
    public class District_DistrictDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long ProvinceId { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public District_ProvinceDTO Province { get; set; }
        public District_StatusDTO Status { get; set; }
        public District_DistrictDTO() { }
        public District_DistrictDTO(District District)
        {
            this.Id = District.Id;
            this.Code = District.Code;
            this.Name = District.Name;
            this.Priority = District.Priority;
            this.ProvinceId = District.ProvinceId;
            this.StatusId = District.StatusId;
            this.Used = District.Used;
            this.Province = District.Province == null ? null : new District_ProvinceDTO(District.Province);
            this.Status = District.Status == null ? null : new District_StatusDTO(District.Status);
            this.Errors = District.Errors;
        }
    }

    public class District_DistrictFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter StatusId { get; set; }
        public DistrictOrder OrderBy { get; set; }
    }
}
