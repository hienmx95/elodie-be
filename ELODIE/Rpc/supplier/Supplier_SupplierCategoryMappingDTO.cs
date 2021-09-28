using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class Supplier_SupplierCategoryMappingDTO : DataDTO
    {
        public long SupplierId { get; set; }
        public long CategoryId { get; set; }
        public Supplier_CategoryDTO Category { get; set; }   

        public Supplier_SupplierCategoryMappingDTO() {}
        public Supplier_SupplierCategoryMappingDTO(SupplierCategoryMapping SupplierCategoryMapping)
        {
            this.SupplierId = SupplierCategoryMapping.SupplierId;
            this.CategoryId = SupplierCategoryMapping.CategoryId;
            this.Category = SupplierCategoryMapping.Category == null ? null : new Supplier_CategoryDTO(SupplierCategoryMapping.Category);
            this.Errors = SupplierCategoryMapping.Errors;
        }
    }

    public class Supplier_SupplierCategoryMappingFilterDTO : FilterDTO
    {
        
        public IdFilter SupplierId { get; set; }
        
        public IdFilter CategoryId { get; set; }
        
        public SupplierCategoryMappingOrder OrderBy { get; set; }
    }
}