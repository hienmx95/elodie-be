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
using ELODIE.Services.MCustomer;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MCustomerSource;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MProfession;
using ELODIE.Services.MProvince;
using ELODIE.Services.MWard;

namespace ELODIE.Rpc.customer
{
    public class CustomerRoute : Root
    {
        public const string Parent = Module + "/customer";
        public const string Master = Module + "/customer/customer-master";
        public const string Detail = Module + "/customer/customer-detail";
        public const string Preview = Module + "/customer/customer-preview";
        private const string Default = Rpc + Module + "/customer";
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
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListCodeGeneratorRule = Default + "/filter-list-code-generator-rule";
        public const string FilterListCustomerSource = Default + "/filter-list-customer-source";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListNation = Default + "/filter-list-nation";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProfession = Default + "/filter-list-profession";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListWard = Default + "/filter-list-ward";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListCodeGeneratorRule = Default + "/single-list-code-generator-rule";
        public const string SingleListCustomerSource = Default + "/single-list-customer-source";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListNation = Default + "/single-list-nation";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProfession = Default + "/single-list-profession";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListWard = Default + "/single-list-ward";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CustomerFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.CodeDraft), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.Phone), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.Address), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.NationId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.ProvinceId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.DistrictId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.WardId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.Birthday), FieldTypeEnum.DATE.Id },
            { nameof(CustomerFilter.Email), FieldTypeEnum.STRING.Id },
            { nameof(CustomerFilter.ProfessionId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.CustomerSourceId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.SexId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.RowId), FieldTypeEnum.ID.Id },
            { nameof(CustomerFilter.CodeGeneratorRuleId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListCodeGeneratorRule,FilterListCustomerSource,FilterListDistrict,FilterListNation,FilterListOrganization,FilterListProfession,FilterListProvince,FilterListWard,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListCodeGeneratorRule, SingleListCustomerSource, SingleListDistrict, SingleListNation, SingleListOrganization, SingleListProfession, SingleListProvince, SingleListWard, 
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
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
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
