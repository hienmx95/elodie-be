using ELODIE.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.unit_of_measure_grouping
{
    [DisplayName("Nhóm đơn vị tính")]
    public class UnitOfMeasureGroupingRoute : Root
    {
        public const string Parent = Module + "/product-category";
        public const string Master = Module + "/product-category/unit-of-measure-grouping/unit-of-measure-grouping-master";
        public const string Detail = Module + "/product-category/unit-of-measure-grouping/unit-of-measure-grouping-detail";
        public const string DetailOther = Module + "/product-category/unit-of-measure-grouping/unit-of-measure-grouping-detail/*";
        private const string Default = Rpc + Module + "/unit-of-measure-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";

        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };
        private static List<string> FilterList = new List<string> {
            FilterListUnitOfMeasure,
        };
        private static List<string> SingleList = new List<string> {
            SingleListStatus, SingleListUnitOfMeasure,
        };
        private static List<string> CountList = new List<string>
        {

        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, 
                Get, 
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, 
                Get,
                Detail, DetailOther,
                Create,
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, 
                Get, 
                Detail, DetailOther,
                Update,
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, 
                Get,
                Detail, DetailOther,
                Delete,
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, 
                Get, 
                BulkDelete 
                }.Concat(FilterList)
            },
        };
    }
}
