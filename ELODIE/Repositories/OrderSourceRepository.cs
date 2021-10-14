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
    public interface IOrderSourceRepository
    {
        Task<int> CountAll(OrderSourceFilter OrderSourceFilter);
        Task<int> Count(OrderSourceFilter OrderSourceFilter);
        Task<List<OrderSource>> List(OrderSourceFilter OrderSourceFilter);
        Task<List<OrderSource>> List(List<long> Ids);
        Task<OrderSource> Get(long Id);
        Task<bool> Create(OrderSource OrderSource);
        Task<bool> Update(OrderSource OrderSource);
        Task<bool> Delete(OrderSource OrderSource);
        Task<bool> BulkMerge(List<OrderSource> OrderSources);
        Task<bool> BulkDelete(List<OrderSource> OrderSources);
        Task<bool> Used(List<long> Ids);
    }
    public class OrderSourceRepository : IOrderSourceRepository
    {
        private DataContext DataContext;
        public OrderSourceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrderSourceDAO> DynamicFilter(IQueryable<OrderSourceDAO> query, OrderSourceFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Priority, filter.Priority);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.RowId, filter.RowId);
            
            return query;
        }

        private IQueryable<OrderSourceDAO> OrFilter(IQueryable<OrderSourceDAO> query, OrderSourceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrderSourceDAO> initQuery = query.Where(q => false);
            foreach (OrderSourceFilter OrderSourceFilter in filter.OrFilter)
            {
                IQueryable<OrderSourceDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.Priority, filter.Priority);
                queryable = queryable.Where(q => q.Description, filter.Description);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<OrderSourceDAO> DynamicOrder(IQueryable<OrderSourceDAO> query, OrderSourceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrderSourceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrderSourceOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrderSourceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case OrderSourceOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case OrderSourceOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case OrderSourceOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case OrderSourceOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case OrderSourceOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrderSourceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrderSourceOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrderSourceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case OrderSourceOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case OrderSourceOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case OrderSourceOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case OrderSourceOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case OrderSourceOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<OrderSource>> DynamicSelect(IQueryable<OrderSourceDAO> query, OrderSourceFilter filter)
        {
            List<OrderSource> OrderSources = await query.Select(q => new OrderSource()
            {
                Id = filter.Selects.Contains(OrderSourceSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrderSourceSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(OrderSourceSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(OrderSourceSelect.Priority) ? q.Priority : default(long?),
                Description = filter.Selects.Contains(OrderSourceSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(OrderSourceSelect.Status) ? q.StatusId : default(long),
                Used = filter.Selects.Contains(OrderSourceSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(OrderSourceSelect.Row) ? q.RowId : default(Guid),
            }).ToListAsync();
            return OrderSources;
        }

        public async Task<int> CountAll(OrderSourceFilter filter)
        {
            IQueryable<OrderSourceDAO> OrderSources = DataContext.OrderSource.AsNoTracking();
            OrderSources = DynamicFilter(OrderSources, filter);
            return await OrderSources.CountAsync();
        }

        public async Task<int> Count(OrderSourceFilter filter)
        {
            IQueryable<OrderSourceDAO> OrderSources = DataContext.OrderSource.AsNoTracking();
            OrderSources = DynamicFilter(OrderSources, filter);
            OrderSources = OrFilter(OrderSources, filter);
            return await OrderSources.CountAsync();
        }

        public async Task<List<OrderSource>> List(OrderSourceFilter filter)
        {
            if (filter == null) return new List<OrderSource>();
            IQueryable<OrderSourceDAO> OrderSourceDAOs = DataContext.OrderSource.AsNoTracking();
            OrderSourceDAOs = DynamicFilter(OrderSourceDAOs, filter);
            OrderSourceDAOs = OrFilter(OrderSourceDAOs, filter);
            OrderSourceDAOs = DynamicOrder(OrderSourceDAOs, filter);
            List<OrderSource> OrderSources = await DynamicSelect(OrderSourceDAOs, filter);
            return OrderSources;
        }

        public async Task<List<OrderSource>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.OrderSource
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<OrderSource> OrderSources = await query.AsNoTracking()
            .Select(x => new OrderSource()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                Description = x.Description,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
            }).ToListAsync();
            

            return OrderSources;
        }

        public async Task<OrderSource> Get(long Id)
        {
            OrderSource OrderSource = await DataContext.OrderSource.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new OrderSource()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                Description = x.Description,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
            }).FirstOrDefaultAsync();

            if (OrderSource == null)
                return null;

            return OrderSource;
        }
        
        public async Task<bool> Create(OrderSource OrderSource)
        {
            OrderSourceDAO OrderSourceDAO = new OrderSourceDAO();
            OrderSourceDAO.Id = OrderSource.Id;
            OrderSourceDAO.Code = OrderSource.Code;
            OrderSourceDAO.Name = OrderSource.Name;
            OrderSourceDAO.Priority = OrderSource.Priority;
            OrderSourceDAO.Description = OrderSource.Description;
            OrderSourceDAO.StatusId = OrderSource.StatusId;
            OrderSourceDAO.Used = OrderSource.Used;
            OrderSourceDAO.RowId = OrderSource.RowId;
            OrderSourceDAO.RowId = Guid.NewGuid();
            OrderSourceDAO.CreatedAt = StaticParams.DateTimeNow;
            OrderSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.OrderSource.Add(OrderSourceDAO);
            await DataContext.SaveChangesAsync();
            OrderSource.Id = OrderSourceDAO.Id;
            await SaveReference(OrderSource);
            return true;
        }

        public async Task<bool> Update(OrderSource OrderSource)
        {
            OrderSourceDAO OrderSourceDAO = DataContext.OrderSource.Where(x => x.Id == OrderSource.Id).FirstOrDefault();
            if (OrderSourceDAO == null)
                return false;
            OrderSourceDAO.Id = OrderSource.Id;
            OrderSourceDAO.Code = OrderSource.Code;
            OrderSourceDAO.Name = OrderSource.Name;
            OrderSourceDAO.Priority = OrderSource.Priority;
            OrderSourceDAO.Description = OrderSource.Description;
            OrderSourceDAO.StatusId = OrderSource.StatusId;
            OrderSourceDAO.Used = OrderSource.Used;
            OrderSourceDAO.RowId = OrderSource.RowId;
            OrderSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(OrderSource);
            return true;
        }

        public async Task<bool> Delete(OrderSource OrderSource)
        {
            await DataContext.OrderSource.Where(x => x.Id == OrderSource.Id).UpdateFromQueryAsync(x => new OrderSourceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<OrderSource> OrderSources)
        {
            List<OrderSourceDAO> OrderSourceDAOs = new List<OrderSourceDAO>();
            foreach (OrderSource OrderSource in OrderSources)
            {
                OrderSourceDAO OrderSourceDAO = new OrderSourceDAO();
                OrderSourceDAO.Id = OrderSource.Id;
                OrderSourceDAO.Code = OrderSource.Code;
                OrderSourceDAO.Name = OrderSource.Name;
                OrderSourceDAO.Priority = OrderSource.Priority;
                OrderSourceDAO.Description = OrderSource.Description;
                OrderSourceDAO.StatusId = OrderSource.StatusId;
                OrderSourceDAO.Used = OrderSource.Used;
                OrderSourceDAO.RowId = OrderSource.RowId;
                OrderSourceDAO.CreatedAt = StaticParams.DateTimeNow;
                OrderSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
                OrderSourceDAOs.Add(OrderSourceDAO);
            }
            await DataContext.BulkMergeAsync(OrderSourceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<OrderSource> OrderSources)
        {
            List<long> Ids = OrderSources.Select(x => x.Id).ToList();
            await DataContext.OrderSource
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new OrderSourceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(OrderSource OrderSource)
        {
        }
        
        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.OrderSource.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new OrderSourceDAO { Used = true });
            return true;
        }
    }
}
