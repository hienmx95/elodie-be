using ELODIE.Common;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ELODIE.Repositories;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Services.MImage;
using ELODIE.Handlers;

namespace ELODIE.Services.MCategory
{
    public interface ICategoryService :  IServiceScoped
    {
        Task<int> Count(CategoryFilter CategoryFilter);
        Task<List<Category>> List(CategoryFilter CategoryFilter);
        Task<Category> Get(long Id);
        Task<Category> Create(Category Category);
        Task<Category> Update(Category Category);
        Task<Category> Delete(Category Category);
        Task<List<Category>> BulkDelete(List<Category> Categories);
        Task<List<Category>> Import(List<Category> Categories);
        Task<CategoryFilter> ToFilter(CategoryFilter CategoryFilter);
        Task<Image> SaveImage(Image Image);
    }

    public class CategoryService : BaseService, ICategoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private ICategoryValidator CategoryValidator;
        //private IRabbitManager RabbitManager;

        public CategoryService(
            IUOW UOW,
            ILogging Logging,
            IImageService ImageService,
            ICurrentContext CurrentContext,
            ICategoryValidator CategoryValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.CategoryValidator = CategoryValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(CategoryFilter CategoryFilter)
        {
            try
            {
                int result = await UOW.CategoryRepository.Count(CategoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Category>> List(CategoryFilter CategoryFilter)
        {
            try
            {
                List<Category> Categories = await UOW.CategoryRepository.List(CategoryFilter);
                return Categories;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Category> Get(long Id)
        {
            Category Category = await UOW.CategoryRepository.Get(Id);
            if (Category == null)
                return null;
            List<Category> AllCategories = await UOW.CategoryRepository.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.Id | CategorySelect.Path | CategorySelect.Level | CategorySelect.Parent
            });
            var Count = AllCategories
                        .Where(x => x.Path.StartsWith(Category.Path)
                                    && x.Id != Category.Id)
                        .Count(); ;
            if (Count > 0)
                Category.HasChildren = true;
            return Category;
        }
       
        public async Task<Category> Create(Category Category)
        {
            if (!await CategoryValidator.Create(Category))
                return Category;

            try
            {
                await UOW.Begin();
                await UOW.CategoryRepository.Create(Category);
                await UOW.Commit();
                List<Category> Categories = await UOW.CategoryRepository.List(new List<long> { Category.Id });
                await Sync(Categories);
                Category = Categories.FirstOrDefault();
                await Logging.CreateAuditLog(Category, new { }, nameof(CategoryService));
                return Category;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Category> Update(Category Category)
        {
            if (!await CategoryValidator.Update(Category))
                return Category;
            try
            {
                var oldData = await UOW.CategoryRepository.Get(Category.Id);

                await UOW.Begin();
                await UOW.CategoryRepository.Update(Category);
                await UOW.Commit();

                List<Category> Categories = await UOW.CategoryRepository.List(new List<long> { Category.Id });
                await Sync(Categories);
                Category = Categories.FirstOrDefault();
                await Logging.CreateAuditLog(Category, oldData, nameof(CategoryService));
                return Category;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Category> Delete(Category Category)
        {
            if (!await CategoryValidator.Delete(Category))
                return Category;

            try
            {
                await UOW.Begin();
                await UOW.CategoryRepository.Delete(Category);
                await UOW.Commit();
                List<Category> Categories = await UOW.CategoryRepository.List(new List<long> { Category.Id });
                await Sync(Categories);
                Category = Categories.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, Category, nameof(CategoryService));
                return Category;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Category>> BulkDelete(List<Category> Categories)
        {
            if (!await CategoryValidator.BulkDelete(Categories))
                return Categories;

            try
            {
                await UOW.Begin();
                await UOW.CategoryRepository.BulkDelete(Categories);
                await UOW.Commit();
                List<long> Ids = Categories.Select(x => x.Id).ToList();
                Categories = await UOW.CategoryRepository.List(Ids);
                await Sync(Categories);
                await Logging.CreateAuditLog(new { }, Categories, nameof(CategoryService));
                return Categories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<Category>> Import(List<Category> Categories)
        {
            if (!await CategoryValidator.Import(Categories))
                return Categories;
            try
            {
                await UOW.Begin();
                await UOW.CategoryRepository.BulkMerge(Categories);
                await UOW.Commit();
                List<long> Ids = Categories.Select(x => x.Id).ToList();
                Categories = await UOW.CategoryRepository.List(Ids);
                await Sync(Categories);
                await Logging.CreateAuditLog(Categories, new { }, nameof(CategoryService));
                return Categories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<CategoryFilter> ToFilter(CategoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CategoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CategoryFilter subFilter = new CategoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = FilterBuilder.Merge(subFilter.ParentId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Path))
                        subFilter.Path = FilterBuilder.Merge(subFilter.Path, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Level))
                        subFilter.Level = FilterBuilder.Merge(subFilter.Level, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/category/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            string thumbnailPath = $"/category/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
        }

        private async Task Sync(List<Category> Categories)
        {
            //RabbitManager.PublishList(Categories, RoutingKeyEnum.CategorySync);
        }
    }
}
