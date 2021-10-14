using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Repositories;
using ELODIE.Services.MImage;
using ELODIE.Handlers;
using ELODIE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Rpc.product;

namespace ELODIE.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<Product> Create(Product Product);
        Task<List<Product>> BulkInsertNewProduct(List<Product> Products);
        Task<Product> Update(Product Product);
        Task<Product> Delete(Product Product);
        Task<List<Product>> BulkDeleteNewProduct(List<Product> Products);
        Task<List<Product>> BulkDelete(List<Product> Products);
        Task<List<Product>> Import(List<Product> Products);
        ProductFilter ToFilter(ProductFilter ProductFilter);
        Task<Image> SaveImage(Image Image);
        Task<CodeGeneratorRule> GetCodeGeneratorRule();
        Task<RequestHistory<Product>> GetProductHistoryDetail(string BsonId);
    }

    public class ProductService : BaseService, IProductService
    {
        private static string HistoryCollection = MongoCollectionEnum.ProductHistory.Code;

        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductValidator ProductValidator;
        private IImageService ImageService;
        //private IRabbitManager RabbitManager;

        public ProductService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductValidator ProductValidator,
            IImageService ImageService
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductValidator = ProductValidator;
            this.ImageService = ImageService;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ProductFilter ProductFilter)
        {
            try
            {
                int result = await UOW.ProductRepository.Count(ProductFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Product>> List(ProductFilter ProductFilter)
        {
            try
            {
                List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
                List<long> ProductIds = Products.Select(p => p.Id).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    ProductId = new IdFilter { In = ProductIds },
                    StatusId = null,
                    Selects = ItemSelect.Id | ItemSelect.ProductId,
                    Skip = 0,
                    Take = int.MaxValue,
                };
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                foreach (Product Product in Products)
                {
                    Product.VariationCounter = Items.Where(i => i.ProductId == Product.Id).Count();
                }
                return Products;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Product> Get(long Id)
        {
            Product Product = await UOW.ProductRepository.Get(Id);
            if (Product == null)
                return null;
            if (Product.Items != null && Product.Items.Any())
            {
                Product.VariationCounter = Product.Items.Count;
            }

            //Product.ProductHistories = await GetHistories(Product);

            return Product;
        }

        public async Task<Product> Create(Product Product)
        {
            if (!await ProductValidator.Create(Product))
                return Product;

            try
            {
                await UOW.Begin();
                CodeGeneratorRule CodeGeneratorRule = await GetCodeGeneratorRule();
                if (CodeGeneratorRule != null)
                {
                    Product.Code = string.Empty; // tạo một empty code để insert tạm
                    await UOW.ProductRepository.Create(Product); // tạo sản phẩm đơn thuần
                    await CodeGenerator(new List<Product> { Product }, CodeGeneratorRule); // sinh mã cho product, đánh dấu đã dùng với codeRule
                    if (Product.UsedVariationId == UsedVariationEnum.NOTUSED.Id)
                    {
                        Product.Items = new List<Item>();
                        Product.Items.Add(new Item
                        {
                            Code = Product.Code,
                            ERPCode = Product.ERPCode,
                            Name = Product.Name,
                            ScanCode = Product.ScanCode,
                            RetailPrice = Product.RetailPrice,
                            SalePrice = Product.SalePrice,
                            ProductId = Product.Id,
                            StatusId = StatusEnum.ACTIVE.Id
                        });
                    } // sau khi sinh mã, nếu không dùng phiên bản thì update một item
                    if (Product.UsedVariationId == UsedVariationEnum.USED.Id && Product.Items != null)
                    {
                        foreach (Item Item in Product.Items)
                        {
                            Item.Code = $"{Product.Code}.{Item.Code}";
                        }
                    } // nếu có đung phiên bản và có items, thay nối thêm code cho item bằng code của product
                } // nếu có codeRule
                else
                {
                    await UOW.ProductRepository.Create(Product);
                    if (Product.UsedVariationId == UsedVariationEnum.NOTUSED.Id)
                    {
                        Product.Items = new List<Item>();
                        Product.Items.Add(new Item
                        {
                            Code = Product.Code,
                            ERPCode = Product.ERPCode,
                            Name = Product.Name,
                            ScanCode = Product.ScanCode,
                            RetailPrice = Product.RetailPrice,
                            SalePrice = Product.SalePrice,
                            ProductId = Product.Id,
                            StatusId = StatusEnum.ACTIVE.Id
                        });
                    }
                    if (Product.UsedVariationId == UsedVariationEnum.USED.Id && Product.Items != null)
                    {
                        foreach (Item Item in Product.Items)
                        {
                            Item.Code = $"{Product.Code}.{Item.Code}";
                        }
                    }
                } // nếu không có codeRule
                await UOW.ProductRepository.Update(Product); // update lại items sau khi thay đổi mã
                await UOW.Commit();
                Product = (await UOW.ProductRepository.List(new List<long> { Product.Id })).FirstOrDefault();
                Sync(new List<Product> { Product });

                //await CreateHistory(Product, "Tạo mới");

                await Logging.CreateAuditLog(Product, new { }, nameof(ProductService));
                return Product;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Product>> BulkInsertNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                Products.ForEach(x => x.IsNew = true);
                await UOW.Begin();
                await UOW.ProductRepository.BulkInsertNewProduct(Products);
                await UOW.Commit();
                List<UserNotification> UserNotifications = new List<UserNotification>();
                var RecipientIds = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { }
                })).Select(x => x.Id).ToList();
                foreach (var Product in Products)
                {
                    foreach (var Id in RecipientIds)
                    {
                        UserNotification UserNotification = new UserNotification
                        {
                            TitleWeb = $"Thông báo từ ELODIE",
                            ContentWeb = $"Sản phẩm {Product.Code} - {Product.Name} đã được đưa vào danh sách sản phẩm mới bởi {CurrentUser.DisplayName}.",
                            LinkWebsite = $"{ProductRoute.Master}/?id=*".Replace("*", Product.Id.ToString()),
                            LinkMobile = $"{ProductRoute.Mobile}".Replace("*", Product.Id.ToString()),
                            RecipientId = Id,
                            SenderId = CurrentContext.UserId,
                            Time = StaticParams.DateTimeNow,
                            Unread = false,
                            RowId = Guid.NewGuid(),
                        };
                        UserNotifications.Add(UserNotification);
                    }
                }

               // RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.UserNotificationSend);

                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Product>> BulkDeleteNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                Products.ForEach(x => x.IsNew = false);
                await UOW.Begin();
                await UOW.ProductRepository.BulkDeleteNewProduct(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Product> Update(Product Product)
        {
            if (!await ProductValidator.Update(Product))
                return Product;
            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);
                var oldProductCode = Product.Code;
                oldProductCode = oldData.Code;
                //if (Product.CodeGeneratorRuleId != null && Product.CodeGeneratorRuleId.HasValue)
                //{
                //    var codeRule = await UOW.CodeGeneratorRuleRepository.Get(Product.CodeGeneratorRuleId.Value);
                //    if (codeRule != null)
                //    {
                //        await CodeGenerator(new List<Product> { Product }, codeRule);
                //    }
                //} // nếu có codeRule thì áp dụng lại codeRule cũ
                //else
                //{
                //    oldProductCode = oldData.Code;
                //} // nếu không có codeRule thì lấy dữ liệu cũ để replace 
                Product.IsNew = oldData.IsNew;
                var Items = Product.Items;
                var OldItems = oldData.Items;
                var OldItemIds = OldItems.Select(x => x.Id).ToList();
                var ItemHistories = await UOW.ItemHistoryRepository.List(new ItemHistoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ItemHistorySelect.ALL,
                    ItemId = new IdFilter { In = OldItemIds }
                });
                foreach (var item in Items)
                {
                    item.ItemHistories = ItemHistories.Where(x => x.ItemId == item.Id).ToList();
                    var oldItem = OldItems.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (oldItem != null)
                    {
                        if (item.SalePrice.HasValue && item.SalePrice != oldItem.SalePrice)
                        {
                            ItemHistory ItemHistory = new ItemHistory
                            {
                                ItemId = item.Id,
                                ModifierId = CurrentContext.UserId,
                                Time = StaticParams.DateTimeNow,
                                OldPrice = oldItem.SalePrice.GetValueOrDefault(0),
                                NewPrice = item.SalePrice.Value,
                            };
                            if (item.ItemHistories == null || !item.ItemHistories.Any())
                            {
                                item.ItemHistories = new List<ItemHistory>();
                                item.ItemHistories.Add(ItemHistory);
                            }
                            else
                            {
                                item.ItemHistories.Add(ItemHistory);
                            }
                        }
                    }
                }
                if (oldData.UsedVariationId == Enums.UsedVariationEnum.NOTUSED.Id)
                {
                    if (Product.StatusId == StatusEnum.ACTIVE.Id)
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.ACTIVE.Id);
                    else
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.INACTIVE.Id);
                    if (!Product.Used)
                    {
                        foreach (var item in Product.Items)
                        {
                            item.Code = Product.Code;
                            item.ERPCode = Product.ERPCode;
                            item.Name = Product.Name;
                            item.ScanCode = Product.ScanCode;
                            item.RetailPrice = Product.RetailPrice;
                            item.SalePrice = Product.SalePrice;
                        }
                    }

                }
                else
                {
                    if (Product.StatusId == StatusEnum.INACTIVE.Id)
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.INACTIVE.Id);
                    if (!Product.Used)
                    {
                        foreach (Item Item in Product.Items)
                        {
                            Item.Code = Item.Code.Replace(oldProductCode, Product.Code);
                        } // thay product code cũ bằng product code mới
                    }
                } // nếu product dùng phiên bản
                await UOW.Begin();
                await UOW.ProductRepository.Update(Product);
                await UOW.Commit();

                Product = (await UOW.ProductRepository.List(new List<long> { Product.Id })).FirstOrDefault();
                Sync(new List<Product> { Product });

                //if (!oldData.Equals(Product))
                //{
                //    await CreateHistory(Product, "Cập nhật");
                //}

                await Logging.CreateAuditLog(Product, oldData, nameof(ProductService));
                return Product;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Product> Delete(Product Product)
        {
            if (!await ProductValidator.Delete(Product))
                return Product;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.Delete(Product);
                await UOW.Commit();

                Product = (await UOW.ProductRepository.List(new List<long> { Product.Id })).FirstOrDefault();
                Sync(new List<Product> { Product });
                await Logging.CreateAuditLog(new { }, Product, nameof(ProductService));
                return Product;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Product>> BulkDelete(List<Product> Products)
        {
            if (!await ProductValidator.BulkDelete(Products))
                return Products;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkDelete(Products);
                await UOW.Commit();

                List<long> Ids = Products.Select(x => x.Id).ToList();
                Products = await UOW.ProductRepository.List(Ids);
                Sync(Products);
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Product>> Import(List<Product> Products)
        {
            var ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();
            if (!await ProductValidator.Import(Products))
                return Products;
            try
            {
                await UOW.Begin();
                CodeGeneratorRule CodeGeneratorRule = await GetCodeGeneratorRule();
                if (CodeGeneratorRule != null)
                {
                    foreach (var Product in Products)
                    {
                        Product.Code = string.Empty;
                        foreach (var Item in Product.Items)
                        {
                            Item.Code = string.Empty;
                        }
                    } // gán một mã rỗng cho sản phẩm đẻ đảm bảo lưu có mã
                    await UOW.ProductRepository.BulkMerge(Products); // lưu trước sản phẩm

                    // rút products từ db ra để đảm bảo các object con có id
                    List<long> ProductIds = Products.Select(x => x.Id).ToList();
                    Products = await UOW.ProductRepository.List(ProductIds);

                    await CodeGenerator(Products, CodeGeneratorRule); // áp quy tắc sinh mã
                    await UOW.CodeGeneratorRuleRepository.Update(CodeGeneratorRule); // lưu trạng thái codeRule vào db

                    foreach (var Product in Products)
                    {
                        foreach (var Item in Product.Items)
                        {
                            Item.Code = $"{Product.Code}.{Item.Code}";
                        }
                    } // sinh mã mới cho từng Item
                    await UOW.ProductRepository.BulkMerge(Products); // update lại products
                } // nếu có quy tắc sinh mã
                else
                {
                    await UOW.ProductRepository.BulkMerge(Products); // nếu không có quy tắc sinh mã thì lưu sản phẩm
                }
                await UOW.Commit();

                List<long> Ids = Products.Select(x => x.Id).ToList();
                Products = await UOW.ProductRepository.List(Ids);
                Sync(Products);
                await Logging.CreateAuditLog(Products, new { }, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public ProductFilter ToFilter(ProductFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProductFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProductFilter subFilter = new ProductFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductTypeId))
                        subFilter.ProductTypeId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductGroupingId))
                        subFilter.ProductGroupingId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}.{fileInfo.Extension}";
            string thumbnailPath = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}.{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
        }

        private async Task CodeGenerator(List<Product> Products, CodeGeneratorRule CodeGeneratorRule)
        {
            CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings.OrderBy(x => x.Sequence).ToList();
            foreach (Product Product in Products)
            {
                string Code = "";
                foreach (var CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
                {
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.AUTO_NUMBER.Id && CodeGeneratorRule.AutoNumberLenth > 0)
                    {
                        // Product.Id + 10^n -> cắt lấy n số cuối
                        string AutoNumber = (Math.Pow(10, CodeGeneratorRule.AutoNumberLenth) + Product.Id).ToString();
                        AutoNumber = AutoNumber.Substring(AutoNumber.Length - (int)CodeGeneratorRule.AutoNumberLenth);
                        Code += $"{AutoNumber}.";
                    }
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.TEXT.Id)
                    {
                        Code += $"{CodeGeneratorRuleEntityComponentMapping.Value}.";
                    }
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.PRODUCT_PRODUCT_TYPE.Id)
                    {
                        Code += $"{Product.ProductType.Code}.";
                    }
                    if (CodeGeneratorRuleEntityComponentMapping.EntityComponentId == EntityComponentEnum.PRODUCT_CATEGORY.Id)
                    {
                        Code += $"{Product.Category.Code}.";
                    }
                }
                Code = Code.Remove(Code.Length - 1);
                Product.Code = Code;
                Product.CodeGeneratorRuleId = CodeGeneratorRule.Id;
            }
            CodeGeneratorRule.Used = true;
        }

        public async Task<CodeGeneratorRule> GetCodeGeneratorRule()
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter
            {
                Skip = 0,
                Take = 1,
                Selects = CodeGeneratorRuleSelect.Id,
                OrderBy = CodeGeneratorRuleOrder.CreatedAt,
                OrderType = OrderType.DESC,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                EntityTypeId = new IdFilter { Equal = EntityTypeEnum.PRODUCT.Id },
            };
            List<CodeGeneratorRule> CodeGeneratorRules = await UOW.CodeGeneratorRuleRepository.List(CodeGeneratorRuleFilter);
            if (CodeGeneratorRules.Count == 0)
                return null;
            else
            {
                CodeGeneratorRule CodeGeneratorRule = CodeGeneratorRules.FirstOrDefault();
                CodeGeneratorRule = await UOW.CodeGeneratorRuleRepository.Get(CodeGeneratorRule.Id);
                return CodeGeneratorRule;
            }
        }

        public async Task<List<RequestHistory<Product>>> GetHistories(Product Product)
        {
            List<RequestHistory<Product>> ProductHistories = await UOW.RequestHistoryRepository.ListRequestHistory<Product>(Product.Id);
            return ProductHistories;
        }

        public async Task<RequestHistory<Product>> GetProductHistoryDetail(string BsonId)
        {
            RequestHistory<Product> ProductHistory = await UOW.RequestHistoryRepository.GetRequestHistory<Product>(BsonId);
            return ProductHistory;
        }

        private void Sync(List<Product> Products)
        {
            List<Brand> Brands = Products.Where(x => x.BrandId.HasValue).Select(x =>new Brand { Id = x.BrandId.Value }).Distinct().ToList();
            List<Category> Categories = Products.Select(x => new Category { Id = x.CategoryId }).Distinct().ToList();
            List<ProductType> ProductTypes = Products.Select(x =>new ProductType { Id = x.ProductTypeId }).Distinct().ToList();
            List<TaxType> TaxTypes = Products.Select(x => new TaxType { Id = x.TaxTypeId }).Distinct().ToList();
            List<UnitOfMeasure> UnitOfMeasures = Products.Select(x => new UnitOfMeasure { Id = x.UnitOfMeasureId }).Distinct().ToList();
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = Products.Where(x => x.UnitOfMeasureGroupingId.HasValue).Select(x => new UnitOfMeasureGrouping { Id = x.UnitOfMeasureGroupingId.Value }).Distinct().ToList();
            List<CodeGeneratorRule> CodeGeneratorRules = Products.Where(x => x.CodeGeneratorRuleId.HasValue).Select(x => new CodeGeneratorRule { Id = x.CodeGeneratorRuleId.Value }).Distinct().ToList();
            //RabbitManager.PublishList(Brands, RoutingKeyEnum.BrandUsed);
            //RabbitManager.PublishList(Categories, RoutingKeyEnum.CategoryUsed);
            //RabbitManager.PublishList(UnitOfMeasures, RoutingKeyEnum.UnitOfMeasureUsed);
            //RabbitManager.PublishList(ProductTypes, RoutingKeyEnum.ProductTypeUsed);
            //RabbitManager.PublishList(TaxTypes, RoutingKeyEnum.TaxTypeUsed);
            //RabbitManager.PublishList(UnitOfMeasureGroupings, RoutingKeyEnum.UnitOfMeasureGroupingUsed);
            //RabbitManager.PublishList(CodeGeneratorRules, RoutingKeyEnum.CodeGeneratorRuleUsed);
            //RabbitManager.PublishList(Products, RoutingKeyEnum.ProductSync);
        }

        private async Task CreateHistory(Product Product, string Method)
        {
            Dictionary<string, object> displayFields = new Dictionary<string, object>();
            displayFields.Add("Method", Method);
            await UOW.RequestHistoryRepository.CreateRequestHistory(
                Product.Id,
                CurrentContext.UserId,
                Product, displayFields);
            return;

        }

    }
}
