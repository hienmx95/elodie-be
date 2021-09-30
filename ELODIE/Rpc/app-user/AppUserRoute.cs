using ELODIE.Common;
using ELODIE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.app_user
{
    public class AppUserRoute : Root
    {
        public const string Parent = Module + "/";
        public const string Master = Module + "/app-user/app-user-master";
        public const string Detail = Module + "/app-user/app-user-detail";
        private const string Default = Rpc + Module + "/app-user";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string SaveImage = Default + "/save-image";
        public const string ChangePassword = Default + "/change-password";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string ExportTemplate = Default + "/export-template";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListSex = Default + "/filter-list-sex";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListRole = Default + "/filter-list-role";

        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListRole = Default + "/single-list-role";
        public const string CountRole = Default + "/count-role";
        public const string ListRole = Default + "/list-role";
        public const string ListSite = Default + "/list-site";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {

        };
        private static List<string> FilterList = new List<string> {
            FilterListOrganization, FilterListSex, FilterListStatus, FilterListRole,
        };
        private static List<string> SingleList = new List<string> {
            SingleListOrganization, SingleListSex, SingleListStatus, SingleListRole,
        };
        private static List<string> CountList = new List<string>
        {
            CountRole, ListRole, ListSite
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, 
                Get,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },
            { "Thêm mới", new List<string> {
                Parent,
                Master, Count, List, 
                Get,
                Detail,Create, SaveImage,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List,
                Get,
                Detail,Get,Update, SaveImage,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List,
                Get,
                Delete
                }.Concat(FilterList)
            },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List,
                Get,
                BulkDelete
                }.Concat(FilterList)
            },
            { "Đổi mật khẩu", new List<string> {
                Parent,
                Master, Count, List,
                Get,
                ChangePassword
                }.Concat(FilterList)
            },
            { "Import", new List<string> {
                Parent,
                Master, Count, List,
                Get,
                Import, ExportTemplate 
                }.Concat(FilterList)
            },
            { "Export", new List<string> {
                Parent,
                Master, Count, List,Get,
                Export 
                }.Concat(FilterList)
            },
        };
    }
}

