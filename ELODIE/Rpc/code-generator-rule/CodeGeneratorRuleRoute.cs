using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.code_generator_rule
{
    [DisplayName("Quy tắc sinh mã")]
    public class CodeGeneratorRuleRoute : Root
    {
        public const string Parent = Module + "/system-configuration/code-generator-rule";
        public const string Master = Module + "/system-configuration/code-generator-rule/code-generator-rule-master";
        public const string Detail = Module + "/system-configuration/code-generator-rule/code-generator-rule-detail/*";
        private const string Default = Rpc + Module + "/code-generator-rule";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ChangeStatus = Default + "/change-status";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListEntityType = Default + "/filter-list-entity-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListEntityComponent = Default + "/filter-list-entity-component";
        
        public const string SingleListEntityType = Default + "/single-list-entity-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListEntityComponent = Default + "/single-list-entity-component";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CodeGeneratorRuleFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CodeGeneratorRuleFilter.EntityTypeId), FieldTypeEnum.ID.Id },
            { nameof(CodeGeneratorRuleFilter.AutoNumberLenth), FieldTypeEnum.LONG.Id },
            { nameof(CodeGeneratorRuleFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(CodeGeneratorRuleFilter.RowId), FieldTypeEnum.ID.Id },
        };
        private static List<string> FilterList = new List<string> {
            FilterListEntityType, FilterListStatus, FilterListEntityComponent,
        };
        private static List<string> SingleList = new List<string> {
            SingleListEntityType, SingleListStatus, SingleListEntityComponent
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
                ChangeStatus,
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> {
                Parent, Master, Count, List, Get, GetPreview, ChangeStatus,
                Detail, Create, 
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Sửa", new List<string> {
                Parent, 
                Master, Count, List, 
                Get, GetPreview, 
                ChangeStatus, 
                Detail, Update, 
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Xoá", new List<string> {
                Parent, 
                Master, Count, List, 
                Get, GetPreview, 
                ChangeStatus,
                Delete, 
                }.Concat(FilterList).Concat(SingleList)
            },

            { "Xoá nhiều", new List<string> {
                Parent, 
                Master, Count, List, 
                Get, GetPreview, 
                ChangeStatus,
                BulkDelete }.Concat(FilterList)
            },

            { "Xuất excel", new List<string> {
                Parent, 
                Master, Count, List,
                Get, GetPreview, 
                ChangeStatus,
                Export
                }.Concat(FilterList)
            },

            { "Nhập excel", new List<string> {
                Parent, 
                Master, Count, List, 
                Get, GetPreview, 
                ChangeStatus, 
                ExportTemplate, Import 
                }.Concat(FilterList)
            },
        };
    }
}
