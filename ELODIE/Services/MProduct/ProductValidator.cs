using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MProduct
{
    public interface IProductValidator : IServiceScoped
    {
        Task<bool> Create(Product Product);
        Task<bool> Update(Product Product);
        Task<bool> Delete(Product Product);
        Task<bool> BulkDelete(List<Product> Products);
        Task<bool> BulkMergeNewProduct(List<Product> Products);
        Task<bool> Import(List<Product> Products);
    }

    public class ProductValidator : IProductValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeRuleNotExisted,
            CodeHasSpecialCharacter,
            CodeEmpty,
            NameEmpty,
            NameExisted,
            NameOverLength,
            ProductTypeNotExisted,
            ProductTypeEmpty,
            SupplierNotExisted,
            BrandNotExisted,
            TaxTypeNotExisted,
            TaxTypeEmpty,
            UnitOfMeasureNotExisted,
            UnitOfMeasureEmpty,
            UnitOfMeasureGroupingNotExisted,
            UnitOfMeasureGroupingInvalid,
            SalePriceInvalid,
            SalePriceEmpty,
            RetailPriceInvalid,
            StatusEmpty,
            ERPCodeHasSpecialCharacter,
            ScanCodeHasSpecialCharacter,
            VariationGroupingsEmpty,
            VariationsEmpty,
            ItemsEmpty,
            ProductInUsed,
            UsedVariationNotExisted,
            CategoryEmpty,
            CategoryNotExisted,
            CategoryInvalid
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        private async Task<bool> ValidateId(Product Product)
        {
            ProductFilter ProductFilter = new ProductFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.Id },
                Selects = ProductSelect.Id
            };

            int count = await UOW.ProductRepository.Count(ProductFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        private async Task<bool> ValidateName(Product Product)
        {
            var oldData = await UOW.ProductRepository.Get(Product.Id);
            if (oldData != null && oldData.Used)
            {
                if (oldData.Name != Product.Name)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Id), ErrorCode.ProductInUsed);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Product.Name))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameEmpty);
                }
                else if (Product.Name.Length > 3000)
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameOverLength);
                }
            }

            return Product.IsValidated;
        }
        private async Task<bool> ValidateCode(Product Product)
        {
            var oldData = await UOW.ProductRepository.Get(Product.Id);
            if (oldData != null && oldData.Used)
            {
                if (oldData.Code != Product.Code)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Id), ErrorCode.ProductInUsed);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Product.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeEmpty);
                }
                else
                {
                    var Code = Product.Code;
                    if (Product.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Product.Code))
                    {
                        Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeHasSpecialCharacter);
                    }

                    ProductFilter ProductFilter = new ProductFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Product.Id },
                        Code = new StringFilter { Equal = Product.Code },
                        Selects = ProductSelect.Code
                    };

                    int count = await UOW.ProductRepository.Count(ProductFilter);
                    if (count != 0)
                        Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeExisted);
                }
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateCategory(Product Product)
        {
            if (Product.CategoryId == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.Category), ErrorCode.CategoryEmpty);
            else
            {
                CategoryFilter CategoryFilter = new CategoryFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Product.CategoryId },
                    Selects = CategorySelect.Id
                };

                int count = await UOW.CategoryRepository.Count(CategoryFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Category), ErrorCode.CategoryNotExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateProductType(Product Product)
        {
            if (Product.ProductTypeId == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.ProductType), ErrorCode.ProductTypeEmpty);
            else
            {
                ProductTypeFilter ProductTypeFilter = new ProductTypeFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Product.ProductTypeId },
                    Selects = ProductTypeSelect.Id
                };

                int count = await UOW.ProductTypeRepository.Count(ProductTypeFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.ProductType), ErrorCode.ProductTypeNotExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateBrand(Product Product)
        {
            if (Product.BrandId.HasValue)
            {
                BrandFilter BrandFilter = new BrandFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Product.BrandId },
                    Selects = BrandSelect.Id
                };

                int count = await UOW.BrandRepository.Count(BrandFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Brand), ErrorCode.BrandNotExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateERPCode(Product Product)
        {
            if (!string.IsNullOrWhiteSpace(Product.ERPCode))
            {
                var ERPCode = Product.ERPCode;
                if (Product.ERPCode.Contains(" ") || !FilterExtension.ChangeToEnglishChar(ERPCode).Equals(Product.ERPCode))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ERPCode), ErrorCode.ERPCodeHasSpecialCharacter);
                }
            }
            return Product.IsValidated;
        }

        private async Task<bool> ValidateScanCode(Product Product)
        {
            if (!string.IsNullOrWhiteSpace(Product.ScanCode))
            {
                var ScanCode = Product.ScanCode;
                if (Product.ScanCode.Contains(" ") || !FilterExtension.ChangeToEnglishChar(ScanCode).Equals(Product.ScanCode))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ScanCode), ErrorCode.ScanCodeHasSpecialCharacter);
                }
            }
            return Product.IsValidated;
        }

        private async Task<bool> ValidateTaxType(Product Product)
        {
            if (Product.TaxTypeId == 0)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.TaxType), ErrorCode.TaxTypeEmpty);
            }
            else
            {
                TaxTypeFilter TaxTypeFilter = new TaxTypeFilter
                {
                    Skip = 0,
                    Take = 10,
                    Selects = TaxTypeSelect.Id,
                    Id = new IdFilter { Equal = Product.TaxTypeId }
                };

                int count = await UOW.TaxTypeRepository.Count(TaxTypeFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.TaxType), ErrorCode.TaxTypeNotExisted);
            }
            return Product.IsValidated;
        }

        private async Task<bool> ValidateUnitOfMeasure(Product Product)
        {
            if (Product.UnitOfMeasureId == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasure), ErrorCode.UnitOfMeasureEmpty);
            else
            {
                UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Product.UnitOfMeasureId },
                    Selects = UnitOfMeasureSelect.Id
                };

                int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateUnitOfMeasureGrouping(Product Product)
        {
            if (Product.UnitOfMeasureGroupingId.HasValue)
            {
                UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Product.UnitOfMeasureGroupingId.Value },
                    UnitOfMeasureId = new IdFilter { Equal = Product.UnitOfMeasureId },
                    Selects = UnitOfMeasureGroupingSelect.Id
                };

                int count = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
                if (count == 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureGrouping), ErrorCode.UnitOfMeasureGroupingNotExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateStatusId(Product Product)
        {
            if (StatusEnum.ACTIVE.Id != Product.StatusId && StatusEnum.INACTIVE.Id != Product.StatusId)
                Product.AddError(nameof(ProductValidator), nameof(Product.Status), ErrorCode.StatusEmpty);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateUsedVariation(Product Product)
        {
            if (UsedVariationEnum.USED.Id != Product.UsedVariationId && UsedVariationEnum.NOTUSED.Id != Product.UsedVariationId)
                Product.AddError(nameof(ProductValidator), nameof(Product.UsedVariation), ErrorCode.UsedVariationNotExisted);
            else if (UsedVariationEnum.USED.Id == Product.UsedVariationId)
            {
                if (Product.VariationGroupings == null)
                    Product.AddError(nameof(ProductValidator), nameof(Product.VariationGroupings), ErrorCode.VariationGroupingsEmpty);
                else
                {
                    foreach (var VariationGrouping in Product.VariationGroupings)
                    {
                        if (VariationGrouping.Variations == null || !VariationGrouping.Variations.Any())
                            VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Variations), ErrorCode.VariationsEmpty);
                        else
                        {
                            foreach (var Variation in VariationGrouping.Variations)
                            {
                                var Code = Variation.Code;
                                if (Variation.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Variation.Code))
                                {
                                    Variation.AddError(nameof(ProductValidator), nameof(Variation.Code), ErrorCode.CodeHasSpecialCharacter);
                                }
                            }
                        }
                    }
                }

                if (Product.Items == null || !Product.Items.Any())
                    Product.AddError(nameof(ProductValidator), nameof(Product.Items), ErrorCode.ItemsEmpty);
                if (Product.Items != null)
                {
                    foreach (var item in Product.Items)
                    {
                        if (item.SalePrice == 0)
                        {
                            item.AddError(nameof(ProductValidator), nameof(item.SalePrice), ErrorCode.SalePriceEmpty);
                        }
                    }
                }
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateItem(Product Product)
        {
            return Product.IsValidated;
            if (Product.Items != null)
            {
                foreach (var item in Product.Items)
                {
                    var Code = item.Code;
                    if (item.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(item.Code))
                    {
                        item.AddError(nameof(ProductValidator), nameof(item.Code), ErrorCode.CodeHasSpecialCharacter);
                        continue;
                    }

                    ItemFilter ItemFilter = new ItemFilter
                    {
                        Id = new IdFilter { NotEqual = item.Id },
                        Code = new StringFilter { Equal = item.Code },
                        ProductId = new IdFilter { Equal = Product.Id }
                    };

                    int count = await UOW.ItemRepository.Count(ItemFilter);
                    if (count > 0)
                    {
                        item.AddError(nameof(ProductValidator), nameof(Item.Code), ErrorCode.CodeExisted);
                    }

                    ItemFilter = new ItemFilter
                    {
                        Id = new IdFilter { NotEqual = item.Id },
                        Name = new StringFilter { Equal = item.Name },
                        ProductId = new IdFilter { Equal = Product.Id }
                    };

                    count = await UOW.ItemRepository.Count(ItemFilter);
                    if (count > 0)
                    {
                        item.AddError(nameof(ProductValidator), nameof(Item.Name), ErrorCode.NameExisted);
                    }
                }
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateSalePrice(Product Product)
        {
            if (Product.SalePrice.HasValue && Product.SalePrice < 0)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.SalePrice), ErrorCode.SalePriceInvalid);
            }

            if (Product.Items != null)
            {
                foreach (var item in Product.Items)
                {
                    if (item.SalePrice.HasValue && item.SalePrice < 0)
                    {
                        item.AddError(nameof(ProductValidator), nameof(item.SalePrice), ErrorCode.SalePriceInvalid);
                    }
                }
            }
            return Product.IsValidated;
        }

        private async Task<bool> ValidateRetailPrice(Product Product)
        {
            if (Product.RetailPrice.HasValue)
            {
                if (Product.RetailPrice < 0)
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.RetailPrice), ErrorCode.RetailPriceInvalid);
                }
            }


            if (Product.Items != null)
            {
                foreach (var item in Product.Items)
                {
                    if (item.RetailPrice.HasValue)
                    {
                        if (item.RetailPrice < 0)
                        {
                            item.AddError(nameof(ProductValidator), nameof(item.RetailPrice), ErrorCode.RetailPriceInvalid);
                        }
                    }

                }
            }
            return Product.IsValidated;
        }

        private async Task<bool> ValidateCodeGeneratorRule(Product Product)
        {
            var codeRuleCounter = 0;
            codeRuleCounter = await UOW.CodeGeneratorRuleRepository.Count(new CodeGeneratorRuleFilter
            {
                EntityTypeId = new IdFilter { Equal = EntityTypeEnum.PRODUCT.Id },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Skip = 0,
                Take = int.MaxValue,
            });
            if (codeRuleCounter == 0)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeRuleNotExisted);
            }
            return Product.IsValidated;
        } // check if at least a codeGeneratorRule for this product is existed

        private async Task<bool> CheckCodeGeneratorRule()
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                Skip = 0,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                EntityTypeId = new IdFilter { Equal = EntityTypeEnum.PRODUCT.Id },
            };
            if (await UOW.CodeGeneratorRuleRepository.Count(CodeGeneratorRuleFilter) > 0)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> ValidateVariations(Product Product)
        {
            if (UsedVariationEnum.USED.Id != Product.UsedVariationId && UsedVariationEnum.NOTUSED.Id != Product.UsedVariationId)
                Product.AddError(nameof(ProductValidator), nameof(Product.UsedVariation), ErrorCode.UsedVariationNotExisted);
            if (Product.VariationGroupings != null)
            {
                foreach (var VariationGrouping in Product.VariationGroupings)
                {
                    if (string.IsNullOrEmpty(VariationGrouping.Name))
                    {
                        VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Name), ErrorCode.NameEmpty);
                    }

                    //if (string.IsNullOrEmpty(VariationGrouping.Code))
                    //{
                    //    VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Code), ErrorCode.CodeEmpty);
                    //}
                    //else
                    //{
                    //    var vgCode = VariationGrouping.Code;
                    //    if (VariationGrouping.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(VariationGrouping.Code).Equals(vgCode))
                    //    {
                    //        VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Code), ErrorCode.CodeHasSpecialCharacter);
                    //    }
                    //}

                    if (VariationGrouping.Variations == null || !VariationGrouping.Variations.Any())
                        VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Variations), ErrorCode.VariationsEmpty);
                    else
                    {
                        foreach (var Variation in VariationGrouping.Variations)
                        {
                            var vgCode = Variation.Code;
                            if (Variation.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(vgCode).Equals(Variation.Code))
                            {
                                Variation.AddError(nameof(ProductValidator), nameof(Variation.Code), ErrorCode.CodeHasSpecialCharacter);
                            }
                        }
                    }
                }
            }
            return Product.IsValidated;
        }

        public async Task<bool> Create(Product Product)
        {
            var hasCodeGeneratorRule = await CheckCodeGeneratorRule();
            if (!hasCodeGeneratorRule)
            {
                await ValidateCode(Product);
            }
            await ValidateName(Product);
            await ValidateCategory(Product);
            await ValidateProductType(Product);
            await ValidateBrand(Product);
            await ValidateTaxType(Product);
            await ValidateUnitOfMeasure(Product);
            await ValidateERPCode(Product);
            await ValidateScanCode(Product);
            await ValidateUnitOfMeasureGrouping(Product);
            await ValidateStatusId(Product);
            await ValidateItem(Product);
            await ValidateUsedVariation(Product);
            await ValidateSalePrice(Product);
            await ValidateRetailPrice(Product);

            await ValidateVariations(Product);
            return Product.IsValidated;
        }

        public async Task<bool> BulkMergeNewProduct(List<Product> Products)
        {
            foreach (Product Product in Products)
            {
                await ValidateId(Product);
            }
            return Products.All(st => st.IsValidated);
        }

        public async Task<bool> Update(Product Product)
        {
            if (await ValidateId(Product))
            {
                if (Product.CodeGeneratorRuleId == null)
                {
                    await ValidateCode(Product);
                }
                await ValidateCode(Product);
                await ValidateName(Product);
                await ValidateCategory(Product);
                await ValidateProductType(Product);
                await ValidateBrand(Product);
                await ValidateTaxType(Product);
                await ValidateERPCode(Product);
                await ValidateScanCode(Product);
                await ValidateUnitOfMeasure(Product);
                await ValidateUnitOfMeasureGrouping(Product);
                await ValidateStatusId(Product);
                await ValidateItem(Product);
                await ValidateUsedVariation(Product);
                await ValidateSalePrice(Product);
                await ValidateRetailPrice(Product);

                await ValidateVariations(Product);
            }
            return Product.IsValidated;
        }

        public async Task<bool> Delete(Product Product)
        {
            if (await ValidateId(Product))
            {
                if (Product.Used)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Id), ErrorCode.ProductInUsed);
            }
            return Product.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Product> Products)
        {
            foreach (Product Product in Products)
            {
                await Delete(Product);
            }
            return Products.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Product> Products)
        {
            var listCodeInDB = (await UOW.ProductRepository.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Code
            })).Select(e => e.Code);
            var listCategoryCodeInDB = (await UOW.CategoryRepository.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.Code | CategorySelect.Path
            })).Select(e => e.Code);
            var listProductTypeCodeInDB = (await UOW.ProductTypeRepository.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.Code
            })).Select(e => e.Code);
            var listBrandCodeInDB = (await UOW.BrandRepository.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.Code
            })).Select(e => e.Code);
            var listTaxTypeCodeInDB = (await UOW.TaxTypeRepository.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.Code
            })).Select(e => e.Code);
            var listUsedVariationCodeInDB = (await UOW.UsedVariationRepository.List(new UsedVariationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UsedVariationSelect.Code
            })).Select(e => e.Code);
            var UOMs = await UOW.UnitOfMeasureRepository.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.Code | UnitOfMeasureSelect.Id
            });
            var listUOMCodeInDB = UOMs.Select(e => e.Code);
            var UOMGs = await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.Code | UnitOfMeasureGroupingSelect.Id | UnitOfMeasureGroupingSelect.UnitOfMeasure
            });
            var listUOMGroupingCodeInDB = UOMGs.Select(e => e.Code);

            foreach (var Product in Products)
            {
                var hasCodeGeneratorRule = await CheckCodeGeneratorRule();
                var Code = Product.Code;
                if (!hasCodeGeneratorRule)
                {
                    await ValidateCode(Product);
                } // nếu không có quy tắc sinh mã thì validate code nhập vào
                if (!listCategoryCodeInDB.Contains(Product.Category.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Category), ErrorCode.CategoryNotExisted);
                }
                if (!listProductTypeCodeInDB.Contains(Product.ProductType.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ProductType), ErrorCode.ProductTypeNotExisted);
                }
                if (Product.Brand != null)
                {
                    if (!listBrandCodeInDB.Contains(Product.Brand.Code))
                    {
                        Product.AddError(nameof(ProductValidator), nameof(Product.Brand), ErrorCode.BrandNotExisted);
                    }
                }
                if (!listTaxTypeCodeInDB.Contains(Product.TaxType.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.TaxType), ErrorCode.TaxTypeNotExisted);
                }
                if (!listUOMCodeInDB.Contains(Product.UnitOfMeasure.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }
                if (Product.UnitOfMeasureGrouping != null)
                {
                    if (!listUOMGroupingCodeInDB.Contains(Product.UnitOfMeasureGrouping.Code))
                    {
                        Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureGrouping), ErrorCode.UnitOfMeasureGroupingNotExisted);
                    }
                    var UOMG = UOMGs.Where(x => x.Code == Product.UnitOfMeasureGrouping.Code && x.UnitOfMeasureId == Product.UnitOfMeasureId).FirstOrDefault();
                    if (UOMG == null)
                    {
                        Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureGrouping), ErrorCode.UnitOfMeasureGroupingInvalid);
                    }
                }

                await ValidateName(Product);
                await ValidateERPCode(Product);
                await ValidateScanCode(Product);
                await ValidateSalePrice(Product);
                await ValidateRetailPrice(Product);
                if (UsedVariationEnum.USED.Id != Product.UsedVariationId && UsedVariationEnum.NOTUSED.Id != Product.UsedVariationId)
                    Product.AddError(nameof(ProductValidator), nameof(Product.UsedVariation), ErrorCode.UsedVariationNotExisted);
                if (Product.VariationGroupings != null)
                {
                    foreach (var VariationGrouping in Product.VariationGroupings)
                    {
                        if (string.IsNullOrEmpty(VariationGrouping.Name))
                        {
                            VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Name), ErrorCode.NameEmpty);
                        }

                        if (VariationGrouping.Variations == null || !VariationGrouping.Variations.Any())
                            VariationGrouping.AddError(nameof(ProductValidator), nameof(VariationGrouping.Variations), ErrorCode.VariationsEmpty);
                        else
                        {
                            foreach (var Variation in VariationGrouping.Variations)
                            {
                                var vgCode = Variation.Code;
                                if (Variation.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(vgCode).Equals(Variation.Code))
                                {
                                    Variation.AddError(nameof(ProductValidator), nameof(Variation.Code), ErrorCode.CodeHasSpecialCharacter);
                                }
                            }
                        }
                    }
                }
                await ValidateStatusId(Product);
            }
            return Products.Any(s => !s.IsValidated) ? false : true;
        }
    }
}
