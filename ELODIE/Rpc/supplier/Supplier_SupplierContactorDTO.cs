using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class Supplier_SupplierContactorDTO : DataDTO
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public Supplier_SupplierContactorDTO() {}
        public Supplier_SupplierContactorDTO(SupplierContactor SupplierContactor)
        {
            this.Id = SupplierContactor.Id;
            this.SupplierId = SupplierContactor.SupplierId;
            this.Name = SupplierContactor.Name;
            this.Phone = SupplierContactor.Phone;
            this.Email = SupplierContactor.Email;
            this.Used = SupplierContactor.Used;
            this.RowId = SupplierContactor.RowId;
            this.RowId = SupplierContactor.RowId;
            this.Errors = SupplierContactor.Errors;
        }
    }

    public class Supplier_SupplierContactorFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter SupplierId { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Phone { get; set; }
        
        public StringFilter Email { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public SupplierContactorOrder OrderBy { get; set; }
    }
}