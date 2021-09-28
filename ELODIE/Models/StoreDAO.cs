using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class StoreDAO
    {
        public StoreDAO()
        {
            InverseParentStore = new HashSet<StoreDAO>();
            StoreImageMappings = new HashSet<StoreImageMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string CodeDraft { get; set; }
        public string Name { get; set; }
        public string UnsignName { get; set; }
        public long? ParentStoreId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreTypeId { get; set; }
        public long? StoreGroupingId { get; set; }
        public string Telephone { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string Address { get; set; }
        public string UnsignAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? DeliveryLatitude { get; set; }
        public decimal? DeliveryLongitude { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public string TaxCode { get; set; }
        public string LegalEntity { get; set; }
        public long? AppUserId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public long StoreStatusId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual DistrictDAO District { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StoreDAO ParentStore { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual StoreGroupingDAO StoreGrouping { get; set; }
        public virtual StoreStatusDAO StoreStatus { get; set; }
        public virtual StoreTypeDAO StoreType { get; set; }
        public virtual WardDAO Ward { get; set; }
        public virtual ICollection<StoreDAO> InverseParentStore { get; set; }
        public virtual ICollection<StoreImageMappingDAO> StoreImageMappings { get; set; }
    }
}
