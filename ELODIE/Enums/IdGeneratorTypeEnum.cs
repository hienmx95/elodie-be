using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Enums
{
    public class IdGeneratorTypeEnum
    {
        public static GenericEnum STORE = new GenericEnum { Id = 1, Code = "STORE", Name = "Đại lý" };
        public static GenericEnum INDIRECT = new GenericEnum { Id = 2, Code = "INDIRECT", Name = "Đơn gián tiếp" };
        public static GenericEnum DIRECT = new GenericEnum { Id = 3, Code = "DIRECT", Name = "Đơn trực tiếp" };
    }
}
