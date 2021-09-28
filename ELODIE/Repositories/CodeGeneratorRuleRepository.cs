using ELODIE.Common;
using ELODIE.Helpers;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Enums;

namespace ELODIE.Repositories
{
    public interface ICodeGeneratorRuleRepository
    {
        Task<int> Count(CodeGeneratorRuleFilter CodeGeneratorRuleFilter);
        Task<List<CodeGeneratorRule>> List(CodeGeneratorRuleFilter CodeGeneratorRuleFilter);
        Task<List<CodeGeneratorRule>> List(List<long> Ids);
        Task<CodeGeneratorRule> Get(long Id);
        Task<bool> Create(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Update(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Delete(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> BulkMerge(List<CodeGeneratorRule> CodeGeneratorRules);
        Task<bool> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules);
        Task<bool> Inactive(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Used(List<long> Id);
    }
    public class CodeGeneratorRuleRepository : ICodeGeneratorRuleRepository
    {
        private DataContext DataContext;
        public CodeGeneratorRuleRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CodeGeneratorRuleDAO> DynamicFilter(IQueryable<CodeGeneratorRuleDAO> query, CodeGeneratorRuleFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.EntityTypeId != null && filter.EntityTypeId.HasValue)
                query = query.Where(q => q.EntityTypeId, filter.EntityTypeId);
            if (filter.AutoNumberLenth != null && filter.AutoNumberLenth.HasValue)
                query = query.Where(q => q.AutoNumberLenth, filter.AutoNumberLenth);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            if (filter.EntityComponentId != null && filter.EntityComponentId.HasValue)
            {
                if (filter.EntityComponentId.Equal != null)
                {
                    query = from q in query
                            join ce in DataContext.CodeGeneratorRuleEntityComponentMapping on q.Id equals ce.CodeGeneratorRuleId
                            join en in DataContext.EntityComponent on ce.EntityComponentId equals en.Id
                            where en.Id.Equals(filter.EntityComponentId.Equal)
                            select q;
                }
                if (filter.EntityComponentId.NotEqual != null)
                {
                    query = from q in query
                            join ce in DataContext.CodeGeneratorRuleEntityComponentMapping on q.Id equals ce.CodeGeneratorRuleId
                            join en in DataContext.EntityComponent on ce.EntityComponentId equals en.Id
                            where en.Id != filter.EntityComponentId.Equal
                            select q;
                }
            }
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<CodeGeneratorRuleDAO> OrFilter(IQueryable<CodeGeneratorRuleDAO> query, CodeGeneratorRuleFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CodeGeneratorRuleDAO> initQuery = query.Where(q => false);
            foreach (CodeGeneratorRuleFilter CodeGeneratorRuleFilter in filter.OrFilter)
            {
                IQueryable<CodeGeneratorRuleDAO> queryable = query;
                if (CodeGeneratorRuleFilter.Id != null && CodeGeneratorRuleFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, CodeGeneratorRuleFilter.Id);
                if (CodeGeneratorRuleFilter.EntityTypeId != null && CodeGeneratorRuleFilter.EntityTypeId.HasValue)
                    queryable = queryable.Where(q => q.EntityTypeId, CodeGeneratorRuleFilter.EntityTypeId);
                if (CodeGeneratorRuleFilter.AutoNumberLenth != null && CodeGeneratorRuleFilter.AutoNumberLenth.HasValue)
                    queryable = queryable.Where(q => q.AutoNumberLenth, CodeGeneratorRuleFilter.AutoNumberLenth);
                if (CodeGeneratorRuleFilter.StatusId != null && CodeGeneratorRuleFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, CodeGeneratorRuleFilter.StatusId);
                if (CodeGeneratorRuleFilter.RowId != null && CodeGeneratorRuleFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, CodeGeneratorRuleFilter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<CodeGeneratorRuleDAO> DynamicOrder(IQueryable<CodeGeneratorRuleDAO> query, CodeGeneratorRuleFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CodeGeneratorRuleOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CodeGeneratorRuleOrder.EntityType:
                            query = query.OrderBy(q => q.EntityTypeId);
                            break;
                        case CodeGeneratorRuleOrder.AutoNumberLenth:
                            query = query.OrderBy(q => q.AutoNumberLenth);
                            break;
                        case CodeGeneratorRuleOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case CodeGeneratorRuleOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case CodeGeneratorRuleOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CodeGeneratorRuleOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CodeGeneratorRuleOrder.EntityType:
                            query = query.OrderByDescending(q => q.EntityTypeId);
                            break;
                        case CodeGeneratorRuleOrder.AutoNumberLenth:
                            query = query.OrderByDescending(q => q.AutoNumberLenth);
                            break;
                        case CodeGeneratorRuleOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case CodeGeneratorRuleOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case CodeGeneratorRuleOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CodeGeneratorRule>> DynamicSelect(IQueryable<CodeGeneratorRuleDAO> query, CodeGeneratorRuleFilter filter)
        {
            List<CodeGeneratorRule> CodeGeneratorRules = await query.Select(q => new CodeGeneratorRule()
            {
                Id = filter.Selects.Contains(CodeGeneratorRuleSelect.Id) ? q.Id : default(long),
                EntityTypeId = filter.Selects.Contains(CodeGeneratorRuleSelect.EntityType) ? q.EntityTypeId : default(long),
                AutoNumberLenth = filter.Selects.Contains(CodeGeneratorRuleSelect.AutoNumberLenth) ? q.AutoNumberLenth : default(long),
                StatusId = filter.Selects.Contains(CodeGeneratorRuleSelect.Status) ? q.StatusId : default(long),
                RowId = filter.Selects.Contains(CodeGeneratorRuleSelect.Row) ? q.RowId : default(Guid),
                Used = filter.Selects.Contains(CodeGeneratorRuleSelect.Used) ? q.Used : default(bool),
                EntityType = filter.Selects.Contains(CodeGeneratorRuleSelect.EntityType) && q.EntityType != null ? new EntityType
                {
                    Id = q.EntityType.Id,
                    Code = q.EntityType.Code,
                    Name = q.EntityType.Name,
                } : null,
                Status = filter.Selects.Contains(CodeGeneratorRuleSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();

            var Ids = CodeGeneratorRules.Select(x => x.Id).ToList();

            var CodeGeneratorRuleEntityComponentMappings = await DataContext.CodeGeneratorRuleEntityComponentMapping
                .Where(x => Ids.Contains(x.CodeGeneratorRuleId))
                .Select(x => new CodeGeneratorRuleEntityComponentMapping
                {
                    CodeGeneratorRuleId = x.CodeGeneratorRuleId,
                    EntityComponentId = x.EntityComponentId,
                    Sequence = x.Sequence,
                    Value = x.Value,
                    EntityComponent = x.EntityComponent == null ? null : new EntityComponent
                    {
                        Id = x.EntityComponent.Id,
                        Code = x.EntityComponent.Code,
                        Name = x.EntityComponent.Name,
                    }
                }).ToListAsync();
            foreach (var CodeGeneratorRule in CodeGeneratorRules)
            {
                CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRuleEntityComponentMappings.Where(x => x.CodeGeneratorRuleId == CodeGeneratorRule.Id).ToList();
            }
            return CodeGeneratorRules;
        }

        public async Task<int> Count(CodeGeneratorRuleFilter filter)
        {
            IQueryable<CodeGeneratorRuleDAO> CodeGeneratorRules = DataContext.CodeGeneratorRule.AsNoTracking();
            CodeGeneratorRules = DynamicFilter(CodeGeneratorRules, filter);
            return await CodeGeneratorRules.CountAsync();
        }

        public async Task<List<CodeGeneratorRule>> List(CodeGeneratorRuleFilter filter)
        {
            if (filter == null) return new List<CodeGeneratorRule>();
            IQueryable<CodeGeneratorRuleDAO> CodeGeneratorRuleDAOs = DataContext.CodeGeneratorRule.AsNoTracking();
            CodeGeneratorRuleDAOs = DynamicFilter(CodeGeneratorRuleDAOs, filter);
            CodeGeneratorRuleDAOs = DynamicOrder(CodeGeneratorRuleDAOs, filter);
            List<CodeGeneratorRule> CodeGeneratorRules = await DynamicSelect(CodeGeneratorRuleDAOs, filter);
            return CodeGeneratorRules;
        }

        public async Task<List<CodeGeneratorRule>> List(List<long> Ids)
        {
            List<CodeGeneratorRule> CodeGeneratorRules = await DataContext.CodeGeneratorRule.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new CodeGeneratorRule()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                EntityTypeId = x.EntityTypeId,
                AutoNumberLenth = x.AutoNumberLenth,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Used = x.Used,
                EntityType = x.EntityType == null ? null : new EntityType
                {
                    Id = x.EntityType.Id,
                    Code = x.EntityType.Code,
                    Name = x.EntityType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();

            List<CodeGeneratorRuleEntityComponentMapping> CodeGeneratorRuleEntityComponentMappings = await DataContext.CodeGeneratorRuleEntityComponentMapping.AsNoTracking()
            .Where(x => Ids.Contains(x.CodeGeneratorRuleId))
            .Select(x => new CodeGeneratorRuleEntityComponentMapping
            {
                CodeGeneratorRuleId = x.CodeGeneratorRuleId,
                EntityComponentId = x.EntityComponentId,
                Sequence = x.Sequence,
                Value = x.Value,
                EntityComponent = new EntityComponent
                {
                    Id = x.EntityComponent.Id,
                    Code = x.EntityComponent.Code,
                    Name = x.EntityComponent.Name,
                },
            }).ToListAsync();

            foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
            {
                CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRuleEntityComponentMappings.Where(x => x.CodeGeneratorRuleId == CodeGeneratorRule.Id).ToList();
            }

            return CodeGeneratorRules;
        }
        public async Task<CodeGeneratorRule> Get(long Id)
        {
            CodeGeneratorRule CodeGeneratorRule = await DataContext.CodeGeneratorRule.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new CodeGeneratorRule()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                EntityTypeId = x.EntityTypeId,
                AutoNumberLenth = x.AutoNumberLenth,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Used = x.Used,
                EntityType = x.EntityType == null ? null : new EntityType
                {
                    Id = x.EntityType.Id,
                    Code = x.EntityType.Code,
                    Name = x.EntityType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (CodeGeneratorRule == null)
                return null;

            CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = await DataContext.CodeGeneratorRuleEntityComponentMapping.AsNoTracking()
                .Where(x => x.CodeGeneratorRuleId == CodeGeneratorRule.Id)
                .Select(x => new CodeGeneratorRuleEntityComponentMapping
                {
                    CodeGeneratorRuleId = x.CodeGeneratorRuleId,
                    EntityComponentId = x.EntityComponentId,
                    Sequence = x.Sequence,
                    Value = x.Value,
                    EntityComponent = new EntityComponent
                    {
                        Id = x.EntityComponent.Id,
                        Code = x.EntityComponent.Code,
                        Name = x.EntityComponent.Name,
                    },
                }).ToListAsync();

            return CodeGeneratorRule;
        }
        public async Task<bool> Create(CodeGeneratorRule CodeGeneratorRule)
        {
            CodeGeneratorRuleDAO CodeGeneratorRuleDAO = new CodeGeneratorRuleDAO();
            CodeGeneratorRuleDAO.Id = CodeGeneratorRule.Id;
            CodeGeneratorRuleDAO.EntityTypeId = CodeGeneratorRule.EntityTypeId;
            CodeGeneratorRuleDAO.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
            CodeGeneratorRuleDAO.StatusId = CodeGeneratorRule.StatusId;
            CodeGeneratorRuleDAO.RowId = Guid.NewGuid();
            CodeGeneratorRuleDAO.Used = CodeGeneratorRule.Used;
            CodeGeneratorRuleDAO.CreatedAt = StaticParams.DateTimeNow;
            CodeGeneratorRuleDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.CodeGeneratorRule.Add(CodeGeneratorRuleDAO);
            await DataContext.SaveChangesAsync();
            CodeGeneratorRule.Id = CodeGeneratorRuleDAO.Id;
            await SaveReference(CodeGeneratorRule);
            return true;
        }

        public async Task<bool> Update(CodeGeneratorRule CodeGeneratorRule)
        {
            CodeGeneratorRuleDAO CodeGeneratorRuleDAO = DataContext.CodeGeneratorRule.Where(x => x.Id == CodeGeneratorRule.Id).FirstOrDefault();
            if (CodeGeneratorRuleDAO == null)
                return false;
            CodeGeneratorRuleDAO.Id = CodeGeneratorRule.Id;
            CodeGeneratorRuleDAO.EntityTypeId = CodeGeneratorRule.EntityTypeId;
            CodeGeneratorRuleDAO.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
            CodeGeneratorRuleDAO.StatusId = CodeGeneratorRule.StatusId;
            CodeGeneratorRuleDAO.RowId = CodeGeneratorRule.RowId;
            CodeGeneratorRuleDAO.Used = CodeGeneratorRule.Used;
            CodeGeneratorRuleDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(CodeGeneratorRule);
            return true;
        }

        public async Task<bool> Delete(CodeGeneratorRule CodeGeneratorRule)
        {
            await DataContext.CodeGeneratorRule.Where(x => x.Id == CodeGeneratorRule.Id).UpdateFromQueryAsync(x => new CodeGeneratorRuleDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            List<CodeGeneratorRuleDAO> CodeGeneratorRuleDAOs = new List<CodeGeneratorRuleDAO>();
            foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
            {
                CodeGeneratorRuleDAO CodeGeneratorRuleDAO = new CodeGeneratorRuleDAO();
                CodeGeneratorRuleDAO.Id = CodeGeneratorRule.Id;
                CodeGeneratorRuleDAO.EntityTypeId = CodeGeneratorRule.EntityTypeId;
                CodeGeneratorRuleDAO.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
                CodeGeneratorRuleDAO.StatusId = CodeGeneratorRule.StatusId;
                CodeGeneratorRuleDAO.RowId = CodeGeneratorRule.RowId;
                CodeGeneratorRuleDAO.Used = CodeGeneratorRule.Used;
                CodeGeneratorRuleDAO.CreatedAt = StaticParams.DateTimeNow;
                CodeGeneratorRuleDAO.UpdatedAt = StaticParams.DateTimeNow;
                CodeGeneratorRuleDAOs.Add(CodeGeneratorRuleDAO);
            }
            await DataContext.BulkMergeAsync(CodeGeneratorRuleDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            List<long> Ids = CodeGeneratorRules.Select(x => x.Id).ToList();
            await DataContext.CodeGeneratorRule
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CodeGeneratorRuleDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> Inactive(CodeGeneratorRule CodeGeneratorRule)
        {
            await DataContext.CodeGeneratorRule.Where(x => x.Id == CodeGeneratorRule.Id)
                .UpdateFromQueryAsync(x => new CodeGeneratorRuleDAO
                {
                    StatusId = StatusEnum.INACTIVE.Id,
                    UpdatedAt = StaticParams.DateTimeNow
                });
            return true;
        }

        private async Task SaveReference(CodeGeneratorRule CodeGeneratorRule)
        {
            await DataContext.CodeGeneratorRuleEntityComponentMapping
                .Where(x => x.CodeGeneratorRuleId == CodeGeneratorRule.Id)
                .DeleteFromQueryAsync();
            List<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMappingDAOs = new List<CodeGeneratorRuleEntityComponentMappingDAO>();
            if (CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings != null)
            {
                foreach (CodeGeneratorRuleEntityComponentMapping CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
                {
                    CodeGeneratorRuleEntityComponentMappingDAO CodeGeneratorRuleEntityComponentMappingDAO = new CodeGeneratorRuleEntityComponentMappingDAO();
                    CodeGeneratorRuleEntityComponentMappingDAO.CodeGeneratorRuleId = CodeGeneratorRule.Id;
                    CodeGeneratorRuleEntityComponentMappingDAO.EntityComponentId = CodeGeneratorRuleEntityComponentMapping.EntityComponentId;
                    CodeGeneratorRuleEntityComponentMappingDAO.Sequence = CodeGeneratorRuleEntityComponentMapping.Sequence;
                    CodeGeneratorRuleEntityComponentMappingDAO.Value = CodeGeneratorRuleEntityComponentMapping.Value;
                    CodeGeneratorRuleEntityComponentMappingDAOs.Add(CodeGeneratorRuleEntityComponentMappingDAO);
                }
                await DataContext.CodeGeneratorRuleEntityComponentMapping.BulkMergeAsync(CodeGeneratorRuleEntityComponentMappingDAOs);
            }
        }

        public async Task<bool> Used(List<long> Id)
        {
            await DataContext.Category.Where(x => Id.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CategoryDAO { Used = true });
            return true;
        }
    }
}
