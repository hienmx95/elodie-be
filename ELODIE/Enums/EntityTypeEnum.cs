using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Enums
{
    public class EntityTypeEnum
    {
        public static GenericEnum SALES_ORDER = new GenericEnum { Id = 1, Code = "SalesOrder", Name = "Đơn hàng" };
        public static GenericEnum PRODUCT = new GenericEnum { Id = 2, Code = "Product", Name = "Sản phẩm" };
        public static GenericEnum STORE = new GenericEnum { Id = 3, Code = "Store", Name = "Đại lý" };
        public static GenericEnum CUSTOMER = new GenericEnum { Id = 4, Code = "Customer", Name = "Khách hàng" };
        public static GenericEnum OPPORTUNITY = new GenericEnum { Id = 5, Code = "Opportunity", Name = "Cơ hội/Dự án" };
        
        public static List<GenericEnum> EntityTypeEnumList = new List<GenericEnum>
        {
            SALES_ORDER, STORE, CUSTOMER, PRODUCT, OPPORTUNITY
        };
    }
}
