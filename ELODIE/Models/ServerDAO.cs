using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class ServerDAO
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
