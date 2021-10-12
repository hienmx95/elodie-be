using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class SexDAO
    {
        public SexDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            Customers = new HashSet<CustomerDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<CustomerDAO> Customers { get; set; }
    }
}
