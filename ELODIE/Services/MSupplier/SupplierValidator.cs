using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Repositories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MSupplier
{
    public interface ISupplierValidator : IServiceScoped
    {
        Task<bool> Create(Supplier Supplier);
        Task<bool> QuickCreate(Supplier Supplier);
        Task<bool> Update(Supplier Supplier);
        Task<bool> Delete(Supplier Supplier);
        Task<bool> BulkDelete(List<Supplier> Suppliers);
        Task<bool> BulkMerge(List<Supplier> Suppliers);
    }

    public class SupplierValidator : ISupplierValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameExisted,
            StatusNotExisted,
            ProvinceNotExisted,
            DistrictNotExisted,
            WardNotExisted,
            PersonInChargeNotExisted,
            SupplierInUsed,
            NameOverLength,
            NationNotExisted,
            CategoryEmpty,
            CategoryNotExisted,
            CategoryIsNotLeaf,
            ContactorEmpty,
            EmailEmpty,
            EmailOverLength,
            PhoneEmpty,
            PhoneOverLength,
            TaxCodeHasSpecialCharacter,
            TaxCodeEmpty,
            TaxCodeExisted,
            EmailWrongFormat
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public SupplierValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        private async Task<bool> ValidateId(Supplier Supplier)
        {
            SupplierFilter SupplierFilter = new SupplierFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Supplier.Id },
                Selects = SupplierSelect.Id
            };

            int count = await UOW.SupplierRepository.Count(SupplierFilter);
            if (count == 0)
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(Supplier Supplier)
        {
            if (string.IsNullOrWhiteSpace(Supplier.Code))
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Supplier.Code;
                if (Supplier.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Supplier.Code))
                {
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                SupplierFilter SupplierFilter = new SupplierFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Supplier.Id },
                    Code = new StringFilter { Equal = Supplier.Code },
                    Selects = SupplierSelect.Code
                };

                int count = await UOW.SupplierRepository.Count(SupplierFilter);
                if (count != 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Code), ErrorCode.CodeExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateTaxCode(Supplier Supplier)
        {
            if (string.IsNullOrWhiteSpace(Supplier.TaxCode))
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.TaxCode), ErrorCode.TaxCodeEmpty);
            }
            else
            {
                var TaxCode = Supplier.TaxCode;
                if (Supplier.TaxCode.Contains(" ") || !FilterExtension.ChangeToEnglishChar(TaxCode).Equals(Supplier.TaxCode))
                {
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.TaxCode), ErrorCode.TaxCodeHasSpecialCharacter);
                }

                SupplierFilter SupplierFilter = new SupplierFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Supplier.Id },
                    TaxCode = new StringFilter { Equal = Supplier.TaxCode },
                    Selects = SupplierSelect.TaxCode
                };

                int count = await UOW.SupplierRepository.Count(SupplierFilter);
                if (count != 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.TaxCode), ErrorCode.TaxCodeExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateName(Supplier Supplier)
        {
            if (string.IsNullOrWhiteSpace(Supplier.Name))
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Name), ErrorCode.NameEmpty);
            }
            else
            {
                if (Supplier.Name.Length > 255)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Name), ErrorCode.NameOverLength);
                SupplierFilter SupplierFilter = new SupplierFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Supplier.Id },
                    Name = new StringFilter { Equal = Supplier.Name },
                    Selects = SupplierSelect.Name
                };

                int count = await UOW.SupplierRepository.Count(SupplierFilter);
                if (count != 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Name), ErrorCode.NameExisted);
            }

            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateNation(Supplier Supplier)
        {
            if (Supplier.NationId.HasValue)
            {
                NationFilter NationFilter = new NationFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Supplier.NationId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = NationSelect.Id
                };

                int count = await UOW.NationRepository.Count(NationFilter);
                if (count == 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Nation), ErrorCode.NationNotExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateProvince(Supplier Supplier)
        {
            if (Supplier.ProvinceId.HasValue)
            {
                ProvinceFilter ProvinceFilter = new ProvinceFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Supplier.ProvinceId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ProvinceSelect.Id
                };

                int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
                if (count == 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Province), ErrorCode.ProvinceNotExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateDistrict(Supplier Supplier)
        {
            if (Supplier.DistrictId.HasValue)
            {
                DistrictFilter DistrictFilter = new DistrictFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Supplier.DistrictId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = DistrictSelect.Id
                };

                int count = await UOW.DistrictRepository.Count(DistrictFilter);
                if (count == 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.District), ErrorCode.DistrictNotExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateWard(Supplier Supplier)
        {
            if (Supplier.WardId.HasValue)
            {
                WardFilter WardFilter = new WardFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Supplier.WardId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = WardSelect.Id
                };

                int count = await UOW.WardRepository.Count(WardFilter);
                if (count == 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Ward), ErrorCode.WardNotExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidatePersonInCharge(Supplier Supplier)
        {
            if (Supplier.PersonInChargeId.HasValue)
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Supplier.PersonInChargeId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = AppUserSelect.Id
                };
                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count == 0)
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.PersonInCharge), ErrorCode.PersonInChargeNotExisted);
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateStatus(Supplier Supplier)
        {
            if (StatusEnum.ACTIVE.Id != Supplier.StatusId && StatusEnum.INACTIVE.Id != Supplier.StatusId)
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.StatusId), ErrorCode.StatusNotExisted);
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateSuppilerInUsed(Supplier Supplier)
        {
            Supplier oldData = await UOW.SupplierRepository.Get(Supplier.Id);
            if (Supplier.Used)
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Id), ErrorCode.SupplierInUsed);
            }

            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateCategoryMapping(Supplier Supplier)
        {
            if (Supplier.SupplierCategoryMappings == null)
            {
                return true;
            }

            List<long> CategoryIds = Supplier.SupplierCategoryMappings.Select(x => x.CategoryId).ToList();
            List<Category> Categories = await UOW.CategoryRepository.List(CategoryIds);
            foreach (SupplierCategoryMapping SupplierCategoryMapping in Supplier.SupplierCategoryMappings)
            {
                if (SupplierCategoryMapping.CategoryId == 0)
                {
                    SupplierCategoryMapping.AddError(nameof(SupplierValidator), nameof(SupplierCategoryMapping.CategoryId), ErrorCode.CategoryEmpty);
                }
                else
                {
                    Category Category = Categories.FirstOrDefault(x => x.Id == SupplierCategoryMapping.CategoryId);
                    if (Category == null)
                    {
                        SupplierCategoryMapping.AddError(nameof(SupplierValidator), nameof(SupplierCategoryMapping.CategoryId), ErrorCode.CategoryNotExisted);
                    }
                    else if (Category.HasChildren)
                    {
                        SupplierCategoryMapping.AddError(nameof(SupplierValidator), nameof(SupplierCategoryMapping.CategoryId), ErrorCode.CategoryIsNotLeaf);
                    }
                }
            }
            return Supplier.IsValidated;
        }

        private async Task<bool> ValidateContactors(Supplier Supplier)
        {
            if(Supplier.SupplierContactors?.Any() != true)
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.SupplierContactors), ErrorCode.ContactorEmpty);
            }
            else
            {
                foreach(SupplierContactor SupplierContactor in Supplier.SupplierContactors)
                {
                    if (string.IsNullOrEmpty(SupplierContactor.Name))
                    {
                        SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Name), ErrorCode.NameEmpty);
                    }
                    else
                    {
                        if(SupplierContactor.Name.Count() > 255)
                        {
                            SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Name), ErrorCode.NameOverLength);
                        }
                    }

                    if (string.IsNullOrEmpty(SupplierContactor.Email))
                    {
                        SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Email), ErrorCode.EmailEmpty);
                    }
                    else
                    {
                        if (SupplierContactor.Email.Count() > 255)
                        {
                            SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Email), ErrorCode.EmailOverLength);
                        }
                        else
                        {
                            if(!new EmailAddressAttribute().IsValid(SupplierContactor.Email))
                            {
                                SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Email), ErrorCode.EmailWrongFormat);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(SupplierContactor.Phone))
                    {
                        SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Phone), ErrorCode.PhoneEmpty);
                    }
                    else
                    {
                        if (SupplierContactor.Phone.Count() > 50)
                        {
                            SupplierContactor.AddError(nameof(SupplierValidator), nameof(SupplierContactor.Phone), ErrorCode.PhoneOverLength);
                        }
                    }
                }
            }

            return true;
        }

        public async Task<bool> Create(Supplier Supplier)
        {
            await ValidateCode(Supplier);
            await ValidateName(Supplier);
            await ValidateTaxCode(Supplier);
            await ValidateNation(Supplier);
            await ValidateProvince(Supplier);
            await ValidateDistrict(Supplier);
            await ValidatePersonInCharge(Supplier);
            await ValidateStatus(Supplier);
            await ValidateWard(Supplier);
            await ValidateCategoryMapping(Supplier);
            await ValidateContactors(Supplier);
            return Supplier.IsValidated;
        }

        public async Task<bool> QuickCreate(Supplier Supplier)
        {
            await ValidateCode(Supplier);
            await ValidateName(Supplier);
            await ValidateTaxCode(Supplier);
            await ValidateNation(Supplier);
            await ValidateProvince(Supplier);
            await ValidateDistrict(Supplier);
            await ValidatePersonInCharge(Supplier);
            await ValidateStatus(Supplier);
            await ValidateWard(Supplier);
            await ValidateCategoryMapping(Supplier);
            //await ValidateContactors(Supplier);
            return Supplier.IsValidated;
        }

        public async Task<bool> Update(Supplier Supplier)
        {
            if (await ValidateId(Supplier))
            {
                await ValidateCode(Supplier);
                await ValidateName(Supplier);
                await ValidateTaxCode(Supplier);
                await ValidateNation(Supplier);
                await ValidateProvince(Supplier);
                await ValidateDistrict(Supplier);
                await ValidatePersonInCharge(Supplier);
                await ValidateStatus(Supplier);
                await ValidateWard(Supplier);
                await ValidateCategoryMapping(Supplier);
                await ValidateContactors(Supplier);
            }
            return Supplier.IsValidated;
        }

        public async Task<bool> Delete(Supplier Supplier)
        {
            if (await ValidateId(Supplier))
            {
                await ValidateSuppilerInUsed(Supplier);
            }
            return Supplier.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Supplier> Suppliers)
        {
            SupplierFilter SupplierFilter = new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = Suppliers.Select(a => a.Id).ToList() },
                Selects = SupplierSelect.Id
            };

            var listInDB = await UOW.SupplierRepository.List(SupplierFilter);
            var listExcept = Suppliers.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var Supplier in listExcept)
            {
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> BulkMerge(List<Supplier> Suppliers)
        {
            var listCodeInDB = (await UOW.SupplierRepository.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.Code
            })).Select(e => e.Code);
            var listNameInDB = (await UOW.SupplierRepository.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.Name
            })).Select(e => e.Name);

            foreach (var Supplier in Suppliers)
            {
                if (listCodeInDB.Contains(Supplier.Code))
                {
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Code), ErrorCode.CodeExisted);
                    return false;
                }

                if (listNameInDB.Contains(Supplier.Name))
                {
                    Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Name), ErrorCode.NameExisted);
                    return false;
                }
                await ValidatePersonInCharge(Supplier);
                await ValidateNation(Supplier);
                await ValidateProvince(Supplier);
                await ValidateDistrict(Supplier);
                await ValidateWard(Supplier);
                await ValidateStatus(Supplier);
            }
            return Suppliers.Any(s => !s.IsValidated) ? false : true;
        }
    }
}
