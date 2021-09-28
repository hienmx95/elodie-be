using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MStatus;
using ELODIE.Services.MWard;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ELODIE.Rpc.ward
{
    [DisplayName("Phường xã")]
    public class WardRoute : Root
    {
        public const string Parent = Module + "/official";
        public const string Master = Module + "/official/ward/ward-master";
        public const string Detail = Module + "/official/ward/ward-detail/*";
        private const string Default = Rpc + Module + "/ward";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
        };
        private static List<string> FilterList = new List<string> {
            FilterListDistrict, FilterListStatus,
        };
        private static List<string> SingleList = new List<string> {
            SingleListDistrict, SingleListStatus,
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
            { "Thêm mới", new List<string> {
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
                Delete, 
                }.Concat(FilterList)
            },
             { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, 
                Get,
                BulkDelete,
                }.Concat(FilterList)
            },
        };
    }
}