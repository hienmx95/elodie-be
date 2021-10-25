using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Services.MImage;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MProduct
{
    public interface IItemService : IServiceScoped
    {
        Task<int> Count(ItemFilter ItemFilter);
        Task<List<Item>> List(ItemFilter ItemFilter);
        Task<Item> Get(long Id);
        Task<Image> SaveImage(Image Image);
        ItemFilter ToFilter(ItemFilter ItemFilter);
    }

    public class ItemService : BaseService, IItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private IItemValidator ItemValidator;

        public ItemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IItemValidator ItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.ItemValidator = ItemValidator;
        }
        public async Task<int> Count(ItemFilter ItemFilter)
        {
            try
            {
                int result = await UOW.ItemRepository.Count(ItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Item>> List(ItemFilter ItemFilter)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);

                return Items;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Item> Get(long Id)
        {
            var appUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            Item Item = await UOW.ItemRepository.Get(Id);
            if (Item == null)
                return null;
            
            return Item;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/item/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            string thumbnailPath = $"/item/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
        }

        public ItemFilter ToFilter(ItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemFilter subFilter = new ItemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductTypeId))
                        subFilter.ProductTypeId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductGroupingId))
                        subFilter.ProductGroupingId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = FilterPermissionDefinition.DecimalFilter;
                }
            }
            return filter;
        }
    }
}
