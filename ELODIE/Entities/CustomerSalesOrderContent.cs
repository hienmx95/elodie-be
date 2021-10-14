using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class CustomerSalesOrderContent : DataEntity,  IEquatable<CustomerSalesOrderContent>
    {
        public long Id { get; set; }
        public long CustomerSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long RequestedQuantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PrimaryPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TaxPercentageOther { get; set; }
        public decimal? TaxAmountOther { get; set; }
        public decimal Amount { get; set; }
        public long? Factor { get; set; }
        public long EditedPriceStatusId { get; set; }
        public long TaxTypeId { get; set; }
        public CustomerSalesOrder CustomerSalesOrder { get; set; }
        public EditedPriceStatus EditedPriceStatus { get; set; }
        public UnitOfMeasure PrimaryUnitOfMeasure { get; set; }
        public TaxType TaxType { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public Item Item { get; set; }

        public bool Equals(CustomerSalesOrderContent other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.CustomerSalesOrderId != other.CustomerSalesOrderId) return false;
            if (this.ItemId != other.ItemId) return false;
            if (this.UnitOfMeasureId != other.UnitOfMeasureId) return false;
            if (this.Quantity != other.Quantity) return false;
            if (this.RequestedQuantity != other.RequestedQuantity) return false;
            if (this.PrimaryUnitOfMeasureId != other.PrimaryUnitOfMeasureId) return false;
            if (this.SalePrice != other.SalePrice) return false;
            if (this.PrimaryPrice != other.PrimaryPrice) return false;
            if (this.DiscountPercentage != other.DiscountPercentage) return false;
            if (this.DiscountAmount != other.DiscountAmount) return false;
            if (this.GeneralDiscountPercentage != other.GeneralDiscountPercentage) return false;
            if (this.GeneralDiscountAmount != other.GeneralDiscountAmount) return false;
            if (this.TaxPercentage != other.TaxPercentage) return false;
            if (this.TaxAmount != other.TaxAmount) return false;
            if (this.TaxPercentageOther != other.TaxPercentageOther) return false;
            if (this.TaxAmountOther != other.TaxAmountOther) return false;
            if (this.Amount != other.Amount) return false;
            if (this.Factor != other.Factor) return false;
            if (this.EditedPriceStatusId != other.EditedPriceStatusId) return false;
            if (this.TaxTypeId != other.TaxTypeId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CustomerSalesOrderContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter CustomerSalesOrderId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public LongFilter Quantity { get; set; }
        public LongFilter RequestedQuantity { get; set; }
        public IdFilter PrimaryUnitOfMeasureId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter PrimaryPrice { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public DecimalFilter DiscountAmount { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public DecimalFilter GeneralDiscountAmount { get; set; }
        public DecimalFilter TaxPercentage { get; set; }
        public DecimalFilter TaxAmount { get; set; }
        public DecimalFilter TaxPercentageOther { get; set; }
        public DecimalFilter TaxAmountOther { get; set; }
        public DecimalFilter Amount { get; set; }
        public LongFilter Factor { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public IdFilter TaxTypeId { get; set; }
        public List<CustomerSalesOrderContentFilter> OrFilter { get; set; }
        public CustomerSalesOrderContentOrder OrderBy {get; set;}
        public CustomerSalesOrderContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerSalesOrderContentOrder
    {
        Id = 0,
        CustomerSalesOrder = 1,
        Item = 2,
        UnitOfMeasure = 3,
        Quantity = 4,
        RequestedQuantity = 5,
        PrimaryUnitOfMeasure = 6,
        SalePrice = 7,
        PrimaryPrice = 8,
        DiscountPercentage = 9,
        DiscountAmount = 10,
        GeneralDiscountPercentage = 11,
        GeneralDiscountAmount = 12,
        TaxPercentage = 13,
        TaxAmount = 14,
        TaxPercentageOther = 15,
        TaxAmountOther = 16,
        Amount = 17,
        Factor = 18,
        EditedPriceStatus = 19,
        TaxType = 20,
    }

    [Flags]
    public enum CustomerSalesOrderContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        CustomerSalesOrder = E._1,
        Item = E._2,
        UnitOfMeasure = E._3,
        Quantity = E._4,
        RequestedQuantity = E._5,
        PrimaryUnitOfMeasure = E._6,
        SalePrice = E._7,
        PrimaryPrice = E._8,
        DiscountPercentage = E._9,
        DiscountAmount = E._10,
        GeneralDiscountPercentage = E._11,
        GeneralDiscountAmount = E._12,
        TaxPercentage = E._13,
        TaxAmount = E._14,
        TaxPercentageOther = E._15,
        TaxAmountOther = E._16,
        Amount = E._17,
        Factor = E._18,
        EditedPriceStatus = E._19,
        TaxType = E._20,
    }
}
