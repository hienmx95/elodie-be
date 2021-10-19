using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Enums
{
    public class OrderPaymentStatusEnum
    {
        public static GenericEnum UNPAID = new GenericEnum { Id = 1, Code = "UNPAID", Name = "Chưa thanh toán" };
        public static GenericEnum PAIDING = new GenericEnum { Id = 2, Code = "PAIDING", Name = "Thanh toán một phần" };
        public static GenericEnum PAID = new GenericEnum { Id = 3, Code = "PAID", Name = "Đã thanh toán" };

        public static List<GenericEnum> OrderPaymentStatusEnumList = new List<GenericEnum>()
        {
            UNPAID, PAIDING, PAID
        };
    }
}
