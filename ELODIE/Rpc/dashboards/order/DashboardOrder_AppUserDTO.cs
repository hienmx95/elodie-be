using ELODIE.Common;
using ELODIE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.dashboards.order
{
    public class DashboardOrder_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public DashboardOrder_AppUserDTO() { }
        public DashboardOrder_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Phone = AppUser.Phone;
            //this.Latitude = AppUser.Latitude;
            //this.Longitude = AppUser.Longitude;
            this.Errors = AppUser.Errors;
        }
    }

    public class DashboardOrder_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }

        public StringFilter Username { get; set; }

        public StringFilter Password { get; set; }

        public StringFilter DisplayName { get; set; }

        public StringFilter Address { get; set; }

        public StringFilter Email { get; set; }

        public StringFilter Phone { get; set; }

        public IdFilter PositionId { get; set; }

        public StringFilter Department { get; set; }

        public IdFilter OrganizationId { get; set; }

        public IdFilter SexId { get; set; }

        public IdFilter StatusId { get; set; }

        public StringFilter Avatar { get; set; }

        public DateFilter Birthday { get; set; }

        public IdFilter ProvinceId { get; set; }

        public AppUserOrder OrderBy { get; set; }
    }
}
