using ELODIE.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.brand
{
    [DisplayName("Nhãn hiệu")]
    public class BrandRoute : Root
    {
        public const string Parent = Module + "/partner";
        public const string Master = Module + "/partner/brand/brand-master";
        public const string Detail = Module + "/partner/brand/brand-detail/*";
        private const string Default = Rpc + Module + "/brand";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };
        private static List<string> FilterList = new List<string> {
            FilterListStatus,
        };
        private static List<string> SingleList = new List<string> {
            SingleListStatus,
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
                Detail, Create, 
                }.Concat(FilterList).Concat(SingleList) 
            },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, 
                Get,  
                Detail, Update,  
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List,
                Get,  
                Detail, Delete,  
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, Get,  
                BulkDelete 
                }.Concat(FilterList)
            },
        };
    }
}
