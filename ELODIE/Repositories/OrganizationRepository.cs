using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Helpers;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;

namespace ELODIE.Repositories
{
    public interface IOrganizationRepository
    {
        Task<int> Count(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(List<long> Ids);
        Task<Organization> Get(long Id);
        Task<bool> Create(Organization Organization);
        Task<bool> Update(Organization Organization);
        Task<bool> Delete(Organization Organization);
        Task<bool> BulkMerge(List<Organization> Organizations);
        Task<bool> UpdateIsDisplay(Organization Organization);
    }
    public class OrganizationRepository : IOrganizationRepository
    {
        private DataContext DataContext;
        public OrganizationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrganizationDAO> DynamicFilter(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
            {
                if (filter.Id.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.Id.Equal.Value).FirstOrDefault();
                    if (OrganizationDAO == null)
                        return query.Where(q => false);
                    else
                        query = query.Where(q => q.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.Id.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.Id.NotEqual.Value).FirstOrDefault();
                    if (OrganizationDAO == null)
                        return query.Where(q => false);
                    else
                        query = query.Where(q => !q.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.Id.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                       .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.Id.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => Ids.Contains(q.Id));
                }
                if (filter.Id.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                       .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.Id.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => !Ids.Contains(q.Id));
                }
            }
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ParentId != null)
            {
                if (filter.ParentId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.ParentId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.ParentId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.ParentId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.ParentId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                       .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.ParentId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => q.ParentId.HasValue && Ids.Contains(q.ParentId.Value));
                }
                if (filter.ParentId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                       .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.ParentId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => q.ParentId.HasValue && !Ids.Contains(q.ParentId.Value));
                }
            }
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.Level != null)
                query = query.Where(q => q.Level, filter.Level);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<OrganizationDAO> OrFilter(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrganizationDAO> initQuery = query.Where(q => false);
            foreach (OrganizationFilter OrganizationFilter in filter.OrFilter)
            {
                IQueryable<OrganizationDAO> queryable = query;
                if (OrganizationFilter.Id != null)
                {
                    if (OrganizationFilter.Id.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == OrganizationFilter.Id.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (OrganizationFilter.Id.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == OrganizationFilter.Id.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (OrganizationFilter.Id.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                           .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => OrganizationFilter.Id.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.Id));
                    }
                    if (OrganizationFilter.Id.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                           .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => OrganizationFilter.Id.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.Id));
                    }
                }
                if (OrganizationFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, OrganizationFilter.Code);
                if (OrganizationFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, OrganizationFilter.Name);
                if (OrganizationFilter.ParentId != null)
                {
                    if (OrganizationFilter.ParentId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == OrganizationFilter.ParentId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (OrganizationFilter.ParentId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == OrganizationFilter.ParentId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (OrganizationFilter.ParentId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                           .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => OrganizationFilter.Id.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => q.ParentId.HasValue && Ids.Contains(q.ParentId.Value));
                    }
                    if (OrganizationFilter.ParentId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                           .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => OrganizationFilter.ParentId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => q.ParentId.HasValue && !Ids.Contains(q.ParentId.Value));
                    }
                }
                if (OrganizationFilter.Path != null)
                    queryable = queryable.Where(q => q.Path, OrganizationFilter.Path);
                if (OrganizationFilter.Level != null)
                    queryable = queryable.Where(q => q.Level, OrganizationFilter.Level);
                if (OrganizationFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, OrganizationFilter.StatusId);
                if (OrganizationFilter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, OrganizationFilter.Phone);
                if (OrganizationFilter.Email != null)
                    queryable = queryable.Where(q => q.Email, OrganizationFilter.Email);
                if (OrganizationFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, OrganizationFilter.Address);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<OrganizationDAO> DynamicOrder(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrganizationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrganizationOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrganizationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case OrganizationOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case OrganizationOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case OrganizationOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case OrganizationOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case OrganizationOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case OrganizationOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case OrganizationOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrganizationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrganizationOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrganizationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case OrganizationOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case OrganizationOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case OrganizationOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case OrganizationOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case OrganizationOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case OrganizationOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case OrganizationOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Organization>> DynamicSelect(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            List<Organization> Organizations = await query.Select(q => new Organization()
            {
                Id = filter.Selects.Contains(OrganizationSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrganizationSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(OrganizationSelect.Name) ? q.Name : default(string),
                ParentId = filter.Selects.Contains(OrganizationSelect.Parent) ? q.ParentId : default(long?),
                Path = filter.Selects.Contains(OrganizationSelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(OrganizationSelect.Level) ? q.Level : default(long),
                StatusId = filter.Selects.Contains(OrganizationSelect.Status) ? q.StatusId : default(long),
                Phone = filter.Selects.Contains(OrganizationSelect.Phone) ? q.Phone : default(string),
                Address = filter.Selects.Contains(OrganizationSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(OrganizationSelect.Email) ? q.Email : default(string),
                IsDisplay = filter.Selects.Contains(OrganizationSelect.IsDisplay) ? q.IsDisplay : default(bool),
                Parent = filter.Selects.Contains(OrganizationSelect.Parent) && q.Parent != null ? new Organization
                {
                    Id = q.Parent.Id,
                    Code = q.Parent.Code,
                    Name = q.Parent.Name,
                    ParentId = q.Parent.ParentId,
                    Path = q.Parent.Path,
                    Level = q.Parent.Level,
                    StatusId = q.Parent.StatusId,
                    Phone = q.Parent.Phone,
                    Address = q.Parent.Address,
                    IsDisplay = q.Parent.IsDisplay,
                    Email = q.Parent.Email,
                } : null,
                Status = filter.Selects.Contains(OrganizationSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                RowId = filter.Selects.Contains(OrganizationSelect.RowId) ? q.RowId : default(Guid)
            }).ToListAsync();
            return Organizations;
        }

        public async Task<int> Count(OrganizationFilter filter)
        {
            IQueryable<OrganizationDAO> Organizations = DataContext.Organization;
            Organizations = DynamicFilter(Organizations, filter);
            return await Organizations.CountAsync();
        }

        public async Task<List<Organization>> List(OrganizationFilter filter)
        {
            if (filter == null) return new List<Organization>();
            IQueryable<OrganizationDAO> OrganizationDAOs = DataContext.Organization.AsNoTracking();
            OrganizationDAOs = DynamicFilter(OrganizationDAOs, filter);
            OrganizationDAOs = DynamicOrder(OrganizationDAOs, filter);
            List<Organization> Organizations = await DynamicSelect(OrganizationDAOs, filter);
            return Organizations;
        }

        public async Task<List<Organization>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);
            var query = from x in DataContext.Organization
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<Organization> Organizations = await query
                .Select(x => new Organization()
                {
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Level = x.Level,
                    StatusId = x.StatusId,
                    Phone = x.Phone,
                    Email = x.Email,
                    Address = x.Address,
                    RowId = x.RowId,
                    Parent = x.Parent == null ? null : new Organization
                    {
                        Id = x.Parent.Id,
                        Code = x.Parent.Code,
                        Name = x.Parent.Name,
                        ParentId = x.Parent.ParentId,
                        Path = x.Parent.Path,
                        Level = x.Parent.Level,
                        StatusId = x.Parent.StatusId,
                        Phone = x.Parent.Phone,
                        Email = x.Parent.Email,
                        Address = x.Parent.Address,
                        RowId = x.Parent.RowId,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();

            var AppUserQuery = from x in DataContext.AppUser
                               join tt in tempTableQuery.Query on x.OrganizationId equals tt.Column1
                               select x;
            List<AppUser> AppUsers = AppUserQuery
                .Select(x => new AppUser
                {
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Email = x.Email,
                    Phone = x.Phone,
                    SexId = x.SexId,
                    Birthday = x.Birthday,
                    Avatar = x.Avatar,
                    Department = x.Department,
                    OrganizationId = x.OrganizationId,
                    StatusId = x.StatusId,
                    RowId = x.RowId,
                }).ToList();
            foreach (var Organization in Organizations)
            {
                Organization.AppUsers = AppUsers.Where(x => x.OrganizationId == Organization.Id).ToList();
            }
            return Organizations;
        }

        public async Task<Organization> Get(long Id)
        {
            Organization Organization = await DataContext.Organization
                .AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Organization()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Level = x.Level,
                    StatusId = x.StatusId,
                    Phone = x.Phone,
                    Address = x.Address,
                    Email = x.Email,
                    IsDisplay = x.IsDisplay,
                    Parent = x.Parent == null ? null : new Organization
                    {
                        Id = x.Parent.Id,
                        Code = x.Parent.Code,
                        Name = x.Parent.Name,
                        ParentId = x.Parent.ParentId,
                        Path = x.Parent.Path,
                        Level = x.Parent.Level,
                        StatusId = x.Parent.StatusId,
                        Phone = x.Parent.Phone,
                        Address = x.Parent.Address,
                        Email = x.Parent.Email,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Organization == null)
                return null;
            Organization.AppUsers = await DataContext.AppUser
                .Where(x => x.OrganizationId == Organization.Id)
                .Select(x => new AppUser
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Email = x.Email,
                    Phone = x.Phone,
                    Department = x.Department,
                    OrganizationId = x.OrganizationId,
                    SexId = x.SexId,
                    StatusId = x.StatusId,
                    Sex = new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();

            return Organization;
        }

        public async Task<bool> Create(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = new OrganizationDAO();
            OrganizationDAO.Id = Organization.Id;
            OrganizationDAO.Code = Organization.Code;
            OrganizationDAO.Name = Organization.Name;
            OrganizationDAO.ParentId = Organization.ParentId;
            OrganizationDAO.Path = Organization.Path;
            OrganizationDAO.Level = Organization.Level;
            OrganizationDAO.StatusId = Organization.StatusId;
            OrganizationDAO.Phone = Organization.Phone;
            OrganizationDAO.Email = Organization.Email;
            OrganizationDAO.Address = Organization.Address;
            OrganizationDAO.RowId = Organization.RowId;
            OrganizationDAO.Used = Organization.Used;
            OrganizationDAO.IsDisplay = Organization.IsDisplay;
            OrganizationDAO.RowId = Guid.NewGuid();
            OrganizationDAO.Path = "";
            OrganizationDAO.Level = 1;
            OrganizationDAO.CreatedAt = StaticParams.DateTimeNow;
            OrganizationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Organization.Add(OrganizationDAO);
            await DataContext.SaveChangesAsync();
            Organization.Id = OrganizationDAO.Id;
            await SaveReference(Organization);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = DataContext.Organization.Where(x => x.Id == Organization.Id).FirstOrDefault();
            if (OrganizationDAO == null)
                return false;
            OrganizationDAO.Id = Organization.Id;
            OrganizationDAO.Code = Organization.Code;
            OrganizationDAO.Name = Organization.Name;
            OrganizationDAO.ParentId = Organization.ParentId;
            OrganizationDAO.Path = Organization.Path;
            OrganizationDAO.Level = Organization.Level;
            OrganizationDAO.StatusId = Organization.StatusId;
            OrganizationDAO.Phone = Organization.Phone;
            OrganizationDAO.Email = Organization.Email;
            OrganizationDAO.Address = Organization.Address;
            OrganizationDAO.RowId = Organization.RowId;
            OrganizationDAO.Used = Organization.Used;
            OrganizationDAO.IsDisplay = Organization.IsDisplay;
            OrganizationDAO.Path = "";
            OrganizationDAO.Level = 1;
            OrganizationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Organization);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = await DataContext.Organization.Where(x => x.Id == Organization.Id).FirstOrDefaultAsync();
            await DataContext.Organization.Where(x => x.Path.StartsWith(OrganizationDAO.Id + ".")).UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await DataContext.Organization.Where(x => x.Id == Organization.Id).UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<Organization> Organizations)
        {
            List<long> Ids = Organizations.Select(x => x.Id).ToList();
            await DataContext.Organization
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(Organization Organization)
        {
        }

        private async Task BuildPath()
        {
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization
                .Where(x => x.DeletedAt == null)
                .AsNoTracking().ToListAsync();
            Queue<OrganizationDAO> queue = new Queue<OrganizationDAO>();
            OrganizationDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    x.Level = 1;
                    queue.Enqueue(x);
                }
            });
            while (queue.Count > 0)
            {
                OrganizationDAO Parent = queue.Dequeue();
                foreach (OrganizationDAO OrganizationDAO in OrganizationDAOs)
                {
                    if (OrganizationDAO.ParentId == Parent.Id)
                    {
                        OrganizationDAO.Path = Parent.Path + OrganizationDAO.Id + ".";
                        OrganizationDAO.Level = Parent.Level + 1;
                        queue.Enqueue(OrganizationDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(OrganizationDAOs);
        }

        public async Task<bool> UpdateIsDisplay(Organization Organization)
        {
            var Path = await DataContext.Organization.Where(x => x.Id == Organization.Id).Select(x => x.Path).FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(Path))
            {
                await DataContext.Organization.Where(x => x.Path.StartsWith(Path)).UpdateFromQueryAsync(x => new OrganizationDAO
                {
                    IsDisplay = Organization.IsDisplay,
                    UpdatedAt = StaticParams.DateTimeNow
                });
            }
            await DataContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> BulkMerge(List<Organization> Organizations)
        {
            var AppUsers = Organizations.Where(x => x.AppUsers != null).SelectMany(x => x.AppUsers).ToList();
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();
            var AppUserDAOs = await DataContext.AppUser.Where(x => AppUserIds.Contains(x.Id)).ToListAsync();

            List<OrganizationDAO> OrganizationDAOs = Organizations.Select(o => new OrganizationDAO
            {
                Id = o.Id,
                Code = o.Code,
                Name = o.Name,
                Address = o.Address,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                DeletedAt = o.DeletedAt,
                Email = o.Email,
                Level = o.Level,
                ParentId = o.ParentId,
                Path = o.Path,
                Phone = o.Phone,
                RowId = o.RowId,
                StatusId = o.StatusId,
            }).ToList();
            await DataContext.Organization.BulkMergeAsync(OrganizationDAOs);

            foreach (var AppUserDAO in AppUserDAOs)
            {
                AppUserDAO.OrganizationId = AppUsers.Where(x => x.Id == AppUserDAO.Id).Select(x => x.OrganizationId).FirstOrDefault();
            }
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
