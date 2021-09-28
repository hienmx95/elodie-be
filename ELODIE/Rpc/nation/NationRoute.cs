using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using ELODIE.Entities;
using ELODIE.Services.MNation;
using ELODIE.Services.MStatus;
using System.ComponentModel;

namespace ELODIE.Rpc.nation
{
    [DisplayName("Quốc gia")]
    public class NationRoute : Root
    {
        public const string Parent = Module + "/official/nation";
        public const string Master = Module + "/official/nation/nation-master";
        public const string Detail = Module + "/official/nation/nation-detail";
        private const string Default = Rpc + Module + "/nation";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListStatus = Default + "/single-list-status";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(NationFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(NationFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(NationFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(NationFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(NationFilter.RowId), FieldTypeEnum.ID.Id },
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
                Get, GetPreview,
                 }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                Parent,
                Master, Count, List, 
                Get, GetPreview,
                Detail, Create, 
                }.Concat(FilterList).Concat(SingleList) },

            { "Sửa", new List<string> { 
                Parent,            
                Master, Count, List, 
                Get, GetPreview, 
                Detail, Update,   
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Xoá", new List<string> { 
                Parent,
                Master, Count, List, 
                Get, GetPreview,
                Delete,
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Xoá nhiều", new List<string> { 
                Parent,
                Master, Count, List, 
                Get, GetPreview,
                BulkDelete 
                }.Concat(FilterList)
            },

            { "Xuất excel", new List<string> { 
                Parent,
                Master, Count, List, 
                Get, GetPreview,
                Export 
                }.Concat(FilterList)
            },

            { "Nhập excel", new List<string> { 
                Parent,
                Master, Count, List, 
                Get, GetPreview,  
                ExportTemplate, Import 
                }.Concat(FilterList)
            },
        };
    }
}
