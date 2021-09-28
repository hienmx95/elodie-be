using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE;
using ELODIE.Repositories;

namespace ELODIE.Services.MCategory
{
    public interface ICategoryValidator : IServiceScoped
    {
        Task<bool> Create(Category Category);
        Task<bool> Update(Category Category);
        Task<bool> Delete(Category Category);
        Task<bool> BulkDelete(List<Category> Categories);
        Task<bool> Import(List<Category> Categories);
    }

    public class CategoryValidator : ICategoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            ParentNotExisted,
            CategoryInUsed,
            LevelNotExisted,
            ParentCantBeTheSameAsSelf
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public CategoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Category Category)
        {
            CategoryFilter CategoryFilter = new CategoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Category.Id },
                Selects = CategorySelect.Id
            };

            int count = await UOW.CategoryRepository.Count(CategoryFilter);
            if (count == 0)
                Category.AddError(nameof(CategoryValidator), nameof(Category.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(Category Category)
        {
            if (string.IsNullOrWhiteSpace(Category.Code))
            {
                Category.AddError(nameof(CategoryValidator), nameof(Category.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Category.Code;
                if (Category.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Category.Code))
                {
                    Category.AddError(nameof(CategoryValidator), nameof(Category.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else
                {
                    CategoryFilter CategoryFilter = new CategoryFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Category.Id },
                        Code = new StringFilter { Equal = Category.Code },
                        Selects = CategorySelect.Code
                    };

                    int count = await UOW.CategoryRepository.Count(CategoryFilter);
                    if (count != 0)
                        Category.AddError(nameof(CategoryValidator), nameof(Category.Code), ErrorCode.CodeExisted);
                }
            }
            return Category.IsValidated;
        }

        private async Task<bool> ValidateName(Category Category)
        {
            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                Category.AddError(nameof(CategoryValidator), nameof(Category.Name), ErrorCode.NameEmpty);
            }
            else if (Category.Name.Length > 255)
            {
                Category.AddError(nameof(CategoryValidator), nameof(Category.Name), ErrorCode.NameOverLength);
            }
            return Category.IsValidated;
        }

        private async Task<bool> ValidateParent(Category Category)
        {
            if (Category.ParentId.HasValue)
            {
                if(Category.ParentId == Category.Id)
                {
                    Category.AddError(nameof(CategoryValidator), nameof(Category.ParentId), ErrorCode.ParentCantBeTheSameAsSelf);
                    return false;
                }

                CategoryFilter CategoryFilter = new CategoryFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Category.ParentId },
                    Selects = CategorySelect.Id
                };

                int count = await UOW.CategoryRepository.Count(CategoryFilter);
                if (count == 0)
                    Category.AddError(nameof(CategoryValidator), nameof(Category.ParentId), ErrorCode.ParentNotExisted);
                Category category = await UOW.CategoryRepository.Get(Category.ParentId.Value);
                if (category.Level >= 4)
                {
                    Category.AddError(nameof(CategoryValidator), nameof(Category.Level), ErrorCode.LevelNotExisted);
                }
            }

            return Category.IsValidated;
        }

        public async Task<bool> Create(Category Category)
        {
            await ValidateCode(Category);
            await ValidateName(Category);
            if (Category.ParentId.HasValue)
                await ValidateParent(Category);
            return Category.IsValidated;
        }

        public async Task<bool> Update(Category Category)
        {
            if (await ValidateId(Category))
            {
                await ValidateCode(Category);
                await ValidateName(Category);
                await ValidateParent(Category);
            }
            return Category.IsValidated;
        }

        public async Task<bool> Delete(Category Category)
        {
            if (await ValidateId(Category))
            {
                CategoryFilter CategoryFilter = new CategoryFilter
                {
                    ParentId = new IdFilter { Equal = Category.Id },
                };

                var count = await UOW.CategoryRepository.Count(CategoryFilter);
                if (count > 0)
                    Category.AddError(nameof(CategoryValidator), nameof(Category.Id), ErrorCode.CategoryInUsed);
            }
            return Category.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Category> Categorys)
        {
            foreach (Category Category in Categorys)
            {
                await Delete(Category);
            }
            return Categorys.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Category> Categorys)
        {
            return true;
        }
    }
}
