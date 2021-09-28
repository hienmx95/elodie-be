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
    public interface IDistrictRepository
    {
        Task<int> Count(DistrictFilter DistrictFilter);
        Task<List<District>> List(DistrictFilter DistrictFilter);
        Task<List<District>> List(List<long> Ids);
        Task<District> Get(long Id);
        Task<bool> Create(District District);
        Task<bool> Update(District District);
        Task<bool> Delete(District District);
        Task<bool> BulkMerge(List<District> Districts);
        Task<bool> BulkDelete(List<District> Districts);
        Task<bool> Used(List<long> Ids);
    }
    public class DistrictRepository : IDistrictRepository
    {
        private DataContext DataContext;
        public DistrictRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DistrictDAO> DynamicFilter(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Priority != null)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<DistrictDAO> OrFilter(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<DistrictDAO> initQuery = query.Where(q => false);
            foreach (DistrictFilter DistrictFilter in filter.OrFilter)
            {
                IQueryable<DistrictDAO> queryable = query;
                if (DistrictFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, DistrictFilter.Id);
                if (DistrictFilter.Code != null)
                    query = query.Where(q => q.Code, DistrictFilter.Code);
                if (DistrictFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, DistrictFilter.Name);
                if (DistrictFilter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, DistrictFilter.Priority);
                if (DistrictFilter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, DistrictFilter.ProvinceId);
                if (DistrictFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, DistrictFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<DistrictDAO> DynamicOrder(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case DistrictOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case DistrictOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case DistrictOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case DistrictOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case DistrictOrder.Province:
                            query = query.OrderBy(q => q.Province.Name);
                            break;
                        case DistrictOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case DistrictOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case DistrictOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case DistrictOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case DistrictOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case DistrictOrder.Province:
                            query = query.OrderByDescending(q => q.Province.Name);
                            break;
                        case DistrictOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<District>> DynamicSelect(IQueryable<DistrictDAO> query, DistrictFilter filter)
        {
            List<District> Districts = await query.Select(q => new District()
            {
                Id = filter.Selects.Contains(DistrictSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(DistrictSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(DistrictSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(DistrictSelect.Priority) ? q.Priority : default(long?),
                ProvinceId = filter.Selects.Contains(DistrictSelect.Province) ? q.ProvinceId : default(long),
                StatusId = filter.Selects.Contains(DistrictSelect.Status) ? q.StatusId : default(long),
                Province = filter.Selects.Contains(DistrictSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Status = filter.Selects.Contains(DistrictSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                RowId = q.RowId,
                Used = q.Used,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Districts;
        }

        public async Task<int> Count(DistrictFilter filter)
        {
            IQueryable<DistrictDAO> Districts = DataContext.District;
            Districts = DynamicFilter(Districts, filter);
            return await Districts.CountAsync();
        }

        public async Task<List<District>> List(DistrictFilter filter)
        {
            if (filter == null) return new List<District>();
            IQueryable<DistrictDAO> DistrictDAOs = DataContext.District;
            DistrictDAOs = DynamicFilter(DistrictDAOs, filter);
            DistrictDAOs = DynamicOrder(DistrictDAOs, filter);
            List<District> Districts = await DynamicSelect(DistrictDAOs, filter);
            return Districts;
        }

        public async Task<List<District>> List(List<long> Ids)
        {
            List<District> Districts = await DataContext.District.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new District()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                ProvinceId = x.ProvinceId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Used = x.Used,
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                    Used = x.Province.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();

            return Districts;
        }

        public async Task<District> Get(long Id)
        {
            District District = await DataContext.District.Where(x => x.Id == Id).AsNoTracking().Select(x => new District()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Priority = x.Priority,
                ProvinceId = x.ProvinceId,
                StatusId = x.StatusId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                RowId = x.RowId,
                Used = x.Used,
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (District == null)
                return null;

            return District;
        }
        public async Task<bool> Create(District District)
        {
            District.RowId = Guid.NewGuid();
            DistrictDAO DistrictDAO = new DistrictDAO();
            DistrictDAO.Id = District.Id;
            DistrictDAO.Code = District.Code;
            DistrictDAO.Name = District.Name;
            DistrictDAO.Priority = District.Priority;
            DistrictDAO.ProvinceId = District.ProvinceId;
            DistrictDAO.StatusId = District.StatusId;
            DistrictDAO.CreatedAt = StaticParams.DateTimeNow;
            DistrictDAO.UpdatedAt = StaticParams.DateTimeNow;
            DistrictDAO.Used = false;
            DistrictDAO.RowId = District.RowId;
            DataContext.District.Add(DistrictDAO);
            await DataContext.SaveChangesAsync();
            District.Id = DistrictDAO.Id;
            return true;
        }

        public async Task<bool> Update(District District)
        {
            DistrictDAO DistrictDAO = DataContext.District.Where(x => x.Id == District.Id).FirstOrDefault();
            if (DistrictDAO == null)
                return false;
            DistrictDAO.Id = District.Id;
            DistrictDAO.Code = District.Code;
            DistrictDAO.Name = District.Name;
            DistrictDAO.Priority = District.Priority;
            DistrictDAO.ProvinceId = District.ProvinceId;
            DistrictDAO.StatusId = District.StatusId;
            DistrictDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            District.RowId = DistrictDAO.RowId;
            return true;
        }

        public async Task<bool> Delete(District District)
        {
            DateTime Now = StaticParams.DateTimeNow;
            District.DeletedAt = Now;
            await DataContext.District.Where(x => x.Id == District.Id).UpdateFromQueryAsync(x => new DistrictDAO { DeletedAt = Now });
            District.RowId = DataContext.District.Where(x => x.Id == District.Id).Select(d => d.RowId).FirstOrDefault();
            return true;
        }

        public async Task<bool> BulkMerge(List<District> Districts)
        {
            List<DistrictDAO> DistrictDAOs = new List<DistrictDAO>();
            foreach (District District in Districts)
            {
                District.RowId = Guid.NewGuid();
                DistrictDAO DistrictDAO = new DistrictDAO();
                DistrictDAO.Id = District.Id;
                DistrictDAO.Code = District.Code;
                DistrictDAO.Name = District.Name;
                DistrictDAO.Priority = District.Priority;
                DistrictDAO.ProvinceId = District.ProvinceId;
                DistrictDAO.StatusId = District.StatusId;
                DistrictDAO.CreatedAt = StaticParams.DateTimeNow;
                DistrictDAO.UpdatedAt = StaticParams.DateTimeNow;
                DistrictDAO.DeletedAt = null;
                DistrictDAO.RowId = District.RowId;
                DistrictDAO.Used = true;
                DistrictDAOs.Add(DistrictDAO);
            }
            await DataContext.BulkMergeAsync(DistrictDAOs);
            foreach (District District in Districts)
            {
                DistrictDAO DistrictDAO = DistrictDAOs.Where(d => d.RowId == District.RowId).FirstOrDefault();
                District.Id = DistrictDAO.Id;
                District.CreatedAt = DistrictDAO.CreatedAt;
                District.UpdatedAt = DistrictDAO.UpdatedAt;
                District.DeletedAt = DistrictDAO.DeletedAt;
                District.Used = true;
            }
            return true;
        }

        public async Task<bool> BulkDelete(List<District> Districts)
        {
            DateTime Now = StaticParams.DateTimeNow;
            Districts.ForEach(d => d.DeletedAt = Now);
            List<long> Ids = Districts.Select(x => x.Id).ToList();
            await DataContext.District
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new DistrictDAO { DeletedAt = Now });
            return true;
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.District.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new DistrictDAO { Used = true });
            return true;
        }
    }
}
