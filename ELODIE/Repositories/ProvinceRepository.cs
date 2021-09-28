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
    public interface IProvinceRepository
    {
        Task<int> Count(ProvinceFilter ProvinceFilter);
        Task<List<Province>> List(ProvinceFilter ProvinceFilter);
        Task<List<Province>> List(List<long> Ids);
        Task<Province> Get(long Id);
        Task<bool> Create(Province Province);
        Task<bool> Update(Province Province);
        Task<bool> Delete(Province Province);
        Task<bool> BulkMerge(List<Province> Provinces);
        Task<bool> BulkDelete(List<Province> Provinces);
        Task<bool> Used(List<long> Ids);
    }
    public class ProvinceRepository : IProvinceRepository
    {
        private DataContext DataContext;
        public ProvinceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProvinceDAO> DynamicFilter(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
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
            if (filter.Priority != null)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProvinceDAO> OrFilter(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProvinceDAO> initQuery = query.Where(q => false);
            foreach (ProvinceFilter ProvinceFilter in filter.OrFilter)
            {
                IQueryable<ProvinceDAO> queryable = query;
                if (ProvinceFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ProvinceFilter.Id);
                if (ProvinceFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ProvinceFilter.Name);
                if (ProvinceFilter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, ProvinceFilter.Priority);
                if (ProvinceFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, ProvinceFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProvinceDAO> DynamicOrder(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProvinceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProvinceOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProvinceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProvinceOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case ProvinceOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProvinceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProvinceOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProvinceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProvinceOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case ProvinceOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Province>> DynamicSelect(IQueryable<ProvinceDAO> query, ProvinceFilter filter)
        {
            List<Province> Provinces = await query.Select(q => new Province()
            {
                Id = filter.Selects.Contains(ProvinceSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProvinceSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProvinceSelect.Name) ? q.Name : default(string),
                Priority = filter.Selects.Contains(ProvinceSelect.Priority) ? q.Priority : default(long?),
                StatusId = filter.Selects.Contains(ProvinceSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(ProvinceSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Districts = filter.Selects.Contains(ProvinceSelect.Districts) && q.Districts == null ? null :
                q.Districts.Select(p => new District
                {
                    Name = p.Name,
                    Code = p.Code,
                    Wards = p.Wards.Select(w => new Ward
                    {
                        Name = w.Name,
                        Code = w.Code,
                    }).ToList(),
                }).ToList(),
                RowId = q.RowId,
                Used = q.Used,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Provinces;
        }

        public async Task<int> Count(ProvinceFilter filter)
        {
            IQueryable<ProvinceDAO> Provinces = DataContext.Province;
            Provinces = DynamicFilter(Provinces, filter);
            return await Provinces.CountAsync();
        }

        public async Task<List<Province>> List(ProvinceFilter filter)
        {
            if (filter == null) return new List<Province>();
            IQueryable<ProvinceDAO> ProvinceDAOs = DataContext.Province;
            ProvinceDAOs = DynamicFilter(ProvinceDAOs, filter);
            ProvinceDAOs = DynamicOrder(ProvinceDAOs, filter);
            List<Province> Provinces = await DynamicSelect(ProvinceDAOs, filter);
            return Provinces;
        }

        public async Task<List<Province>> List(List<long> Ids)
        {
            List<Province> Provinces = await DataContext.Province.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Province()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Priority = x.Priority,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Used = x.Used,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();

            List<District> Districts = await DataContext.District.AsNoTracking()
                .Where(x => Ids.Contains(x.ProvinceId))
                .Select(x => new District
                {
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ProvinceId = x.ProvinceId,
                    Priority = x.Priority,
                    StatusId = x.StatusId,
                    RowId = x.RowId,
                    Used = x.Used,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                })
                .ToListAsync();

            List<Ward> Wards = await DataContext.Ward.AsNoTracking()
               .Where(x => Ids.Contains(x.District.ProvinceId))
               .Select(x => new Ward
               {
                   CreatedAt = x.CreatedAt,
                   UpdatedAt = x.UpdatedAt,
                   DeletedAt = x.DeletedAt,
                   Id = x.Id,
                   Code = x.Code,
                   Name = x.Name,
                   DistrictId = x.DistrictId,
                   Priority = x.Priority,
                   StatusId = x.StatusId,
                   RowId = x.RowId,
                   Used = x.Used,
                   Status = x.Status == null ? null : new Status
                   {
                       Id = x.Status.Id,
                       Code = x.Status.Code,
                       Name = x.Status.Name,
                   },
               })
               .ToListAsync();
            foreach(Province Province in Provinces)
            {
                Province.Districts = Districts.Where(x => x.ProvinceId == Province.Id).ToList();
                foreach(District District in Province.Districts)
                {
                    District.Wards = Wards.Where(x => x.DistrictId == District.Id).ToList();
                }    
            }    
            return Provinces;
        }

        public async Task<Province> Get(long Id)
        {
            Province Province = await DataContext.Province
                .Where(x => x.Id == Id).AsNoTracking()
                .Select(x => new Province()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Priority = x.Priority,
                    StatusId = x.StatusId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    RowId = x.RowId,
                    Used = x.Used,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Province == null)
                return null;

            return Province;
        }

        public async Task<bool> Create(Province Province)
        {
            Province.RowId = Guid.NewGuid();
            ProvinceDAO ProvinceDAO = new ProvinceDAO();
            ProvinceDAO.Id = Province.Id;
            ProvinceDAO.Code = Province.Code;
            ProvinceDAO.Name = Province.Name;
            ProvinceDAO.Priority = Province.Priority;
            ProvinceDAO.StatusId = Province.StatusId;
            ProvinceDAO.CreatedAt = StaticParams.DateTimeNow;
            ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
            ProvinceDAO.Used = false;
            ProvinceDAO.RowId = Province.RowId;
            DataContext.Province.Add(ProvinceDAO);
            await DataContext.SaveChangesAsync();
            Province.Id = ProvinceDAO.Id;
            await SaveReference(Province);
            return true;
        }

        public async Task<bool> Update(Province Province)
        {
            ProvinceDAO ProvinceDAO = DataContext.Province.Where(x => x.Id == Province.Id).FirstOrDefault();
            if (ProvinceDAO == null)
                return false;
            ProvinceDAO.Id = Province.Id;
            ProvinceDAO.Code = Province.Code;
            ProvinceDAO.Name = Province.Name;
            ProvinceDAO.Priority = Province.Priority;
            ProvinceDAO.StatusId = Province.StatusId;
            ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            Province.RowId = ProvinceDAO.RowId;
            await SaveReference(Province);
            return true;
        }

        public async Task<bool> Delete(Province Province)
        {
            DateTime Now = StaticParams.DateTimeNow;
            Province.DeletedAt = Now;
            await DataContext.Province.Where(x => x.Id == Province.Id).UpdateFromQueryAsync(x => new ProvinceDAO { DeletedAt = Now });
            Province.RowId = DataContext.Province.Where(x => x.Id == Province.Id).Select(p => p.RowId).FirstOrDefault();
            return true;
        }

        public async Task<bool> BulkMerge(List<Province> Provinces)
        {
            List<ProvinceDAO> ProvinceDAOs = new List<ProvinceDAO>();
            foreach (Province Province in Provinces)
            {
                Province.RowId = Guid.NewGuid();
                ProvinceDAO ProvinceDAO = new ProvinceDAO();
                ProvinceDAO.Id = Province.Id;
                ProvinceDAO.Code = Province.Code;
                ProvinceDAO.Name = Province.Name;
                ProvinceDAO.Priority = Province.Priority;
                ProvinceDAO.StatusId = Province.StatusId;
                ProvinceDAO.CreatedAt = StaticParams.DateTimeNow;
                ProvinceDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProvinceDAO.DeletedAt = null;
                ProvinceDAO.RowId = Province.RowId;
                ProvinceDAO.Used = true;
                ProvinceDAOs.Add(ProvinceDAO);
            }
            await DataContext.BulkMergeAsync(ProvinceDAOs);
            foreach (Province Province in Provinces)
            {
                ProvinceDAO ProvinceDAO = ProvinceDAOs.Where(d => d.RowId == Province.RowId).FirstOrDefault();
                Province.Id = ProvinceDAO.Id;
                Province.CreatedAt = ProvinceDAO.CreatedAt;
                Province.UpdatedAt = ProvinceDAO.UpdatedAt;
                Province.DeletedAt = ProvinceDAO.DeletedAt;
                Province.Used = true;
            }
            return true;
        }

        public async Task<bool> BulkDelete(List<Province> Provinces)
        {
            List<long> Ids = Provinces.Select(x => x.Id).ToList();
            await DataContext.Province
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProvinceDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Province Province)
        {
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Province.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProvinceDAO { Used = true });
            return true;
        }
    }
}
