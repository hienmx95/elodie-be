using Microsoft.EntityFrameworkCore;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Helpers;
using ELODIE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Used(List<long> Ids);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AppUserDAO> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.DeletedAt == null);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Username != null)
                query = query.Where(q => q.Username.ToLower(), filter.Username.ToLower());
            if (filter.Password != null)
                query = query.Where(q => q.Password, filter.Password);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.SexId != null)
                query = query.Where(q => q.SexId, filter.SexId);
            if (filter.Birthday != null)
                query = query.Where(q => q.Birthday, filter.Birthday);
            if (filter.Department != null)
                query = query.Where(q => q.Department, filter.Department);
            if (filter.OrganizationId != null)
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => Ids.Contains(q.OrganizationId));
                }
                if (filter.OrganizationId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => !Ids.Contains(q.OrganizationId));
                }
            }
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<AppUserDAO> OrFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AppUserDAO> initQuery = query.Where(q => false);
            foreach (AppUserFilter AppUserFilter in filter.OrFilter)
            {
                IQueryable<AppUserDAO> queryable = query;
                if (AppUserFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, AppUserFilter.Id);
                if (AppUserFilter.Username != null)
                    queryable = queryable.Where(q => q.Username, AppUserFilter.Username);
                if (AppUserFilter.Password != null)
                    queryable = queryable.Where(q => q.Password, AppUserFilter.Password);
                if (AppUserFilter.DisplayName != null)
                    queryable = queryable.Where(q => q.DisplayName, AppUserFilter.DisplayName);
                if (AppUserFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, AppUserFilter.Address);
                if (AppUserFilter.Email != null)
                    queryable = queryable.Where(q => q.Email, AppUserFilter.Email);
                if (AppUserFilter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, AppUserFilter.Phone);
                if (AppUserFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, AppUserFilter.StatusId);
                if (AppUserFilter.SexId != null)
                    queryable = queryable.Where(q => q.SexId, AppUserFilter.SexId);
                if (AppUserFilter.Birthday != null)
                    queryable = queryable.Where(q => q.Birthday, AppUserFilter.Birthday);
                if (AppUserFilter.Department != null)
                    queryable = queryable.Where(q => q.Department, AppUserFilter.Department);
                if (AppUserFilter.OrganizationId != null)
                {
                    if (AppUserFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == AppUserFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (AppUserFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == AppUserFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (AppUserFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => AppUserFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        query = query.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (AppUserFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => AppUserFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        query = query.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderBy(q => q.Sex);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                        case AppUserOrder.Department:
                            query = query.OrderBy(q => q.Department);
                            break;
                        case AppUserOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderByDescending(q => q.Sex);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                        case AppUserOrder.Department:
                            query = query.OrderByDescending(q => q.Department);
                            break;
                        case AppUserOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                OtpCode = q.OtpCode,
                Used = q.Used,
                OtpExpired = q.OtpExpired == null ? default(DateTime?) : q.OtpExpired,
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Avatar = filter.Selects.Contains(AppUserSelect.Avatar) ? q.Avatar : default(string),
                Address = filter.Selects.Contains(AppUserSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                StatusId = filter.Selects.Contains(AppUserSelect.Status) ? q.StatusId : default(long),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AppUserSelect.Birthday) ? q.Birthday : null,
                Department = filter.Selects.Contains(AppUserSelect.Department) ? q.Department : default(string),
                OrganizationId = filter.Selects.Contains(AppUserSelect.Organization) ? q.OrganizationId : default(long),
                Organization = filter.Selects.Contains(AppUserSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    RowId = q.Organization.RowId,
                } : null,
                Status = filter.Selects.Contains(AppUserSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Sex = filter.Selects.Contains(AppUserSelect.Sex) && q.Sex != null ? new Sex
                {
                    Id = q.Sex.Id,
                    Code = q.Sex.Code,
                    Name = q.Sex.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();

            var Ids = AppUsers.Select(x => x.Id).ToList();
            var AppUserSiteMappings = await DataContext.AppUserSiteMapping.Where(x => Ids.Contains(x.AppUserId)).Select(x => new AppUserSiteMapping
            {
                AppUserId = x.AppUserId,
                SiteId = x.SiteId,
                Enabled = x.Enabled,
                Site = x.Site == null ? null : new Site
                {
                    Id = x.Site.Id,
                    Code = x.Site.Code,
                    Name = x.Site.Name,
                }
            }).ToListAsync();

            foreach (var AppUser in AppUsers)
            {
                AppUser.AppUserSiteMappings = AppUserSiteMappings.Where(x => x.AppUserId == AppUser.Id).ToList();
            }
            return AppUsers;
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUser.AsNoTracking();
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            List<AppUser> AppUsers = await DataContext.AppUser.AsNoTracking()
                .Where(x => Ids.Contains(x.Id)).Select(x => new AppUser()
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Avatar = x.Avatar,
                    Birthday = x.Birthday,
                    Email = x.Email,
                    Phone = x.Phone,
                    StatusId = x.StatusId,
                    SexId = x.SexId,
                    Department = x.Department,
                    OrganizationId = x.OrganizationId,
                    RowId = x.RowId,
                    Used = x.Used,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Organization = x.Organization == null ? null : new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        Address = x.Organization.Address,
                        Phone = x.Organization.Phone,
                        Path = x.Organization.Path,
                        ParentId = x.Organization.ParentId,
                        Email = x.Organization.Email,
                        StatusId = x.Organization.StatusId,
                        Level = x.Organization.Level,
                        RowId = x.Organization.RowId,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Sex = x.Sex == null ? null : new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    }
                }).ToListAsync();

            var AppUserRoleMappings = await DataContext.AppUserRoleMapping
                .Where(x => Ids.Contains(x.AppUserId))
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                    Role = x.Role == null ? null : new Role
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name,
                        Code = x.Role.Code,
                    },
                }).ToListAsync();

            var AppUserSiteMappings = await DataContext.AppUserSiteMapping
                .Where(x => Ids.Contains(x.AppUserId))
                .Select(x => new AppUserSiteMapping
                {
                    AppUserId = x.AppUserId,
                    SiteId = x.SiteId,
                    Enabled = x.Enabled,
                    Site = x.Site == null ? null : new Site
                    {
                        Id = x.Site.Id,
                        Code = x.Site.Code,
                        Name = x.Site.Name,
                        Description = x.Site.Description,
                        ThemeId = x.Site.ThemeId,
                        Icon = x.Site.Icon,
                        IsDisplay = x.Site.IsDisplay,
                        RowId = x.Site.RowId,
                    }
                }).ToListAsync();

            foreach (var AppUser in AppUsers)
            {
                AppUser.AppUserRoleMappings = AppUserRoleMappings.Where(x => x.AppUserId == AppUser.Id).ToList();
                AppUser.AppUserSiteMappings = AppUserSiteMappings.Where(x => x.AppUserId == AppUser.Id).ToList();
            }
            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = DataContext.AppUser.AsNoTracking()
                .Where(x => x.Id == Id)
                .Select(x => new AppUser()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    OtpCode = x.OtpCode,
                    OtpExpired = x.OtpExpired,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Avatar = x.Avatar,
                    Birthday = x.Birthday,
                    Email = x.Email,
                    Phone = x.Phone,
                    StatusId = x.StatusId,
                    SexId = x.SexId,
                    Department = x.Department,
                    OrganizationId = x.OrganizationId,
                    RowId = x.RowId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Used = x.Used,
                    Organization = x.Organization == null ? null : new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        Path = x.Organization.Path,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Sex = x.Sex == null ? null : new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    }
                }).FirstOrDefault();

            if (AppUser == null)
                return null;
            AppUser.AppUserRoleMappings = DataContext.AppUserRoleMapping
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                    Role = new Role
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name,
                        Code = x.Role.Code,
                    },
                }).ToList();

            AppUser.AppUserSiteMappings = DataContext.AppUserSiteMapping
                .Where(x => x.AppUserId == AppUser.Id && x.Site.IsDisplay)
                .OrderBy(x => x.SiteId)
                .Select(x => new AppUserSiteMapping
                {
                    AppUserId = x.AppUserId,
                    SiteId = x.SiteId,
                    Enabled = x.Enabled,
                    Site = new Site
                    {
                        Id = x.Site.Id,
                        Code = x.Site.Code,
                        Name = x.Site.Name,
                        Icon = x.Site.Icon,
                    }
                }).ToList();
            return AppUser;
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.Department = AppUser.Department;
            AppUserDAO.OrganizationId = AppUser.OrganizationId;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            AppUserDAO.RowId = Guid.NewGuid();
            AppUserDAO.Used = false;
            DataContext.AppUser.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            AppUser.RowId = AppUserDAO.RowId;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.OtpCode = AppUser.OtpCode;
            AppUserDAO.OtpExpired = AppUser.OtpExpired;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.Department = AppUser.Department;
            AppUserDAO.OrganizationId = AppUser.OrganizationId;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            AppUser.RowId = AppUserDAO.RowId;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUser.Where(x => x.Id == AppUser.Id).UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow });
            AppUser.RowId = DataContext.AppUser.Where(x => x.Id == AppUser.Id).Select(a => a.RowId).FirstOrDefault();
            return true;
        }

        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUser.RowId = Guid.NewGuid();
                AppUserDAO AppUserDAO = new AppUserDAO();
                AppUserDAO.Id = AppUser.Id;
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Address = AppUser.Address;
                AppUserDAO.Avatar = AppUser.Avatar;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.Birthday = AppUser.Birthday;
                AppUserDAO.Department = AppUser.Department;
                AppUserDAO.OrganizationId = AppUser.OrganizationId;
                AppUserDAO.StatusId = AppUser.StatusId;

                AppUserDAO.CreatedAt = DateTime.Now;
                AppUserDAO.UpdatedAt = DateTime.Now;
                AppUserDAO.RowId = AppUser.RowId;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUser
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(AppUser AppUser)
        {
            if (AppUser.AppUserSiteMappings != null)
            {
                await DataContext.AppUserSiteMapping.Where(a => a.AppUserId == AppUser.Id).DeleteFromQueryAsync();
                List<AppUserSiteMappingDAO> AppUserSiteMappingDAOs = AppUser.AppUserSiteMappings.Select(a => new AppUserSiteMappingDAO
                {
                    AppUserId = AppUser.Id,
                    Enabled = a.Enabled,
                    SiteId = a.SiteId,
                }).ToList();
                await DataContext.AppUserSiteMapping.BulkInsertAsync(AppUserSiteMappingDAOs);
            }
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.AppUser.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AppUserDAO { Used = true });
            return true;
        }
    }
}
