using ELODIE.Common;
using ELODIE.Helpers;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace ELODIE.Repositories
{
    public interface ICustomerRepository
    {
        Task<int> CountAll(CustomerFilter CustomerFilter);
        Task<int> Count(CustomerFilter CustomerFilter);
        Task<List<Customer>> List(CustomerFilter CustomerFilter);
        Task<List<Customer>> List(List<long> Ids);
        Task<Customer> Get(long Id);
        Task<bool> Create(Customer Customer);
        Task<bool> Update(Customer Customer);
        Task<bool> Delete(Customer Customer);
        Task<bool> BulkMerge(List<Customer> Customers);
        Task<bool> BulkDelete(List<Customer> Customers);
        Task<bool> Used(List<long> Ids);
    }
    public class CustomerRepository : ICustomerRepository
    {
        private DataContext DataContext;
        public CustomerRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CustomerDAO> DynamicFilter(IQueryable<CustomerDAO> query, CustomerFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.CodeDraft, filter.CodeDraft);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Phone, filter.Phone);
            query = query.Where(q => q.Address, filter.Address);
            query = query.Where(q => q.NationId, filter.NationId);
            query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            query = query.Where(q => q.DistrictId, filter.DistrictId);
            query = query.Where(q => q.WardId, filter.WardId);
            query = query.Where(q => q.Birthday, filter.Birthday);
            query = query.Where(q => q.Email, filter.Email);
            query = query.Where(q => q.ProfessionId, filter.ProfessionId);
            query = query.Where(q => q.CustomerSourceId, filter.CustomerSourceId);
            query = query.Where(q => q.SexId, filter.SexId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            query = query.Where(q => q.RowId, filter.RowId);
            query = query.Where(q => q.CodeGeneratorRuleId, filter.CodeGeneratorRuleId);
            
            return query;
        }

        private IQueryable<CustomerDAO> OrFilter(IQueryable<CustomerDAO> query, CustomerFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CustomerDAO> initQuery = query.Where(q => false);
            foreach (CustomerFilter CustomerFilter in filter.OrFilter)
            {
                IQueryable<CustomerDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.CodeDraft, filter.CodeDraft);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.Phone, filter.Phone);
                queryable = queryable.Where(q => q.Address, filter.Address);
                queryable = queryable.Where(q => q.NationId, filter.NationId);
                queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                queryable = queryable.Where(q => q.WardId, filter.WardId);
                queryable = queryable.Where(q => q.Birthday, filter.Birthday);
                queryable = queryable.Where(q => q.Email, filter.Email);
                queryable = queryable.Where(q => q.ProfessionId, filter.ProfessionId);
                queryable = queryable.Where(q => q.CustomerSourceId, filter.CustomerSourceId);
                queryable = queryable.Where(q => q.SexId, filter.SexId);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                queryable = queryable.Where(q => q.CodeGeneratorRuleId, filter.CodeGeneratorRuleId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CustomerDAO> DynamicOrder(IQueryable<CustomerDAO> query, CustomerFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CustomerOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CustomerOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CustomerOrder.CodeDraft:
                            query = query.OrderBy(q => q.CodeDraft);
                            break;
                        case CustomerOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case CustomerOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case CustomerOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case CustomerOrder.Nation:
                            query = query.OrderBy(q => q.NationId);
                            break;
                        case CustomerOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case CustomerOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case CustomerOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case CustomerOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                        case CustomerOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case CustomerOrder.Profession:
                            query = query.OrderBy(q => q.ProfessionId);
                            break;
                        case CustomerOrder.CustomerSource:
                            query = query.OrderBy(q => q.CustomerSourceId);
                            break;
                        case CustomerOrder.Sex:
                            query = query.OrderBy(q => q.SexId);
                            break;
                        case CustomerOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case CustomerOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case CustomerOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case CustomerOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case CustomerOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case CustomerOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case CustomerOrder.CodeGeneratorRule:
                            query = query.OrderBy(q => q.CodeGeneratorRuleId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CustomerOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CustomerOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CustomerOrder.CodeDraft:
                            query = query.OrderByDescending(q => q.CodeDraft);
                            break;
                        case CustomerOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case CustomerOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case CustomerOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case CustomerOrder.Nation:
                            query = query.OrderByDescending(q => q.NationId);
                            break;
                        case CustomerOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case CustomerOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case CustomerOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case CustomerOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                        case CustomerOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case CustomerOrder.Profession:
                            query = query.OrderByDescending(q => q.ProfessionId);
                            break;
                        case CustomerOrder.CustomerSource:
                            query = query.OrderByDescending(q => q.CustomerSourceId);
                            break;
                        case CustomerOrder.Sex:
                            query = query.OrderByDescending(q => q.SexId);
                            break;
                        case CustomerOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case CustomerOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case CustomerOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case CustomerOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case CustomerOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case CustomerOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case CustomerOrder.CodeGeneratorRule:
                            query = query.OrderByDescending(q => q.CodeGeneratorRuleId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Customer>> DynamicSelect(IQueryable<CustomerDAO> query, CustomerFilter filter)
        {
            List<Customer> Customers = await query.Select(q => new Customer()
            {
                Id = filter.Selects.Contains(CustomerSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CustomerSelect.Code) ? q.Code : default(string),
                CodeDraft = filter.Selects.Contains(CustomerSelect.CodeDraft) ? q.CodeDraft : default(string),
                Name = filter.Selects.Contains(CustomerSelect.Name) ? q.Name : default(string),
                Phone = filter.Selects.Contains(CustomerSelect.Phone) ? q.Phone : default(string),
                Address = filter.Selects.Contains(CustomerSelect.Address) ? q.Address : default(string),
                NationId = filter.Selects.Contains(CustomerSelect.Nation) ? q.NationId : default(long?),
                ProvinceId = filter.Selects.Contains(CustomerSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(CustomerSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(CustomerSelect.Ward) ? q.WardId : default(long?),
                Birthday = filter.Selects.Contains(CustomerSelect.Birthday) ? q.Birthday : default(DateTime?),
                Email = filter.Selects.Contains(CustomerSelect.Email) ? q.Email : default(string),
                ProfessionId = filter.Selects.Contains(CustomerSelect.Profession) ? q.ProfessionId : default(long?),
                CustomerSourceId = filter.Selects.Contains(CustomerSelect.CustomerSource) ? q.CustomerSourceId : default(long?),
                SexId = filter.Selects.Contains(CustomerSelect.Sex) ? q.SexId : default(long?),
                StatusId = filter.Selects.Contains(CustomerSelect.Status) ? q.StatusId : default(long),
                AppUserId = filter.Selects.Contains(CustomerSelect.AppUser) ? q.AppUserId : default(long),
                CreatorId = filter.Selects.Contains(CustomerSelect.Creator) ? q.CreatorId : default(long),
                OrganizationId = filter.Selects.Contains(CustomerSelect.Organization) ? q.OrganizationId : default(long),
                Used = filter.Selects.Contains(CustomerSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(CustomerSelect.Row) ? q.RowId : default(Guid),
                CodeGeneratorRuleId = filter.Selects.Contains(CustomerSelect.CodeGeneratorRule) ? q.CodeGeneratorRuleId : default(long?),
                AppUser = filter.Selects.Contains(CustomerSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                    Address = q.AppUser.Address,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                    SexId = q.AppUser.SexId,
                    Birthday = q.AppUser.Birthday,
                    Avatar = q.AppUser.Avatar,
                    Department = q.AppUser.Department,
                    OrganizationId = q.AppUser.OrganizationId,
                    StatusId = q.AppUser.StatusId,
                    RowId = q.AppUser.RowId,
                    Used = q.AppUser.Used,
                    Password = q.AppUser.Password,
                    OtpCode = q.AppUser.OtpCode,
                    OtpExpired = q.AppUser.OtpExpired,
                } : null,
                CodeGeneratorRule = filter.Selects.Contains(CustomerSelect.CodeGeneratorRule) && q.CodeGeneratorRule != null ? new CodeGeneratorRule
                {
                    Id = q.CodeGeneratorRule.Id,
                    EntityTypeId = q.CodeGeneratorRule.EntityTypeId,
                    AutoNumberLenth = q.CodeGeneratorRule.AutoNumberLenth,
                    StatusId = q.CodeGeneratorRule.StatusId,
                    RowId = q.CodeGeneratorRule.RowId,
                    Used = q.CodeGeneratorRule.Used,
                } : null,
                Creator = filter.Selects.Contains(CustomerSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                    Avatar = q.Creator.Avatar,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    StatusId = q.Creator.StatusId,
                    RowId = q.Creator.RowId,
                    Used = q.Creator.Used,
                    Password = q.Creator.Password,
                    OtpCode = q.Creator.OtpCode,
                    OtpExpired = q.Creator.OtpExpired,
                } : null,
                CustomerSource = filter.Selects.Contains(CustomerSelect.CustomerSource) && q.CustomerSource != null ? new CustomerSource
                {
                    Id = q.CustomerSource.Id,
                    Code = q.CustomerSource.Code,
                    Name = q.CustomerSource.Name,
                    StatusId = q.CustomerSource.StatusId,
                    Description = q.CustomerSource.Description,
                    Used = q.CustomerSource.Used,
                    RowId = q.CustomerSource.RowId,
                } : null,
                District = filter.Selects.Contains(CustomerSelect.District) && q.District != null ? new District
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
                Nation = filter.Selects.Contains(CustomerSelect.Nation) && q.Nation != null ? new Nation
                {
                    Id = q.Nation.Id,
                    Code = q.Nation.Code,
                    Name = q.Nation.Name,
                    Priority = q.Nation.Priority,
                    StatusId = q.Nation.StatusId,
                    Used = q.Nation.Used,
                    RowId = q.Nation.RowId,
                } : null,
                Organization = filter.Selects.Contains(CustomerSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Email = q.Organization.Email,
                    Address = q.Organization.Address,
                    RowId = q.Organization.RowId,
                    Used = q.Organization.Used,
                    IsDisplay = q.Organization.IsDisplay,
                } : null,
                Profession = filter.Selects.Contains(CustomerSelect.Profession) && q.Profession != null ? new Profession
                {
                    Id = q.Profession.Id,
                    Code = q.Profession.Code,
                    Name = q.Profession.Name,
                    StatusId = q.Profession.StatusId,
                    RowId = q.Profession.RowId,
                    Used = q.Profession.Used,
                } : null,
                Province = filter.Selects.Contains(CustomerSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                    RowId = q.Province.RowId,
                    Used = q.Province.Used,
                } : null,
                Ward = filter.Selects.Contains(CustomerSelect.Ward) && q.Ward != null ? new Ward
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
            return Customers;
        }

        public async Task<int> CountAll(CustomerFilter filter)
        {
            IQueryable<CustomerDAO> Customers = DataContext.Customer.AsNoTracking();
            Customers = DynamicFilter(Customers, filter);
            return await Customers.CountAsync();
        }

        public async Task<int> Count(CustomerFilter filter)
        {
            IQueryable<CustomerDAO> Customers = DataContext.Customer.AsNoTracking();
            Customers = DynamicFilter(Customers, filter);
            Customers = OrFilter(Customers, filter);
            return await Customers.CountAsync();
        }

        public async Task<List<Customer>> List(CustomerFilter filter)
        {
            if (filter == null) return new List<Customer>();
            IQueryable<CustomerDAO> CustomerDAOs = DataContext.Customer.AsNoTracking();
            CustomerDAOs = DynamicFilter(CustomerDAOs, filter);
            CustomerDAOs = OrFilter(CustomerDAOs, filter);
            CustomerDAOs = DynamicOrder(CustomerDAOs, filter);
            List<Customer> Customers = await DynamicSelect(CustomerDAOs, filter);
            return Customers;
        }

        public async Task<List<Customer>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.Customer
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<Customer> Customers = await query.AsNoTracking()
            .Select(x => new Customer()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                CodeDraft = x.CodeDraft,
                Name = x.Name,
                Phone = x.Phone,
                Address = x.Address,
                NationId = x.NationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                Birthday = x.Birthday,
                Email = x.Email,
                ProfessionId = x.ProfessionId,
                CustomerSourceId = x.CustomerSourceId,
                SexId = x.SexId,
                StatusId = x.StatusId,
                AppUserId = x.AppUserId,
                CreatorId = x.CreatorId,
                OrganizationId = x.OrganizationId,
                Used = x.Used,
                RowId = x.RowId,
                CodeGeneratorRuleId = x.CodeGeneratorRuleId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    StatusId = x.AppUser.StatusId,
                    RowId = x.AppUser.RowId,
                    Used = x.AppUser.Used,
                    Password = x.AppUser.Password,
                    OtpCode = x.AppUser.OtpCode,
                    OtpExpired = x.AppUser.OtpExpired,
                },
                CodeGeneratorRule = x.CodeGeneratorRule == null ? null : new CodeGeneratorRule
                {
                    Id = x.CodeGeneratorRule.Id,
                    EntityTypeId = x.CodeGeneratorRule.EntityTypeId,
                    AutoNumberLenth = x.CodeGeneratorRule.AutoNumberLenth,
                    StatusId = x.CodeGeneratorRule.StatusId,
                    RowId = x.CodeGeneratorRule.RowId,
                    Used = x.CodeGeneratorRule.Used,
                },
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    StatusId = x.Creator.StatusId,
                    RowId = x.Creator.RowId,
                    Used = x.Creator.Used,
                    Password = x.Creator.Password,
                    OtpCode = x.Creator.OtpCode,
                    OtpExpired = x.Creator.OtpExpired,
                },
                CustomerSource = x.CustomerSource == null ? null : new CustomerSource
                {
                    Id = x.CustomerSource.Id,
                    Code = x.CustomerSource.Code,
                    Name = x.CustomerSource.Name,
                    StatusId = x.CustomerSource.StatusId,
                    Description = x.CustomerSource.Description,
                    Used = x.CustomerSource.Used,
                    RowId = x.CustomerSource.RowId,
                },
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
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                    Phone = x.Organization.Phone,
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                    RowId = x.Organization.RowId,
                    Used = x.Organization.Used,
                    IsDisplay = x.Organization.IsDisplay,
                },
                Profession = x.Profession == null ? null : new Profession
                {
                    Id = x.Profession.Id,
                    Code = x.Profession.Code,
                    Name = x.Profession.Name,
                    StatusId = x.Profession.StatusId,
                    RowId = x.Profession.RowId,
                    Used = x.Profession.Used,
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
            

            return Customers;
        }

        public async Task<Customer> Get(long Id)
        {
            Customer Customer = await DataContext.Customer.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Customer()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                CodeDraft = x.CodeDraft,
                Name = x.Name,
                Phone = x.Phone,
                Address = x.Address,
                NationId = x.NationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                Birthday = x.Birthday,
                Email = x.Email,
                ProfessionId = x.ProfessionId,
                CustomerSourceId = x.CustomerSourceId,
                SexId = x.SexId,
                StatusId = x.StatusId,
                AppUserId = x.AppUserId,
                CreatorId = x.CreatorId,
                OrganizationId = x.OrganizationId,
                Used = x.Used,
                RowId = x.RowId,
                CodeGeneratorRuleId = x.CodeGeneratorRuleId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    StatusId = x.AppUser.StatusId,
                    RowId = x.AppUser.RowId,
                    Used = x.AppUser.Used,
                    Password = x.AppUser.Password,
                    OtpCode = x.AppUser.OtpCode,
                    OtpExpired = x.AppUser.OtpExpired,
                },
                CodeGeneratorRule = x.CodeGeneratorRule == null ? null : new CodeGeneratorRule
                {
                    Id = x.CodeGeneratorRule.Id,
                    EntityTypeId = x.CodeGeneratorRule.EntityTypeId,
                    AutoNumberLenth = x.CodeGeneratorRule.AutoNumberLenth,
                    StatusId = x.CodeGeneratorRule.StatusId,
                    RowId = x.CodeGeneratorRule.RowId,
                    Used = x.CodeGeneratorRule.Used,
                },
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    StatusId = x.Creator.StatusId,
                    RowId = x.Creator.RowId,
                    Used = x.Creator.Used,
                    Password = x.Creator.Password,
                    OtpCode = x.Creator.OtpCode,
                    OtpExpired = x.Creator.OtpExpired,
                },
                CustomerSource = x.CustomerSource == null ? null : new CustomerSource
                {
                    Id = x.CustomerSource.Id,
                    Code = x.CustomerSource.Code,
                    Name = x.CustomerSource.Name,
                    StatusId = x.CustomerSource.StatusId,
                    Description = x.CustomerSource.Description,
                    Used = x.CustomerSource.Used,
                    RowId = x.CustomerSource.RowId,
                },
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
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                    Phone = x.Organization.Phone,
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                    RowId = x.Organization.RowId,
                    Used = x.Organization.Used,
                    IsDisplay = x.Organization.IsDisplay,
                },
                Profession = x.Profession == null ? null : new Profession
                {
                    Id = x.Profession.Id,
                    Code = x.Profession.Code,
                    Name = x.Profession.Name,
                    StatusId = x.Profession.StatusId,
                    RowId = x.Profession.RowId,
                    Used = x.Profession.Used,
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

            if (Customer == null)
                return null;

            return Customer;
        }
        
        public async Task<bool> Create(Customer Customer)
        {
            CustomerDAO CustomerDAO = new CustomerDAO();
            CustomerDAO.Id = Customer.Id;
            CustomerDAO.Code = Customer.Code;
            CustomerDAO.CodeDraft = Customer.CodeDraft;
            CustomerDAO.Name = Customer.Name;
            CustomerDAO.Phone = Customer.Phone;
            CustomerDAO.Address = Customer.Address;
            CustomerDAO.NationId = Customer.NationId;
            CustomerDAO.ProvinceId = Customer.ProvinceId;
            CustomerDAO.DistrictId = Customer.DistrictId;
            CustomerDAO.WardId = Customer.WardId;
            CustomerDAO.Birthday = Customer.Birthday;
            CustomerDAO.Email = Customer.Email;
            CustomerDAO.ProfessionId = Customer.ProfessionId;
            CustomerDAO.CustomerSourceId = Customer.CustomerSourceId;
            CustomerDAO.SexId = Customer.SexId;
            CustomerDAO.StatusId = Customer.StatusId;
            CustomerDAO.AppUserId = Customer.AppUserId;
            CustomerDAO.CreatorId = Customer.CreatorId;
            CustomerDAO.OrganizationId = Customer.OrganizationId;
            CustomerDAO.Used = Customer.Used;
            CustomerDAO.RowId = Customer.RowId;
            CustomerDAO.CodeGeneratorRuleId = Customer.CodeGeneratorRuleId;
            CustomerDAO.RowId = Guid.NewGuid();
            CustomerDAO.CreatedAt = StaticParams.DateTimeNow;
            CustomerDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Customer.Add(CustomerDAO);
            await DataContext.SaveChangesAsync();
            Customer.Id = CustomerDAO.Id;
            await SaveReference(Customer);
            return true;
        }

        public async Task<bool> Update(Customer Customer)
        {
            CustomerDAO CustomerDAO = DataContext.Customer.Where(x => x.Id == Customer.Id).FirstOrDefault();
            if (CustomerDAO == null)
                return false;
            CustomerDAO.Id = Customer.Id;
            CustomerDAO.Code = Customer.Code;
            CustomerDAO.CodeDraft = Customer.CodeDraft;
            CustomerDAO.Name = Customer.Name;
            CustomerDAO.Phone = Customer.Phone;
            CustomerDAO.Address = Customer.Address;
            CustomerDAO.NationId = Customer.NationId;
            CustomerDAO.ProvinceId = Customer.ProvinceId;
            CustomerDAO.DistrictId = Customer.DistrictId;
            CustomerDAO.WardId = Customer.WardId;
            CustomerDAO.Birthday = Customer.Birthday;
            CustomerDAO.Email = Customer.Email;
            CustomerDAO.ProfessionId = Customer.ProfessionId;
            CustomerDAO.CustomerSourceId = Customer.CustomerSourceId;
            CustomerDAO.SexId = Customer.SexId;
            CustomerDAO.StatusId = Customer.StatusId;
            CustomerDAO.AppUserId = Customer.AppUserId;
            CustomerDAO.CreatorId = Customer.CreatorId;
            CustomerDAO.OrganizationId = Customer.OrganizationId;
            CustomerDAO.Used = Customer.Used;
            CustomerDAO.RowId = Customer.RowId;
            CustomerDAO.CodeGeneratorRuleId = Customer.CodeGeneratorRuleId;
            CustomerDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Customer);
            return true;
        }

        public async Task<bool> Delete(Customer Customer)
        {
            await DataContext.Customer.Where(x => x.Id == Customer.Id).UpdateFromQueryAsync(x => new CustomerDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Customer> Customers)
        {
            List<CustomerDAO> CustomerDAOs = new List<CustomerDAO>();
            foreach (Customer Customer in Customers)
            {
                CustomerDAO CustomerDAO = new CustomerDAO();
                CustomerDAO.Id = Customer.Id;
                CustomerDAO.Code = Customer.Code;
                CustomerDAO.CodeDraft = Customer.CodeDraft;
                CustomerDAO.Name = Customer.Name;
                CustomerDAO.Phone = Customer.Phone;
                CustomerDAO.Address = Customer.Address;
                CustomerDAO.NationId = Customer.NationId;
                CustomerDAO.ProvinceId = Customer.ProvinceId;
                CustomerDAO.DistrictId = Customer.DistrictId;
                CustomerDAO.WardId = Customer.WardId;
                CustomerDAO.Birthday = Customer.Birthday;
                CustomerDAO.Email = Customer.Email;
                CustomerDAO.ProfessionId = Customer.ProfessionId;
                CustomerDAO.CustomerSourceId = Customer.CustomerSourceId;
                CustomerDAO.SexId = Customer.SexId;
                CustomerDAO.StatusId = Customer.StatusId;
                CustomerDAO.AppUserId = Customer.AppUserId;
                CustomerDAO.CreatorId = Customer.CreatorId;
                CustomerDAO.OrganizationId = Customer.OrganizationId;
                CustomerDAO.Used = Customer.Used;
                CustomerDAO.RowId = Customer.RowId;
                CustomerDAO.CodeGeneratorRuleId = Customer.CodeGeneratorRuleId;
                CustomerDAO.CreatedAt = StaticParams.DateTimeNow;
                CustomerDAO.UpdatedAt = StaticParams.DateTimeNow;
                CustomerDAOs.Add(CustomerDAO);
            }
            await DataContext.BulkMergeAsync(CustomerDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Customer> Customers)
        {
            List<long> Ids = Customers.Select(x => x.Id).ToList();
            await DataContext.Customer
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Customer Customer)
        {
            await DataContext.CustomerCustomerGroupingMapping
                .Where(x => x.CustomerId == Customer.Id)
                .DeleteFromQueryAsync();
            List<CustomerCustomerGroupingMappingDAO> CustomerCustomerGroupingMappingDAOs = new List<CustomerCustomerGroupingMappingDAO>();
            if (Customer.CustomerCustomerGroupingMappings != null)
            {
                foreach (CustomerCustomerGroupingMapping CustomerCustomerGroupingMapping in Customer.CustomerCustomerGroupingMappings)
                {
                    CustomerCustomerGroupingMappingDAO CustomerCustomerGroupingMappingDAO = new CustomerCustomerGroupingMappingDAO();
                    CustomerCustomerGroupingMappingDAO.CustomerId = Customer.Id;
                    CustomerCustomerGroupingMappingDAO.CustomerGroupingId = CustomerCustomerGroupingMapping.CustomerGroupingId;
                    CustomerCustomerGroupingMappingDAOs.Add(CustomerCustomerGroupingMappingDAO);
                }
                await DataContext.CustomerCustomerGroupingMapping.BulkMergeAsync(CustomerCustomerGroupingMappingDAOs);
            }
        }
        
        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Customer.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerDAO { Used = true });
            return true;
        }
    }
}
