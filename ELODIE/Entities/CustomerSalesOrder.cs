using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class CustomerSalesOrder : DataEntity,  IEquatable<CustomerSalesOrder>
    {
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
        public Guid RowId { get; set; }
        public long? CodeGeneratorRuleId { get; set; }
        public CodeGeneratorRule CodeGeneratorRule { get; set; }
        public AppUser Creator { get; set; }
        public Customer Customer { get; set; }
        public District DeliveryDistrict { get; set; }
        public Nation DeliveryNation { get; set; }
        public Province DeliveryProvince { get; set; }
        public Ward DeliveryWard { get; set; }
        public EditedPriceStatus EditedPriceStatus { get; set; }
        public District InvoiceDistrict { get; set; }
        public Nation InvoiceNation { get; set; }
        public Province InvoiceProvince { get; set; }
        public Ward InvoiceWard { get; set; }
        public OrderPaymentStatus OrderPaymentStatus { get; set; }
        public OrderSource OrderSource { get; set; }
        public Organization Organization { get; set; }
        public RequestState RequestState { get; set; }
        public AppUser SalesEmployee { get; set; }
        public List<CustomerSalesOrderContent> CustomerSalesOrderContents { get; set; }
        public List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories { get; set; }
        public List<CustomerSalesOrderPromotion> CustomerSalesOrderPromotions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(CustomerSalesOrder other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.CustomerId != other.CustomerId) return false;
            if (this.OrderSourceId != other.OrderSourceId) return false;
            if (this.RequestStateId != other.RequestStateId) return false;
            if (this.OrderPaymentStatusId != other.OrderPaymentStatusId) return false;
            if (this.EditedPriceStatusId != other.EditedPriceStatusId) return false;
            if (this.ShippingName != other.ShippingName) return false;
            if (this.OrderDate != other.OrderDate) return false;
            if (this.DeliveryDate != other.DeliveryDate) return false;
            if (this.SalesEmployeeId != other.SalesEmployeeId) return false;
            if (this.Note != other.Note) return false;
            if (this.InvoiceAddress != other.InvoiceAddress) return false;
            if (this.InvoiceNationId != other.InvoiceNationId) return false;
            if (this.InvoiceProvinceId != other.InvoiceProvinceId) return false;
            if (this.InvoiceDistrictId != other.InvoiceDistrictId) return false;
            if (this.InvoiceWardId != other.InvoiceWardId) return false;
            if (this.InvoiceZIPCode != other.InvoiceZIPCode) return false;
            if (this.DeliveryAddress != other.DeliveryAddress) return false;
            if (this.DeliveryNationId != other.DeliveryNationId) return false;
            if (this.DeliveryProvinceId != other.DeliveryProvinceId) return false;
            if (this.DeliveryDistrictId != other.DeliveryDistrictId) return false;
            if (this.DeliveryWardId != other.DeliveryWardId) return false;
            if (this.DeliveryZIPCode != other.DeliveryZIPCode) return false;
            if (this.SubTotal != other.SubTotal) return false;
            if (this.GeneralDiscountPercentage != other.GeneralDiscountPercentage) return false;
            if (this.GeneralDiscountAmount != other.GeneralDiscountAmount) return false;
            if (this.TotalTaxOther != other.TotalTaxOther) return false;
            if (this.TotalTax != other.TotalTax) return false;
            if (this.Total != other.Total) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.OrganizationId != other.OrganizationId) return false;
            if (this.RowId != other.RowId) return false;
            if (this.CodeGeneratorRuleId != other.CodeGeneratorRuleId) return false;
            if (this.CustomerSalesOrderContents?.Count != other.CustomerSalesOrderContents?.Count) return false;
            else if (this.CustomerSalesOrderContents != null && other.CustomerSalesOrderContents != null)
            {
                for (int i = 0; i < CustomerSalesOrderContents.Count; i++)
                {
                    CustomerSalesOrderContent CustomerSalesOrderContent = CustomerSalesOrderContents[i];
                    CustomerSalesOrderContent otherCustomerSalesOrderContent = other.CustomerSalesOrderContents[i];
                    if (CustomerSalesOrderContent == null && otherCustomerSalesOrderContent != null)
                        return false;
                    if (CustomerSalesOrderContent != null && otherCustomerSalesOrderContent == null)
                        return false;
                    if (CustomerSalesOrderContent.Equals(otherCustomerSalesOrderContent) == false)
                        return false;
                }
            }
            if (this.CustomerSalesOrderPaymentHistories?.Count != other.CustomerSalesOrderPaymentHistories?.Count) return false;
            else if (this.CustomerSalesOrderPaymentHistories != null && other.CustomerSalesOrderPaymentHistories != null)
            {
                for (int i = 0; i < CustomerSalesOrderPaymentHistories.Count; i++)
                {
                    CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory = CustomerSalesOrderPaymentHistories[i];
                    CustomerSalesOrderPaymentHistory otherCustomerSalesOrderPaymentHistory = other.CustomerSalesOrderPaymentHistories[i];
                    if (CustomerSalesOrderPaymentHistory == null && otherCustomerSalesOrderPaymentHistory != null)
                        return false;
                    if (CustomerSalesOrderPaymentHistory != null && otherCustomerSalesOrderPaymentHistory == null)
                        return false;
                    if (CustomerSalesOrderPaymentHistory.Equals(otherCustomerSalesOrderPaymentHistory) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CustomerSalesOrderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter CustomerId { get; set; }
        public IdFilter OrderSourceId { get; set; }
        public IdFilter RequestStateId { get; set; }
        public IdFilter OrderPaymentStatusId { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public StringFilter ShippingName { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter SalesEmployeeId { get; set; }
        public StringFilter Note { get; set; }
        public StringFilter InvoiceAddress { get; set; }
        public IdFilter InvoiceNationId { get; set; }
        public IdFilter InvoiceProvinceId { get; set; }
        public IdFilter InvoiceDistrictId { get; set; }
        public IdFilter InvoiceWardId { get; set; }
        public StringFilter InvoiceZIPCode { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public IdFilter DeliveryNationId { get; set; }
        public IdFilter DeliveryProvinceId { get; set; }
        public IdFilter DeliveryDistrictId { get; set; }
        public IdFilter DeliveryWardId { get; set; }
        public StringFilter DeliveryZIPCode { get; set; }
        public DecimalFilter SubTotal { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public DecimalFilter GeneralDiscountAmount { get; set; }
        public DecimalFilter TotalTaxOther { get; set; }
        public DecimalFilter TotalTax { get; set; }
        public DecimalFilter Total { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public GuidFilter RowId { get; set; }
        public IdFilter CodeGeneratorRuleId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<CustomerSalesOrderFilter> OrFilter { get; set; }
        public CustomerSalesOrderOrder OrderBy {get; set;}
        public CustomerSalesOrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerSalesOrderOrder
    {
        Id = 0,
        Code = 1,
        Customer = 2,
        OrderSource = 3,
        RequestState = 4,
        OrderPaymentStatus = 5,
        EditedPriceStatus = 6,
        ShippingName = 7,
        OrderDate = 8,
        DeliveryDate = 9,
        SalesEmployee = 10,
        Note = 11,
        InvoiceAddress = 12,
        InvoiceNation = 13,
        InvoiceProvince = 14,
        InvoiceDistrict = 15,
        InvoiceWard = 16,
        InvoiceZIPCode = 17,
        DeliveryAddress = 18,
        DeliveryNation = 19,
        DeliveryProvince = 20,
        DeliveryDistrict = 21,
        DeliveryWard = 22,
        DeliveryZIPCode = 23,
        SubTotal = 24,
        GeneralDiscountPercentage = 25,
        GeneralDiscountAmount = 26,
        TotalTaxOther = 27,
        TotalTax = 28,
        Total = 29,
        Creator = 30,
        Organization = 31,
        Row = 35,
        CodeGeneratorRule = 36,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum CustomerSalesOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Customer = E._2,
        OrderSource = E._3,
        RequestState = E._4,
        OrderPaymentStatus = E._5,
        EditedPriceStatus = E._6,
        ShippingName = E._7,
        OrderDate = E._8,
        DeliveryDate = E._9,
        SalesEmployee = E._10,
        Note = E._11,
        InvoiceAddress = E._12,
        InvoiceNation = E._13,
        InvoiceProvince = E._14,
        InvoiceDistrict = E._15,
        InvoiceWard = E._16,
        InvoiceZIPCode = E._17,
        DeliveryAddress = E._18,
        DeliveryNation = E._19,
        DeliveryProvince = E._20,
        DeliveryDistrict = E._21,
        DeliveryWard = E._22,
        DeliveryZIPCode = E._23,
        SubTotal = E._24,
        GeneralDiscountPercentage = E._25,
        GeneralDiscountAmount = E._26,
        TotalTaxOther = E._27,
        TotalTax = E._28,
        Total = E._29,
        Creator = E._30,
        Organization = E._31,
        Row = E._35,
        CodeGeneratorRule = E._36,
    }
}
