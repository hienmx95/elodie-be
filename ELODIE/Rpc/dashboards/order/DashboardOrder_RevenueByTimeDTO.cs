using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.dashboards.order
{
    public class DashboardOrder_RevenueByTimeDTO : DataDTO
    {
        public List<DashboardOrder_RevenueByTimeByMonthDTO> Month { get; set; }
        public List<DashboardOrder_RevenueByTimeByQuarterDTO> Quarter { get; set; }
        public List<DashboardOrder_RevenueByTimeByYearDTO> Year { get; set; }
    }

    public class DashboardOrder_RevenueByTimeByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal Win { get; set; }
        public decimal Lost { get; set; }
    }

    public class DashboardOrder_RevenueByTimeByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Win { get; set; }
        public decimal Lost { get; set; }
    }

    public class DashboardOrder_RevenueByTimeByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Win { get; set; }
        public decimal Lost { get; set; }
    }

    public class DashboardOrder_RevenueByTimeFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
