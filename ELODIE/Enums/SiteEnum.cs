using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Enums
{
    public class SiteEnum
    {
        public static GenericEnum LANDING => new GenericEnum { Id = 100, Name = "Cổng thông tin Rạng Đông !", Code = "/landing-page" };
        public static GenericEnum ELODIE => new GenericEnum { Id = 1, Name = "ELODIE", Code = "/ELODIE/" };
        public static GenericEnum DMS => new GenericEnum { Id = 2, Name = "DMS", Code = "/dms/" };
        public static GenericEnum CRM => new GenericEnum { Id = 3, Name = "CRM", Code = "/crm/" };
        public static GenericEnum REPORT => new GenericEnum { Id = 4, Name = "REPORT", Code = "/report/" };
        public static GenericEnum AMS => new GenericEnum { Id = 6, Name = "AMS", Code = "/ams/" };
        public static GenericEnum CMS_EXPORT => new GenericEnum { Id = 7, Name = "CMS Export", Code = "/cms-export-admin/" };

        public static List<GenericEnum> SiteEnumList = new List<GenericEnum>
        {
            LANDING, ELODIE, REPORT, DMS, CRM, ELODIE, AMS, CMS_EXPORT
        };
    }
}
