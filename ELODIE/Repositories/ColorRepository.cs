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
    public interface IColorRepository
    {
        Task<int> Count(ColorFilter ColorFilter);
        Task<List<Color>> List(ColorFilter ColorFilter);
        Task<List<Color>> List(List<long> Ids);
        Task<Color> Get(long Id);
    }
    public class ColorRepository : IColorRepository
    {
        private DataContext DataContext;
        public ColorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ColorDAO> DynamicFilter(IQueryable<ColorDAO> query, ColorFilter filter)
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

        private IQueryable<ColorDAO> OrFilter(IQueryable<ColorDAO> query, ColorFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ColorDAO>
                initQuery = query.Where(q => false);
            foreach (ColorFilter ColorFilter in filter.OrFilter)
            {
                IQueryable<ColorDAO> queryable = query;
                if (ColorFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ColorFilter.Id);
                if (ColorFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ColorFilter.Code);
                if (ColorFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ColorFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ColorDAO> DynamicOrder(IQueryable<ColorDAO> query, ColorFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ColorOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ColorOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ColorOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ColorOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ColorOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ColorOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Color>> DynamicSelect(IQueryable<ColorDAO> query, ColorFilter filter)
        {
            List<Color> Colors = await query.Select(q => new Color()
            {
                Id = filter.Selects.Contains(ColorSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ColorSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ColorSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Colors;
        }

        public async Task<int> Count(ColorFilter filter)
        {
            IQueryable<ColorDAO> Colors = DataContext.Color.AsNoTracking();
            Colors = DynamicFilter(Colors, filter);
            return await Colors.CountAsync();
        }

        public async Task<List<Color>> List(ColorFilter filter)
        {
            if (filter == null) return new List<Color>();
            IQueryable<ColorDAO> ColorDAOs = DataContext.Color.AsNoTracking();
            ColorDAOs = DynamicFilter(ColorDAOs, filter);
            ColorDAOs = DynamicOrder(ColorDAOs, filter);
            List<Color> Colors = await DynamicSelect(ColorDAOs, filter);
            return Colors;
        }

        public async Task<Color> Get(long Id)
        {
            Color Color = await DataContext.Color.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Color()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (Color == null)
                return null;

            return Color;
        }

        public async Task<List<Color>> List(List<long> Ids)
        {
            List<Color> Colors = await DataContext.Color.AsNoTracking()
                .Where(x => Ids.Contains(x.Id))
                .Select(x => new Color()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).ToListAsync();
            return Colors;
        }

    }
}
