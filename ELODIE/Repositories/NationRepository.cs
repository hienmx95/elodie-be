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
    public interface INationRepository
    {
        Task<int> Count(NationFilter NationFilter);
        Task<List<Nation>> List(NationFilter NationFilter);
        Task<List<Nation>> List(List<long> Ids);
        Task<Nation> Get(long Id);
        Task<bool> Create(Nation Nation);
        Task<bool> Update(Nation Nation);
        Task<bool> Delete(Nation Nation);
        Task<bool> BulkMerge(List<Nation> Nations);
        Task<bool> BulkDelete(List<Nation> Nations);
        Task<bool> Used(List<long> Ids);
    }
    public class NationRepository : INationRepository
    {
        private DataContext DataContext;
        public NationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<NationDAO> DynamicFilter(IQueryable<NationDAO> query, NationFilter filter)
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
            if (filter.Priority != null && filter.Priority.HasValue)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<NationDAO> OrFilter(IQueryable<NationDAO> query, NationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NationDAO> initQuery = query.Where(q => false);
            foreach (NationFilter NationFilter in filter.OrFilter)
            {
                IQueryable<NationDAO> queryable = query;
                if (NationFilter.Id != null && NationFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, NationFilter.Id);
                if (NationFilter.Code != null && NationFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, NationFilter.Code);
                if (NationFilter.Name != null && NationFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, NationFilter.Name);
                if(NationFilter.Priority != null && NationFilter.Priority.HasValue)
                    queryable = queryable.Where(q => q.Priority, NationFilter.Priority);
                if (NationFilter.StatusId != null && NationFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, NationFilter.StatusId);
                if (NationFilter.RowId != null && NationFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, NationFilter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<NationDAO> DynamicOrder(IQueryable<NationDAO> query, NationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NationOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case NationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case NationOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case NationOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case NationOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case NationOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NationOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case NationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case NationOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case NationOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case NationOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case NationOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Nation>> DynamicSelect(IQueryable<NationDAO> query, NationFilter filter)
        {
            List<Nation> Nations = await query.Select(q => new Nation()
            {
                Id = filter.Selects.Contains(NationSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(NationSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(NationSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(NationSelect.Priority) ? q.Priority : default(long?),
                StatusId = filter.Selects.Contains(NationSelect.Status) ? q.StatusId : default(long),
                Used = filter.Selects.Contains(NationSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(NationSelect.Row) ? q.RowId : default(Guid),
                Status = filter.Selects.Contains(NationSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Nations;
        }

        public async Task<int> Count(NationFilter filter)
        {
            IQueryable<NationDAO> Nations = DataContext.Nation.AsNoTracking();
            Nations = DynamicFilter(Nations, filter);
            return await Nations.CountAsync();
        }

        public async Task<List<Nation>> List(NationFilter filter)
        {
            if (filter == null) return new List<Nation>();
            IQueryable<NationDAO> NationDAOs = DataContext.Nation.AsNoTracking();
            NationDAOs = DynamicFilter(NationDAOs, filter);
            NationDAOs = DynamicOrder(NationDAOs, filter);
            List<Nation> Nations = await DynamicSelect(NationDAOs, filter);
            return Nations;
        }

        public async Task<List<Nation>> List(List<long> Ids)
        {
            List<Nation> Nations = await DataContext.Nation.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Nation()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();
            

            return Nations;
        }

        public async Task<Nation> Get(long Id)
        {
            Nation Nation = await DataContext.Nation.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Nation()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Nation == null)
                return null;

            return Nation;
        }
        public async Task<bool> Create(Nation Nation)
        {
            NationDAO NationDAO = new NationDAO();
            NationDAO.Id = Nation.Id;
            NationDAO.Code = Nation.Code;
            NationDAO.Name = Nation.Name;
            NationDAO.Priority = Nation.Priority;
            NationDAO.StatusId = Nation.StatusId;
            NationDAO.Used = Nation.Used;
            NationDAO.RowId = Guid.NewGuid();
            NationDAO.CreatedAt = StaticParams.DateTimeNow;
            NationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Nation.Add(NationDAO);
            await DataContext.SaveChangesAsync();
            Nation.Id = NationDAO.Id;
            await SaveReference(Nation);
            return true;
        }

        public async Task<bool> Update(Nation Nation)
        {
            NationDAO NationDAO = DataContext.Nation.Where(x => x.Id == Nation.Id).FirstOrDefault();
            if (NationDAO == null)
                return false;
            NationDAO.Id = Nation.Id;
            NationDAO.Code = Nation.Code;
            NationDAO.Name = Nation.Name;
            NationDAO.Priority = Nation.Priority;
            NationDAO.StatusId = Nation.StatusId;
            NationDAO.Used = Nation.Used;
            NationDAO.RowId = Nation.RowId;
            NationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Nation);
            return true;
        }

        public async Task<bool> Delete(Nation Nation)
        {
            await DataContext.Nation.Where(x => x.Id == Nation.Id).UpdateFromQueryAsync(x => new NationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Nation> Nations)
        {
            List<NationDAO> NationDAOs = new List<NationDAO>();
            foreach (Nation Nation in Nations)
            {
                NationDAO NationDAO = new NationDAO();
                NationDAO.Id = Nation.Id;
                NationDAO.Code = Nation.Code;
                NationDAO.Name = Nation.Name;
                NationDAO.Priority = Nation.Priority;
                NationDAO.StatusId = Nation.StatusId;
                NationDAO.Used = Nation.Used;
                NationDAO.RowId = Nation.RowId;
                NationDAO.CreatedAt = StaticParams.DateTimeNow;
                NationDAO.UpdatedAt = StaticParams.DateTimeNow;
                NationDAOs.Add(NationDAO);
            }
            await DataContext.BulkMergeAsync(NationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Nation> Nations)
        {
            List<long> Ids = Nations.Select(x => x.Id).ToList();
            await DataContext.Nation
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new NationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Nation Nation)
        {
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Nation.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new NationDAO { Used = true });
            return true;
        }
    }
}
