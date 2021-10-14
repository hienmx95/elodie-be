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
    public interface IPaymentTypeRepository
    {
        Task<int> CountAll(PaymentTypeFilter PaymentTypeFilter);
        Task<int> Count(PaymentTypeFilter PaymentTypeFilter);
        Task<List<PaymentType>> List(PaymentTypeFilter PaymentTypeFilter);
        Task<List<PaymentType>> List(List<long> Ids);
        Task<PaymentType> Get(long Id);
        Task<bool> Create(PaymentType PaymentType);
        Task<bool> Update(PaymentType PaymentType);
        Task<bool> Delete(PaymentType PaymentType);
        Task<bool> BulkMerge(List<PaymentType> PaymentTypes);
        Task<bool> BulkDelete(List<PaymentType> PaymentTypes);
        Task<bool> Used(List<long> Ids);
    }
    public class PaymentTypeRepository : IPaymentTypeRepository
    {
        private DataContext DataContext;
        public PaymentTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PaymentTypeDAO> DynamicFilter(IQueryable<PaymentTypeDAO> query, PaymentTypeFilter filter)
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
            query = query.Where(q => q.RowId, filter.RowId);
            
            return query;
        }

        private IQueryable<PaymentTypeDAO> OrFilter(IQueryable<PaymentTypeDAO> query, PaymentTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PaymentTypeDAO> initQuery = query.Where(q => false);
            foreach (PaymentTypeFilter PaymentTypeFilter in filter.OrFilter)
            {
                IQueryable<PaymentTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PaymentTypeDAO> DynamicOrder(IQueryable<PaymentTypeDAO> query, PaymentTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PaymentTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PaymentTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PaymentTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PaymentTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case PaymentTypeOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case PaymentTypeOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PaymentTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PaymentTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PaymentTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PaymentTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case PaymentTypeOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case PaymentTypeOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PaymentType>> DynamicSelect(IQueryable<PaymentTypeDAO> query, PaymentTypeFilter filter)
        {
            List<PaymentType> PaymentTypes = await query.Select(q => new PaymentType()
            {
                Id = filter.Selects.Contains(PaymentTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PaymentTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PaymentTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(PaymentTypeSelect.Status) ? q.StatusId : default(long),
                Used = filter.Selects.Contains(PaymentTypeSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(PaymentTypeSelect.Row) ? q.RowId : default(Guid),
            }).ToListAsync();
            return PaymentTypes;
        }

        public async Task<int> CountAll(PaymentTypeFilter filter)
        {
            IQueryable<PaymentTypeDAO> PaymentTypes = DataContext.PaymentType.AsNoTracking();
            PaymentTypes = DynamicFilter(PaymentTypes, filter);
            return await PaymentTypes.CountAsync();
        }

        public async Task<int> Count(PaymentTypeFilter filter)
        {
            IQueryable<PaymentTypeDAO> PaymentTypes = DataContext.PaymentType.AsNoTracking();
            PaymentTypes = DynamicFilter(PaymentTypes, filter);
            PaymentTypes = OrFilter(PaymentTypes, filter);
            return await PaymentTypes.CountAsync();
        }

        public async Task<List<PaymentType>> List(PaymentTypeFilter filter)
        {
            if (filter == null) return new List<PaymentType>();
            IQueryable<PaymentTypeDAO> PaymentTypeDAOs = DataContext.PaymentType.AsNoTracking();
            PaymentTypeDAOs = DynamicFilter(PaymentTypeDAOs, filter);
            PaymentTypeDAOs = OrFilter(PaymentTypeDAOs, filter);
            PaymentTypeDAOs = DynamicOrder(PaymentTypeDAOs, filter);
            List<PaymentType> PaymentTypes = await DynamicSelect(PaymentTypeDAOs, filter);
            return PaymentTypes;
        }

        public async Task<List<PaymentType>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.PaymentType
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<PaymentType> PaymentTypes = await query.AsNoTracking()
            .Select(x => new PaymentType()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
            }).ToListAsync();
            

            return PaymentTypes;
        }

        public async Task<PaymentType> Get(long Id)
        {
            PaymentType PaymentType = await DataContext.PaymentType.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new PaymentType()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
            }).FirstOrDefaultAsync();

            if (PaymentType == null)
                return null;

            return PaymentType;
        }
        
        public async Task<bool> Create(PaymentType PaymentType)
        {
            PaymentTypeDAO PaymentTypeDAO = new PaymentTypeDAO();
            PaymentTypeDAO.Id = PaymentType.Id;
            PaymentTypeDAO.Code = PaymentType.Code;
            PaymentTypeDAO.Name = PaymentType.Name;
            PaymentTypeDAO.StatusId = PaymentType.StatusId;
            PaymentTypeDAO.Used = PaymentType.Used;
            PaymentTypeDAO.RowId = PaymentType.RowId;
            PaymentTypeDAO.RowId = Guid.NewGuid();
            PaymentTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            PaymentTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.PaymentType.Add(PaymentTypeDAO);
            await DataContext.SaveChangesAsync();
            PaymentType.Id = PaymentTypeDAO.Id;
            await SaveReference(PaymentType);
            return true;
        }

        public async Task<bool> Update(PaymentType PaymentType)
        {
            PaymentTypeDAO PaymentTypeDAO = DataContext.PaymentType.Where(x => x.Id == PaymentType.Id).FirstOrDefault();
            if (PaymentTypeDAO == null)
                return false;
            PaymentTypeDAO.Id = PaymentType.Id;
            PaymentTypeDAO.Code = PaymentType.Code;
            PaymentTypeDAO.Name = PaymentType.Name;
            PaymentTypeDAO.StatusId = PaymentType.StatusId;
            PaymentTypeDAO.Used = PaymentType.Used;
            PaymentTypeDAO.RowId = PaymentType.RowId;
            PaymentTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(PaymentType);
            return true;
        }

        public async Task<bool> Delete(PaymentType PaymentType)
        {
            await DataContext.PaymentType.Where(x => x.Id == PaymentType.Id).UpdateFromQueryAsync(x => new PaymentTypeDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PaymentType> PaymentTypes)
        {
            List<PaymentTypeDAO> PaymentTypeDAOs = new List<PaymentTypeDAO>();
            foreach (PaymentType PaymentType in PaymentTypes)
            {
                PaymentTypeDAO PaymentTypeDAO = new PaymentTypeDAO();
                PaymentTypeDAO.Id = PaymentType.Id;
                PaymentTypeDAO.Code = PaymentType.Code;
                PaymentTypeDAO.Name = PaymentType.Name;
                PaymentTypeDAO.StatusId = PaymentType.StatusId;
                PaymentTypeDAO.Used = PaymentType.Used;
                PaymentTypeDAO.RowId = PaymentType.RowId;
                PaymentTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                PaymentTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                PaymentTypeDAOs.Add(PaymentTypeDAO);
            }
            await DataContext.BulkMergeAsync(PaymentTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PaymentType> PaymentTypes)
        {
            List<long> Ids = PaymentTypes.Select(x => x.Id).ToList();
            await DataContext.PaymentType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PaymentTypeDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(PaymentType PaymentType)
        {
        }
        
        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.PaymentType.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PaymentTypeDAO { Used = true });
            return true;
        }
    }
}
