using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.tax_type
{
    public class TaxType_TaxTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public TaxType_StatusDTO Status { get; set; }
        public TaxType_TaxTypeDTO() { }
        public TaxType_TaxTypeDTO(TaxType TaxType)
        {
            this.Id = TaxType.Id;
            this.Code = TaxType.Code;
            this.Name = TaxType.Name;
            this.Percentage = TaxType.Percentage;
            this.StatusId = TaxType.StatusId;
            this.Used = TaxType.Used;
            this.Status = TaxType.Status == null ? null : new TaxType_StatusDTO(TaxType.Status);
            this.Errors = TaxType.Errors;
        }
    }

    public class TaxType_TaxTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public DecimalFilter Percentage { get; set; }
        public IdFilter StatusId { get; set; }
        public TaxTypeOrder OrderBy { get; set; }
        public string Search { get; set; }
    }
}
