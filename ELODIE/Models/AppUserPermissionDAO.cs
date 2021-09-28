using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class AppUserPermissionDAO
    {
        public long PermissionId { get; set; }
        public long AppUserId { get; set; }
        public string Path { get; set; }
        public long? Count { get; set; }
    }
}
