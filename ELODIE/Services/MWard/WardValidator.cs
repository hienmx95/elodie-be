using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MWard
{
    public interface IWardValidator : IServiceScoped
    {
        Task<bool> Create(Ward Ward);
        Task<bool> Update(Ward Ward);
        Task<bool> Delete(Ward Ward);
        Task<bool> BulkDelete(List<Ward> Wards);
        Task<bool> Import(List<Ward> Wards);
    }

    public class WardValidator : IWardValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            WardInUsed,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeOverLength,
            NameEmpty,
            CodeExisted,
            NameOverLength
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WardValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Ward Ward)
        {
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Ward.Id },
                Selects = WardSelect.Id
            };

            int count = await UOW.WardRepository.Count(WardFilter);
            if (count == 0)
                Ward.AddError(nameof(WardValidator), nameof(Ward.Id), ErrorCode.IdNotExisted);
            return Ward.IsValidated;
        }
        private async Task<bool> ValidateCode(Ward Ward)
        {
            if (string.IsNullOrEmpty(Ward.Code))
            {
                Ward.AddError(nameof(WardValidator), nameof(Ward.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Ward.Code;
                if (Ward.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Ward.Code))
                {
                    Ward.AddError(nameof(WardValidator), nameof(Ward.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else if (Ward.Code.Length > 50)
                {
                    Ward.AddError(nameof(WardValidator), nameof(Ward.Code), ErrorCode.CodeOverLength);
                }
                else
                {
                    WardFilter WardFilter = new WardFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Ward.Id },
                        Code = new StringFilter { Equal = Ward.Code },
                        Selects = WardSelect.Code
                    };

                    int count = await UOW.WardRepository.Count(WardFilter);
                    if (count != 0)
                        Ward.AddError(nameof(WardValidator), nameof(Ward.Code), ErrorCode.CodeExisted);
                }
            }
            return Ward.IsValidated;
        }

        private async Task<bool> ValidateName(Ward Ward)
        {
            if (string.IsNullOrEmpty(Ward.Name))
            {
                Ward.AddError(nameof(WardValidator), nameof(Ward.Name), ErrorCode.NameEmpty);
            }
            else if (Ward.Name.Length > 255)
            {
                Ward.AddError(nameof(WardValidator), nameof(Ward.Name), ErrorCode.NameOverLength);
            }
            return Ward.IsValidated;
        }

        public async Task<bool> Create(Ward Ward)
        {
            await ValidateCode(Ward);
            await ValidateName(Ward);
            return Ward.IsValidated;
        }

        public async Task<bool> Update(Ward Ward)
        {
            if (await ValidateId(Ward))
            {
                await ValidateCode(Ward);
                await ValidateName(Ward);
            }
            return Ward.IsValidated;
        }


        public async Task<bool> Delete(Ward Ward)
        {
            if (await ValidateId(Ward))
            {
                if (Ward.Used)
                    Ward.AddError(nameof(WardValidator), nameof(Ward.Id), ErrorCode.WardInUsed);
            }
            return Ward.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Ward> Wards)
        {
            foreach (Ward Ward in Wards)
            {
                await Delete(Ward);
            }
            return Wards.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Ward> Wards)
        {
            return true;
        }
    }
}
