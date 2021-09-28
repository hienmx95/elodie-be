using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Helpers;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace ELODIE.Repositories
{
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
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
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Username != null && filter.Username.HasValue)
                query = query.Where(q => q.Username.ToLower(), filter.Username.ToLower());
            if (filter.DisplayName != null && filter.DisplayName.HasValue)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Address != null && filter.Address.HasValue)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Email != null && filter.Email.HasValue)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null && filter.Phone.HasValue)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.SexId != null && filter.SexId.HasValue)
                query = query.Where(q => q.SexId, filter.SexId);
            if (filter.Birthday != null && filter.Birthday.HasValue)
                query = query.Where(q => q.Birthday, filter.Birthday);
            if (filter.Department != null && filter.Department.HasValue)
                query = query.Where(q => q.Department, filter.Department);
            if (filter.OrganizationId != null && filter.OrganizationId.HasValue)
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
                if (AppUserFilter.Id != null && AppUserFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, AppUserFilter.Id);
                if (AppUserFilter.Username != null && AppUserFilter.Username.HasValue)
                    queryable = queryable.Where(q => q.Username, AppUserFilter.Username);
                if (AppUserFilter.DisplayName != null && AppUserFilter.DisplayName.HasValue)
                    queryable = queryable.Where(q => q.DisplayName, AppUserFilter.DisplayName);
                if (AppUserFilter.Address != null && AppUserFilter.Address.HasValue)
                    queryable = queryable.Where(q => q.Address, AppUserFilter.Address);
                if (AppUserFilter.Email != null && AppUserFilter.Email.HasValue)
                    queryable = queryable.Where(q => q.Email, AppUserFilter.Email);
                if (AppUserFilter.Phone != null && AppUserFilter.Phone.HasValue)
                    queryable = queryable.Where(q => q.Phone, AppUserFilter.Phone);
                if (AppUserFilter.StatusId != null && AppUserFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, AppUserFilter.StatusId);
                if (AppUserFilter.SexId != null && AppUserFilter.SexId.HasValue)
                    queryable = queryable.Where(q => q.SexId, AppUserFilter.SexId);
                if (AppUserFilter.Birthday != null && AppUserFilter.Birthday.HasValue)
                    queryable = queryable.Where(q => q.Birthday, AppUserFilter.Birthday);
                if (AppUserFilter.Department != null && AppUserFilter.Department.HasValue)
                    queryable = queryable.Where(q => q.Department, AppUserFilter.Department);
                if (AppUserFilter.OrganizationId != null && AppUserFilter.OrganizationId.HasValue)
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
                Used = q.Used,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
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

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new AppUser()
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
                }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            AppUser.AppUserRoleMappings = await DataContext.AppUserRoleMapping
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
                }).ToListAsync();
            AppUser.AppUserSiteMappings = await DataContext.AppUserSiteMapping
                .Where(x => x.AppUserId == AppUser.Id)
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

            return AppUser;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            List<AppUser> AppUsers = await DataContext.AppUser.AsNoTracking()
                .Where(x => Ids.Contains(x.Id))
                .Select(x => new AppUser()
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
                })
                .ToListAsync();

            List<AppUserRoleMapping> AppUserRoleMappings = await DataContext.AppUserRoleMapping
                .Where(x => Ids.Contains(x.AppUserId))
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
                }).ToListAsync();

            foreach (AppUser AppUser in AppUsers)
            {
                AppUser.AppUserRoleMappings = AppUserRoleMappings
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            List<AppUserSiteMapping> AppUserSiteMappings = await DataContext.AppUserSiteMapping
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
                })
                .ToListAsync();

            foreach (AppUser AppUser in AppUsers)
            {
                AppUser.AppUserSiteMappings = AppUserSiteMappings
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            return AppUsers;
        }

        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = AppUsers.Select(x => new AppUserDAO
            {
                Id = x.Id,
                Address = x.Address,
                Avatar = x.Avatar,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Department = x.Department,
                DisplayName = x.DisplayName,
                Email = x.Email,
                OrganizationId = x.OrganizationId,
                Phone = x.Phone,
                RowId = x.RowId,
                StatusId = x.StatusId,
                Username = x.Username,
                SexId = x.SexId,
                Birthday = x.Birthday,
            }).ToList();

            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUserSiteMapping.Where(x => Ids.Contains(x.AppUserId)).DeleteFromQueryAsync();

            List<AppUserSiteMappingDAO> AppUserSiteMappingDAOs = AppUsers.Where(x => x.AppUserSiteMappings != null)
                .SelectMany(x => x.AppUserSiteMappings)
                .Select(x => new AppUserSiteMappingDAO
                {
                    SiteId = x.SiteId,
                    AppUserId = x.AppUserId,
                    Enabled = x.Enabled,
                }).ToList();

            await DataContext.BulkMergeAsync(AppUserDAOs);
            await DataContext.BulkMergeAsync(AppUserSiteMappingDAOs);
            return true;
        }
    }
}
