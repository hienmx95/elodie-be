using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE;
using ELODIE.Repositories;
using ELODIE.Enums;

namespace ELODIE.Services.MCodeGeneratorRule
{
    public interface ICodeGeneratorRuleValidator : IServiceScoped
    {
        Task<bool> Create(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Update(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Delete(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> Inactive(CodeGeneratorRule CodeGeneratorRule);
        Task<bool> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules);
        Task<bool> Import(List<CodeGeneratorRule> CodeGeneratorRules);
    }

    public class CodeGeneratorRuleValidator : ICodeGeneratorRuleValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            EntityTypeEmpty,
            EntityTypeNotExisted,
            EntityTypeInUsed,
            EntityComponentEmpty,
            EntityComponentNotExisted,
            StatusNotExisted,
            SequenceInvalid,
            SequenceDuplicated,
            CodeRuleExisted,
            CodeRuleInUsed,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public CodeGeneratorRuleValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(CodeGeneratorRule CodeGeneratorRule)
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CodeGeneratorRule.Id },
                Selects = CodeGeneratorRuleSelect.Id
            };

            int count = await UOW.CodeGeneratorRuleRepository.Count(CodeGeneratorRuleFilter);
            if (count == 0)
                CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateEntityTypeId(CodeGeneratorRule CodeGeneratorRule)
        {
            var EntityTypeIds = EntityTypeEnum.EntityTypeEnumList.Select(x => x.Id).ToList();
            if (!EntityTypeIds.Contains(CodeGeneratorRule.EntityTypeId))
            {
                CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.EntityType), ErrorCode.EntityTypeEmpty);
            }

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                EntityTypeId = new IdFilter { Equal = CodeGeneratorRule.EntityTypeId },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Id = new IdFilter { NotEqual = CodeGeneratorRule.Id }
            };
            var count = await UOW.CodeGeneratorRuleRepository.Count(CodeGeneratorRuleFilter);
            if (count > 0 && CodeGeneratorRule.StatusId == StatusEnum.ACTIVE.Id)
            {
                CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.EntityType), ErrorCode.EntityTypeInUsed);
            }

            return CodeGeneratorRule.IsValidated;
        }

        private async Task<bool> ValidateStatusId(CodeGeneratorRule CodeGeneratorRule)
        {
            if (StatusEnum.ACTIVE.Id != CodeGeneratorRule.StatusId && StatusEnum.INACTIVE.Id != CodeGeneratorRule.StatusId)
                CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.Status), ErrorCode.StatusNotExisted);
            return CodeGeneratorRule.IsValidated;
        }

        private async Task<bool> ValidateCodeGeneratorRuleEntityComponentMappings(CodeGeneratorRule CodeGeneratorRule)
        {
            var counter = new Dictionary<long, long>();
            foreach (CodeGeneratorRuleEntityComponentMapping CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
            {
                var EntityComponentIds = EntityComponentEnum.EntityComponentEnumList.Select(x => x.Id).ToList();
                if (!EntityComponentIds.Contains(CodeGeneratorRuleEntityComponentMapping.EntityComponentId))
                {
                    CodeGeneratorRuleEntityComponentMapping.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRuleEntityComponentMapping.EntityComponent), ErrorCode.EntityComponentEmpty);
                } // check if any entityComponent empty

                if (CodeGeneratorRuleEntityComponentMapping.Sequence == 0)
                {
                    CodeGeneratorRuleEntityComponentMapping.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRuleEntityComponentMapping.Sequence), ErrorCode.SequenceInvalid);
                } // check if any sequence equal 0

                if (counter.ContainsKey(CodeGeneratorRuleEntityComponentMapping.Sequence))
                {
                    CodeGeneratorRuleEntityComponentMapping.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRuleEntityComponentMapping.Sequence), ErrorCode.SequenceDuplicated);
                    counter[CodeGeneratorRuleEntityComponentMapping.Sequence]++;
                    break;
                } // check if sequence is duplicated 
                else
                {
                    counter.Add(CodeGeneratorRuleEntityComponentMapping.Sequence, 1);
                }
            }

            if (CodeGeneratorRule.IsValidated)
            {
                CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
                {
                    Skip = 0,
                    Take = 1,
                    Selects = CodeGeneratorRuleSelect.Id,
                    Id = new IdFilter { NotEqual = CodeGeneratorRule.Id },
                    EntityTypeId = new IdFilter { Equal = EntityTypeEnum.PRODUCT.Id },
                    AutoNumberLenth = new LongFilter { Equal = CodeGeneratorRule.AutoNumberLenth }
                };
                List<CodeGeneratorRule> CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(CodeGeneratorRuleFilter);
                if (CodeGeneratorRules.Count > 0)
                {
                    var CodeGeneratorRuleIds = CodeGeneratorRules.Select(x => x.Id).ToList();
                    CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(CodeGeneratorRuleIds);
                    foreach (var old in CodeGeneratorRules)
                    {
                        if (!ScrambledEquals(old.CodeGeneratorRuleEntityComponentMappings, CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings))
                        {
                            CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.Id), ErrorCode.CodeRuleExisted);
                            break;
                        } // if two rules are absolute equal then add error 
                    }
                }

            }
            return CodeGeneratorRule.IsValidated;
        } // check if each mapping mising sequence 

        private bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public async Task<bool> Create(CodeGeneratorRule CodeGeneratorRule)
        {
            await ValidateEntityTypeId(CodeGeneratorRule);
            await ValidateStatusId(CodeGeneratorRule);
            return CodeGeneratorRule.IsValidated;
        }

        public async Task<bool> Update(CodeGeneratorRule CodeGeneratorRule)
        {
            if (await ValidateId(CodeGeneratorRule))
            {
                await ValidateEntityTypeId(CodeGeneratorRule);
                await ValidateStatusId(CodeGeneratorRule);

            }
            return CodeGeneratorRule.IsValidated;
        }

        public async Task<bool> Inactive(CodeGeneratorRule CodeGeneratorRule)
        {
            if (await ValidateId(CodeGeneratorRule))
            {
            }
            return CodeGeneratorRule.IsValidated;
        }

        public async Task<bool> Delete(CodeGeneratorRule CodeGeneratorRule)
        {
            if (await ValidateId(CodeGeneratorRule))
            {
                if (CodeGeneratorRule.Used)
                    CodeGeneratorRule.AddError(nameof(CodeGeneratorRuleValidator), nameof(CodeGeneratorRule.Id), ErrorCode.CodeRuleInUsed);
            }
            return CodeGeneratorRule.IsValidated;
        }

        public async Task<bool> BulkDelete(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
            {
                await Delete(CodeGeneratorRule);
            }
            return CodeGeneratorRules.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            return true;
        }
    }
}
