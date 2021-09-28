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
    public interface IUnitOfMeasureGroupingRepository
    {
        Task<int> Count(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<List<UnitOfMeasureGrouping>> List(List<long> Ids);
        Task<UnitOfMeasureGrouping> Get(long Id);
        Task<bool> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> BulkMerge(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
        Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
        Task<bool> Used(List<long> Ids);
    }
    public class UnitOfMeasureGroupingRepository : IUnitOfMeasureGroupingRepository
    {
        private DataContext DataContext;
        public UnitOfMeasureGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UnitOfMeasureGroupingDAO> DynamicFilter(IQueryable<UnitOfMeasureGroupingDAO> query, UnitOfMeasureGroupingFilter filter)
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
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.UnitOfMeasureId != null)
                query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
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

        private IQueryable<UnitOfMeasureGroupingDAO> OrFilter(IQueryable<UnitOfMeasureGroupingDAO> query, UnitOfMeasureGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<UnitOfMeasureGroupingDAO> initQuery = query.Where(q => false);
            foreach (UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter in filter.OrFilter)
            {
                IQueryable<UnitOfMeasureGroupingDAO> queryable = query;
                if (UnitOfMeasureGroupingFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, UnitOfMeasureGroupingFilter.Id);
                if (UnitOfMeasureGroupingFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, UnitOfMeasureGroupingFilter.Code);
                if (UnitOfMeasureGroupingFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, UnitOfMeasureGroupingFilter.Name);
                if (UnitOfMeasureGroupingFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, UnitOfMeasureGroupingFilter.Description);
                if (UnitOfMeasureGroupingFilter.UnitOfMeasureId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureId, UnitOfMeasureGroupingFilter.UnitOfMeasureId);
                if (UnitOfMeasureGroupingFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, UnitOfMeasureGroupingFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<UnitOfMeasureGroupingDAO> DynamicOrder(IQueryable<UnitOfMeasureGroupingDAO> query, UnitOfMeasureGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case UnitOfMeasureGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case UnitOfMeasureGroupingOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case UnitOfMeasureGroupingOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingOrder.Status:
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
                        case UnitOfMeasureGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case UnitOfMeasureGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case UnitOfMeasureGroupingOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case UnitOfMeasureGroupingOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UnitOfMeasureGrouping>> DynamicSelect(IQueryable<UnitOfMeasureGroupingDAO> query, UnitOfMeasureGroupingFilter filter)
        {
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await query.Select(q => new UnitOfMeasureGrouping()
            {
                Id = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Description) ? q.Description : default(string),
                UnitOfMeasureId = filter.Selects.Contains(UnitOfMeasureGroupingSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                StatusId = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(UnitOfMeasureGroupingSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                UnitOfMeasure = filter.Selects.Contains(UnitOfMeasureGroupingSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                } : null,
                UnitOfMeasureGroupingContents = filter.Selects.Contains(UnitOfMeasureGroupingSelect.UnitOfMeasureGroupingContents) && q.UnitOfMeasureGroupingContents == null ? null :
                q.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasureGroupingContent
                {
                    Id = x.Id,
                    Factor = x.Factor,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Name = x.UnitOfMeasure.Name,
                        Code = x.UnitOfMeasure.Code,
                    }
                }).ToList(),
                Used = q.Used,
            }).ToListAsync();
            return UnitOfMeasureGroupings;
        }

        public async Task<int> Count(UnitOfMeasureGroupingFilter filter)
        {
            IQueryable<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings = DataContext.UnitOfMeasureGrouping;
            UnitOfMeasureGroupings = DynamicFilter(UnitOfMeasureGroupings, filter);
            return await UnitOfMeasureGroupings.CountAsync();
        }

        public async Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter filter)
        {
            if (filter == null) return new List<UnitOfMeasureGrouping>();
            IQueryable<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupingDAOs = DataContext.UnitOfMeasureGrouping.AsNoTracking();
            UnitOfMeasureGroupingDAOs = DynamicFilter(UnitOfMeasureGroupingDAOs, filter);
            UnitOfMeasureGroupingDAOs = DynamicOrder(UnitOfMeasureGroupingDAOs, filter);
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await DynamicSelect(UnitOfMeasureGroupingDAOs, filter);
            return UnitOfMeasureGroupings;
        }

        public async Task<List<UnitOfMeasureGrouping>> List(List<long> Ids)
        {
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await DataContext.UnitOfMeasureGrouping.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new UnitOfMeasureGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                UnitOfMeasureId = x.UnitOfMeasureId,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                    Used = x.UnitOfMeasure.Used,
                    RowId = x.UnitOfMeasure.RowId,
                },
            }).ToListAsync();

            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await DataContext.UnitOfMeasureGroupingContent.AsNoTracking()
                .Where(x => Ids.Contains(x.UnitOfMeasureGroupingId))
                .Select(x => new UnitOfMeasureGroupingContent
                {
                    Id = x.Id,
                    UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Factor = x.Factor,
                    RowId = x.RowId,
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Used = x.UnitOfMeasure.Used,
                        RowId = x.UnitOfMeasure.RowId,
                    },
                }).ToListAsync();
            foreach (UnitOfMeasureGrouping UnitOfMeasureGrouping in UnitOfMeasureGroupings)
            {
                UnitOfMeasureGrouping.UnitOfMeasureGroupingContents = UnitOfMeasureGroupingContents
                    .Where(x => x.UnitOfMeasureGroupingId == UnitOfMeasureGrouping.Id)
                    .ToList();
            }

            return UnitOfMeasureGroupings;
        }

        public async Task<UnitOfMeasureGrouping> Get(long Id)
        {
            UnitOfMeasureGrouping UnitOfMeasureGrouping = await DataContext.UnitOfMeasureGrouping.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new UnitOfMeasureGrouping()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    UnitOfMeasureId = x.UnitOfMeasureId,
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
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        RowId = x.UnitOfMeasure.RowId,
                        CreatedAt = x.UnitOfMeasure.CreatedAt,
                        UpdatedAt = x.UnitOfMeasure.UpdatedAt,
                        DeletedAt = x.UnitOfMeasure.DeletedAt
                    },
                }).FirstOrDefaultAsync();

            if (UnitOfMeasureGrouping == null)
                return null;
            UnitOfMeasureGrouping.UnitOfMeasureGroupingContents = await DataContext.UnitOfMeasureGroupingContent
                .Where(x => x.UnitOfMeasureGroupingId == UnitOfMeasureGrouping.Id)
                .Select(x => new UnitOfMeasureGroupingContent
                {
                    Id = x.Id,
                    UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Factor = x.Factor,
                    UnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        RowId = x.UnitOfMeasure.RowId,
                        CreatedAt = x.UnitOfMeasure.CreatedAt,
                        UpdatedAt = x.UnitOfMeasure.UpdatedAt,
                        DeletedAt = x.UnitOfMeasure.DeletedAt
                    }
                }).ToListAsync();

            return UnitOfMeasureGrouping;
        }
        public async Task<bool> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            UnitOfMeasureGroupingDAO UnitOfMeasureGroupingDAO = new UnitOfMeasureGroupingDAO();
            UnitOfMeasureGroupingDAO.Id = UnitOfMeasureGrouping.Id;
            UnitOfMeasureGroupingDAO.Code = UnitOfMeasureGrouping.Code;
            UnitOfMeasureGroupingDAO.Name = UnitOfMeasureGrouping.Name;
            UnitOfMeasureGroupingDAO.Description = UnitOfMeasureGrouping.Description;
            UnitOfMeasureGroupingDAO.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
            UnitOfMeasureGroupingDAO.StatusId = UnitOfMeasureGrouping.StatusId;
            UnitOfMeasureGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            UnitOfMeasureGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            UnitOfMeasureGroupingDAO.Used = false;
            UnitOfMeasureGroupingDAO.RowId = Guid.NewGuid();
            DataContext.UnitOfMeasureGrouping.Add(UnitOfMeasureGroupingDAO);
            await DataContext.SaveChangesAsync();
            UnitOfMeasureGrouping.Id = UnitOfMeasureGroupingDAO.Id;
            await SaveReference(UnitOfMeasureGrouping);
            return true;
        }

        public async Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            UnitOfMeasureGroupingDAO UnitOfMeasureGroupingDAO = DataContext.UnitOfMeasureGrouping
                .Where(x => x.Id == UnitOfMeasureGrouping.Id).FirstOrDefault();
            if (UnitOfMeasureGroupingDAO == null)
                return false;
            UnitOfMeasureGroupingDAO.Id = UnitOfMeasureGrouping.Id;
            UnitOfMeasureGroupingDAO.Code = UnitOfMeasureGrouping.Code;
            UnitOfMeasureGroupingDAO.Name = UnitOfMeasureGrouping.Name;
            UnitOfMeasureGroupingDAO.Description = UnitOfMeasureGrouping.Description;
            UnitOfMeasureGroupingDAO.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
            UnitOfMeasureGroupingDAO.StatusId = UnitOfMeasureGrouping.StatusId;
            UnitOfMeasureGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(UnitOfMeasureGrouping);
            return true;
        }

        public async Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            await DataContext.Product
               .Where(x => x.UnitOfMeasureGroupingId.HasValue && x.UnitOfMeasureGroupingId.Value == UnitOfMeasureGrouping.Id)
               .UpdateFromQueryAsync(x => new ProductDAO { UnitOfMeasureGroupingId = null });

            await DataContext.UnitOfMeasureGrouping.Where(x => x.Id == UnitOfMeasureGrouping.Id)
                .UpdateFromQueryAsync(x => new UnitOfMeasureGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            List<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupingDAOs = new List<UnitOfMeasureGroupingDAO>();
            foreach (UnitOfMeasureGrouping UnitOfMeasureGrouping in UnitOfMeasureGroupings)
            {
                UnitOfMeasureGroupingDAO UnitOfMeasureGroupingDAO = new UnitOfMeasureGroupingDAO();
                UnitOfMeasureGroupingDAO.Id = UnitOfMeasureGrouping.Id;
                UnitOfMeasureGroupingDAO.Code = UnitOfMeasureGrouping.Code;
                UnitOfMeasureGroupingDAO.Name = UnitOfMeasureGrouping.Name;
                UnitOfMeasureGroupingDAO.Description = UnitOfMeasureGrouping.Description;
                UnitOfMeasureGroupingDAO.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
                UnitOfMeasureGroupingDAO.StatusId = UnitOfMeasureGrouping.StatusId;
                UnitOfMeasureGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                UnitOfMeasureGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                UnitOfMeasureGroupingDAOs.Add(UnitOfMeasureGroupingDAO);
            }
            await DataContext.BulkMergeAsync(UnitOfMeasureGroupingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            List<long> Ids = UnitOfMeasureGroupings.Select(x => x.Id).ToList();

            await DataContext.Product
                .Where(x => x.UnitOfMeasureGroupingId.HasValue && Ids.Contains(x.UnitOfMeasureGroupingId.Value))
                .UpdateFromQueryAsync(x => new ProductDAO { UnitOfMeasureGroupingId = null });

            await DataContext.UnitOfMeasureGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new UnitOfMeasureGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            await DataContext.UnitOfMeasureGroupingContent
                .Where(x => x.UnitOfMeasureGroupingId == UnitOfMeasureGrouping.Id)
                .DeleteFromQueryAsync();
            List<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentDAOs = new List<UnitOfMeasureGroupingContentDAO>();
            if (UnitOfMeasureGrouping.UnitOfMeasureGroupingContents != null)
            {
                foreach (UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent in UnitOfMeasureGrouping.UnitOfMeasureGroupingContents)
                {
                    UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO = new UnitOfMeasureGroupingContentDAO();
                    UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId = UnitOfMeasureGrouping.Id;
                    UnitOfMeasureGroupingContentDAO.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
                    UnitOfMeasureGroupingContentDAO.Factor = UnitOfMeasureGroupingContent.Factor;
                    UnitOfMeasureGroupingContentDAO.RowId = Guid.NewGuid();
                    UnitOfMeasureGroupingContentDAOs.Add(UnitOfMeasureGroupingContentDAO);
                }
                await DataContext.UnitOfMeasureGroupingContent.BulkMergeAsync(UnitOfMeasureGroupingContentDAOs);
            }
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.UnitOfMeasureGrouping.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new UnitOfMeasureGroupingDAO { Used = true });
            return true;
        }
    }
}
