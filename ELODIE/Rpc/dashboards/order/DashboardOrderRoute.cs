using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.dashboards.order
{
    [DisplayName("Dashboard đơn hàng")]
    public class DashboardOrderRoute : Root
    {
        public const string Parent = Module + "/dashboards";
        public const string Master = Module + "/dashboards/order";
        private const string Default = Rpc + Module + "/dashboards/order";
        public const string TotalRevenue = Default + "/total-revenue";
        public const string OrderCounter = Default + "/order-counter";
        public const string CompletedOrderCounter = Default + "/completed-order-counter";
        public const string ProcessingOrderCounter = Default + "/processing-order-counter";
        public const string RejectedOrderCounter = Default + "/rejected-order-counter";
        public const string RevenueBySource = Default + "/revenue-by-source";
        public const string RevenueByTime = Default + "/revenue-by-time";
        public const string RevenueByStatus = Default + "/revenue-by-status";


        public const string FilterListTime = Default + "/filter-list-time";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProvince = Default + "/filter-list-province";

        public const string ListOrderItem = Default + "/list-order-item";
        public const string CountItem = Default + "/count-item";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(DashboardOrder_RevenueByTimeFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Hiển thị", new List<string> {
                Parent, CompletedOrderCounter, ProcessingOrderCounter,
                Master, TotalRevenue, OrderCounter, RejectedOrderCounter, RevenueBySource, RevenueByTime, RevenueByStatus,
                FilterListTime, FilterListAppUser, FilterListOrganization, FilterListProvince, ListOrderItem, CountItem
            } },
        };
    }
}
