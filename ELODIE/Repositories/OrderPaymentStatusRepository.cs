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
    public interface IOrderPaymentStatusRepository
    {
        Task<int> CountAll(OrderPaymentStatusFilter OrderPaymentStatusFilter);
        Task<int> Count(OrderPaymentStatusFilter OrderPaymentStatusFilter);
        Task<List<OrderPaymentStatus>> List(OrderPaymentStatusFilter OrderPaymentStatusFilter);
        Task<List<OrderPaymentStatus>> List(List<long> Ids);
    }
    public class OrderPaymentStatusRepository : IOrderPaymentStatusRepository
    {
        private DataContext DataContext;
        public OrderPaymentStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrderPaymentStatusDAO> DynamicFilter(IQueryable<OrderPaymentStatusDAO> query, OrderPaymentStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            
            return query;
        }

        private IQueryable<OrderPaymentStatusDAO> OrFilter(IQueryable<OrderPaymentStatusDAO> query, OrderPaymentStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrderPaymentStatusDAO> initQuery = query.Where(q => false);
            foreach (OrderPaymentStatusFilter OrderPaymentStatusFilter in filter.OrFilter)
            {
                IQueryable<OrderPaymentStatusDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<OrderPaymentStatusDAO> DynamicOrder(IQueryable<OrderPaymentStatusDAO> query, OrderPaymentStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrderPaymentStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrderPaymentStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrderPaymentStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrderPaymentStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrderPaymentStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrderPaymentStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<OrderPaymentStatus>> DynamicSelect(IQueryable<OrderPaymentStatusDAO> query, OrderPaymentStatusFilter filter)
        {
            List<OrderPaymentStatus> OrderPaymentStatuses = await query.Select(q => new OrderPaymentStatus()
            {
                Id = filter.Selects.Contains(OrderPaymentStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrderPaymentStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(OrderPaymentStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return OrderPaymentStatuses;
        }

        public async Task<int> CountAll(OrderPaymentStatusFilter filter)
        {
            IQueryable<OrderPaymentStatusDAO> OrderPaymentStatuses = DataContext.OrderPaymentStatus.AsNoTracking();
            OrderPaymentStatuses = DynamicFilter(OrderPaymentStatuses, filter);
            return await OrderPaymentStatuses.CountAsync();
        }

        public async Task<int> Count(OrderPaymentStatusFilter filter)
        {
            IQueryable<OrderPaymentStatusDAO> OrderPaymentStatuses = DataContext.OrderPaymentStatus.AsNoTracking();
            OrderPaymentStatuses = DynamicFilter(OrderPaymentStatuses, filter);
            OrderPaymentStatuses = OrFilter(OrderPaymentStatuses, filter);
            return await OrderPaymentStatuses.CountAsync();
        }

        public async Task<List<OrderPaymentStatus>> List(OrderPaymentStatusFilter filter)
        {
            if (filter == null) return new List<OrderPaymentStatus>();
            IQueryable<OrderPaymentStatusDAO> OrderPaymentStatusDAOs = DataContext.OrderPaymentStatus.AsNoTracking();
            OrderPaymentStatusDAOs = DynamicFilter(OrderPaymentStatusDAOs, filter);
            OrderPaymentStatusDAOs = OrFilter(OrderPaymentStatusDAOs, filter);
            OrderPaymentStatusDAOs = DynamicOrder(OrderPaymentStatusDAOs, filter);
            List<OrderPaymentStatus> OrderPaymentStatuses = await DynamicSelect(OrderPaymentStatusDAOs, filter);
            return OrderPaymentStatuses;
        }

        public async Task<List<OrderPaymentStatus>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.OrderPaymentStatus
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<OrderPaymentStatus> OrderPaymentStatuses = await query.AsNoTracking()
            .Select(x => new OrderPaymentStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return OrderPaymentStatuses;
        }

    }
}
