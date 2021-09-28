using ELODIE.Common;
using ELODIE.Helpers;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface ISupplierRepository
    {
        Task<int> Count(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(List<long> Ids);
        Task<Supplier> Get(long Id);
        Task<bool> Create(Supplier Supplier);
        Task<bool> Update(Supplier Supplier);
        Task<bool> Delete(Supplier Supplier);
        Task<bool> BulkMerge(List<Supplier> Suppliers);
        Task<bool> BulkDelete(List<Supplier> Suppliers);
        Task<bool> BulkUpdate(List<Supplier> Suppliers);
        Task<bool> Used(List<long> Ids);
    }
    public class SupplierRepository : ISupplierRepository
    {
        private DataContext DataContext;
        public SupplierRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SupplierDAO> DynamicFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.TaxCode != null && filter.TaxCode.HasValue)
                query = query.Where(q => q.TaxCode, filter.TaxCode);
            if (filter.Phone != null && filter.Phone.HasValue)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Email != null && filter.Email.HasValue)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Avatar != null && filter.Avatar.HasValue)
                query = query.Where(q => q.Avatar, filter.Avatar);
            if (filter.Address != null && filter.Address.HasValue)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.NationId != null && filter.NationId.HasValue)
                query = query.Where(q => q.NationId.HasValue).Where(q => q.NationId.Value, filter.NationId);
            if (filter.ProvinceId != null && filter.ProvinceId.HasValue)
                query = query.Where(q => q.ProvinceId.HasValue).Where(q => q.ProvinceId.Value, filter.ProvinceId);
            if (filter.DistrictId != null && filter.DistrictId.HasValue)
                query = query.Where(q => q.DistrictId.HasValue).Where(q => q.DistrictId.Value, filter.DistrictId);
            if (filter.WardId != null && filter.WardId.HasValue)
                query = query.Where(q => q.WardId.HasValue).Where(q => q.WardId.Value, filter.WardId);
            if (filter.OwnerName != null && filter.OwnerName.HasValue)
                query = query.Where(q => q.OwnerName, filter.OwnerName);
            if (filter.PersonInChargeId != null && filter.PersonInChargeId.HasValue)
                query = query.Where(q => q.PersonInChargeId.HasValue).Where(q => q.PersonInChargeId.Value, filter.PersonInChargeId);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.Description != null && filter.Description.HasValue)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                List<string> Tokens = filter.Search.Split(" ").Select(x => x.ToLower()).ToList();
                var queryForCode = query;
                var queryForName = query;
                var queryForPhone = query;
                var queryForAddress = query;
                var queryForEmail = query;
                foreach (string Token in Tokens)
                {
                    if (string.IsNullOrWhiteSpace(Token))
                        continue;
                    queryForCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                    queryForName = queryForName.Where(x => x.Name.ToLower().Contains(Token));
                    queryForPhone = queryForPhone.Where(x => x.Phone.ToLower().Contains(Token));
                    queryForAddress = queryForAddress.Where(x => x.Address.ToLower().Contains(Token));
                    queryForEmail = queryForEmail.Where(x => x.Email.ToLower().Contains(Token));
                }
                query = queryForCode.Union(queryForName)
                                    .Union(queryForPhone)
                                    .Union(queryForAddress)
                                    .Union(queryForEmail);
                query = query.Distinct();
            }

            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<SupplierDAO> OrFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SupplierDAO> initQuery = query.Where(q => false);
            foreach (SupplierFilter SupplierFilter in filter.OrFilter)
            {
                IQueryable<SupplierDAO> queryable = query;
                if (SupplierFilter.Id != null && SupplierFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (SupplierFilter.Code != null && SupplierFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (SupplierFilter.Name != null && SupplierFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (SupplierFilter.TaxCode != null && SupplierFilter.TaxCode.HasValue)
                    queryable = queryable.Where(q => q.TaxCode, filter.TaxCode);
                if (SupplierFilter.Phone != null && SupplierFilter.Phone.HasValue)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (SupplierFilter.Email != null && SupplierFilter.Email.HasValue)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (SupplierFilter.Avatar != null && SupplierFilter.Avatar.HasValue)
                    queryable = queryable.Where(q => q.Avatar, filter.Avatar);
                if (SupplierFilter.Address != null && SupplierFilter.Address.HasValue)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (SupplierFilter.NationId != null && SupplierFilter.NationId.HasValue)
                    queryable = queryable.Where(q => q.NationId.HasValue).Where(q => q.NationId.Value, filter.NationId);
                if (SupplierFilter.ProvinceId != null && SupplierFilter.ProvinceId.HasValue)
                    queryable = queryable.Where(q => q.ProvinceId.HasValue).Where(q => q.ProvinceId.Value, filter.ProvinceId);
                if (SupplierFilter.DistrictId != null && SupplierFilter.DistrictId.HasValue)
                    queryable = queryable.Where(q => q.DistrictId.HasValue).Where(q => q.DistrictId.Value, filter.DistrictId);
                if (SupplierFilter.WardId != null && SupplierFilter.WardId.HasValue)
                    queryable = queryable.Where(q => q.WardId.HasValue).Where(q => q.WardId.Value, filter.WardId);
                if (SupplierFilter.OwnerName != null && SupplierFilter.OwnerName.HasValue)
                    queryable = queryable.Where(q => q.OwnerName, filter.OwnerName);
                if (SupplierFilter.PersonInChargeId != null && SupplierFilter.PersonInChargeId.HasValue)
                    queryable = queryable.Where(q => q.PersonInChargeId.HasValue).Where(q => q.PersonInChargeId.Value, filter.PersonInChargeId);
                if (SupplierFilter.StatusId != null && SupplierFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (SupplierFilter.Description != null && SupplierFilter.Description.HasValue)
                    queryable = queryable.Where(q => q.Description, filter.Description);
                if (SupplierFilter.RowId != null && SupplierFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<SupplierDAO> DynamicOrder(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderBy(q => q.TaxCode);
                            break;
                        case SupplierOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case SupplierOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case SupplierOrder.Avatar:
                            query = query.OrderBy(q => q.Avatar);
                            break;
                        case SupplierOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case SupplierOrder.Nation:
                            query = query.OrderBy(q => q.NationId);
                            break;
                        case SupplierOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case SupplierOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case SupplierOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case SupplierOrder.OwnerName:
                            query = query.OrderBy(q => q.OwnerName);
                            break;
                        case SupplierOrder.PersonInCharge:
                            query = query.OrderBy(q => q.PersonInChargeId);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case SupplierOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case SupplierOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case SupplierOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderByDescending(q => q.TaxCode);
                            break;
                        case SupplierOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case SupplierOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case SupplierOrder.Avatar:
                            query = query.OrderByDescending(q => q.Avatar);
                            break;
                        case SupplierOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case SupplierOrder.Nation:
                            query = query.OrderByDescending(q => q.NationId);
                            break;
                        case SupplierOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case SupplierOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case SupplierOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case SupplierOrder.OwnerName:
                            query = query.OrderByDescending(q => q.OwnerName);
                            break;
                        case SupplierOrder.PersonInCharge:
                            query = query.OrderByDescending(q => q.PersonInChargeId);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case SupplierOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case SupplierOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case SupplierOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Supplier>> DynamicSelect(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            List<Supplier> Suppliers = await query.Select(q => new Supplier()
            {
                Id = filter.Selects.Contains(SupplierSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(SupplierSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(SupplierSelect.Name) ? q.Name : default(string),
                TaxCode = filter.Selects.Contains(SupplierSelect.TaxCode) ? q.TaxCode : default(string),
                Phone = filter.Selects.Contains(SupplierSelect.Phone) ? q.Phone : default(string),
                Email = filter.Selects.Contains(SupplierSelect.Email) ? q.Email : default(string),
                Avatar = filter.Selects.Contains(SupplierSelect.Avatar) ? q.Avatar : default(string),
                Address = filter.Selects.Contains(SupplierSelect.Address) ? q.Address : default(string),
                NationId = filter.Selects.Contains(SupplierSelect.Nation) ? q.NationId : default(long?),
                ProvinceId = filter.Selects.Contains(SupplierSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(SupplierSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(SupplierSelect.Ward) ? q.WardId : default(long?),
                OwnerName = filter.Selects.Contains(SupplierSelect.OwnerName) ? q.OwnerName : default(string),
                PersonInChargeId = filter.Selects.Contains(SupplierSelect.PersonInCharge) ? q.PersonInChargeId : default(long?),
                StatusId = filter.Selects.Contains(SupplierSelect.Status) ? q.StatusId : default(long),
                Description = filter.Selects.Contains(SupplierSelect.Description) ? q.Description : default(string),
                Used = filter.Selects.Contains(SupplierSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(SupplierSelect.Row) ? q.RowId : default(Guid),
                District = filter.Selects.Contains(SupplierSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                    RowId = q.District.RowId,
                    Used = q.District.Used,
                } : null,
                Nation = filter.Selects.Contains(SupplierSelect.Nation) && q.Nation != null ? new Nation
                {
                    Id = q.Nation.Id,
                    Code = q.Nation.Code,
                    Name = q.Nation.Name,
                    Priority = q.Nation.Priority,
                    StatusId = q.Nation.StatusId,
                    Used = q.Nation.Used,
                    RowId = q.Nation.RowId,
                } : null,
                PersonInCharge = filter.Selects.Contains(SupplierSelect.PersonInCharge) && q.PersonInCharge != null ? new AppUser
                {
                    Id = q.PersonInCharge.Id,
                    Username = q.PersonInCharge.Username,
                    DisplayName = q.PersonInCharge.DisplayName,
                    Address = q.PersonInCharge.Address,
                    Email = q.PersonInCharge.Email,
                    Phone = q.PersonInCharge.Phone,
                    SexId = q.PersonInCharge.SexId,
                    Birthday = q.PersonInCharge.Birthday,
                    Avatar = q.PersonInCharge.Avatar,
                    Department = q.PersonInCharge.Department,
                    OrganizationId = q.PersonInCharge.OrganizationId,
                    StatusId = q.PersonInCharge.StatusId,
                    RowId = q.PersonInCharge.RowId,
                    Used = q.PersonInCharge.Used,
                } : null,
                Province = filter.Selects.Contains(SupplierSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                    RowId = q.Province.RowId,
                    Used = q.Province.Used,
                } : null,
                Status = filter.Selects.Contains(SupplierSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Ward = filter.Selects.Contains(SupplierSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                    RowId = q.Ward.RowId,
                    Used = q.Ward.Used,
                } : null,
            }).ToListAsync();
            return Suppliers;
        }

        public async Task<int> Count(SupplierFilter filter)
        {
            IQueryable<SupplierDAO> Suppliers = DataContext.Supplier.AsNoTracking();
            Suppliers = DynamicFilter(Suppliers, filter);
            return await Suppliers.CountAsync();
        }

        public async Task<List<Supplier>> List(SupplierFilter filter)
        {
            if (filter == null) return new List<Supplier>();
            IQueryable<SupplierDAO> SupplierDAOs = DataContext.Supplier.AsNoTracking();
            SupplierDAOs = DynamicFilter(SupplierDAOs, filter);
            SupplierDAOs = DynamicOrder(SupplierDAOs, filter);
            List<Supplier> Suppliers = await DynamicSelect(SupplierDAOs, filter);
            return Suppliers;
        }

        public async Task<List<Supplier>> List(List<long> Ids)
        {
            List<Supplier> Suppliers = await DataContext.Supplier.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Supplier()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                TaxCode = x.TaxCode,
                Phone = x.Phone,
                Email = x.Email,
                Avatar = x.Avatar,
                Address = x.Address,
                NationId = x.NationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                OwnerName = x.OwnerName,
                PersonInChargeId = x.PersonInChargeId,
                StatusId = x.StatusId,
                Description = x.Description,
                Used = x.Used,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
                    Used = x.District.Used,
                },
                Nation = x.Nation == null ? null : new Nation
                {
                    Id = x.Nation.Id,
                    Code = x.Nation.Code,
                    Name = x.Nation.Name,
                    Priority = x.Nation.Priority,
                    StatusId = x.Nation.StatusId,
                    Used = x.Nation.Used,
                    RowId = x.Nation.RowId,
                },
                PersonInCharge = x.PersonInCharge == null ? null : new AppUser
                {
                    Id = x.PersonInCharge.Id,
                    Username = x.PersonInCharge.Username,
                    DisplayName = x.PersonInCharge.DisplayName,
                    Address = x.PersonInCharge.Address,
                    Email = x.PersonInCharge.Email,
                    Phone = x.PersonInCharge.Phone,
                    SexId = x.PersonInCharge.SexId,
                    Birthday = x.PersonInCharge.Birthday,
                    Avatar = x.PersonInCharge.Avatar,
                    Department = x.PersonInCharge.Department,
                    OrganizationId = x.PersonInCharge.OrganizationId,
                    StatusId = x.PersonInCharge.StatusId,
                    RowId = x.PersonInCharge.RowId,
                    Used = x.PersonInCharge.Used,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                    Used = x.Province.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                    Used = x.Ward.Used,
                },
            }).ToListAsync();

            List<SupplierBankAccount> SupplierBankAccounts = await DataContext.SupplierBankAccount.AsNoTracking()
                .Where(x => Ids.Contains(x.SupplierId))
                .Select(x => new SupplierBankAccount
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                    BankAccountOwnerName = x.BankAccountOwnerName,
                    Used = x.Used,
                    RowId = x.RowId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                }).ToListAsync();
            foreach (Supplier Supplier in Suppliers)
            {
                Supplier.SupplierBankAccounts = SupplierBankAccounts
                    .Where(x => x.SupplierId == Supplier.Id)
                    .ToList();
            }
            List<SupplierCategoryMapping> SupplierCategoryMappings = await DataContext.SupplierCategoryMapping.AsNoTracking()
                .Where(x => Ids.Contains(x.SupplierId))
                .Select(x => new SupplierCategoryMapping
                {
                    SupplierId = x.SupplierId,
                    CategoryId = x.CategoryId,
                    Category = new Category
                    {
                        Id = x.Category.Id,
                        Code = x.Category.Code,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        Path = x.Category.Path,
                        Level = x.Category.Level,
                        StatusId = x.Category.StatusId,
                        ImageId = x.Category.ImageId,
                        RowId = x.Category.RowId,
                        Used = x.Category.Used,
                    },
                }).ToListAsync();
            foreach (Supplier Supplier in Suppliers)
            {
                Supplier.SupplierCategoryMappings = SupplierCategoryMappings
                    .Where(x => x.SupplierId == Supplier.Id)
                    .ToList();
            }
            List<SupplierContactor> SupplierContactors = await DataContext.SupplierContactor.AsNoTracking()
                .Where(x => Ids.Contains(x.SupplierId))
                .Select(x => new SupplierContactor
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    Name = x.Name,
                    Phone = x.Phone,
                    Email = x.Email,
                    Used = x.Used,
                    RowId = x.RowId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                }).ToListAsync();
            foreach (Supplier Supplier in Suppliers)
            {
                Supplier.SupplierContactors = SupplierContactors
                    .Where(x => x.SupplierId == Supplier.Id)
                    .ToList();
            }

            return Suppliers;
        }

        public async Task<Supplier> Get(long Id)
        {
            Supplier Supplier = await DataContext.Supplier.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Supplier()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                TaxCode = x.TaxCode,
                Phone = x.Phone,
                Email = x.Email,
                Avatar = x.Avatar,
                Address = x.Address,
                NationId = x.NationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                OwnerName = x.OwnerName,
                PersonInChargeId = x.PersonInChargeId,
                StatusId = x.StatusId,
                Description = x.Description,
                Used = x.Used,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
                    Used = x.District.Used,
                },
                Nation = x.Nation == null ? null : new Nation
                {
                    Id = x.Nation.Id,
                    Code = x.Nation.Code,
                    Name = x.Nation.Name,
                    Priority = x.Nation.Priority,
                    StatusId = x.Nation.StatusId,
                    Used = x.Nation.Used,
                    RowId = x.Nation.RowId,
                },
                PersonInCharge = x.PersonInCharge == null ? null : new AppUser
                {
                    Id = x.PersonInCharge.Id,
                    Username = x.PersonInCharge.Username,
                    DisplayName = x.PersonInCharge.DisplayName,
                    Address = x.PersonInCharge.Address,
                    Email = x.PersonInCharge.Email,
                    Phone = x.PersonInCharge.Phone,
                    SexId = x.PersonInCharge.SexId,
                    Birthday = x.PersonInCharge.Birthday,
                    Avatar = x.PersonInCharge.Avatar,
                    Department = x.PersonInCharge.Department,
                    OrganizationId = x.PersonInCharge.OrganizationId,
                    StatusId = x.PersonInCharge.StatusId,
                    RowId = x.PersonInCharge.RowId,
                    Used = x.PersonInCharge.Used,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                    Used = x.Province.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                    Used = x.Ward.Used,
                },
            }).FirstOrDefaultAsync();

            if (Supplier == null)
                return null;
            Supplier.SupplierBankAccounts = await DataContext.SupplierBankAccount.AsNoTracking()
                .Where(x => x.SupplierId == Supplier.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new SupplierBankAccount
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                    BankAccountOwnerName = x.BankAccountOwnerName,
                    Used = x.Used,
                    RowId = x.RowId,
                }).ToListAsync();
            Supplier.SupplierCategoryMappings = await DataContext.SupplierCategoryMapping.AsNoTracking()
                .Where(x => x.SupplierId == Supplier.Id)
                .Where(x => x.Category.DeletedAt == null)
                .Select(x => new SupplierCategoryMapping
                {
                    SupplierId = x.SupplierId,
                    CategoryId = x.CategoryId,
                    Category = new Category
                    {
                        Id = x.Category.Id,
                        Code = x.Category.Code,
                        Name = x.Category.Name,
                        ParentId = x.Category.ParentId,
                        Path = x.Category.Path,
                        Level = x.Category.Level,
                        StatusId = x.Category.StatusId,
                        ImageId = x.Category.ImageId,
                        RowId = x.Category.RowId,
                        Used = x.Category.Used,
                    },
                }).ToListAsync();
            Supplier.SupplierContactors = await DataContext.SupplierContactor.AsNoTracking()
                .Where(x => x.SupplierId == Supplier.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new SupplierContactor
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    Name = x.Name,
                    Phone = x.Phone,
                    Email = x.Email,
                    Used = x.Used,
                    RowId = x.RowId,
                }).ToListAsync();

            return Supplier;
        }

        public async Task<bool> Create(Supplier Supplier)
        {
            Supplier.RowId = Guid.NewGuid();
            SupplierDAO SupplierDAO = new SupplierDAO();
            SupplierDAO.Id = Supplier.Id;
            SupplierDAO.Code = Supplier.Code;
            SupplierDAO.Name = Supplier.Name;
            SupplierDAO.TaxCode = Supplier.TaxCode;
            SupplierDAO.Phone = Supplier.Phone;
            SupplierDAO.Email = Supplier.Email;
            SupplierDAO.Avatar = Supplier.Avatar;
            SupplierDAO.Address = Supplier.Address;
            SupplierDAO.NationId = Supplier.NationId;
            SupplierDAO.ProvinceId = Supplier.ProvinceId;
            SupplierDAO.DistrictId = Supplier.DistrictId;
            SupplierDAO.WardId = Supplier.WardId;
            SupplierDAO.OwnerName = Supplier.OwnerName;
            SupplierDAO.PersonInChargeId = Supplier.PersonInChargeId;
            SupplierDAO.StatusId = Supplier.StatusId;
            SupplierDAO.Description = Supplier.Description;
            SupplierDAO.Used = Supplier.Used;
            SupplierDAO.RowId = Supplier.RowId;
            //SupplierDAO.RowId = Guid.NewGuid();
            SupplierDAO.CreatedAt = StaticParams.DateTimeNow;
            SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Supplier.Add(SupplierDAO);
            await DataContext.SaveChangesAsync();
            Supplier.Id = SupplierDAO.Id;
            await SaveReference(Supplier);
            return true;
        }

        public async Task<bool> Update(Supplier Supplier)
        {
            SupplierDAO SupplierDAO = DataContext.Supplier.Where(x => x.Id == Supplier.Id).FirstOrDefault();
            if (SupplierDAO == null)
                return false;
            SupplierDAO.Id = Supplier.Id;
            SupplierDAO.Code = Supplier.Code;
            SupplierDAO.Name = Supplier.Name;
            SupplierDAO.TaxCode = Supplier.TaxCode;
            SupplierDAO.Phone = Supplier.Phone;
            SupplierDAO.Email = Supplier.Email;
            SupplierDAO.Avatar = Supplier.Avatar;
            SupplierDAO.Address = Supplier.Address;
            SupplierDAO.NationId = Supplier.NationId;
            SupplierDAO.ProvinceId = Supplier.ProvinceId;
            SupplierDAO.DistrictId = Supplier.DistrictId;
            SupplierDAO.WardId = Supplier.WardId;
            SupplierDAO.OwnerName = Supplier.OwnerName;
            SupplierDAO.PersonInChargeId = Supplier.PersonInChargeId;
            SupplierDAO.StatusId = Supplier.StatusId;
            SupplierDAO.Description = Supplier.Description;
            SupplierDAO.Used = Supplier.Used;
            SupplierDAO.RowId = Supplier.RowId;
            SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Supplier);
            return true;
        }

        public async Task<bool> Delete(Supplier Supplier)
        {
            await DataContext.Supplier.Where(x => x.Id == Supplier.Id).UpdateFromQueryAsync(x => new SupplierDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Supplier> Suppliers)
        {
            List<SupplierDAO> SupplierDAOs = new List<SupplierDAO>();
            foreach (Supplier Supplier in Suppliers)
            {
                SupplierDAO SupplierDAO = new SupplierDAO();
                SupplierDAO.Id = Supplier.Id;
                SupplierDAO.Code = Supplier.Code;
                SupplierDAO.Name = Supplier.Name;
                SupplierDAO.TaxCode = Supplier.TaxCode;
                SupplierDAO.Phone = Supplier.Phone;
                SupplierDAO.Email = Supplier.Email;
                SupplierDAO.Avatar = Supplier.Avatar;
                SupplierDAO.Address = Supplier.Address;
                SupplierDAO.NationId = Supplier.NationId;
                SupplierDAO.ProvinceId = Supplier.ProvinceId;
                SupplierDAO.DistrictId = Supplier.DistrictId;
                SupplierDAO.WardId = Supplier.WardId;
                SupplierDAO.OwnerName = Supplier.OwnerName;
                SupplierDAO.PersonInChargeId = Supplier.PersonInChargeId;
                SupplierDAO.StatusId = Supplier.StatusId;
                SupplierDAO.Description = Supplier.Description;
                SupplierDAO.Used = Supplier.Used;
                SupplierDAO.RowId = Supplier.RowId;
                SupplierDAO.CreatedAt = StaticParams.DateTimeNow;
                SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
                SupplierDAOs.Add(SupplierDAO);
            }
            await DataContext.BulkMergeAsync(SupplierDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Supplier> Suppliers)
        {
            List<long> Ids = Suppliers.Select(x => x.Id).ToList();
            await DataContext.Supplier
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new SupplierDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkUpdate(List<Supplier> Suppliers)
        {
            #region correct input
            foreach (Supplier Supplier in Suppliers)
            {
                Supplier.SupplierBankAccounts.ForEach(x => x.SupplierId = Supplier.Id);
                Supplier.SupplierCategoryMappings.ForEach(x => x.SupplierId = Supplier.Id);
                Supplier.SupplierContactors.ForEach(x => x.SupplierId = Supplier.Id);
            }
            #endregion

            List<long> SupplierIds = Suppliers.Select(x => x.Id).ToList();

            #region Supplier
            List<SupplierDAO> SupplierDAOs = await DataContext.Supplier
                .Where(x => SupplierIds.Contains(x.Id))
                .ToListAsync();
            foreach (SupplierDAO SupplierDAO in SupplierDAOs)
            {
                Supplier Supplier = Suppliers
                    .Where(x => x.Id == SupplierDAO.Id)
                    .FirstOrDefault();
                SupplierDAO.Id = Supplier.Id;
                SupplierDAO.Code = Supplier.Code;
                SupplierDAO.Name = Supplier.Name;
                SupplierDAO.TaxCode = Supplier.TaxCode;
                SupplierDAO.Phone = Supplier.Phone;
                SupplierDAO.Email = Supplier.Email;
                SupplierDAO.Avatar = Supplier.Avatar;
                SupplierDAO.Address = Supplier.Address;
                SupplierDAO.NationId = Supplier.NationId;
                SupplierDAO.ProvinceId = Supplier.ProvinceId;
                SupplierDAO.DistrictId = Supplier.DistrictId;
                SupplierDAO.WardId = Supplier.WardId;
                SupplierDAO.OwnerName = Supplier.OwnerName;
                SupplierDAO.PersonInChargeId = Supplier.PersonInChargeId;
                SupplierDAO.StatusId = Supplier.StatusId;
                SupplierDAO.Description = Supplier.Description;
                SupplierDAO.Used = Supplier.Used;
                SupplierDAO.RowId = Supplier.RowId;
                SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
            }
            await DataContext.BulkMergeAsync(SupplierDAOs);
            #endregion

            #region SupplierBankAccount
            List<SupplierBankAccount> SupplierBankAccounts = Suppliers.SelectMany(x => x.SupplierBankAccounts).ToList();
            List<SupplierBankAccountDAO> SupplierBankAccountDAOs = await DataContext.SupplierBankAccount
                .Where(x => SupplierIds.Contains(x.SupplierId))
                .ToListAsync();
            SupplierBankAccountDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            foreach (SupplierBankAccount SupplierBankAccount in SupplierBankAccounts)
            {
                SupplierBankAccountDAO SupplierBankAccountDAO = SupplierBankAccountDAOs
                    .Where(x => x.Id == SupplierBankAccount.Id && x.Id != 0).FirstOrDefault();
                if (SupplierBankAccountDAO == null)
                {
                    SupplierBankAccountDAO = new SupplierBankAccountDAO();
                    SupplierBankAccountDAO.Id = SupplierBankAccount.Id;
                    SupplierBankAccountDAO.SupplierId = SupplierBankAccount.SupplierId;
                    SupplierBankAccountDAO.BankName = SupplierBankAccount.BankName;
                    SupplierBankAccountDAO.BankAccount = SupplierBankAccount.BankAccount;
                    SupplierBankAccountDAO.BankAccountOwnerName = SupplierBankAccount.BankAccountOwnerName;
                    SupplierBankAccountDAO.Used = SupplierBankAccount.Used;
                    SupplierBankAccountDAO.RowId = SupplierBankAccount.RowId;
                    SupplierBankAccountDAOs.Add(SupplierBankAccountDAO);
                    SupplierBankAccountDAO.CreatedAt = StaticParams.DateTimeNow;
                    SupplierBankAccountDAO.UpdatedAt = StaticParams.DateTimeNow;
                    SupplierBankAccountDAO.DeletedAt = null;
                }
                else
                {
                    SupplierBankAccountDAO.Id = SupplierBankAccount.Id;
                    SupplierBankAccountDAO.SupplierId = SupplierBankAccount.SupplierId;
                    SupplierBankAccountDAO.BankName = SupplierBankAccount.BankName;
                    SupplierBankAccountDAO.BankAccount = SupplierBankAccount.BankAccount;
                    SupplierBankAccountDAO.BankAccountOwnerName = SupplierBankAccount.BankAccountOwnerName;
                    SupplierBankAccountDAO.Used = SupplierBankAccount.Used;
                    SupplierBankAccountDAO.RowId = SupplierBankAccount.RowId;
                    SupplierBankAccountDAO.UpdatedAt = StaticParams.DateTimeNow;
                    SupplierBankAccountDAO.DeletedAt = null;
                }
            }
            await DataContext.BulkMergeAsync(SupplierBankAccounts);
            #endregion

            #region SupplierCategoryMapping
            await DataContext.SupplierCategoryMapping
                .Where(x => SupplierIds.Contains(x.SupplierId))
                .DeleteFromQueryAsync();
            List<SupplierCategoryMappingDAO> SupplierCategoryMappingDAOs = Suppliers
                .SelectMany(x => x.SupplierCategoryMappings)
                .Select(x => new SupplierCategoryMappingDAO
                {
                    SupplierId = x.SupplierId,
                    CategoryId = x.CategoryId,
                })
                .ToList();
            await DataContext.BulkMergeAsync(SupplierCategoryMappingDAOs);
            #endregion

            #region SupplierContactor
            List<SupplierContactor> SupplierContactors = Suppliers.SelectMany(x => x.SupplierContactors).ToList();
            List<SupplierContactorDAO> SupplierContactorDAOs = await DataContext.SupplierContactor
                .Where(x => SupplierIds.Contains(x.SupplierId))
                .ToListAsync();
            SupplierContactorDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            foreach (SupplierContactor SupplierContactor in SupplierContactors)
            {
                SupplierContactorDAO SupplierContactorDAO = SupplierContactorDAOs
                    .Where(x => x.Id == SupplierContactor.Id && x.Id != 0).FirstOrDefault();
                if (SupplierContactorDAO == null)
                {
                    SupplierContactorDAO = new SupplierContactorDAO();
                    SupplierContactorDAO.Id = SupplierContactor.Id;
                    SupplierContactorDAO.SupplierId = SupplierContactor.SupplierId;
                    SupplierContactorDAO.Name = SupplierContactor.Name;
                    SupplierContactorDAO.Phone = SupplierContactor.Phone;
                    SupplierContactorDAO.Email = SupplierContactor.Email;
                    SupplierContactorDAO.Used = SupplierContactor.Used;
                    SupplierContactorDAO.RowId = SupplierContactor.RowId;
                    SupplierContactorDAOs.Add(SupplierContactorDAO);
                    SupplierContactorDAO.CreatedAt = StaticParams.DateTimeNow;
                    SupplierContactorDAO.UpdatedAt = StaticParams.DateTimeNow;
                    SupplierContactorDAO.DeletedAt = null;
                }
                else
                {
                    SupplierContactorDAO.Id = SupplierContactor.Id;
                    SupplierContactorDAO.SupplierId = SupplierContactor.SupplierId;
                    SupplierContactorDAO.Name = SupplierContactor.Name;
                    SupplierContactorDAO.Phone = SupplierContactor.Phone;
                    SupplierContactorDAO.Email = SupplierContactor.Email;
                    SupplierContactorDAO.Used = SupplierContactor.Used;
                    SupplierContactorDAO.RowId = SupplierContactor.RowId;
                    SupplierContactorDAO.UpdatedAt = StaticParams.DateTimeNow;
                    SupplierContactorDAO.DeletedAt = null;
                }
            }
            await DataContext.SupplierContactor.BulkMergeAsync(SupplierContactorDAOs);
            #endregion

            return true;
        }

        private async Task SaveReference(Supplier Supplier)
        {
            List<SupplierBankAccountDAO> SupplierBankAccountDAOs = await DataContext.SupplierBankAccount
                .Where(x => x.SupplierId == Supplier.Id).ToListAsync();
            SupplierBankAccountDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Supplier.SupplierBankAccounts != null)
            {
                foreach (SupplierBankAccount SupplierBankAccount in Supplier.SupplierBankAccounts)
                {
                    SupplierBankAccountDAO SupplierBankAccountDAO = SupplierBankAccountDAOs
                        .Where(x => x.Id == SupplierBankAccount.Id && x.Id != 0).FirstOrDefault();
                    if (SupplierBankAccountDAO == null)
                    {
                        SupplierBankAccountDAO = new SupplierBankAccountDAO();
                        SupplierBankAccountDAO.Id = SupplierBankAccount.Id;
                        SupplierBankAccountDAO.SupplierId = Supplier.Id;
                        SupplierBankAccountDAO.BankName = SupplierBankAccount.BankName;
                        SupplierBankAccountDAO.BankAccount = SupplierBankAccount.BankAccount;
                        SupplierBankAccountDAO.BankAccountOwnerName = SupplierBankAccount.BankAccountOwnerName;
                        SupplierBankAccountDAO.Used = SupplierBankAccount.Used;
                        SupplierBankAccountDAO.RowId = SupplierBankAccount.RowId;
                        SupplierBankAccountDAOs.Add(SupplierBankAccountDAO);
                        SupplierBankAccountDAO.CreatedAt = StaticParams.DateTimeNow;
                        SupplierBankAccountDAO.UpdatedAt = StaticParams.DateTimeNow;
                        SupplierBankAccountDAO.DeletedAt = null;
                    }
                    else
                    {
                        SupplierBankAccountDAO.Id = SupplierBankAccount.Id;
                        SupplierBankAccountDAO.SupplierId = Supplier.Id;
                        SupplierBankAccountDAO.BankName = SupplierBankAccount.BankName;
                        SupplierBankAccountDAO.BankAccount = SupplierBankAccount.BankAccount;
                        SupplierBankAccountDAO.BankAccountOwnerName = SupplierBankAccount.BankAccountOwnerName;
                        SupplierBankAccountDAO.Used = SupplierBankAccount.Used;
                        SupplierBankAccountDAO.RowId = SupplierBankAccount.RowId;
                        SupplierBankAccountDAO.UpdatedAt = StaticParams.DateTimeNow;
                        SupplierBankAccountDAO.DeletedAt = null;
                    }
                }
                await DataContext.SupplierBankAccount.BulkMergeAsync(SupplierBankAccountDAOs);
            }
            await DataContext.SupplierCategoryMapping
                .Where(x => x.SupplierId == Supplier.Id)
                .DeleteFromQueryAsync();
            List<SupplierCategoryMappingDAO> SupplierCategoryMappingDAOs = new List<SupplierCategoryMappingDAO>();
            if (Supplier.SupplierCategoryMappings != null)
            {
                foreach (SupplierCategoryMapping SupplierCategoryMapping in Supplier.SupplierCategoryMappings)
                {
                    SupplierCategoryMappingDAO SupplierCategoryMappingDAO = new SupplierCategoryMappingDAO();
                    SupplierCategoryMappingDAO.SupplierId = Supplier.Id;
                    SupplierCategoryMappingDAO.CategoryId = SupplierCategoryMapping.CategoryId;
                    SupplierCategoryMappingDAOs.Add(SupplierCategoryMappingDAO);
                }
                await DataContext.SupplierCategoryMapping.BulkMergeAsync(SupplierCategoryMappingDAOs);
            }
            List<SupplierContactorDAO> SupplierContactorDAOs = await DataContext.SupplierContactor
                .Where(x => x.SupplierId == Supplier.Id).ToListAsync();
            SupplierContactorDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Supplier.SupplierContactors != null)
            {
                foreach (SupplierContactor SupplierContactor in Supplier.SupplierContactors)
                {
                    SupplierContactorDAO SupplierContactorDAO = SupplierContactorDAOs
                        .Where(x => x.Id == SupplierContactor.Id && x.Id != 0).FirstOrDefault();
                    if (SupplierContactorDAO == null)
                    {
                        SupplierContactorDAO = new SupplierContactorDAO();
                        SupplierContactorDAO.Id = SupplierContactor.Id;
                        SupplierContactorDAO.SupplierId = Supplier.Id;
                        SupplierContactorDAO.Name = SupplierContactor.Name;
                        SupplierContactorDAO.Phone = SupplierContactor.Phone;
                        SupplierContactorDAO.Email = SupplierContactor.Email;
                        SupplierContactorDAO.Used = SupplierContactor.Used;
                        SupplierContactorDAO.RowId = SupplierContactor.RowId;
                        SupplierContactorDAOs.Add(SupplierContactorDAO);
                        SupplierContactorDAO.CreatedAt = StaticParams.DateTimeNow;
                        SupplierContactorDAO.UpdatedAt = StaticParams.DateTimeNow;
                        SupplierContactorDAO.DeletedAt = null;
                    }
                    else
                    {
                        SupplierContactorDAO.Id = SupplierContactor.Id;
                        SupplierContactorDAO.SupplierId = Supplier.Id;
                        SupplierContactorDAO.Name = SupplierContactor.Name;
                        SupplierContactorDAO.Phone = SupplierContactor.Phone;
                        SupplierContactorDAO.Email = SupplierContactor.Email;
                        SupplierContactorDAO.Used = SupplierContactor.Used;
                        SupplierContactorDAO.RowId = SupplierContactor.RowId;
                        SupplierContactorDAO.UpdatedAt = StaticParams.DateTimeNow;
                        SupplierContactorDAO.DeletedAt = null;
                    }
                }
                await DataContext.SupplierContactor.BulkMergeAsync(SupplierContactorDAOs);
            }
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Supplier.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new SupplierDAO { Used = true });
            return true;
        }
    }
}
