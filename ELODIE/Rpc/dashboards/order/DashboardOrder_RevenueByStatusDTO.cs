using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.dashboards.order
{
    public class DashboardOrder_RevenueByStatusDTO : DataDTO
    {
        public decimal Total { get; set; }
        public List<DashboardOrder_RevenueByStatusContentDTO> Contents { get; set; }
    }
    public class DashboardOrder_RevenueByStatusContentDTO : DataDTO
    {
        public long RequestStateId { get; set; }
        public string RequestStateName { get; set; }
        public decimal Revenue { get; set; }
        public decimal Rate { get; set; }
    }

    public class DashboardOrder_RevenueByStatusFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
