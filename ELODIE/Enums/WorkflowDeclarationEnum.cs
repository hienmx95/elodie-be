using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Enums
{
    public class WorkflowTypeEnum
    {
        public static GenericEnum STORE = new GenericEnum { Id = 1, Code = "STORE", Name = "Đại lý" };
        public static GenericEnum EROUTE = new GenericEnum { Id = 2, Code = "EROUTE", Name = "Tuyến" };
        public static GenericEnum INDIRECT_SALES_ORDER = new GenericEnum { Id = 3, Code = "INDIRECT_SALES_ORDER", Name = "Đơn hàng gián tiếp" };
        public static GenericEnum PRICE_LIST = new GenericEnum { Id = 4, Code = "PRICE_LIST", Name = "Bảng giá" };
        public static GenericEnum DIRECT_SALES_ORDER = new GenericEnum { Id = 5, Code = "DIRECT_SALES_ORDER", Name = "Đơn hàng trực tiếp" };
        public static List<GenericEnum> WorkflowTypeEnumList = new List<GenericEnum>()
        {
             EROUTE, INDIRECT_SALES_ORDER, PRICE_LIST, DIRECT_SALES_ORDER
        };
    }

    public class WorkflowParameterEnum
    {
        public static GenericEnum STORE_ORGANIZATION = new GenericEnum { Id = WorkflowTypeEnum.STORE.Id * 100 + 1, Code = "OrganizationId", Name = "Đơn vị tổ chức", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static List<GenericEnum> StoreEnumList = new List<GenericEnum>()
        {
           STORE_ORGANIZATION
        };

        public static GenericEnum EROUTE_ORGANIZATION = new GenericEnum { Id = WorkflowTypeEnum.EROUTE.Id * 100 + 1, Code = "OrganizationId", Name = "Đơn vị tổ chức", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static GenericEnum EROUTE_TOTAL_STORE_COUNTER = new GenericEnum { Id = WorkflowTypeEnum.EROUTE.Id * 100 + 2, Code = "TotalStoreCounter", Name = "Tổng số cửa hàng trong tuyến", Value = WorkflowParameterTypeEnum.LONG.Id.ToString() };
        public static GenericEnum EROUTE_EROUTE_TYPE = new GenericEnum { Id = WorkflowTypeEnum.EROUTE.Id * 100 + 3, Code = "ERouteTypeId", Name = "Loại tuyến", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static List<GenericEnum> ERouteEnumList = new List<GenericEnum>()
        {
           EROUTE_ORGANIZATION, EROUTE_TOTAL_STORE_COUNTER
        };

        public static GenericEnum INDIRECT_SALES_ORDER_ORGANIZATION = new GenericEnum { Id = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id * 100 + 1, Code = "OrganizationId", Name = "Đơn vị tổ chức", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static GenericEnum INDIRECT_SALES_ORDER_TOTAL = new GenericEnum { Id = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id * 100 + 2, Code = "Total", Name = "Thành tiền", Value = WorkflowParameterTypeEnum.DECIMAL.Id.ToString() };
        public static GenericEnum INDIRECT_SALES_ORDER_TOTAL_DISCOUNT = new GenericEnum { Id = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id * 100 + 3, Code = "TotalDiscountAmount", Name = "Tổng chiết khấu", Value = WorkflowParameterTypeEnum.DECIMAL.Id.ToString() };
        public static GenericEnum INDIRECT_SALES_ORDER_TOTAL_REQUESTED_QUANTITY = new GenericEnum { Id = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id * 100 + 4, Code = "TotalRequestedQuantity", Name = "Tổng sản phẩm", Value = WorkflowParameterTypeEnum.LONG.Id.ToString() };
        public static List<GenericEnum> IndirectSalesOrderEnumList = new List<GenericEnum>()
        {
           INDIRECT_SALES_ORDER_ORGANIZATION, INDIRECT_SALES_ORDER_TOTAL, INDIRECT_SALES_ORDER_TOTAL_DISCOUNT, INDIRECT_SALES_ORDER_TOTAL_REQUESTED_QUANTITY
        };

        public static GenericEnum DIRECT_SALES_ORDER_ORGANIZATION = new GenericEnum { Id = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id * 100 + 1, Code = "OrganizationId", Name = "Đơn vị tổ chức", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static GenericEnum DIRECT_SALES_ORDER_TOTAL = new GenericEnum { Id = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id * 100 + 2, Code = "Total", Name = "Thành tiền", Value = WorkflowParameterTypeEnum.DECIMAL.Id.ToString() };
        public static GenericEnum DIRECT_SALES_ORDER_TOTAL_DISCOUNT = new GenericEnum { Id = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id * 100 + 3, Code = "TotalDiscountAmount", Name = "Tổng chiết khấu", Value = WorkflowParameterTypeEnum.DECIMAL.Id.ToString() };
        public static GenericEnum DIRECT_SALES_ORDER_TOTAL_REQUESTED_QUANTITY = new GenericEnum { Id = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id * 100 + 4, Code = "TotalRequestedQuantity", Name = "Tổng sản phẩm", Value = WorkflowParameterTypeEnum.LONG.Id.ToString() };
        public static List<GenericEnum> DirectSalesOrderEnumList = new List<GenericEnum>()
        {
           DIRECT_SALES_ORDER_ORGANIZATION, DIRECT_SALES_ORDER_TOTAL, DIRECT_SALES_ORDER_TOTAL_DISCOUNT, DIRECT_SALES_ORDER_TOTAL_REQUESTED_QUANTITY
        };

        public static GenericEnum PRICE_LIST_ORGANIZATION = new GenericEnum { Id = WorkflowTypeEnum.PRICE_LIST.Id * 100 + 1, Code = "OrganizationId", Name = "Đơn vị tổ chức", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static GenericEnum PRICE_LIST_SALES_ORDER_TYPE = new GenericEnum { Id = WorkflowTypeEnum.PRICE_LIST.Id * 100 + 2, Code = "SalesOrderTypeId", Name = "Loại đơn hàng", Value = WorkflowParameterTypeEnum.ID.Id.ToString() };
        public static List<GenericEnum> PriceListEnumList = new List<GenericEnum>()
        {
           PRICE_LIST_ORGANIZATION, PRICE_LIST_SALES_ORDER_TYPE
        };
    }
}
