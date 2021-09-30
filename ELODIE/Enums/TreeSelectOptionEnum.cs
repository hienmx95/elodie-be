using ELODIE.Common;
using System.Collections.Generic;

namespace ELODIE.Enums
{
    public class TreeSelectOptionEnum
    {
        public static GenericEnum ALL = new GenericEnum { Id = 1, Code = "ALL", Name = "Tất cả" };
        public static GenericEnum CURRENT_NODE = new GenericEnum { Id = 2, Code = "CURRENT_NODE", Name = "Node hiện tại" };
        public static GenericEnum CHILD_NODES = new GenericEnum { Id = 3, Code = "CHILD_NODES", Name = "Các node con" };

        public static List<GenericEnum> TreeSelectOptionEnumList = new List<GenericEnum>
        {
            ALL, CURRENT_NODE, CHILD_NODES,
        };
    }
}
