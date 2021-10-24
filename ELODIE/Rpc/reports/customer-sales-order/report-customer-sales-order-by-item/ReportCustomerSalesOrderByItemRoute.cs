using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.reports.report_customer_sales_order.report_customer_sales_order_by_item
{
    [DisplayName("Báo cáo đơn hàng trực tiếp theo sản phẩm")]
    public class ReportCustomerSalesOrderByItemRoute : Root
    {
        public const string Parent = Module + "/customer-sales-order-report";
        public const string Master = Module + "/customer-sales-order-report/customer-sales-order-item-report-master";

        private const string Default = Rpc + Module + "/customer-sales-order-item-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            //{ nameof(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductGroupingId), FieldTypeEnum.ID.Id },
            //{ nameof(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductTypeId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Total, Export, 
                FilterListOrganization, FilterListItem, FilterListProductType, FilterListProductGrouping  } },

        };
    }
}
