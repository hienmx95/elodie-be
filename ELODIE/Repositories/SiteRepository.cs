using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Helpers;

namespace ELODIE.Repositories
{
    public interface ISiteRepository
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
        Task<bool> BulkMerge(List<Site> Sites);
    }
    public class SiteRepository : ISiteRepository
    {
        private DataContext DataContext;
        public SiteRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SiteDAO> DynamicFilter(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.IsDisplay != null)
                query = query.Where(q => q.IsDisplay == filter.IsDisplay.Value);
            return query;
        }

        private IQueryable<SiteDAO> DynamicOrder(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SiteOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SiteOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Site>> DynamicSelect(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            List<Site> Sites = await query.Select(q => new Site()
            {
                Id = q.Id,
                Code = q.Code,
                Name = q.Name,
                Description = q.Description,
                Icon = q.Icon,
                Logo = q.Logo,
                IsDisplay = q.IsDisplay,
                RowId = q.RowId,
            }).ToListAsync();
            return Sites;
        }

        public async Task<int> Count(SiteFilter filter)
        {
            IQueryable<SiteDAO> Sites = DataContext.Site;
            Sites = DynamicFilter(Sites, filter);
            return await Sites.CountAsync();
        }

        public async Task<List<Site>> List(SiteFilter filter)
        {
            if (filter == null) return new List<Site>();
            IQueryable<SiteDAO> SiteDAOs = DataContext.Site;
            SiteDAOs = DynamicFilter(SiteDAOs, filter);
            SiteDAOs = DynamicOrder(SiteDAOs, filter);
            List<Site> Sites = await DynamicSelect(SiteDAOs, filter);
            return Sites;
        }

        public async Task<Site> Get(long Id)
        {
            Site Site = await DataContext.Site.Where(x => x.Id == Id).Select(x => new Site()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Icon = x.Icon,
                Logo = x.Logo,
                IsDisplay = x.IsDisplay,
                RowId = x.RowId,
                ThemeId = x.ThemeId
            }).FirstOrDefaultAsync();

            if (Site == null)
                return null;

            return Site;
        }

        public async Task<bool> BulkMerge(List<Site> Sites)
        {
            List<SiteDAO> SiteDAOs = Sites.Select(x => new SiteDAO
            {
                Code = x.Code,
                Id = x.Id,
                Name = x.Name,
                ThemeId = x.ThemeId,
                Description = x.Description,
                RowId = x.RowId,
                Icon = x.Icon,
                Logo = x.Logo,
                IsDisplay = x.IsDisplay,
            }).ToList();
            await DataContext.BulkMergeAsync(SiteDAOs);
            return true;
        }
    }
}
