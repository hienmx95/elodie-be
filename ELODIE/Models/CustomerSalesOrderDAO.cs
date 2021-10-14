using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerSalesOrderDAO
    {
        public CustomerSalesOrderDAO()
        {
            CustomerSalesOrderContents = new HashSet<CustomerSalesOrderContentDAO>();
            CustomerSalesOrderPaymentHistories = new HashSet<CustomerSalesOrderPaymentHistoryDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long CustomerId { get; set; }
        public long? OrderSourceId { get; set; }
        public long? RequestStateId { get; set; }
        public long? OrderPaymentStatusId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string ShippingName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long SalesEmployeeId { get; set; }
        public string Note { get; set; }
        public string InvoiceAddress { get; set; }
        public long? InvoiceNationId { get; set; }
        public long? InvoiceProvinceId { get; set; }
        public long? InvoiceDistrictId { get; set; }
        public long? InvoiceWardId { get; set; }
        public string InvoiceZIPCode { get; set; }
        public string DeliveryAddress { get; set; }
        public long? DeliveryNationId { get; set; }
        public long? DeliveryProvinceId { get; set; }
        public long? DeliveryDistrictId { get; set; }
        public long? DeliveryWardId { get; set; }
        public string DeliveryZIPCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal TotalTaxOther { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Total { get; set; }
        public long CreatorId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public long? CodeGeneratorRuleId { get; set; }

        public virtual CodeGeneratorRuleDAO CodeGeneratorRule { get; set; }
        public virtual AppUserDAO Creator { get; set; }
        public virtual CustomerDAO Customer { get; set; }
        public virtual DistrictDAO DeliveryDistrict { get; set; }
        public virtual NationDAO DeliveryNation { get; set; }
        public virtual ProvinceDAO DeliveryProvince { get; set; }
        public virtual WardDAO DeliveryWard { get; set; }
        public virtual EditedPriceStatusDAO EditedPriceStatus { get; set; }
        public virtual DistrictDAO InvoiceDistrict { get; set; }
        public virtual NationDAO InvoiceNation { get; set; }
        public virtual ProvinceDAO InvoiceProvince { get; set; }
        public virtual WardDAO InvoiceWard { get; set; }
        public virtual OrderPaymentStatusDAO OrderPaymentStatus { get; set; }
        public virtual OrderSourceDAO OrderSource { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual RequestStateDAO RequestState { get; set; }
        public virtual AppUserDAO SalesEmployee { get; set; }
        public virtual ICollection<CustomerSalesOrderContentDAO> CustomerSalesOrderContents { get; set; }
        public virtual ICollection<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistories { get; set; }
    }
}
