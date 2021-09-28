using ELODIE.Common;
using Microsoft.EntityFrameworkCore;
using ELODIE.Entities;
using ELODIE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IThemeRepository
    {
        Task<int> Count(ThemeFilter ThemeFilter);
        Task<List<Theme>> List(ThemeFilter ThemeFilter);
        Task<Theme> Get(long Id);
    }
    public class ThemeRepository : IThemeRepository
    {
        private DataContext DataContext;
        public ThemeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ThemeDAO> DynamicFilter(IQueryable<ThemeDAO> query, ThemeFilter filter)
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

        private IQueryable<ThemeDAO> OrFilter(IQueryable<ThemeDAO> query, ThemeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ThemeDAO> initQuery = query.Where(q => false);
            foreach (ThemeFilter ThemeFilter in filter.OrFilter)
            {
                IQueryable<ThemeDAO> queryable = query;
                if (ThemeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ThemeFilter.Id);
                if (ThemeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ThemeFilter.Code);
                if (ThemeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ThemeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ThemeDAO> DynamicOrder(IQueryable<ThemeDAO> query, ThemeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ThemeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ThemeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ThemeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ThemeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ThemeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ThemeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Theme>> DynamicSelect(IQueryable<ThemeDAO> query, ThemeFilter filter)
        {
            List<Theme> Themees = await query.Select(q => new Theme()
            {
                Id = filter.Selects.Contains(ThemeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ThemeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ThemeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Themees;
        }

        public async Task<int> Count(ThemeFilter filter)
        {
            IQueryable<ThemeDAO> Themes = DataContext.Theme;
            Themes = DynamicFilter(Themes, filter);
            return await Themes.CountAsync();
        }

        public async Task<List<Theme>> List(ThemeFilter filter)
        {
            if (filter == null) return new List<Theme>();
            IQueryable<ThemeDAO> ThemeDAOs = DataContext.Theme;
            ThemeDAOs = DynamicFilter(ThemeDAOs, filter);
            ThemeDAOs = DynamicOrder(ThemeDAOs, filter);
            List<Theme> Themees = await DynamicSelect(ThemeDAOs, filter);
            return Themees;
        }

        public async Task<Theme> Get(long Id)
        {
            Theme Theme = await DataContext.Theme.Where(x => x.Id == Id).Select(x => new Theme()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            return Theme;
        }
    }
}
