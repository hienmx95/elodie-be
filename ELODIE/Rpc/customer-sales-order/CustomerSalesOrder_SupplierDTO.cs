using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_SupplierDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public string TaxCode { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public string Address { get; set; }
        
        public long? ProvinceId { get; set; }
        
        public long? DistrictId { get; set; }
        
        public long? WardId { get; set; }
        
        public string OwnerName { get; set; }
        
        public long? PersonInChargeId { get; set; }
        
        public long StatusId { get; set; }
        
        public string Description { get; set; }
        
        public bool Used { get; set; }
        

        public CustomerSalesOrder_SupplierDTO() {}
        public CustomerSalesOrder_SupplierDTO(Supplier Supplier)
        {
            
            this.Id = Supplier.Id;
            
            this.Code = Supplier.Code;
            
            this.Name = Supplier.Name;
            
            this.TaxCode = Supplier.TaxCode;
            
            this.Phone = Supplier.Phone;
            
            this.Email = Supplier.Email;
            
            this.Address = Supplier.Address;
            
            this.ProvinceId = Supplier.ProvinceId;
            
            this.DistrictId = Supplier.DistrictId;
            
            this.WardId = Supplier.WardId;
            
            this.OwnerName = Supplier.OwnerName;
            
            this.PersonInChargeId = Supplier.PersonInChargeId;
            
            this.StatusId = Supplier.StatusId;
            
            this.Description = Supplier.Description;
            
            this.Used = Supplier.Used;
            
            this.Errors = Supplier.Errors;
        }
    }

    public class CustomerSalesOrder_SupplierFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter TaxCode { get; set; }
        
        public StringFilter Phone { get; set; }
        
        public StringFilter Email { get; set; }
        
        public StringFilter Address { get; set; }
        
        public IdFilter ProvinceId { get; set; }
        
        public IdFilter DistrictId { get; set; }
        
        public IdFilter WardId { get; set; }
        
        public StringFilter OwnerName { get; set; }
        
        public IdFilter PersonInChargeId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public StringFilter Description { get; set; }
        
        public SupplierOrder OrderBy { get; set; }
    }
}