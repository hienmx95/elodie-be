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
    public interface ICustomerGroupingRepository
    {
        Task<int> CountAll(CustomerGroupingFilter CustomerGroupingFilter);
        Task<int> Count(CustomerGroupingFilter CustomerGroupingFilter);
        Task<List<CustomerGrouping>> List(CustomerGroupingFilter CustomerGroupingFilter);
        Task<List<CustomerGrouping>> List(List<long> Ids);
        Task<CustomerGrouping> Get(long Id);
        Task<bool> Create(CustomerGrouping CustomerGrouping);
        Task<bool> Update(CustomerGrouping CustomerGrouping);
        Task<bool> Delete(CustomerGrouping CustomerGrouping);
        Task<bool> BulkMerge(List<CustomerGrouping> CustomerGroupings);
        Task<bool> BulkDelete(List<CustomerGrouping> CustomerGroupings);
    }
    public class CustomerGroupingRepository : ICustomerGroupingRepository
    {
        private DataContext DataContext;
        public CustomerGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CustomerGroupingDAO> DynamicFilter(IQueryable<CustomerGroupingDAO> query, CustomerGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.ParentId, filter.ParentId);
            query = query.Where(q => q.Path, filter.Path);
            query = query.Where(q => q.Level, filter.Level);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.Description, filter.Description);
            
            return query;
        }

        private IQueryable<CustomerGroupingDAO> OrFilter(IQueryable<CustomerGroupingDAO> query, CustomerGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CustomerGroupingDAO> initQuery = query.Where(q => false);
            foreach (CustomerGroupingFilter CustomerGroupingFilter in filter.OrFilter)
            {
                IQueryable<CustomerGroupingDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.ParentId, filter.ParentId);
                queryable = queryable.Where(q => q.Path, filter.Path);
                queryable = queryable.Where(q => q.Level, filter.Level);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.Description, filter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CustomerGroupingDAO> DynamicOrder(IQueryable<CustomerGroupingDAO> query, CustomerGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CustomerGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CustomerGroupingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CustomerGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case CustomerGroupingOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case CustomerGroupingOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case CustomerGroupingOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case CustomerGroupingOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case CustomerGroupingOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CustomerGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CustomerGroupingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CustomerGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case CustomerGroupingOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case CustomerGroupingOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case CustomerGroupingOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case CustomerGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case CustomerGroupingOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CustomerGrouping>> DynamicSelect(IQueryable<CustomerGroupingDAO> query, CustomerGroupingFilter filter)
        {
            List<CustomerGrouping> CustomerGroupings = await query.Select(q => new CustomerGrouping()
            {
                Id = filter.Selects.Contains(CustomerGroupingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CustomerGroupingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(CustomerGroupingSelect.Name) ? q.Name : default(string),
                ParentId = filter.Selects.Contains(CustomerGroupingSelect.Parent) ? q.ParentId : default(long?),
                Path = filter.Selects.Contains(CustomerGroupingSelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(CustomerGroupingSelect.Level) ? q.Level : default(long),
                StatusId = filter.Selects.Contains(CustomerGroupingSelect.Status) ? q.StatusId : default(long),
                Description = filter.Selects.Contains(CustomerGroupingSelect.Description) ? q.Description : default(string),
            }).ToListAsync();
            return CustomerGroupings;
        }

        public async Task<int> CountAll(CustomerGroupingFilter filter)
        {
            IQueryable<CustomerGroupingDAO> CustomerGroupings = DataContext.CustomerGrouping.AsNoTracking();
            CustomerGroupings = DynamicFilter(CustomerGroupings, filter);
            return await CustomerGroupings.CountAsync();
        }

        public async Task<int> Count(CustomerGroupingFilter filter)
        {
            IQueryable<CustomerGroupingDAO> CustomerGroupings = DataContext.CustomerGrouping.AsNoTracking();
            CustomerGroupings = DynamicFilter(CustomerGroupings, filter);
            CustomerGroupings = OrFilter(CustomerGroupings, filter);
            return await CustomerGroupings.CountAsync();
        }

        public async Task<List<CustomerGrouping>> List(CustomerGroupingFilter filter)
        {
            if (filter == null) return new List<CustomerGrouping>();
            IQueryable<CustomerGroupingDAO> CustomerGroupingDAOs = DataContext.CustomerGrouping.AsNoTracking();
            CustomerGroupingDAOs = DynamicFilter(CustomerGroupingDAOs, filter);
            CustomerGroupingDAOs = OrFilter(CustomerGroupingDAOs, filter);
            CustomerGroupingDAOs = DynamicOrder(CustomerGroupingDAOs, filter);
            List<CustomerGrouping> CustomerGroupings = await DynamicSelect(CustomerGroupingDAOs, filter);
            return CustomerGroupings;
        }

        public async Task<List<CustomerGrouping>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.CustomerGrouping
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<CustomerGrouping> CustomerGroupings = await query.AsNoTracking()
            .Select(x => new CustomerGrouping()
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
                Description = x.Description,
            }).ToListAsync();
            

            return CustomerGroupings;
        }

        public async Task<CustomerGrouping> Get(long Id)
        {
            CustomerGrouping CustomerGrouping = await DataContext.CustomerGrouping.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new CustomerGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ParentId = x.ParentId,
                Path = x.Path,
                Level = x.Level,
                StatusId = x.StatusId,
                Description = x.Description,
            }).FirstOrDefaultAsync();

            if (CustomerGrouping == null)
                return null;

            return CustomerGrouping;
        }
        
        public async Task<bool> Create(CustomerGrouping CustomerGrouping)
        {
            CustomerGroupingDAO CustomerGroupingDAO = new CustomerGroupingDAO();
            CustomerGroupingDAO.Id = CustomerGrouping.Id;
            CustomerGroupingDAO.Code = CustomerGrouping.Code;
            CustomerGroupingDAO.Name = CustomerGrouping.Name;
            CustomerGroupingDAO.ParentId = CustomerGrouping.ParentId;
            CustomerGroupingDAO.Path = CustomerGrouping.Path;
            CustomerGroupingDAO.Level = CustomerGrouping.Level;
            CustomerGroupingDAO.StatusId = CustomerGrouping.StatusId;
            CustomerGroupingDAO.Description = CustomerGrouping.Description;
            CustomerGroupingDAO.Path = "";
            CustomerGroupingDAO.Level = 1;
            CustomerGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            CustomerGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.CustomerGrouping.Add(CustomerGroupingDAO);
            await DataContext.SaveChangesAsync();
            CustomerGrouping.Id = CustomerGroupingDAO.Id;
            await SaveReference(CustomerGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(CustomerGrouping CustomerGrouping)
        {
            CustomerGroupingDAO CustomerGroupingDAO = DataContext.CustomerGrouping.Where(x => x.Id == CustomerGrouping.Id).FirstOrDefault();
            if (CustomerGroupingDAO == null)
                return false;
            CustomerGroupingDAO.Id = CustomerGrouping.Id;
            CustomerGroupingDAO.Code = CustomerGrouping.Code;
            CustomerGroupingDAO.Name = CustomerGrouping.Name;
            CustomerGroupingDAO.ParentId = CustomerGrouping.ParentId;
            CustomerGroupingDAO.Path = CustomerGrouping.Path;
            CustomerGroupingDAO.Level = CustomerGrouping.Level;
            CustomerGroupingDAO.StatusId = CustomerGrouping.StatusId;
            CustomerGroupingDAO.Description = CustomerGrouping.Description;
            CustomerGroupingDAO.Path = "";
            CustomerGroupingDAO.Level = 1;
            CustomerGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(CustomerGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(CustomerGrouping CustomerGrouping)
        {
            CustomerGroupingDAO CustomerGroupingDAO = await DataContext.CustomerGrouping.Where(x => x.Id == CustomerGrouping.Id).FirstOrDefaultAsync();
            await DataContext.CustomerGrouping.Where(x => x.Path.StartsWith(CustomerGroupingDAO.Id + ".")).UpdateFromQueryAsync(x => new CustomerGroupingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await DataContext.CustomerGrouping.Where(x => x.Id == CustomerGrouping.Id).UpdateFromQueryAsync(x => new CustomerGroupingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<CustomerGrouping> CustomerGroupings)
        {
            List<CustomerGroupingDAO> CustomerGroupingDAOs = new List<CustomerGroupingDAO>();
            foreach (CustomerGrouping CustomerGrouping in CustomerGroupings)
            {
                CustomerGroupingDAO CustomerGroupingDAO = new CustomerGroupingDAO();
                CustomerGroupingDAO.Id = CustomerGrouping.Id;
                CustomerGroupingDAO.Code = CustomerGrouping.Code;
                CustomerGroupingDAO.Name = CustomerGrouping.Name;
                CustomerGroupingDAO.ParentId = CustomerGrouping.ParentId;
                CustomerGroupingDAO.Path = CustomerGrouping.Path;
                CustomerGroupingDAO.Level = CustomerGrouping.Level;
                CustomerGroupingDAO.StatusId = CustomerGrouping.StatusId;
                CustomerGroupingDAO.Description = CustomerGrouping.Description;
                CustomerGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                CustomerGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                CustomerGroupingDAOs.Add(CustomerGroupingDAO);
            }
            await DataContext.BulkMergeAsync(CustomerGroupingDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<CustomerGrouping> CustomerGroupings)
        {
            List<long> Ids = CustomerGroupings.Select(x => x.Id).ToList();
            await DataContext.CustomerGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerGroupingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(CustomerGrouping CustomerGrouping)
        {
        }
        
        private async Task BuildPath()
        {
            List<CustomerGroupingDAO> CustomerGroupingDAOs = await DataContext.CustomerGrouping
                .Where(x => x.DeletedAt == null)
                .AsNoTracking().ToListAsync();
            Queue<CustomerGroupingDAO> queue = new Queue<CustomerGroupingDAO>();
            CustomerGroupingDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    x.Level = 1;
                    queue.Enqueue(x);
                }
            });
            while(queue.Count > 0)
            {
                CustomerGroupingDAO Parent = queue.Dequeue();
                foreach (CustomerGroupingDAO CustomerGroupingDAO in CustomerGroupingDAOs)
                {
                    if (CustomerGroupingDAO.ParentId == Parent.Id)
                    {
                        CustomerGroupingDAO.Path = Parent.Path + CustomerGroupingDAO.Id + ".";
                        CustomerGroupingDAO.Level = Parent.Level + 1;
                        queue.Enqueue(CustomerGroupingDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(CustomerGroupingDAOs);
        }
    }
}
