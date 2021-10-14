using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class UnitOfMeasureDAO
    {
        public UnitOfMeasureDAO()
        {
            CustomerSalesOrderContentPrimaryUnitOfMeasures = new HashSet<CustomerSalesOrderContentDAO>();
            CustomerSalesOrderContentUnitOfMeasures = new HashSet<CustomerSalesOrderContentDAO>();
            Products = new HashSet<ProductDAO>();
            UnitOfMeasureGroupingContents = new HashSet<UnitOfMeasureGroupingContentDAO>();
            UnitOfMeasureGroupings = new HashSet<UnitOfMeasureGroupingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<CustomerSalesOrderContentDAO> CustomerSalesOrderContentPrimaryUnitOfMeasures { get; set; }
        public virtual ICollection<CustomerSalesOrderContentDAO> CustomerSalesOrderContentUnitOfMeasures { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContents { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
    }
}
