using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IStatusRepository
    {
        Task<int> Count(StatusFilter StatusFilter);
        Task<List<Status>> List(StatusFilter StatusFilter);
        Task<Status> Get(long Id);
    }
    public class StatusRepository : IStatusRepository
    {
        private DataContext DataContext;
        public StatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StatusDAO> DynamicFilter(IQueryable<StatusDAO> query, StatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<StatusDAO> OrFilter(IQueryable<StatusDAO> query, StatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StatusDAO> initQuery = query.Where(q => false);
            foreach (StatusFilter StatusFilter in filter.OrFilter)
            {
                IQueryable<StatusDAO> queryable = query;
                if (StatusFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, StatusFilter.Id);
                if (StatusFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, StatusFilter.Code);
                if (StatusFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, StatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<StatusDAO> DynamicOrder(IQueryable<StatusDAO> query, StatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Status>> DynamicSelect(IQueryable<StatusDAO> query, StatusFilter filter)
        {
            List<Status> Statuses = await query.Select(q => new Status()
            {
                Id = filter.Selects.Contains(StatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Statuses;
        }

        public async Task<int> Count(StatusFilter filter)
        {
            IQueryable<StatusDAO> Statuss = DataContext.Status;
            Statuss = DynamicFilter(Statuss, filter);
            return await Statuss.CountAsync();
        }

        public async Task<List<Status>> List(StatusFilter filter)
        {
            if (filter == null) return new List<Status>();
            IQueryable<StatusDAO> StatusDAOs = DataContext.Status;
            StatusDAOs = DynamicFilter(StatusDAOs, filter);
            StatusDAOs = DynamicOrder(StatusDAOs, filter);
            List<Status> Statuses = await DynamicSelect(StatusDAOs, filter);
            return Statuses;
        }

        public async Task<Status> Get(long Id)
        {
            Status Status = await DataContext.Status.Where(x => x.Id == Id).Select(x => new Status()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (Status == null)
                return null;

            return Status;
        }
    }
}
