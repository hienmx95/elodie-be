using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.dashboards.order
{
    public class DashboardOrder_RevenueBySourceDTO : DataDTO
    {
        public decimal Total { get; set; }
        public List<DashboardOrder_RevenueBySourceContentDTO> Contents { get; set; }
    }

    public class DashboardOrder_RevenueBySourceContentDTO : DataDTO
    {
        public long OrderSourceId { get; set; }
        public string OrderSourceName { get; set; }
        public decimal Revenue { get; set; }
        public decimal Rate { get; set; }
    }

    public class DashboardOrder_RevenueBySourceFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
