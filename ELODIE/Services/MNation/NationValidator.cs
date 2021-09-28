using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE;
using ELODIE.Repositories;

namespace ELODIE.Services.MNation
{
    public interface INationValidator : IServiceScoped
    {
        Task<bool> Create(Nation Nation);
        Task<bool> Update(Nation Nation);
        Task<bool> Delete(Nation Nation);
        Task<bool> BulkDelete(List<Nation> Nations);
        Task<bool> Import(List<Nation> Nations);
    }

    public class NationValidator : INationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            NationInUsed,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeOverLength,
            NameEmpty,
            CodeExisted,
            NameOverLength
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public NationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Nation Nation)
        {
            NationFilter NationFilter = new NationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Nation.Id },
                Selects = NationSelect.Id
            };

            int count = await UOW.NationRepository.Count(NationFilter);
            if (count == 0)
                Nation.AddError(nameof(NationValidator), nameof(Nation.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(Nation Nation)
        {
            if (string.IsNullOrEmpty(Nation.Code))
            {
                Nation.AddError(nameof(NationValidator), nameof(Nation.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Nation.Code;
                if (Nation.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Nation.Code))
                {
                    Nation.AddError(nameof(NationValidator), nameof(Nation.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else if (Nation.Code.Length > 50)
                {
                    Nation.AddError(nameof(NationValidator), nameof(Nation.Code), ErrorCode.CodeOverLength);
                }
                else
                {
                    NationFilter NationFilter = new NationFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Nation.Id },
                        Code = new StringFilter { Equal = Nation.Code },
                        Selects = NationSelect.Code
                    };

                    int count = await UOW.NationRepository.Count(NationFilter);
                    if (count != 0)
                        Nation.AddError(nameof(NationValidator), nameof(Nation.Code), ErrorCode.CodeExisted);
                }
            }

            return Nation.IsValidated;
        }

        private async Task<bool> ValidateName(Nation Nation)
        {
            if (string.IsNullOrEmpty(Nation.Name))
            {
                Nation.AddError(nameof(NationValidator), nameof(Nation.Name), ErrorCode.NameEmpty);
            }
            else if (Nation.Name.Length > 255)
            {
                Nation.AddError(nameof(NationValidator), nameof(Nation.Name), ErrorCode.NameOverLength);
            }
            return Nation.IsValidated;
        }

        public async Task<bool> Create(Nation Nation)
        {
            await ValidateCode(Nation);
            await ValidateName(Nation);
            return Nation.IsValidated;
        }

        public async Task<bool> Update(Nation Nation)
        {
            if (await ValidateId(Nation))
            {
                await ValidateCode(Nation);
                await ValidateName(Nation);
            }
            return Nation.IsValidated;
        }

        public async Task<bool> Delete(Nation Nation)
        {
            if (await ValidateId(Nation))
            {
                if (Nation.Used)
                    Nation.AddError(nameof(NationValidator), nameof(Nation.Id), ErrorCode.NationInUsed);
            }
            return Nation.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Nation> Nations)
        {
            foreach (Nation Nation in Nations)
            {
                await Delete(Nation);
            }
            return Nations.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Nation> Nations)
        {
            return true;
        }
    }
}
