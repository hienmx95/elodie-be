using System.Collections.Generic;
using System.Linq;
using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class SupplierRoute : Root
    {
        public const string Parent = Module + "/supplier";
        public const string Master = Module + "/supplier/supplier-master";
        public const string Detail = Module + "/supplier/supplier-detail";
        public const string Preview = Module + "/supplier/supplier-preview";
        private const string Default = Rpc + Module + "/supplier";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string QuickCreate = Default + "/quick-create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";
        
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListNation = Default + "/filter-list-nation";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListSupplierBankAccount = Default + "/filter-list-supplier-bank-account";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListSupplierContactor = Default + "/filter-list-supplier-contactor";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListNation = Default + "/single-list-nation";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListSupplierBankAccount = Default + "/single-list-supplier-bank-account";
        public const string SingleListCategory = Default + "/single-list-category";
        public const string SingleListSupplierContactor = Default + "/single-list-supplier-contactor";

        public const string CountCategory = Default + "/count-category";
        public const string ListCategory = Default + "/list-category";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(SupplierFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.TaxCode), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.Phone), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.Email), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.Avatar), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.Address), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.NationId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.ProvinceId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.DistrictId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.WardId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.OwnerName), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.PersonInChargeId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(SupplierFilter.Description), FieldTypeEnum.STRING.Id },
            { nameof(SupplierFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListDistrict,FilterListNation,FilterListAppUser,FilterListProvince,FilterListStatus,FilterListWard,FilterListSupplierBankAccount,FilterListCategory,FilterListSupplierContactor,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListDistrict, SingleListNation, SingleListAppUser, SingleListProvince, SingleListStatus, SingleListWard, SingleListSupplierBankAccount, SingleListCategory, SingleListSupplierContactor, 
        };
        private static List<string> CountList = new List<string> { 
            CountCategory, ListCategory, 
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
                    Detail, Create, SaveImage, 
                    QuickCreate,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, SaveImage,
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
