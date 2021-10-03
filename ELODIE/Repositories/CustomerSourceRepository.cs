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
    public interface ICustomerSourceRepository
    {
        Task<int> CountAll(CustomerSourceFilter CustomerSourceFilter);
        Task<int> Count(CustomerSourceFilter CustomerSourceFilter);
        Task<List<CustomerSource>> List(CustomerSourceFilter CustomerSourceFilter);
        Task<List<CustomerSource>> List(List<long> Ids);
        Task<CustomerSource> Get(long Id);
        Task<bool> Create(CustomerSource CustomerSource);
        Task<bool> Update(CustomerSource CustomerSource);
        Task<bool> Delete(CustomerSource CustomerSource);
        Task<bool> BulkMerge(List<CustomerSource> CustomerSources);
        Task<bool> BulkDelete(List<CustomerSource> CustomerSources);
        Task<bool> Used(List<long> Ids);
    }
    public class CustomerSourceRepository : ICustomerSourceRepository
    {
        private DataContext DataContext;
        public CustomerSourceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CustomerSourceDAO> DynamicFilter(IQueryable<CustomerSourceDAO> query, CustomerSourceFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.RowId, filter.RowId);
            
            return query;
        }

        private IQueryable<CustomerSourceDAO> OrFilter(IQueryable<CustomerSourceDAO> query, CustomerSourceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CustomerSourceDAO> initQuery = query.Where(q => false);
            foreach (CustomerSourceFilter CustomerSourceFilter in filter.OrFilter)
            {
                IQueryable<CustomerSourceDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.Description, filter.Description);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CustomerSourceDAO> DynamicOrder(IQueryable<CustomerSourceDAO> query, CustomerSourceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSourceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CustomerSourceOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CustomerSourceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case CustomerSourceOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case CustomerSourceOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case CustomerSourceOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case CustomerSourceOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSourceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CustomerSourceOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CustomerSourceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case CustomerSourceOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case CustomerSourceOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case CustomerSourceOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case CustomerSourceOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CustomerSource>> DynamicSelect(IQueryable<CustomerSourceDAO> query, CustomerSourceFilter filter)
        {
            List<CustomerSource> CustomerSources = await query.Select(q => new CustomerSource()
            {
                Id = filter.Selects.Contains(CustomerSourceSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CustomerSourceSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(CustomerSourceSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(CustomerSourceSelect.Status) ? q.StatusId : default(long),
                Description = filter.Selects.Contains(CustomerSourceSelect.Description) ? q.Description : default(string),
                Used = filter.Selects.Contains(CustomerSourceSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(CustomerSourceSelect.Row) ? q.RowId : default(Guid),
            }).ToListAsync();
            return CustomerSources;
        }

        public async Task<int> CountAll(CustomerSourceFilter filter)
        {
            IQueryable<CustomerSourceDAO> CustomerSources = DataContext.CustomerSource.AsNoTracking();
            CustomerSources = DynamicFilter(CustomerSources, filter);
            return await CustomerSources.CountAsync();
        }

        public async Task<int> Count(CustomerSourceFilter filter)
        {
            IQueryable<CustomerSourceDAO> CustomerSources = DataContext.CustomerSource.AsNoTracking();
            CustomerSources = DynamicFilter(CustomerSources, filter);
            CustomerSources = OrFilter(CustomerSources, filter);
            return await CustomerSources.CountAsync();
        }

        public async Task<List<CustomerSource>> List(CustomerSourceFilter filter)
        {
            if (filter == null) return new List<CustomerSource>();
            IQueryable<CustomerSourceDAO> CustomerSourceDAOs = DataContext.CustomerSource.AsNoTracking();
            CustomerSourceDAOs = DynamicFilter(CustomerSourceDAOs, filter);
            CustomerSourceDAOs = OrFilter(CustomerSourceDAOs, filter);
            CustomerSourceDAOs = DynamicOrder(CustomerSourceDAOs, filter);
            List<CustomerSource> CustomerSources = await DynamicSelect(CustomerSourceDAOs, filter);
            return CustomerSources;
        }

        public async Task<List<CustomerSource>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.CustomerSource
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<CustomerSource> CustomerSources = await query.AsNoTracking()
            .Select(x => new CustomerSource()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Description = x.Description,
                Used = x.Used,
                RowId = x.RowId,
            }).ToListAsync();
            

            return CustomerSources;
        }

        public async Task<CustomerSource> Get(long Id)
        {
            CustomerSource CustomerSource = await DataContext.CustomerSource.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new CustomerSource()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Description = x.Description,
                Used = x.Used,
                RowId = x.RowId,
            }).FirstOrDefaultAsync();

            if (CustomerSource == null)
                return null;

            return CustomerSource;
        }
        
        public async Task<bool> Create(CustomerSource CustomerSource)
        {
            CustomerSourceDAO CustomerSourceDAO = new CustomerSourceDAO();
            CustomerSourceDAO.Id = CustomerSource.Id;
            CustomerSourceDAO.Code = CustomerSource.Code;
            CustomerSourceDAO.Name = CustomerSource.Name;
            CustomerSourceDAO.StatusId = CustomerSource.StatusId;
            CustomerSourceDAO.Description = CustomerSource.Description;
            CustomerSourceDAO.Used = CustomerSource.Used;
            CustomerSourceDAO.RowId = CustomerSource.RowId;
            CustomerSourceDAO.RowId = Guid.NewGuid();
            CustomerSourceDAO.CreatedAt = StaticParams.DateTimeNow;
            CustomerSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.CustomerSource.Add(CustomerSourceDAO);
            await DataContext.SaveChangesAsync();
            CustomerSource.Id = CustomerSourceDAO.Id;
            await SaveReference(CustomerSource);
            return true;
        }

        public async Task<bool> Update(CustomerSource CustomerSource)
        {
            CustomerSourceDAO CustomerSourceDAO = DataContext.CustomerSource.Where(x => x.Id == CustomerSource.Id).FirstOrDefault();
            if (CustomerSourceDAO == null)
                return false;
            CustomerSourceDAO.Id = CustomerSource.Id;
            CustomerSourceDAO.Code = CustomerSource.Code;
            CustomerSourceDAO.Name = CustomerSource.Name;
            CustomerSourceDAO.StatusId = CustomerSource.StatusId;
            CustomerSourceDAO.Description = CustomerSource.Description;
            CustomerSourceDAO.Used = CustomerSource.Used;
            CustomerSourceDAO.RowId = CustomerSource.RowId;
            CustomerSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(CustomerSource);
            return true;
        }

        public async Task<bool> Delete(CustomerSource CustomerSource)
        {
            await DataContext.CustomerSource.Where(x => x.Id == CustomerSource.Id).UpdateFromQueryAsync(x => new CustomerSourceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<CustomerSource> CustomerSources)
        {
            List<CustomerSourceDAO> CustomerSourceDAOs = new List<CustomerSourceDAO>();
            foreach (CustomerSource CustomerSource in CustomerSources)
            {
                CustomerSourceDAO CustomerSourceDAO = new CustomerSourceDAO();
                CustomerSourceDAO.Id = CustomerSource.Id;
                CustomerSourceDAO.Code = CustomerSource.Code;
                CustomerSourceDAO.Name = CustomerSource.Name;
                CustomerSourceDAO.StatusId = CustomerSource.StatusId;
                CustomerSourceDAO.Description = CustomerSource.Description;
                CustomerSourceDAO.Used = CustomerSource.Used;
                CustomerSourceDAO.RowId = CustomerSource.RowId;
                CustomerSourceDAO.CreatedAt = StaticParams.DateTimeNow;
                CustomerSourceDAO.UpdatedAt = StaticParams.DateTimeNow;
                CustomerSourceDAOs.Add(CustomerSourceDAO);
            }
            await DataContext.BulkMergeAsync(CustomerSourceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<CustomerSource> CustomerSources)
        {
            List<long> Ids = CustomerSources.Select(x => x.Id).ToList();
            await DataContext.CustomerSource
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerSourceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(CustomerSource CustomerSource)
        {
        }
        
        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.CustomerSource.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerSourceDAO { Used = true });
            return true;
        }
    }
}
