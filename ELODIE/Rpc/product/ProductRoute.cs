using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.product
{
    [DisplayName("Sản phẩm")]
    public class ProductRoute : Root
    {
        public const string Parent = Module + "/product-category";
        public const string Master = Module + "/product-category/product/product-master";
        public const string Detail = Module + "/product-category/product/product-detail";
        public const string DetailOther = Module + "/product-category/product/product-detail/*";
        public const string Mobile = Module + ".product.*";


        private const string Default = Rpc + Module + "/product";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string GetItem = Default + "/get-item";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";
        public const string SaveItemImage = Default + "/save-item-image";
        public const string CheckCodeGeneratorRule = Default + "/check-code-generator-rule";
        public const string GetProductHistory = Default + "/get-product-history";


        public const string FilterListBrand = Default + "/filter-list-brand";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSupplier = Default + "/filter-list-supplier";
        public const string FilterListTaxType = Default + "/filter-list-tax-type";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListUnitOfMeasureGrouping = Default + "/filter-list-unit-of-measure-grouping";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListUsedVariation = Default + "/filter-list-used-variation";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListCategory = Default + "/single-list-category";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListUnitOfMeasureGrouping = Default + "/single-list-unit-of-measure-grouping";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListUsedVariation = Default + "/single-list-used-variation";
        public const string SingleListGeneralVariationGrouping = Default + "/single-list-general-variation-grouping";

        public const string CountProductGrouping = Default + "/count-product-grouping";
        public const string ListProductGrouping = Default + "/list-product-grouping";
        public const string ListItem = Default + "/list-item";
        public const string CountItem = Default + "/count-item";
        public const string ListInventory = Default + "/list-inventory";
        public const string CountInventory = Default + "/count-inventory";
        public const string ListItemHistory = Default + "/list-item-history";
        public const string CountItemHistory = Default + "/count-item-history";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProductFilter.ProductTypeId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.ProductGroupingId), FieldTypeEnum.ID.Id },
        };
        private static List<string> FilterList = new List<string> {
             FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping, FilterListCategory,
        };
        private static List<string> SingleList = new List<string> {
            SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping, SingleListCategory, SingleListGeneralVariationGrouping,
        };
        private static List<string> CountList = new List<string>
        {
            CountItem, ListItem, ListItemHistory, CountItemHistory,
        };
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem,
                GetProductHistory
               }.Concat(FilterList) 
            },

            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem,
                Detail, DetailOther,
                Create, SaveImage, SaveItemImage, CheckCodeGeneratorRule,
                GetProductHistory
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },

            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem, 
                Detail,  DetailOther,
                Update, SaveImage, SaveItemImage, CheckCodeGeneratorRule,
                GetProductHistory
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },

            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem,
                Delete,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem,
                BulkDelete }.Concat(FilterList).Concat(CountList)
            },

            { "Xuất excel", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem,
                Export }.Concat(FilterList).Concat(CountList)
            },

            { "Nhập excel", new List<string> {
                Parent,
                Master, Count, List, 
                Get, GetItem, 
                ExportTemplate, Import }.Concat(FilterList).Concat(CountList)
            },
        };
    }
}
