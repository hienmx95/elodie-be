using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE;
using ELODIE.Repositories;
using ELODIE.Enums;

namespace ELODIE.Services.MOrganization
{
    public interface IOrganizationValidator : IServiceScoped
    {
        Task<bool> Create(Organization Organization);
        Task<bool> Update(Organization Organization);
        Task<bool> Delete(Organization Organization);
        Task<bool> BulkDelete(List<Organization> Organizations);
        Task<bool> Import(List<Organization> Organizations);
    }

    public class OrganizationValidator : IOrganizationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            OrganizationInUsed,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            ParentNotExisted,
            AddressOverLength,
            EmailEmpty,
            EmailInvalid,
            StatusNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public OrganizationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Organization Organization)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Organization.Id },
                Selects = OrganizationSelect.Id
            };

            int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
            if (count == 0)
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Id), ErrorCode.IdNotExisted);
            return Organization.IsValidated;
        }

        #region Code
        private async Task<bool> ValidateCode(Organization Organization)
        {
            if (string.IsNullOrWhiteSpace(Organization.Code))
            {
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Organization.Code;
                if (Organization.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Organization.Code))
                {
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else
                {
                    OrganizationFilter OrganizationFilter = new OrganizationFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Organization.Id },
                        Code = new StringFilter { Equal = Organization.Code },
                        Selects = OrganizationSelect.Code
                    };

                    int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                    if (count != 0)
                        Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeExisted);
                }
            }

            return Organization.IsValidated;
        }
        #endregion

        #region Name
        private async Task<bool> ValidateName(Organization Organization)
        {
            if (string.IsNullOrEmpty(Organization.Name))
            {
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Name), ErrorCode.NameEmpty);
            }
            else if (Organization.Name.Length > 255)
            {
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Name), ErrorCode.NameOverLength);
            }
            return Organization.IsValidated;
        }
        #endregion

        #region Organization
        private async Task<bool> ValidateParentOrganization(Organization Organization)
        {
            if (Organization.ParentId.HasValue && Organization.ParentId != 0)
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Organization.ParentId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = OrganizationSelect.Id
                };

                int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Parent), ErrorCode.ParentNotExisted);
            }
            return Organization.IsValidated;
        }
        #endregion

        public async Task<bool> ValidateEmail(Organization Organization)
        {
            if (!string.IsNullOrWhiteSpace(Organization.Email) && !IsValidEmail(Organization.Email))
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Email), ErrorCode.EmailInvalid);

            return Organization.IsValidated;
        }

        private async Task<bool> ValidateAddress(Organization Organization)
        {
            if (!string.IsNullOrEmpty(Organization.Address) && Organization.Address.Length > 255)
            {
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Address), ErrorCode.AddressOverLength);
            }
            return Organization.IsValidated;
        }

        public async Task<bool> ValidateStatus(Organization Organization)
        {
            if (StatusEnum.ACTIVE.Id != Organization.StatusId && StatusEnum.INACTIVE.Id != Organization.StatusId)
                Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Status), ErrorCode.StatusNotExisted);
            return true;
        }

        public async Task<bool> Create(Organization Organization)
        {
            await ValidateCode(Organization);
            await ValidateName(Organization);
            await ValidateParentOrganization(Organization);
            await ValidateAddress(Organization);
            await ValidateEmail(Organization);
            await ValidateStatus(Organization);
            return Organization.IsValidated;
        }

        public async Task<bool> Update(Organization Organization)
        {
            if (await ValidateId(Organization))
            {
                await ValidateCode(Organization);
                await ValidateName(Organization);
                await ValidateParentOrganization(Organization);
                await ValidateAddress(Organization);
                await ValidateEmail(Organization);
                await ValidateStatus(Organization);
            }
            return Organization.IsValidated;
        }

        public async Task<bool> Delete(Organization Organization)
        {
            if (await ValidateId(Organization))
            {
                if (Organization.Used)
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Id), ErrorCode.OrganizationInUsed);
            }
            return Organization.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Organization> Organizations)
        {
            foreach (var Organization in Organizations)
            {
                await Delete(Organization);
            }

            return Organizations.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<Organization> Organizations)
        {
            var listCodeInDB = (await UOW.OrganizationRepository.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code
            })).Select(e => e.Code);

            foreach (var Organization in Organizations)
            {
                var Code = Organization.Code;
                if (Organization.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Organization.Code))
                {
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                if (listCodeInDB.Contains(Organization.Code))
                {
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeExisted);
                }
                else if (listCodeInDB.Contains(Organization.Code))
                {
                    Organization.AddError(nameof(OrganizationValidator), nameof(Organization.Code), ErrorCode.CodeExisted);
                }

                await ValidateName(Organization);
                await ValidateAddress(Organization);
                await ValidateEmail(Organization);
                await ValidateStatus(Organization);
            }

            return Organizations.Any(o => !o.IsValidated) ? false : true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
