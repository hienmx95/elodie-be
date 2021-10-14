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
using ELODIE.Services.MCustomerSalesOrder;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCustomer;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MProvince;
using ELODIE.Services.MWard;
using ELODIE.Services.MEditedPriceStatus;
using ELODIE.Services.MOrderPaymentStatus;
using ELODIE.Services.MOrderSource;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MRequestState;
using ELODIE.Services.MCustomerSalesOrderContent;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MTaxType;
using ELODIE.Services.MCustomerSalesOrderPaymentHistory;
using ELODIE.Services.MPaymentType;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrderRoute : Root
    {
        public const string Parent = Module + "/customer-sales-order";
        public const string Master = Module + "/customer-sales-order/customer-sales-order-master";
        public const string Detail = Module + "/customer-sales-order/customer-sales-order-detail";
        public const string Preview = Module + "/customer-sales-order/customer-sales-order-preview";
        private const string Default = Rpc + Module + "/customer-sales-order";
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
        
        public const string FilterListCodeGeneratorRule = Default + "/filter-list-code-generator-rule";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListCustomer = Default + "/filter-list-customer";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListNation = Default + "/filter-list-nation";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListEditedPriceStatus = Default + "/filter-list-edited-price-status";
        public const string FilterListOrderPaymentStatus = Default + "/filter-list-order-payment-status";
        public const string FilterListOrderSource = Default + "/filter-list-order-source";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListRequestState = Default + "/filter-list-request-state";
        public const string FilterListCustomerSalesOrderContent = Default + "/filter-list-customer-sales-order-content";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListTaxType = Default + "/filter-list-tax-type";
        public const string FilterListCustomerSalesOrderPaymentHistory = Default + "/filter-list-customer-sales-order-payment-history";
        public const string FilterListPaymentType = Default + "/filter-list-payment-type";

        public const string SingleListCodeGeneratorRule = Default + "/single-list-code-generator-rule";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListCustomer = Default + "/single-list-customer";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListNation = Default + "/single-list-nation";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListEditedPriceStatus = Default + "/single-list-edited-price-status";
        public const string SingleListOrderPaymentStatus = Default + "/single-list-order-payment-status";
        public const string SingleListOrderSource = Default + "/single-list-order-source";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListRequestState = Default + "/single-list-request-state";
        public const string SingleListCustomerSalesOrderContent = Default + "/single-list-customer-sales-order-content";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListCustomerSalesOrderPaymentHistory = Default + "/single-list-customer-sales-order-payment-history";
        public const string SingleListPaymentType = Default + "/single-list-payment-type";

        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CustomerSalesOrderFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.CustomerId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.OrderSourceId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.RequestStateId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.OrderPaymentStatusId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.EditedPriceStatusId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.ShippingName), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.OrderDate), FieldTypeEnum.DATE.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryDate), FieldTypeEnum.DATE.Id },
            { nameof(CustomerSalesOrderFilter.SalesEmployeeId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.Note), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceAddress), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceNationId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceProvinceId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceDistrictId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceWardId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.InvoiceZIPCode), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryAddress), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryNationId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryProvinceId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryDistrictId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryWardId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.DeliveryZIPCode), FieldTypeEnum.STRING.Id },
            { nameof(CustomerSalesOrderFilter.SubTotal), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.GeneralDiscountPercentage), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.GeneralDiscountAmount), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.TotalTaxOther), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.TotalTax), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.Total), FieldTypeEnum.DECIMAL.Id },
            { nameof(CustomerSalesOrderFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.RowId), FieldTypeEnum.ID.Id },
            { nameof(CustomerSalesOrderFilter.CodeGeneratorRuleId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListCodeGeneratorRule,FilterListAppUser,FilterListCustomer,FilterListDistrict,FilterListNation,FilterListProvince,FilterListWard,FilterListEditedPriceStatus,FilterListOrderPaymentStatus,FilterListOrderSource,FilterListOrganization,FilterListRequestState,FilterListCustomerSalesOrderContent,FilterListUnitOfMeasure,FilterListTaxType,FilterListCustomerSalesOrderPaymentHistory,FilterListPaymentType,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListCodeGeneratorRule, SingleListAppUser, SingleListCustomer, SingleListDistrict, SingleListNation, SingleListProvince, SingleListWard, SingleListEditedPriceStatus, SingleListOrderPaymentStatus, SingleListOrderSource, SingleListOrganization, SingleListRequestState, SingleListCustomerSalesOrderContent, SingleListUnitOfMeasure, SingleListTaxType, SingleListCustomerSalesOrderPaymentHistory, SingleListPaymentType, 
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
