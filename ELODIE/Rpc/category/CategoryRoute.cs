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
using ELODIE.Services.MCategory;
using ELODIE.Services.MImage;
using ELODIE.Services.MStatus;
using System.ComponentModel;

namespace ELODIE.Rpc.category
{
    [DisplayName("Danh mục sản phẩm")]
    public class CategoryRoute : Root
    {
        public const string Master = Module + "/product-category/category/category-master";
        public const string Detail = Module + "/product-category/category/category-detail/*";
        private const string Default = Rpc + Module + "/category";
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
        public const string SaveImage = Default + "/save-image";

        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListCategory = Default + "/single-list-category";
        public const string SingleListStatus = Default + "/single-list-status";
        
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CategoryFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.ParentId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.Path), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.Level), FieldTypeEnum.LONG.Id },
            { nameof(CategoryFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.ImageId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.RowId), FieldTypeEnum.ID.Id },
        };
        private static List<string> FilterList = new List<string> {
            FilterListImage, FilterListCategory, FilterListStatus,
        };
        private static List<string> SingleList = new List<string> {
            SingleListImage, SingleListCategory, SingleListStatus,
        };
        private static List<string> CountList = new List<string>
        {

        };
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                Get, GetPreview,
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                Detail, Create, SaveImage,
                }.Concat(FilterList).Concat(SingleList)
            },
            { "Sửa", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                Detail, Update, SaveImage,
                }.Concat(FilterList).Concat(SingleList) 
            },

            { "Xoá", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                Delete, SaveImage,
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                BulkDelete 
                }.Concat(FilterList)
            },

            { "Xuất excel", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                Export 
                }.Concat(FilterList)
            },

            { "Nhập excel", new List<string> { 
                Master, Count, List, 
                Get, GetPreview,
                ExportTemplate, Import 
                }.Concat(FilterList)
            },
        };
    }
}
