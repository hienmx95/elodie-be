using ELODIE.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ELODIE.Rpc.product_grouping
{
    [DisplayName("Nhóm sản phẩm")]
    public class ProductGroupingRoute : Root
    {
        public const string Parent = Module + "/product-category";
        public const string Master = Module + "/product-category/product-grouping/product-grouping-master";
        public const string Detail = Module + "/product-category/product-grouping/product-grouping-detail/*";
        private const string Default = Rpc + Module + "/product-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkCreate = Default + "/bulk-create";
        public const string BulkDeleteProduct = Default + "/bulk-delete-product";
        public const string DeleteProduct = Default + "/delete-product";

        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListProduct = Default + "/filter-list-product";
        public const string FilterListBrand = Default + "/filter-list-brand";
        public const string FilterListCategory = Default + "/filter-list-category";

        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProduct = Default + "/single-list-product";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountProduct = Default + "/count-product";
        public const string ListProduct = Default + "/list-product";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };
        private static List<string> FilterList = new List<string> {
            FilterListProductGrouping, FilterListProduct,FilterListBrand,FilterListCategory
        };
        private static List<string> SingleList = new List<string> {
            SingleListProductGrouping, SingleListProduct,SingleListStatus
        };
        private static List<string> CountList = new List<string>
        {
            CountProduct, ListProduct,
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
                Detail, Create, BulkCreate, BulkDeleteProduct, DeleteProduct,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },

            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, 
                Get, 
                Detail, Update, BulkCreate, BulkDeleteProduct, DeleteProduct,
                }.Concat(FilterList).Concat(SingleList).Concat(CountList)
            },

            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, 
                Get, 
                Detail, Delete,
                }.Concat(FilterList).Concat(SingleList)
            },
            
        };
    }
}
