using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class StatusDAO
    {
        public StatusDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            Brands = new HashSet<BrandDAO>();
            Categories = new HashSet<CategoryDAO>();
            CodeGeneratorRules = new HashSet<CodeGeneratorRuleDAO>();
            Districts = new HashSet<DistrictDAO>();
            Items = new HashSet<ItemDAO>();
            Nations = new HashSet<NationDAO>();
            Organizations = new HashSet<OrganizationDAO>();
            Permissions = new HashSet<PermissionDAO>();
            ProductGroupings = new HashSet<ProductGroupingDAO>();
            ProductTypes = new HashSet<ProductTypeDAO>();
            Products = new HashSet<ProductDAO>();
            Provinces = new HashSet<ProvinceDAO>();
            Roles = new HashSet<RoleDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            TaxTypes = new HashSet<TaxTypeDAO>();
            UnitOfMeasureGroupings = new HashSet<UnitOfMeasureGroupingDAO>();
            UnitOfMeasures = new HashSet<UnitOfMeasureDAO>();
            Wards = new HashSet<WardDAO>();
            Warehouses = new HashSet<WarehouseDAO>();
            WorkflowDefinitions = new HashSet<WorkflowDefinitionDAO>();
            WorkflowDirections = new HashSet<WorkflowDirectionDAO>();
            WorkflowSteps = new HashSet<WorkflowStepDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<BrandDAO> Brands { get; set; }
        public virtual ICollection<CategoryDAO> Categories { get; set; }
        public virtual ICollection<CodeGeneratorRuleDAO> CodeGeneratorRules { get; set; }
        public virtual ICollection<DistrictDAO> Districts { get; set; }
        public virtual ICollection<ItemDAO> Items { get; set; }
        public virtual ICollection<NationDAO> Nations { get; set; }
        public virtual ICollection<OrganizationDAO> Organizations { get; set; }
        public virtual ICollection<PermissionDAO> Permissions { get; set; }
        public virtual ICollection<ProductGroupingDAO> ProductGroupings { get; set; }
        public virtual ICollection<ProductTypeDAO> ProductTypes { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<ProvinceDAO> Provinces { get; set; }
        public virtual ICollection<RoleDAO> Roles { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<TaxTypeDAO> TaxTypes { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
        public virtual ICollection<UnitOfMeasureDAO> UnitOfMeasures { get; set; }
        public virtual ICollection<WardDAO> Wards { get; set; }
        public virtual ICollection<WarehouseDAO> Warehouses { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitions { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirections { get; set; }
        public virtual ICollection<WorkflowStepDAO> WorkflowSteps { get; set; }
    }
}
