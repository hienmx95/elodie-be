using ELODIE.Common;
using ELODIE.Helpers;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IProductGroupingRepository
    {
        Task<int> Count(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(List<long> Ids);
        Task<ProductGrouping> Get(long Id);
        Task<bool> Create(ProductGrouping ProductGrouping);
        Task<bool> Update(ProductGrouping ProductGrouping);
        Task<bool> Delete(ProductGrouping ProductGrouping);
        Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings);
        Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings);
    }
    public class ProductGroupingRepository : IProductGroupingRepository
    {
        private DataContext DataContext;
        public ProductGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProductGroupingDAO> DynamicFilter(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.ParentId, filter.ParentId);
            query = query.Where(q => q.Path, filter.Path);
            query = query.Where(q => q.Level, filter.Level);
            query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProductGroupingDAO> OrFilter(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductGroupingDAO> initQuery = query.Where(q => false);
            foreach (ProductGroupingFilter ProductGroupingFilter in filter.OrFilter)
            {
                IQueryable<ProductGroupingDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.Description, filter.Description);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.ParentId, filter.ParentId);
                queryable = queryable.Where(q => q.Path, filter.Path);
                queryable = queryable.Where(q => q.Level, filter.Level);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductGroupingDAO> DynamicOrder(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductGroupingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProductGroupingOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ProductGroupingOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ProductGroupingOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case ProductGroupingOrder.HasChildren:
                            query = query.OrderBy(q => q.HasChildren);
                            break;
                        case ProductGroupingOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case ProductGroupingOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case ProductGroupingOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductGroupingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProductGroupingOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ProductGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ProductGroupingOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case ProductGroupingOrder.HasChildren:
                            query = query.OrderByDescending(q => q.HasChildren);
                            break;
                        case ProductGroupingOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case ProductGroupingOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case ProductGroupingOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProductGrouping>> DynamicSelect(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            List<ProductGrouping> ProductGroupings = await query.Select(q => new ProductGrouping()
            {
                Id = filter.Selects.Contains(ProductGroupingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductGroupingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProductGroupingSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(ProductGroupingSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(ProductGroupingSelect.Status) ? q.StatusId : default(long),
                ParentId = filter.Selects.Contains(ProductGroupingSelect.Parent) ? q.ParentId : default(long?),
                HasChildren = filter.Selects.Contains(ProductGroupingSelect.HasChildren) ? q.HasChildren : default(bool),
                Path = filter.Selects.Contains(ProductGroupingSelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(ProductGroupingSelect.Level) ? q.Level : default(long),
                RowId = filter.Selects.Contains(ProductGroupingSelect.Row) ? q.RowId : default(Guid),
                Parent = filter.Selects.Contains(ProductGroupingSelect.Parent) && q.Parent != null ? new ProductGrouping
                {
                    Id = q.Parent.Id,
                    Code = q.Parent.Code,
                    Name = q.Parent.Name,
                    Description = q.Parent.Description,
                    StatusId = q.Parent.StatusId,
                    ParentId = q.Parent.ParentId,
                    HasChildren = q.Parent.HasChildren,
                    Path = q.Parent.Path,
                    Level = q.Parent.Level,
                    RowId = q.Parent.RowId,
                } : null,
                Status = filter.Selects.Contains(ProductGroupingSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();

            //if (filter.Selects.Contains(ProductSelect.ProductProductGroupingMapping))
            //{
            //    List<string> ProductGroupingPaths = ProductGroupings.Select(x => x.Path).ToList();

            //    var queryProductProductGroupingMapping = DataContext.ProductProductGroupingMapping.Where(q => false);
            //    foreach(string path in ProductGroupingPaths)
            //    {
            //        var temp = DataContext.ProductProductGroupingMapping.Where(x => x.ProductGrouping.Path.Contains(path));
            //        queryProductProductGroupingMapping = queryProductProductGroupingMapping.Union(temp);
            //    }
            //    queryProductProductGroupingMapping = queryProductProductGroupingMapping.Distinct();

            //    List<ProductProductGroupingMapping> ProductProductGroupingMappings = await queryProductProductGroupingMapping
            //        .Select(x => new ProductProductGroupingMapping
            //        {
            //            ProductId = x.ProductId,
            //            ProductGroupingId = x.ProductGroupingId,
            //            ProductGrouping = new ProductGrouping
            //            {
            //                Id = x.ProductGrouping.Id,
            //                Path = x.ProductGrouping.Path,
            //            },
            //            Product = new Product
            //            {
            //                Id = x.Product.Id,
            //                Code = x.Product.Code,
            //                Name = x.Product.Name,
            //                Description = x.Product.Description,
            //            },
            //        }).ToListAsync();

            //    foreach(ProductGrouping ProductGrouping in ProductGroupings)
            //    {
            //        ProductGrouping.ProductProductGroupingMappings = ProductProductGroupingMappings
            //            .Where(x => x.ProductGrouping.Path.Contains(ProductGrouping.Path))
            //            .GroupBy(x => x.ProductId)
            //            .Select(grpby => grpby.FirstOrDefault())
            //            .ToList();

            //    }
            //}

            return ProductGroupings;
        }

        public async Task<int> Count(ProductGroupingFilter filter)
        {
            IQueryable<ProductGroupingDAO> ProductGroupings = DataContext.ProductGrouping.AsNoTracking();
            ProductGroupings = DynamicFilter(ProductGroupings, filter);
            return await ProductGroupings.CountAsync();
        }

        public async Task<List<ProductGrouping>> List(ProductGroupingFilter filter)
        {
            if (filter == null) return new List<ProductGrouping>();
            IQueryable<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping.AsNoTracking();
            ProductGroupingDAOs = DynamicFilter(ProductGroupingDAOs, filter);
            ProductGroupingDAOs = DynamicOrder(ProductGroupingDAOs, filter);
            List<ProductGrouping> ProductGroupings = await DynamicSelect(ProductGroupingDAOs, filter);
            return ProductGroupings;
        }

        public async Task<List<ProductGrouping>> List(List<long> Ids)
        {
            List<ProductGrouping> ProductGroupings = await DataContext.ProductGrouping.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ProductGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StatusId = x.StatusId,
                ParentId = x.ParentId,
                HasChildren = x.HasChildren,
                Path = x.Path,
                Level = x.Level,
                RowId = x.RowId,
                Parent = x.Parent == null ? null : new ProductGrouping
                {
                    Id = x.Parent.Id,
                    Code = x.Parent.Code,
                    Name = x.Parent.Name,
                    Description = x.Parent.Description,
                    StatusId = x.Parent.StatusId,
                    ParentId = x.Parent.ParentId,
                    HasChildren = x.Parent.HasChildren,
                    Path = x.Parent.Path,
                    Level = x.Parent.Level,
                    RowId = x.Parent.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();

            List<ProductProductGroupingMapping> ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping.AsNoTracking()
                .Where(x => Ids.Contains(x.ProductGroupingId))
                .Where(x => x.Product.DeletedAt == null)
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    ProductGroupingId = x.ProductGroupingId,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ERPCode = x.Product.ERPCode,
                        CategoryId = x.Product.CategoryId,
                        ProductTypeId = x.Product.ProductTypeId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        CodeGeneratorRuleId = x.Product.CodeGeneratorRuleId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                        OtherName = x.Product.OtherName,
                        TechnicalName = x.Product.TechnicalName,
                        Note = x.Product.Note,
                        IsPurchasable = x.Product.IsPurchasable,
                        IsSellable = x.Product.IsSellable,
                        IsNew = x.Product.IsNew,
                        UsedVariationId = x.Product.UsedVariationId,
                        Used = x.Product.Used,
                        RowId = x.Product.RowId,
                    },
                }).ToListAsync();
            foreach (ProductGrouping ProductGrouping in ProductGroupings)
            {
                ProductGrouping.ProductProductGroupingMappings = ProductProductGroupingMappings
                    .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                    .ToList();
            }

            return ProductGroupings;
        }

        public async Task<ProductGrouping> Get(long Id)
        {
            ProductGrouping ProductGrouping = await DataContext.ProductGrouping.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ProductGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StatusId = x.StatusId,
                ParentId = x.ParentId,
                HasChildren = x.HasChildren,
                Path = x.Path,
                Level = x.Level,
                RowId = x.RowId,
                Parent = x.Parent == null ? null : new ProductGrouping
                {
                    Id = x.Parent.Id,
                    Code = x.Parent.Code,
                    Name = x.Parent.Name,
                    Description = x.Parent.Description,
                    StatusId = x.Parent.StatusId,
                    ParentId = x.Parent.ParentId,
                    HasChildren = x.Parent.HasChildren,
                    Path = x.Parent.Path,
                    Level = x.Parent.Level,
                    RowId = x.Parent.RowId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (ProductGrouping == null)
                return null;
            ProductGrouping.ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping.AsNoTracking()
                .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                .Where(x => x.Product.DeletedAt == null)
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    ProductGroupingId = x.ProductGroupingId,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ERPCode = x.Product.ERPCode,
                        CategoryId = x.Product.CategoryId,
                        ProductTypeId = x.Product.ProductTypeId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        CodeGeneratorRuleId = x.Product.CodeGeneratorRuleId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                        OtherName = x.Product.OtherName,
                        TechnicalName = x.Product.TechnicalName,
                        Note = x.Product.Note,
                        IsPurchasable = x.Product.IsPurchasable,
                        IsSellable = x.Product.IsSellable,
                        IsNew = x.Product.IsNew,
                        UsedVariationId = x.Product.UsedVariationId,
                        Used = x.Product.Used,
                        RowId = x.Product.RowId,
                        Brand = x.Product.Brand == null ? null : new Brand
                        {
                            Id = x.Product.Brand.Id,
                            Code = x.Product.Brand.Code,
                            Name = x.Product.Brand.Name,
                            Description = x.Product.Brand.Description,
                            StatusId = x.Product.Brand.StatusId,
                        },
                        Category = x.Product.Category == null ? null : new Category
                        {
                            Id = x.Product.Category.Id,
                            Code = x.Product.Category.Code,
                            Name = x.Product.Category.Name,
                        },
                        ProductType = x.Product.ProductType == null ? null : new ProductType
                        {
                            Id = x.Product.ProductType.Id,
                            Code = x.Product.ProductType.Code,
                            Name = x.Product.ProductType.Name,
                            Description = x.Product.ProductType.Description,
                            StatusId = x.Product.ProductType.StatusId,
                        },
                        Status = x.Product.Status == null ? null : new Status
                        {
                            Id = x.Product.Status.Id,
                            Code = x.Product.Status.Code,
                            Name = x.Product.Status.Name,
                        },
                        TaxType = x.Product.TaxType == null ? null : new TaxType
                        {
                            Id = x.Product.TaxType.Id,
                            Code = x.Product.TaxType.Code,
                            Name = x.Product.TaxType.Name,
                            Percentage = x.Product.TaxType.Percentage,
                            StatusId = x.Product.TaxType.StatusId,
                        },
                        UnitOfMeasure = x.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                        {
                            Id = x.Product.UnitOfMeasure.Id,
                            Code = x.Product.UnitOfMeasure.Code,
                            Name = x.Product.UnitOfMeasure.Name,
                            Description = x.Product.UnitOfMeasure.Description,
                            StatusId = x.Product.UnitOfMeasure.StatusId,
                        },
                        UnitOfMeasureGrouping = x.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                        {
                            Id = x.Product.UnitOfMeasureGrouping.Id,
                            Name = x.Product.UnitOfMeasureGrouping.Name,
                            UnitOfMeasureId = x.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            StatusId = x.Product.UnitOfMeasureGrouping.StatusId
                        },
                    },
                }).ToListAsync();

            List<long> ProductIds = ProductGrouping.ProductProductGroupingMappings.Select(x => x.ProductId).ToList();
            List<ProductImageMapping> ProductImageMappings = await DataContext.ProductImageMapping.AsNoTracking()
                .Where(x => ProductIds.Contains(x.ProductId))
                .Select(x => new ProductImageMapping
                {
                    ProductId = x.ProductId,
                    ImageId = x.ImageId,

                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    }
                }).ToListAsync();

            foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductGrouping.ProductProductGroupingMappings)
            {
                ProductProductGroupingMapping.Product.ProductImageMappings = ProductImageMappings
                    .Where(x => x.ProductId == ProductProductGroupingMapping.ProductId)
                    .ToList();
            }

            return ProductGrouping;
        }
        public async Task<bool> Create(ProductGrouping ProductGrouping)
        {
            ProductGroupingDAO ProductGroupingDAO = new ProductGroupingDAO();
            ProductGroupingDAO.Id = ProductGrouping.Id;
            ProductGroupingDAO.Code = ProductGrouping.Code;
            ProductGroupingDAO.Name = ProductGrouping.Name;
            ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
            ProductGroupingDAO.Path = "";
            ProductGroupingDAO.Level = 1;
            ProductGroupingDAO.HasChildren = false;
            ProductGroupingDAO.StatusId = ProductGrouping.StatusId;
            ProductGroupingDAO.Description = ProductGrouping.Description;
            ProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            ProductGroupingDAO.RowId = Guid.NewGuid();
            DataContext.ProductGrouping.Add(ProductGroupingDAO);
            await DataContext.SaveChangesAsync();
            ProductGrouping.Id = ProductGroupingDAO.Id;
            await SaveReference(ProductGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(ProductGrouping ProductGrouping)
        {
            ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).FirstOrDefault();
            if (ProductGroupingDAO == null)
                return false;
            ProductGroupingDAO.Id = ProductGrouping.Id;
            ProductGroupingDAO.Code = ProductGrouping.Code;
            ProductGroupingDAO.Name = ProductGrouping.Name;
            ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
            ProductGroupingDAO.Path = "";
            ProductGroupingDAO.Level = 1;
            ProductGroupingDAO.HasChildren = false;
            ProductGroupingDAO.StatusId = ProductGrouping.StatusId;
            ProductGroupingDAO.Description = ProductGrouping.Description;
            ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ProductGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(ProductGrouping ProductGrouping)
        {
            await DataContext.ProductProductGroupingMapping.Where(x => x.ProductGroupingId == ProductGrouping.Id).DeleteFromQueryAsync();
            ProductGroupingDAO ProductGroupingDAO = await DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).FirstOrDefaultAsync();
            await DataContext.ProductGrouping.Where(x => x.Path.StartsWith(ProductGroupingDAO.Id + ".")).UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings)
        {
            List<ProductGroupingDAO> ProductGroupingDAOs = new List<ProductGroupingDAO>();
            foreach (ProductGrouping ProductGrouping in ProductGroupings)
            {
                ProductGroupingDAO ProductGroupingDAO = new ProductGroupingDAO();
                ProductGroupingDAO.Id = ProductGrouping.Id;
                ProductGroupingDAO.Code = ProductGrouping.Code;
                ProductGroupingDAO.Name = ProductGrouping.Name;
                ProductGroupingDAO.Description = ProductGrouping.Description;
                ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
                ProductGroupingDAO.StatusId = ProductGrouping.StatusId;
                ProductGroupingDAO.RowId = ProductGrouping.RowId;
                ProductGroupingDAO.Path = "";
                ProductGroupingDAO.Level = 1;
                ProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProductGroupingDAOs.Add(ProductGroupingDAO);
            }
            await DataContext.BulkMergeAsync(ProductGroupingDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings)
        {
            List<long> Ids = ProductGroupings.Select(x => x.Id).ToList();
            await DataContext.ProductProductGroupingMapping
                .Where(x => Ids.Contains(x.ProductGroupingId))
                .DeleteFromQueryAsync();
            await DataContext.ProductGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(ProductGrouping ProductGrouping)
        {
            await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                .DeleteFromQueryAsync();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            if (ProductGrouping.ProductProductGroupingMappings != null)
            {
                ProductGrouping.ProductProductGroupingMappings = ProductGrouping.ProductProductGroupingMappings.Distinct().ToList();
                foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductGrouping.ProductProductGroupingMappings)
                {
                    ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO();
                    ProductProductGroupingMappingDAO.ProductId = ProductProductGroupingMapping.ProductId;
                    ProductProductGroupingMappingDAO.ProductGroupingId = ProductGrouping.Id;
                    ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                }
                await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);
            }
        }

        private async Task BuildPath()
        {
            List<ProductGroupingDAO> ProductGroupingDAOs = await DataContext.ProductGrouping.AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .ToListAsync();
            Queue<ProductGroupingDAO> queue = new Queue<ProductGroupingDAO>();
            ProductGroupingDAOs.ForEach(x =>
            {
                x.HasChildren = false;
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    x.Level = 1;
                    queue.Enqueue(x);
                }
            });
            while (queue.Count > 0)
            {
                ProductGroupingDAO Parent = queue.Dequeue();
                foreach (ProductGroupingDAO ProductGroupingDAO in ProductGroupingDAOs)
                {
                    if (ProductGroupingDAO.ParentId == Parent.Id)
                    {
                        Parent.HasChildren = true;
                        ProductGroupingDAO.Path = Parent.Path + ProductGroupingDAO.Id + ".";
                        ProductGroupingDAO.Level = Parent.Level + 1;
                        queue.Enqueue(ProductGroupingDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(ProductGroupingDAOs);
        }
    }
}
