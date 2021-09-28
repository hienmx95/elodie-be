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
    public interface ICategoryRepository
    {
        Task<int> Count(CategoryFilter CategoryFilter);
        Task<List<Category>> List(CategoryFilter CategoryFilter);
        Task<List<Category>> List(List<long> Ids);
        Task<Category> Get(long Id);
        Task<bool> Create(Category Category);
        Task<bool> Update(Category Category);
        Task<bool> Delete(Category Category);
        Task<bool> BulkMerge(List<Category> Categories);
        Task<bool> BulkDelete(List<Category> Categories);
        Task<bool> Used(List<long> Ids);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext DataContext;
        public CategoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CategoryDAO> DynamicFilter(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);

            if (filter.Code != null && filter.Code.HasValue)
            {
                var queryCode = query.Where(q => q.Code, filter.Code);
                var queryChildren = from q in query
                                    from c in queryCode
                                    where q.Path.StartsWith(c.Path)
                                    select q;
                var queryParent = from q in query
                                  from c in queryCode
                                  where c.Path.StartsWith(q.Path)
                                  select q;
                query = queryCode.Union(queryChildren).Union(queryParent).Distinct();

            }
            if (filter.Name != null && filter.Name.HasValue)
            {
                var queryName = query.Where(q => q.Name, filter.Name);
                var queryChildren = from q in query
                                    from c in queryName
                                    where q.Path.StartsWith(c.Path)
                                    select q;
                var queryParent = from q in query
                                  from c in queryName
                                  where c.Path.StartsWith(q.Path)
                                  select q;
                query = queryName.Union(queryChildren).Union(queryParent).Distinct();
            }
            if (filter.Prefix != null && filter.Prefix.HasValue)
                query = query.Where(q => q.Prefix, filter.Prefix);
            if (filter.Description != null && filter.Description.HasValue)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.ParentId != null && filter.ParentId.HasValue)
                query = query.Where(q => q.ParentId.HasValue).Where(q => q.ParentId.Value, filter.ParentId);
            if (filter.Path != null && filter.Path.HasValue)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.Level != null && filter.Level.HasValue)
                query = query.Where(q => q.Level, filter.Level);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.ImageId != null && filter.ImageId.HasValue)
                query = query.Where(q => q.ImageId.HasValue).Where(q => q.ImageId.Value, filter.ImageId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                List<string> Tokens = filter.Search.Split(" ").Select(x => x.ToLower()).ToList();
                var queryForCode = query;
                var queryForName = query;
                foreach (string Token in Tokens)
                {
                    if (string.IsNullOrWhiteSpace(Token))
                        continue;

                    {
                        var newQueryCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                        var queryChildren = from q in queryForCode
                                            from c in newQueryCode
                                            where q.Path.StartsWith(c.Path)
                                            select q;
                        var queryParent = from q in queryForCode
                                          from c in newQueryCode
                                          where c.Path.StartsWith(q.Path)
                                          select q;
                        queryForCode = newQueryCode.Union(queryChildren).Union(queryParent).Distinct();
                    }

                    {
                        var newQueryName = queryForName.Where(x => x.Name.ToLower().Contains(Token));
                        var queryChildren = from q in queryForName
                                            from c in newQueryName
                                            where q.Path.StartsWith(c.Path)
                                            select q;
                        var queryParent = from q in queryForName
                                          from c in newQueryName
                                          where c.Path.StartsWith(q.Path)
                                          select q;
                        queryForName = newQueryName.Union(queryChildren).Union(queryParent).Distinct();
                    }

                }
                query = queryForCode.Union(queryForName);
                query = query.Distinct();
            }


            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<CategoryDAO> OrFilter(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CategoryDAO> initQuery = query.Where(q => false);
            foreach (CategoryFilter CategoryFilter in filter.OrFilter)
            {
                IQueryable<CategoryDAO> queryable = query;
                if (CategoryFilter.Id != null && CategoryFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (CategoryFilter.Code != null && CategoryFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (CategoryFilter.Name != null && CategoryFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (CategoryFilter.Prefix != null && CategoryFilter.Prefix.HasValue)
                    queryable = queryable.Where(q => q.Prefix, filter.Prefix);
                if (CategoryFilter.Description != null && CategoryFilter.Description.HasValue)
                    queryable = queryable.Where(q => q.Description, filter.Description);
                if (CategoryFilter.ParentId != null && CategoryFilter.ParentId.HasValue)
                    queryable = queryable.Where(q => q.ParentId.HasValue).Where(q => q.ParentId.Value, filter.ParentId);
                if (CategoryFilter.Path != null && CategoryFilter.Path.HasValue)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                if (CategoryFilter.Level != null && CategoryFilter.Level.HasValue)
                    queryable = queryable.Where(q => q.Level, filter.Level);
                if (CategoryFilter.StatusId != null && CategoryFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (CategoryFilter.ImageId != null && CategoryFilter.ImageId.HasValue)
                    queryable = queryable.Where(q => q.ImageId.HasValue).Where(q => q.ImageId.Value, filter.ImageId);
                if (CategoryFilter.RowId != null && CategoryFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<CategoryDAO> DynamicOrder(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CategoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CategoryOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CategoryOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case CategoryOrder.Prefix:
                            query = query.OrderBy(q => q.Prefix);
                            break;
                        case CategoryOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case CategoryOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case CategoryOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case CategoryOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case CategoryOrder.HasChildren:
                            query = query.OrderBy(q => q.HasChildren);
                            break;
                        case CategoryOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case CategoryOrder.Image:
                            query = query.OrderBy(q => q.ImageId);
                            break;
                        case CategoryOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case CategoryOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CategoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CategoryOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CategoryOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case CategoryOrder.Prefix:
                            query = query.OrderByDescending(q => q.Prefix);
                            break;
                        case CategoryOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case CategoryOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case CategoryOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case CategoryOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case CategoryOrder.HasChildren:
                            query = query.OrderByDescending(q => q.HasChildren);
                            break;
                        case CategoryOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case CategoryOrder.Image:
                            query = query.OrderByDescending(q => q.ImageId);
                            break;
                        case CategoryOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case CategoryOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Category>> DynamicSelect(IQueryable<CategoryDAO> query, CategoryFilter filter)
        {
            List<Category> Categories = await query.Select(q => new Category()
            {
                Id = filter.Selects.Contains(CategorySelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CategorySelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(CategorySelect.Name) ? q.Name : default(string),
                Prefix = filter.Selects.Contains(CategorySelect.Prefix) ? q.Prefix : default(string),
                Description = filter.Selects.Contains(CategorySelect.Description) ? q.Description : default(string),
                ParentId = filter.Selects.Contains(CategorySelect.Parent) ? q.ParentId : default(long?),
                Path = filter.Selects.Contains(CategorySelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(CategorySelect.Level) ? q.Level : default(long),
                HasChildren = filter.Selects.Contains(CategorySelect.HasChildren) ? q.HasChildren : default(bool),
                StatusId = filter.Selects.Contains(CategorySelect.Status) ? q.StatusId : default(long),
                ImageId = filter.Selects.Contains(CategorySelect.Image) ? q.ImageId : default(long?),
                RowId = filter.Selects.Contains(CategorySelect.Row) ? q.RowId : default(Guid),
                Used = filter.Selects.Contains(CategorySelect.Used) ? q.Used : default(bool),
                Image = filter.Selects.Contains(CategorySelect.Image) && q.Image != null ? new Image
                {
                    Id = q.Image.Id,
                    Name = q.Image.Name,
                    Url = q.Image.Url,
                    ThumbnailUrl = q.Image.ThumbnailUrl,
                    RowId = q.Image.RowId,
                } : null,
                Parent = filter.Selects.Contains(CategorySelect.Parent) && q.Parent != null ? new Category
                {
                    Id = q.Parent.Id,
                    Code = q.Parent.Code,
                    Name = q.Parent.Name,
                    Prefix = q.Parent.Prefix,
                    Description = q.Parent.Description,
                    ParentId = q.Parent.ParentId,
                    Path = q.Parent.Path,
                    Level = q.Parent.Level,
                    HasChildren = q.Parent.HasChildren,
                    StatusId = q.Parent.StatusId,
                    ImageId = q.Parent.ImageId,
                    RowId = q.Parent.RowId,
                    Used = q.Parent.Used,
                } : null,
                Status = filter.Selects.Contains(CategorySelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return Categories;
        }

        public async Task<int> Count(CategoryFilter filter)
        {
            IQueryable<CategoryDAO> Categories = DataContext.Category.AsNoTracking();
            Categories = DynamicFilter(Categories, filter);
            return await Categories.CountAsync();
        }

        public async Task<List<Category>> List(CategoryFilter filter)
        {
            if (filter == null) return new List<Category>();
            IQueryable<CategoryDAO> CategoryDAOs = DataContext.Category.AsNoTracking();
            CategoryDAOs = DynamicFilter(CategoryDAOs, filter);
            CategoryDAOs = DynamicOrder(CategoryDAOs, filter);
            List<Category> Categories = await DynamicSelect(CategoryDAOs, filter);
            return Categories;
        }

        public async Task<List<Category>> List(List<long> Ids)
        {
            List<Category> Categories = await DataContext.Category.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Category()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Prefix = x.Prefix,
                Description = x.Description,
                ParentId = x.ParentId,
                Path = x.Path,
                Level = x.Level,
                HasChildren = x.HasChildren,
                StatusId = x.StatusId,
                ImageId = x.ImageId,
                RowId = x.RowId,
                Used = x.Used,
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                    ThumbnailUrl = x.Image.ThumbnailUrl,
                    RowId = x.Image.RowId,
                    CreatedAt = x.Image.CreatedAt,
                    UpdatedAt = x.Image.UpdatedAt,
                },
                Parent = x.Parent == null ? null : new Category
                {
                    Id = x.Parent.Id,
                    Code = x.Parent.Code,
                    Name = x.Parent.Name,
                    Prefix = x.Parent.Prefix,
                    Description = x.Parent.Description,
                    ParentId = x.Parent.ParentId,
                    Path = x.Parent.Path,
                    Level = x.Parent.Level,
                    HasChildren = x.Parent.HasChildren,
                    StatusId = x.Parent.StatusId,
                    ImageId = x.Parent.ImageId,
                    RowId = x.Parent.RowId,
                    Used = x.Parent.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();


            return Categories;
        }

        public async Task<Category> Get(long Id)
        {
            Category Category = await DataContext.Category.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Category()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Prefix = x.Prefix,
                Description = x.Description,
                ParentId = x.ParentId,
                Path = x.Path,
                Level = x.Level,
                HasChildren = x.HasChildren,
                StatusId = x.StatusId,
                ImageId = x.ImageId,
                RowId = x.RowId,
                Used = x.Used,
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                    ThumbnailUrl = x.Image.ThumbnailUrl,
                    RowId = x.Image.RowId,
                },
                Parent = x.Parent == null ? null : new Category
                {
                    Id = x.Parent.Id,
                    Code = x.Parent.Code,
                    Name = x.Parent.Name,
                    Prefix = x.Parent.Prefix,
                    Description = x.Parent.Description,
                    ParentId = x.Parent.ParentId,
                    Path = x.Parent.Path,
                    Level = x.Parent.Level,
                    HasChildren = x.Parent.HasChildren,
                    StatusId = x.Parent.StatusId,
                    ImageId = x.Parent.ImageId,
                    RowId = x.Parent.RowId,
                    Used = x.Parent.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Category == null)
                return null;

            return Category;
        }
        public async Task<bool> Create(Category Category)
        {
            CategoryDAO CategoryDAO = new CategoryDAO();
            CategoryDAO.Id = Category.Id;
            CategoryDAO.Code = Category.Code;
            CategoryDAO.Name = Category.Name;
            CategoryDAO.Prefix = Category.Prefix;
            CategoryDAO.Description = Category.Description;
            CategoryDAO.ParentId = Category.ParentId;
            CategoryDAO.Path = Category.Path;
            CategoryDAO.Level = Category.Level;
            CategoryDAO.HasChildren = false;
            CategoryDAO.StatusId = Category.StatusId;
            CategoryDAO.ImageId = Category.ImageId;
            CategoryDAO.RowId = Category.RowId;
            CategoryDAO.Used = Category.Used;
            CategoryDAO.RowId = Guid.NewGuid();
            CategoryDAO.Path = "";
            CategoryDAO.Level = 1;
            CategoryDAO.CreatedAt = StaticParams.DateTimeNow;
            CategoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Category.Add(CategoryDAO);
            await DataContext.SaveChangesAsync();
            Category.Id = CategoryDAO.Id;
            await SaveReference(Category);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(Category Category)
        {
            CategoryDAO CategoryDAO = DataContext.Category.Where(x => x.Id == Category.Id).FirstOrDefault();
            if (CategoryDAO == null)
                return false;
            CategoryDAO.Id = Category.Id;
            CategoryDAO.Code = Category.Code;
            CategoryDAO.Name = Category.Name;
            CategoryDAO.Prefix = Category.Prefix;
            CategoryDAO.Description = Category.Description;
            CategoryDAO.ParentId = Category.ParentId;
            CategoryDAO.Path = Category.Path;
            CategoryDAO.Level = Category.Level;
            CategoryDAO.HasChildren = false;
            CategoryDAO.StatusId = Category.StatusId;
            CategoryDAO.ImageId = Category.ImageId;
            CategoryDAO.RowId = Category.RowId;
            CategoryDAO.Used = Category.Used;
            CategoryDAO.Path = "";
            CategoryDAO.Level = 1;
            CategoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Category);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(Category Category)
        {
            CategoryDAO CategoryDAO = await DataContext.Category.Where(x => x.Id == Category.Id).FirstOrDefaultAsync();
            await DataContext.Category.Where(x => x.Path.StartsWith(CategoryDAO.Id + ".")).UpdateFromQueryAsync(x => new CategoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await DataContext.Category.Where(x => x.Id == Category.Id).UpdateFromQueryAsync(x => new CategoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkMerge(List<Category> Categories)
        {
            List<CategoryDAO> CategoryDAOs = new List<CategoryDAO>();
            foreach (Category Category in Categories)
            {
                CategoryDAO CategoryDAO = new CategoryDAO();
                CategoryDAO.Id = Category.Id;
                CategoryDAO.Code = Category.Code;
                CategoryDAO.Name = Category.Name;
                CategoryDAO.Prefix = Category.Prefix;
                CategoryDAO.Description = Category.Description;
                CategoryDAO.ParentId = Category.ParentId;
                CategoryDAO.Path = Category.Path;
                CategoryDAO.Level = Category.Level;
                CategoryDAO.StatusId = Category.StatusId;
                CategoryDAO.ImageId = Category.ImageId;
                CategoryDAO.RowId = Category.RowId;
                CategoryDAO.Used = Category.Used;

                CategoryDAO.HasChildren = false;
                CategoryDAO.Path = "";
                CategoryDAO.Level = 1;

                CategoryDAO.CreatedAt = StaticParams.DateTimeNow;
                CategoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                CategoryDAOs.Add(CategoryDAO);
            }
            await DataContext.BulkMergeAsync(CategoryDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<Category> Categories)
        {
            List<long> Ids = Categories.Select(x => x.Id).ToList();
            await DataContext.Category
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CategoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(Category Category)
        {
        }

        private async Task BuildPath()
        {
            List<CategoryDAO> CategoryDAOs = await DataContext.Category
                .Where(x => x.DeletedAt == null)
                .AsNoTracking().ToListAsync();
            Queue<CategoryDAO> queue = new Queue<CategoryDAO>();
            CategoryDAOs.ForEach(x =>
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
                CategoryDAO Parent = queue.Dequeue();
                foreach (CategoryDAO CategoryDAO in CategoryDAOs)
                {
                    if (CategoryDAO.ParentId == Parent.Id)
                    {
                        Parent.HasChildren = true;
                        CategoryDAO.Path = Parent.Path + CategoryDAO.Id + ".";
                        CategoryDAO.Level = Parent.Level + 1;
                        queue.Enqueue(CategoryDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(CategoryDAOs);
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Category.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CategoryDAO { Used = true });
            return true;
        }

    }
}
