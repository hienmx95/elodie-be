using ELODIE.Common;

namespace ELODIE.Enums
{
    public class StatusEnum
    {
        public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "Active", Name = "Hoạt động" };
        public static GenericEnum INACTIVE = new GenericEnum { Id = 0, Code = "Inactive", Name = "Dừng hoạt động" };
    }
}
