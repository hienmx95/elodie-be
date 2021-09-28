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
    public interface IVariationRepository
    {
        Task<int> Count(VariationFilter VariationFilter);
        Task<List<Variation>> List(VariationFilter VariationFilter);
        Task<Variation> Get(long Id);
        Task<bool> Create(Variation Variation);
        Task<bool> Update(Variation Variation);
        Task<bool> Delete(Variation Variation);
        Task<bool> BulkMerge(List<Variation> Variations);
        Task<bool> BulkDelete(List<Variation> Variations);
    }
    public class VariationRepository : IVariationRepository
    {
        private DataContext DataContext;
        public VariationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<VariationDAO> DynamicFilter(IQueryable<VariationDAO> query, VariationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.VariationGroupingId != null)
                query = query.Where(q => q.VariationGroupingId, filter.VariationGroupingId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<VariationDAO> OrFilter(IQueryable<VariationDAO> query, VariationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<VariationDAO> initQuery = query.Where(q => false);
            foreach (VariationFilter VariationFilter in filter.OrFilter)
            {
                IQueryable<VariationDAO> queryable = query;
                if (VariationFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, VariationFilter.Id);
                if (VariationFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, VariationFilter.Code);
                if (VariationFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, VariationFilter.Name);
                if (VariationFilter.VariationGroupingId != null)
                    queryable = queryable.Where(q => q.VariationGroupingId, VariationFilter.VariationGroupingId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<VariationDAO> DynamicOrder(IQueryable<VariationDAO> query, VariationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case VariationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case VariationOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case VariationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case VariationOrder.VariationGrouping:
                            query = query.OrderBy(q => q.VariationGroupingId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case VariationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case VariationOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case VariationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case VariationOrder.VariationGrouping:
                            query = query.OrderByDescending(q => q.VariationGroupingId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Variation>> DynamicSelect(IQueryable<VariationDAO> query, VariationFilter filter)
        {
            List<Variation> Variations = await query.Select(q => new Variation()
            {
                Id = filter.Selects.Contains(VariationSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(VariationSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(VariationSelect.Name) ? q.Name : default(string),
                VariationGroupingId = filter.Selects.Contains(VariationSelect.VariationGrouping) ? q.VariationGroupingId : default(long),
                VariationGrouping = filter.Selects.Contains(VariationSelect.VariationGrouping) && q.VariationGrouping != null ? new VariationGrouping
                {
                    Id = q.VariationGrouping.Id,
                    Name = q.VariationGrouping.Name,
                    ProductId = q.VariationGrouping.ProductId,
                } : null,
            }).ToListAsync();
            return Variations;
        }

        public async Task<int> Count(VariationFilter filter)
        {
            IQueryable<VariationDAO> Variations = DataContext.Variation;
            Variations = DynamicFilter(Variations, filter);
            return await Variations.CountAsync();
        }

        public async Task<List<Variation>> List(VariationFilter filter)
        {
            if (filter == null) return new List<Variation>();
            IQueryable<VariationDAO> VariationDAOs = DataContext.Variation;
            VariationDAOs = DynamicFilter(VariationDAOs, filter);
            VariationDAOs = DynamicOrder(VariationDAOs, filter);
            List<Variation> Variations = await DynamicSelect(VariationDAOs, filter);
            return Variations;
        }

        public async Task<Variation> Get(long Id)
        {
            Variation Variation = await DataContext.Variation.Where(x => x.Id == Id).Select(x => new Variation()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                VariationGroupingId = x.VariationGroupingId,
                VariationGrouping = x.VariationGrouping == null ? null : new VariationGrouping
                {
                    Id = x.VariationGrouping.Id,
                    Name = x.VariationGrouping.Name,
                    ProductId = x.VariationGrouping.ProductId,
                },
            }).FirstOrDefaultAsync();

            if (Variation == null)
                return null;

            return Variation;
        }
        public async Task<bool> Create(Variation Variation)
        {
            VariationDAO VariationDAO = new VariationDAO();
            VariationDAO.Id = Variation.Id;
            VariationDAO.Code = Variation.Code;
            VariationDAO.Name = Variation.Name;
            VariationDAO.VariationGroupingId = Variation.VariationGroupingId;
            DataContext.Variation.Add(VariationDAO);
            await DataContext.SaveChangesAsync();
            Variation.Id = VariationDAO.Id;
            await SaveReference(Variation);
            return true;
        }

        public async Task<bool> Update(Variation Variation)
        {
            VariationDAO VariationDAO = DataContext.Variation.Where(x => x.Id == Variation.Id).FirstOrDefault();
            if (VariationDAO == null)
                return false;
            VariationDAO.Id = Variation.Id;
            VariationDAO.Code = Variation.Code;
            VariationDAO.Name = Variation.Name;
            VariationDAO.VariationGroupingId = Variation.VariationGroupingId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Variation);
            return true;
        }

        public async Task<bool> Delete(Variation Variation)
        {
            await DataContext.Variation.Where(x => x.Id == Variation.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Variation> Variations)
        {
            List<VariationDAO> VariationDAOs = new List<VariationDAO>();
            foreach (Variation Variation in Variations)
            {
                VariationDAO VariationDAO = new VariationDAO();
                VariationDAO.Id = Variation.Id;
                VariationDAO.Code = Variation.Code;
                VariationDAO.Name = Variation.Name;
                VariationDAO.VariationGroupingId = Variation.VariationGroupingId;
                VariationDAOs.Add(VariationDAO);
            }
            await DataContext.BulkMergeAsync(VariationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Variation> Variations)
        {
            List<long> Ids = Variations.Select(x => x.Id).ToList();
            await DataContext.Variation
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Variation Variation)
        {
        }

    }
}
