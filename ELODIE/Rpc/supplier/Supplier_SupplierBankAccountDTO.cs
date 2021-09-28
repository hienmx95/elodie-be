using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class Supplier_SupplierBankAccountDTO : DataDTO
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountOwnerName { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public Supplier_SupplierBankAccountDTO() {}
        public Supplier_SupplierBankAccountDTO(SupplierBankAccount SupplierBankAccount)
        {
            this.Id = SupplierBankAccount.Id;
            this.SupplierId = SupplierBankAccount.SupplierId;
            this.BankName = SupplierBankAccount.BankName;
            this.BankAccount = SupplierBankAccount.BankAccount;
            this.BankAccountOwnerName = SupplierBankAccount.BankAccountOwnerName;
            this.Used = SupplierBankAccount.Used;
            this.RowId = SupplierBankAccount.RowId;
            this.RowId = SupplierBankAccount.RowId;
            this.Errors = SupplierBankAccount.Errors;
        }
    }

    public class Supplier_SupplierBankAccountFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter SupplierId { get; set; }
        
        public StringFilter BankName { get; set; }
        
        public StringFilter BankAccount { get; set; }
        
        public StringFilter BankAccountOwnerName { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public SupplierBankAccountOrder OrderBy { get; set; }
    }
}