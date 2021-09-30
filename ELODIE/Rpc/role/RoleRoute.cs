using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.role
{
    [DisplayName("Vai trò")]
    public class RoleRoute : Root
    {
        public const string Parent = Module + "/role";
        public const string Master = Module + "/role/role-master";
        public const string Detail = Module + "/role/role-detail";
        public const string Other = Module + "/role/role-detail/*";

        public const string Default = Rpc + Module + "/role";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Clone = Default + "/clone";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string AssignAppUser = Default + "/assign-app-user";
        public const string GetMenu = Default + "/get-menu";

        public const string GetSingleListAPI = Default + "/get-single-list-api";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListCurrentUser = Default + "/single-list-current-user";

        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListMenu = Default + "/single-list-menu";
        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProduct = Default + "/single-list-product";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListWarehouse = Default + "/single-list-warehouse";
        public const string SingleListField = Default + "/single-list-field";
        public const string SingleListPermissionOperator = Default + "/single-list-permission-operator";
        public const string SingleListERouteType = Default + "/single-list-e-route-type";
        public const string SingleListRequestState = Default + "/single-list-request-state";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";

        public const string CountPermission = Default + "/count-permission";
        public const string ListPermission = Default + "/list-permission";
        public const string GetPermission = Default + "/get-permission";
        public const string CreatePermission = Default + "/create-permission";
        public const string UpdatePermission = Default + "/update-permission";
        public const string DeletePermission = Default + "/delete-permission";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(RoleFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(RoleFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(RoleFilter.StatusId), FieldTypeEnum.ID.Id },
        };

        public static List<string> SingleList = new List<string>
        {
            GetSingleListAPI,
            SingleListAppUser,
            SingleListBrand,
            SingleListCurrentUser,
            SingleListERouteType,
            SingleListField,
            SingleListMenu,
            SingleListOrganization,
            SingleListPermissionOperator,
            SingleListProduct,
            SingleListProductGrouping,
            SingleListProductType,
            SingleListRequestState,
            SingleListStatus,
            SingleListSupplier,
            SingleListWarehouse,
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get, Clone,
                }.Concat(SingleList)
            },
            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get, Clone, CountPermission, ListPermission, GetPermission, CreatePermission, UpdatePermission, DeletePermission,
                Detail, Create, GetMenu,
                SingleListStatus, SingleListCurrentUser
            }.Concat(SingleList)
            },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get, Clone, CountPermission, ListPermission, GetPermission, CreatePermission, UpdatePermission, DeletePermission,
                Detail, Update, GetMenu,
            }.Concat(SingleList)
            },
            { "Gán người dùng", new List<string> {
                 Parent,
                Master, Count, List, Get, Clone,
                CountAppUser, ListAppUser,
                Detail, AssignAppUser, 
                
                Other,
            }.Concat(SingleList)
            },
            { "Tạo nhanh quyền", new List<string> {
                 Parent,
                Master, Count, List, Get, Clone,
                Detail, CreatePermission, GetMenu, Master, Count, List, Get, CountPermission, ListPermission, GetPermission, CreatePermission, UpdatePermission, DeletePermission,
            }.Concat(SingleList)
            },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get, Clone,
                Detail, Delete, BulkDelete,
            }.Concat(SingleList)
            },
        };
    }
}
