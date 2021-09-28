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
    public interface IUnitOfMeasureGroupingContentRepository
    {
        Task<int> Count(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<UnitOfMeasureGroupingContent> Get(long Id);
        Task<bool> Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> BulkMerge(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents);
        Task<bool> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents);
    }
    public class UnitOfMeasureGroupingContentRepository : IUnitOfMeasureGroupingContentRepository
    {
        private DataContext DataContext;
        public UnitOfMeasureGroupingContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> DynamicFilter(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.UnitOfMeasureGroupingId != null)
                query = query.Where(q => q.UnitOfMeasureGroupingId, filter.UnitOfMeasureGroupingId);
            if (filter.UnitOfMeasureId != null)
                query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            if (filter.Factor != null)
                query = query.Where(q => q.Factor, filter.Factor);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> OrFilter(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<UnitOfMeasureGroupingContentDAO> initQuery = query.Where(q => false);
            foreach (UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter in filter.OrFilter)
            {
                IQueryable<UnitOfMeasureGroupingContentDAO> queryable = query;
                if (UnitOfMeasureGroupingContentFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, UnitOfMeasureGroupingContentFilter.Id);
                if (UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureGroupingId, UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId);
                if (UnitOfMeasureGroupingContentFilter.UnitOfMeasureId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureId, UnitOfMeasureGroupingContentFilter.UnitOfMeasureId);
                if (UnitOfMeasureGroupingContentFilter.Factor != null)
                    queryable = queryable.Where(q => q.Factor, UnitOfMeasureGroupingContentFilter.Factor);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> DynamicOrder(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureGroupingContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasureGrouping:
                            query = query.OrderBy(q => q.UnitOfMeasureGroupingId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.Factor:
                            query = query.OrderBy(q => q.Factor);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureGroupingContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasureGrouping:
                            query = query.OrderByDescending(q => q.UnitOfMeasureGroupingId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.Factor:
                            query = query.OrderByDescending(q => q.Factor);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UnitOfMeasureGroupingContent>> DynamicSelect(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await query.Select(q => new UnitOfMeasureGroupingContent()
            {
                Id = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.Id) ? q.Id : default(long),
                UnitOfMeasureGroupingId = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasureGrouping) ? q.UnitOfMeasureGroupingId : default(long),
                UnitOfMeasureId = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                Factor = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.Factor) ? q.Factor : default(long?),
                UnitOfMeasure = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                } : null,
                UnitOfMeasureGrouping = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasureGrouping) && q.UnitOfMeasureGrouping != null ? new UnitOfMeasureGrouping
                {
                    Id = q.UnitOfMeasureGrouping.Id,
                    Name = q.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = q.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = q.UnitOfMeasureGrouping.StatusId,
                } : null,
            }).ToListAsync();
            return UnitOfMeasureGroupingContents;
        }

        public async Task<int> Count(UnitOfMeasureGroupingContentFilter filter)
        {
            IQueryable<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContents = DataContext.UnitOfMeasureGroupingContent;
            UnitOfMeasureGroupingContents = DynamicFilter(UnitOfMeasureGroupingContents, filter);
            return await UnitOfMeasureGroupingContents.CountAsync();
        }

        public async Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter == null) return new List<UnitOfMeasureGroupingContent>();
            IQueryable<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentDAOs = DataContext.UnitOfMeasureGroupingContent;
            UnitOfMeasureGroupingContentDAOs = DynamicFilter(UnitOfMeasureGroupingContentDAOs, filter);
            UnitOfMeasureGroupingContentDAOs = DynamicOrder(UnitOfMeasureGroupingContentDAOs, filter);
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await DynamicSelect(UnitOfMeasureGroupingContentDAOs, filter);
            return UnitOfMeasureGroupingContents;
        }

        public async Task<UnitOfMeasureGroupingContent> Get(long Id)
        {
            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = await DataContext.UnitOfMeasureGroupingContent.Where(x => x.Id == Id).AsNoTracking().Select(x => new UnitOfMeasureGroupingContent()
            {
                Id = x.Id,
                UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Factor = x.Factor,
                RowId = x.RowId,
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                },
                UnitOfMeasureGrouping = x.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                {
                    Id = x.UnitOfMeasureGrouping.Id,
                    Name = x.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = x.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = x.UnitOfMeasureGrouping.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (UnitOfMeasureGroupingContent == null)
                return null;

            return UnitOfMeasureGroupingContent;
        }
        public async Task<bool> Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO = new UnitOfMeasureGroupingContentDAO();
            UnitOfMeasureGroupingContentDAO.Id = UnitOfMeasureGroupingContent.Id;
            UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
            UnitOfMeasureGroupingContentDAO.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
            UnitOfMeasureGroupingContentDAO.Factor = UnitOfMeasureGroupingContent.Factor;
            DataContext.UnitOfMeasureGroupingContent.Add(UnitOfMeasureGroupingContentDAO);
            await DataContext.SaveChangesAsync();
            UnitOfMeasureGroupingContent.Id = UnitOfMeasureGroupingContentDAO.Id;
            await SaveReference(UnitOfMeasureGroupingContent);
            return true;
        }

        public async Task<bool> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO = DataContext.UnitOfMeasureGroupingContent
                .Where(x => x.Id == UnitOfMeasureGroupingContent.Id).FirstOrDefault();
            if (UnitOfMeasureGroupingContentDAO == null)
                return false;
            UnitOfMeasureGroupingContentDAO.Id = UnitOfMeasureGroupingContent.Id;
            UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
            UnitOfMeasureGroupingContentDAO.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
            UnitOfMeasureGroupingContentDAO.Factor = UnitOfMeasureGroupingContent.Factor;
            await DataContext.SaveChangesAsync();
            await SaveReference(UnitOfMeasureGroupingContent);
            return true;
        }

        public async Task<bool> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            await DataContext.UnitOfMeasureGroupingContent.Where(x => x.Id == UnitOfMeasureGroupingContent.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents)
        {
            List<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentDAOs = new List<UnitOfMeasureGroupingContentDAO>();
            foreach (UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent in UnitOfMeasureGroupingContents)
            {
                UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO = new UnitOfMeasureGroupingContentDAO();
                UnitOfMeasureGroupingContentDAO.Id = UnitOfMeasureGroupingContent.Id;
                UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
                UnitOfMeasureGroupingContentDAO.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
                UnitOfMeasureGroupingContentDAO.Factor = UnitOfMeasureGroupingContent.Factor;
                UnitOfMeasureGroupingContentDAOs.Add(UnitOfMeasureGroupingContentDAO);
            }
            await DataContext.BulkMergeAsync(UnitOfMeasureGroupingContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents)
        {
            List<long> Ids = UnitOfMeasureGroupingContents.Select(x => x.Id).ToList();
            await DataContext.UnitOfMeasureGroupingContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
        }

    }
}
