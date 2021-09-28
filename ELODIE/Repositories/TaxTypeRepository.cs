using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using ELODIE.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface ITaxTypeRepository
    {
        Task<int> Count(TaxTypeFilter TaxTypeFilter);
        Task<List<TaxType>> List(TaxTypeFilter TaxTypeFilter);
        Task<List<TaxType>> List(List<long> Ids);
        Task<TaxType> Get(long Id);
        Task<bool> Create(TaxType TaxType);
        Task<bool> Update(TaxType TaxType);
        Task<bool> Delete(TaxType TaxType);
        Task<bool> BulkMerge(List<TaxType> TaxTypes);
        Task<bool> BulkDelete(List<TaxType> TaxTypes);
        Task<bool> Used(List<long> RowIds);
    }
    public class TaxTypeRepository : ITaxTypeRepository
    {
        private DataContext DataContext;
        public TaxTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<TaxTypeDAO> DynamicFilter(IQueryable<TaxTypeDAO> query, TaxTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Percentage != null)
                query = query.Where(q => q.Percentage, filter.Percentage);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                List<string> Tokens = filter.Search.Split(" ").Select(x => x.ToLower()).ToList();
                var queryForCode = query;
                var queryForName = query;
                foreach (string Token in Tokens)
                {
                    if (string.IsNullOrWhiteSpace(Token))
                        continue;
                    queryForCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                    queryForName = queryForName.Where(x => x.Name.ToLower().Contains(Token));
                }
                query = queryForCode.Union(queryForName);
                query = query.Distinct();
            }

            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<TaxTypeDAO> OrFilter(IQueryable<TaxTypeDAO> query, TaxTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<TaxTypeDAO> initQuery = query.Where(q => false);
            foreach (TaxTypeFilter TaxTypeFilter in filter.OrFilter)
            {
                IQueryable<TaxTypeDAO> queryable = query;
                if (TaxTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, TaxTypeFilter.Id);
                if (TaxTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, TaxTypeFilter.Code);
                if (TaxTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, TaxTypeFilter.Name);
                if (TaxTypeFilter.Percentage != null)
                    queryable = queryable.Where(q => q.Percentage, TaxTypeFilter.Percentage);
                if (TaxTypeFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, TaxTypeFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<TaxTypeDAO> DynamicOrder(IQueryable<TaxTypeDAO> query, TaxTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case TaxTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case TaxTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case TaxTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case TaxTypeOrder.Percentage:
                            query = query.OrderBy(q => q.Percentage);
                            break;
                        case TaxTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        default:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case TaxTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case TaxTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case TaxTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case TaxTypeOrder.Percentage:
                            query = query.OrderByDescending(q => q.Percentage);
                            break;
                        case TaxTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<TaxType>> DynamicSelect(IQueryable<TaxTypeDAO> query, TaxTypeFilter filter)
        {
            List<TaxType> TaxTypes = await query.Select(q => new TaxType()
            {
                Id = filter.Selects.Contains(TaxTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(TaxTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(TaxTypeSelect.Name) ? q.Name : default(string),
                Percentage = filter.Selects.Contains(TaxTypeSelect.Percentage) ? q.Percentage : default(decimal),
                StatusId = filter.Selects.Contains(TaxTypeSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(TaxTypeSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Used = q.Used,
            }).ToListAsync();
            return TaxTypes;
        }

        public async Task<int> Count(TaxTypeFilter filter)
        {
            IQueryable<TaxTypeDAO> TaxTypes = DataContext.TaxType;
            TaxTypes = DynamicFilter(TaxTypes, filter);
            return await TaxTypes.CountAsync();
        }

        public async Task<List<TaxType>> List(TaxTypeFilter filter)
        {
            if (filter == null) return new List<TaxType>();
            IQueryable<TaxTypeDAO> TaxTypeDAOs = DataContext.TaxType;
            TaxTypeDAOs = DynamicFilter(TaxTypeDAOs, filter);
            TaxTypeDAOs = DynamicOrder(TaxTypeDAOs, filter);
            List<TaxType> TaxTypes = await DynamicSelect(TaxTypeDAOs, filter);
            return TaxTypes;
        }

        public async Task<List<TaxType>> List(List<long> Ids)
        {
            IQueryable<TaxTypeDAO> TaxTypeDAOs = DataContext.TaxType.AsNoTracking();
            List<TaxType> TaxTypes = TaxTypeDAOs.Where(x => Ids.Contains(x.Id))
                .Select(x => new TaxType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Percentage = x.Percentage,
                    StatusId = x.StatusId,
                    Used = x.Used,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    RowId = x.RowId,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            return TaxTypes;
        }

        public async Task<TaxType> Get(long Id)
        {
            TaxType TaxType = await DataContext.TaxType.Where(x => x.Id == Id).Select(x => new TaxType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Percentage = x.Percentage,
                StatusId = x.StatusId,
                Used = x.Used,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (TaxType == null)
                return null;

            return TaxType;
        }
        public async Task<bool> Create(TaxType TaxType)
        {
            TaxTypeDAO TaxTypeDAO = new TaxTypeDAO();
            TaxTypeDAO.Id = TaxType.Id;
            TaxTypeDAO.Code = TaxType.Code;
            TaxTypeDAO.Name = TaxType.Name;
            TaxTypeDAO.Percentage = TaxType.Percentage;
            TaxTypeDAO.StatusId = TaxType.StatusId;
            TaxTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            TaxTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            TaxTypeDAO.RowId = Guid.NewGuid();
            TaxTypeDAO.Used = false;
            DataContext.TaxType.Add(TaxTypeDAO);
            await DataContext.SaveChangesAsync();
            TaxType.Id = TaxTypeDAO.Id;
            return true;
        }

        public async Task<bool> Update(TaxType TaxType)
        {
            TaxTypeDAO TaxTypeDAO = DataContext.TaxType.Where(x => x.Id == TaxType.Id).FirstOrDefault();
            if (TaxTypeDAO == null)
                return false;
            TaxTypeDAO.Id = TaxType.Id;
            TaxTypeDAO.Code = TaxType.Code;
            TaxTypeDAO.Name = TaxType.Name;
            TaxTypeDAO.Percentage = TaxType.Percentage;
            TaxTypeDAO.StatusId = TaxType.StatusId;
            TaxTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(TaxType TaxType)
        {
            await DataContext.TaxType.Where(x => x.Id == TaxType.Id).UpdateFromQueryAsync(x => new TaxTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<TaxType> TaxTypes)
        {
            List<TaxTypeDAO> TaxTypeDAOs = new List<TaxTypeDAO>();
            foreach (TaxType TaxType in TaxTypes)
            {
                TaxTypeDAO TaxTypeDAO = new TaxTypeDAO();
                TaxTypeDAO.Id = TaxType.Id;
                TaxTypeDAO.Code = TaxType.Code;
                TaxTypeDAO.Name = TaxType.Name;
                TaxTypeDAO.Percentage = TaxType.Percentage;
                TaxTypeDAO.StatusId = TaxType.StatusId;
                TaxTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                TaxTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                TaxTypeDAOs.Add(TaxTypeDAO);
            }
            await DataContext.BulkMergeAsync(TaxTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<TaxType> TaxTypes)
        {
            List<long> Ids = TaxTypes.Select(x => x.Id).ToList();
            await DataContext.TaxType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new TaxTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.TaxType.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new TaxTypeDAO { Used = true });
            return true;

        }
    }
}
