using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MDistrict
{
    public interface IDistrictValidator : IServiceScoped
    {
        Task<bool> Create(District District);
        Task<bool> Update(District District);
        Task<bool> Delete(District District);
        Task<bool> BulkDelete(List<District> Districts);
        Task<bool> Import(List<District> Districts);
    }

    public class DistrictValidator : IDistrictValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            DistrictInUsed,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeOverLength,
            NameEmpty,
            CodeExisted,
            NameOverLength
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DistrictValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(District District)
        {
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = District.Id },
                Selects = DistrictSelect.Id
            };

            int count = await UOW.DistrictRepository.Count(DistrictFilter);
            if (count == 0)
                District.AddError(nameof(DistrictValidator), nameof(District.Id), ErrorCode.IdNotExisted);
            return District.IsValidated;
        }
        private async Task<bool> ValidateCode(District District)
        {
            if (string.IsNullOrEmpty(District.Code))
            {
                District.AddError(nameof(DistrictValidator), nameof(District.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = District.Code;
                if (District.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(District.Code))
                {
                    District.AddError(nameof(DistrictValidator), nameof(District.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else if (District.Code.Length > 50)
                {
                    District.AddError(nameof(DistrictValidator), nameof(District.Code), ErrorCode.CodeOverLength);
                }
                else
                {
                    DistrictFilter DistrictFilter = new DistrictFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = District.Id },
                        Code = new StringFilter { Equal = District.Code },
                        Selects = DistrictSelect.Code
                    };

                    int count = await UOW.DistrictRepository.Count(DistrictFilter);
                    if (count != 0)
                        District.AddError(nameof(DistrictValidator), nameof(District.Code), ErrorCode.CodeExisted);
                }
            }

            return District.IsValidated;
        }

        private async Task<bool> ValidateName(District District)
        {
            if (string.IsNullOrEmpty(District.Name))
            {
                District.AddError(nameof(DistrictValidator), nameof(District.Name), ErrorCode.NameEmpty);
            }
            else if (District.Name.Length > 255)
            {
                District.AddError(nameof(DistrictValidator), nameof(District.Name), ErrorCode.NameOverLength);
            }
            return District.IsValidated;
        }

        public async Task<bool> Create(District District)
        {
            await ValidateCode(District);
            await ValidateName(District);
            return District.IsValidated;
        }

        public async Task<bool> Update(District District)
        {
            if (await ValidateId(District))
            {
                await ValidateCode(District);
                await ValidateName(District);
            }
            return District.IsValidated;
        }

        public async Task<bool> Delete(District District)
        {
            if (await ValidateId(District))
            {
                if(District.Used)
                    District.AddError(nameof(DistrictValidator), nameof(District.Id), ErrorCode.DistrictInUsed);
            }
            return District.IsValidated;
        }

        public async Task<bool> BulkDelete(List<District> Districts)
        {
            foreach (var District in Districts)
            {
                await Delete(District);
            }

            return Districts.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<District> Districts)
        {
            return true;
        }
    }
}
