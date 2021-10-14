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
using ELODIE.Services.MWarehouse;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MProvince;
using ELODIE.Services.MStatus;
using ELODIE.Services.MWard;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MProduct;

namespace ELODIE.Rpc.warehouse
{
    public class WarehouseRoute : Root
    {
        public const string Parent = Module + "/warehouse";
        public const string Master = Module + "/warehouse/warehouse-master";
        public const string Detail = Module + "/warehouse/warehouse-detail";
        public const string Preview = Module + "/warehouse/warehouse-preview";
        private const string Default = Rpc + Module + "/warehouse";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListInventory = Default + "/filter-list-inventory";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListItem = Default + "/filter-list-item";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListInventory = Default + "/single-list-inventory";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListItem = Default + "/single-list-item";

        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(WarehouseFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(WarehouseFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(WarehouseFilter.Address), FieldTypeEnum.STRING.Id },
            { nameof(WarehouseFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.ProvinceId), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.DistrictId), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.WardId), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(WarehouseFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListDistrict,FilterListOrganization,FilterListProvince,FilterListStatus,FilterListWard,FilterListInventory,FilterListUnitOfMeasure,FilterListItem,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListWard, SingleListInventory, SingleListUnitOfMeasure, SingleListItem, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, CountItem, ListItem,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, CountItem, ListItem,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
