using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.reports.report_customer_sales_order.report_customer_sales_order_by_item
{
    public class ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportCustomerSalesOrderByItem_ItemDetailDTO> ItemDetails { get; set; }
    }

    public class ReportCustomerSalesOrderByItem_ItemDetailDTO : DataDTO
    {
        public long STT { get; set; }
        public long ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public long SaleStock { get; set; }
        public long PromotionStock { get; set; }
        public decimal Discount { get; set; }
        public decimal Revenue { get; set; }
        public int SalesOrderCounter => CustomerSalesOrderIds.Count();
        public int BuyerStoreCounter => BuyerStoreIds.Count();
        internal HashSet<long> CustomerSalesOrderIds { get; set; }
        internal HashSet<long> BuyerStoreIds { get; set; }
    }

    public class ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ItemId { get; set; }
        //public IdFilter ProductGroupingId { get; set; }
        //public IdFilter ProductTypeId { get; set; }
        public DateFilter Date { get; set; }
        internal bool HasValue => (ItemId != null && ItemId.HasValue) ||
            //(ProductGroupingId != null && ProductGroupingId.HasValue) ||
            //(ProductTypeId != null && ProductTypeId.HasValue) ||
            (Date != null && Date.HasValue);
    }
}
