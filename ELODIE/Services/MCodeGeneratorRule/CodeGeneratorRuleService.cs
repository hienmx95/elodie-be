using ELODIE.Common;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ELODIE.Repositories;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Handlers;
using ELODIE.Models;

namespace ELODIE.Services.MCodeGeneratorRule
{
    public interface ICodeGeneratorRuleService :  IServiceScoped
    {
        Task<int> Count(CodeGeneratorRuleFilter CodeGeneratorRuleFilter);
        Task<List<CodeGeneratorRule>> List(CodeGeneratorRuleFilter CodeGeneratorRuleFilter);
        Task<CodeGeneratorRule> Get(long Id);
        Task<CodeGeneratorRule> Create(CodeGeneratorRule CodeGeneratorRule);
        Task<CodeGeneratorRule> Update(CodeGeneratorRule CodeGeneratorRule);
        Task<CodeGeneratorRule> Delete(CodeGeneratorRule CodeGeneratorRule);
        Task<List<CodeGeneratorRule>> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules);
        Task<List<CodeGeneratorRule>> Import(List<CodeGeneratorRule> CodeGeneratorRules);
        Task<CodeGeneratorRuleFilter> ToFilter(CodeGeneratorRuleFilter CodeGeneratorRuleFilter);
    }

    public class CodeGeneratorRuleService : BaseService, ICodeGeneratorRuleService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICodeGeneratorRuleValidator CodeGeneratorRuleValidator;
        //private IRabbitManager RabbitManager;

        public CodeGeneratorRuleService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ICodeGeneratorRuleValidator CodeGeneratorRuleValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CodeGeneratorRuleValidator = CodeGeneratorRuleValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(CodeGeneratorRuleFilter CodeGeneratorRuleFilter)
        {
            try
            {
                int result = await UOW.CodeGeneratorRuleRepository.Count(CodeGeneratorRuleFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<CodeGeneratorRule>> List(CodeGeneratorRuleFilter CodeGeneratorRuleFilter)
        {
            try
            {
                List<CodeGeneratorRule> CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(CodeGeneratorRuleFilter);
                return CodeGeneratorRules;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<CodeGeneratorRule> Get(long Id)
        {
            CodeGeneratorRule CodeGeneratorRule = await UOW.CodeGeneratorRuleRepository.Get(Id);
            CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings.OrderBy(x => x.Sequence).ToList(); // sắp xếp lại mappings
            if (CodeGeneratorRule == null)
                return null;
            return CodeGeneratorRule;
        }

        public async Task<CodeGeneratorRule> Create(CodeGeneratorRule CodeGeneratorRule)
        {
            if (!await CodeGeneratorRuleValidator.Create(CodeGeneratorRule))
                return CodeGeneratorRule;

            try
            {
                await UOW.Begin();
                await UOW.CodeGeneratorRuleRepository.Create(CodeGeneratorRule);
                await UOW.Commit();
                CodeGeneratorRule = (await UOW.CodeGeneratorRuleRepository.List(new List<long> { CodeGeneratorRule.Id })).FirstOrDefault(); // get to sync
                Sync(new List<CodeGeneratorRule> { CodeGeneratorRule }); // sync
                await Logging.CreateAuditLog(CodeGeneratorRule, new { }, nameof(CodeGeneratorRuleService));
                return CodeGeneratorRule;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<CodeGeneratorRule> Update(CodeGeneratorRule CodeGeneratorRule)
        {
            if (!await CodeGeneratorRuleValidator.Update(CodeGeneratorRule))
                return CodeGeneratorRule;
            try
            {
                var oldData = await UOW.CodeGeneratorRuleRepository.Get(CodeGeneratorRule.Id);

                await UOW.Begin();
                if(oldData.Used)
                {
                    oldData.StatusId = CodeGeneratorRule.StatusId;
                    CodeGeneratorRule = oldData;
                } // nếu codeRule đã sử dụng thì chỉ update trạng thái
                await UOW.CodeGeneratorRuleRepository.Update(CodeGeneratorRule);
                await UOW.Commit();

                CodeGeneratorRule = (await UOW.CodeGeneratorRuleRepository.List(new List<long> { CodeGeneratorRule.Id })).FirstOrDefault(); // get to sync
                Sync(new List<CodeGeneratorRule> { CodeGeneratorRule }); // sync
                await Logging.CreateAuditLog(CodeGeneratorRule, oldData, nameof(CodeGeneratorRuleService));
                return CodeGeneratorRule;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<CodeGeneratorRule> Inactive(CodeGeneratorRule CodeGeneratorRule)
        {
            if (!await CodeGeneratorRuleValidator.Inactive(CodeGeneratorRule))
                return CodeGeneratorRule;
            try
            {
                var oldData = await UOW.CodeGeneratorRuleRepository.Get(CodeGeneratorRule.Id);
                await UOW.Begin();
                await UOW.CodeGeneratorRuleRepository.Inactive(CodeGeneratorRule); // inactive old codeGeneratorRule
                await UOW.Commit();
                CodeGeneratorRule = (await UOW.CodeGeneratorRuleRepository.List(new List<long> { CodeGeneratorRule.Id })).FirstOrDefault(); // get to sync
                Sync(new List<CodeGeneratorRule> { CodeGeneratorRule }); // sync
                await Logging.CreateAuditLog(CodeGeneratorRule, oldData, nameof(CodeGeneratorRuleService)); // write audit log
                return CodeGeneratorRule;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<CodeGeneratorRule> Delete(CodeGeneratorRule CodeGeneratorRule)
        {
            if (!await CodeGeneratorRuleValidator.Delete(CodeGeneratorRule))
                return CodeGeneratorRule;

            try
            {
                await UOW.Begin();
                await UOW.CodeGeneratorRuleRepository.Delete(CodeGeneratorRule);
                await UOW.Commit();
                CodeGeneratorRule = (await UOW.CodeGeneratorRuleRepository.List(new List<long> { CodeGeneratorRule.Id })).FirstOrDefault(); // get to sync
                Sync(new List<CodeGeneratorRule> { CodeGeneratorRule }); // sync
                await Logging.CreateAuditLog(new { }, CodeGeneratorRule, nameof(CodeGeneratorRuleService));
                return CodeGeneratorRule;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<CodeGeneratorRule>> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            if (!await CodeGeneratorRuleValidator.BulkDelete(CodeGeneratorRules))
                return CodeGeneratorRules;

            try
            {
                await UOW.Begin();
                await UOW.CodeGeneratorRuleRepository.BulkDelete(CodeGeneratorRules);
                await UOW.Commit();
                List<long> Ids = CodeGeneratorRules.Select(x => x.Id).ToList();
                CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(Ids);
                Sync(CodeGeneratorRules);
                await Logging.CreateAuditLog(new { }, CodeGeneratorRules, nameof(CodeGeneratorRuleService));
                return CodeGeneratorRules;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<CodeGeneratorRule>> Import(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            if (!await CodeGeneratorRuleValidator.Import(CodeGeneratorRules))
                return CodeGeneratorRules;
            try
            {
                await UOW.Begin();
                await UOW.CodeGeneratorRuleRepository.BulkMerge(CodeGeneratorRules);
                await UOW.Commit();

                await Logging.CreateAuditLog(CodeGeneratorRules, new { }, nameof(CodeGeneratorRuleService));
                return CodeGeneratorRules;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CodeGeneratorRuleService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<CodeGeneratorRuleFilter> ToFilter(CodeGeneratorRuleFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CodeGeneratorRuleFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CodeGeneratorRuleFilter subFilter = new CodeGeneratorRuleFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EntityTypeId))
                        subFilter.EntityTypeId = FilterBuilder.Merge(subFilter.EntityTypeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AutoNumberLenth))
                        subFilter.AutoNumberLenth = FilterBuilder.Merge(subFilter.AutoNumberLenth, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            //RabbitManager.PublishList(CodeGeneratorRules, RoutingKeyEnum.CodeGeneratorRuleSync);
        }
    }
}
