using ELODIE.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.organization
{
    [DisplayName("Cây tổ chức")]
    public class OrganizationRoute : Root
    {
        public const string Parent = Module + "/organization";
        public const string Master = Module + "/organization/organization-master";
        public const string Detail = Module + "/organization/organization-detail/*";
        private const string Default = Rpc + Module + "/organization";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string UpdateIsDisplay = Default + "/update-is-display";
        public const string Export = Default + "/export";
        public const string ExportAppUser = Default + "/export-app-user";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListTreeSelectOption = Default + "/single-list-tree-select-option";

        public const string ListAppUser = Default + "/list-app-user";
        public const string CountAppUser = Default + "/count-app-user";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static List<string> SingleList = new List<string>
        {
            SingleListOrganization, SingleListStatus, SingleListAppUser, SingleListTreeSelectOption
        };

        public static List<string> ListCount = new List<string>
        {
            ListAppUser, CountAppUser,
        };


        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent, Detail,
                Master, Count, List, Get,
                FilterListOrganization, FilterListStatus, FilterListAppUser, } },
            { "Thêm", new List<string> {
                Parent, Create, Detail,
                Master, Count, List, Get, UpdateIsDisplay,
                FilterListOrganization, FilterListStatus, FilterListAppUser, }.Concat(SingleList).Concat(ListCount) },
            { "Sửa", new List<string> {
                Parent, Update, Detail,
                Master, Count, List, Get, UpdateIsDisplay,
                FilterListOrganization, FilterListStatus, FilterListAppUser, }.Concat(SingleList).Concat(ListCount) },
            { "Xóa", new List<string> {
                Parent, Delete, Detail,
                Master, Count, List, Get,
                FilterListOrganization, FilterListStatus, FilterListAppUser, }.Concat(SingleList).Concat(ListCount) },
            { "Xuất excel", new List<string> {
                Parent, Detail,
                Master, Count, List, Get,
                FilterListOrganization, FilterListStatus, FilterListAppUser, Export, ExportAppUser } },
        };
    }
}
