using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Handlers;
using ELODIE.Enums;

namespace ELODIE.Services.MProductGrouping
{
    public interface IProductGroupingService : IServiceScoped
    {
        Task<int> Count(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter);
        Task<ProductGrouping> Get(long Id);
        Task<ProductGrouping> Create(ProductGrouping ProductGrouping);
        Task<ProductGrouping> Update(ProductGrouping ProductGrouping);
        Task<ProductGrouping> Delete(ProductGrouping ProductGrouping);
        ProductGroupingFilter ToFilter(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> Import(List<ProductGrouping> ProductGroupings);
    }

    public class ProductGroupingService : BaseService, IProductGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductGroupingValidator ProductGroupingValidator;
        //private IRabbitManager RabbitManager;

        public ProductGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductGroupingValidator ProductGroupingValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            //this.RabbitManager = RabbitManager;
            this.ProductGroupingValidator = ProductGroupingValidator;
        }
        public async Task<int> Count(ProductGroupingFilter ProductGroupingFilter)
        {
            try
            {
                int result = await UOW.ProductGroupingRepository.Count(ProductGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter)
        {
            try
            {
                List<ProductGrouping> ProductGroupings = await UOW.ProductGroupingRepository.List(ProductGroupingFilter);
                return ProductGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<ProductGrouping> Get(long Id)
        {
            ProductGrouping ProductGrouping = await UOW.ProductGroupingRepository.Get(Id);
            if (ProductGrouping == null)
                return null;
            return ProductGrouping;
        }

        public async Task<ProductGrouping> Create(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Create(ProductGrouping))
                return ProductGrouping;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.Create(ProductGrouping);
                await UOW.Commit();
                ProductGrouping = (await UOW.ProductGroupingRepository.List(new List<long> { ProductGrouping.Id })).FirstOrDefault();
                Sync(new List<ProductGrouping> { ProductGrouping });
                await Logging.CreateAuditLog(ProductGrouping, new { }, nameof(ProductGroupingService));
                return await UOW.ProductGroupingRepository.Get(ProductGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<ProductGrouping> Update(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Update(ProductGrouping))
                return ProductGrouping;
            try
            {
                var oldData = await UOW.ProductGroupingRepository.Get(ProductGrouping.Id);

                await UOW.Begin();
                await UOW.ProductGroupingRepository.Update(ProductGrouping);
                await UOW.Commit();

                ProductGrouping = (await UOW.ProductGroupingRepository.List(new List<long> { ProductGrouping.Id })).FirstOrDefault();
                Sync(new List<ProductGrouping> { ProductGrouping });
                await Logging.CreateAuditLog(ProductGrouping, oldData, nameof(ProductGroupingService));
                return ProductGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<ProductGrouping> Delete(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Delete(ProductGrouping))
                return ProductGrouping;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.Delete(ProductGrouping);
                await UOW.Commit();
                ProductGrouping = (await UOW.ProductGroupingRepository.List(new List<long> { ProductGrouping.Id })).FirstOrDefault();
                Sync(new List<ProductGrouping> { ProductGrouping });
                await Logging.CreateAuditLog(new { }, ProductGrouping, nameof(ProductGroupingService));
                return ProductGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public ProductGroupingFilter ToFilter(ProductGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProductGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProductGroupingFilter subFilter = new ProductGroupingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }

        private void Sync(List<ProductGrouping> ProductGroupings)
        {
            //RabbitManager.PublishList(ProductGroupings, RoutingKeyEnum.ProductGroupingSync);
        }

        public async Task<List<ProductGrouping>> Import(List<ProductGrouping> ProductGroupings)
        {
            if (!await ProductGroupingValidator.Import(ProductGroupings))
                return ProductGroupings;
            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.BulkMerge(ProductGroupings);
                await UOW.Commit();
                ProductGroupings = await UOW.ProductGroupingRepository.List(ProductGroupings.Select(q => q.Id).ToList());
                Sync(ProductGroupings);
                await Logging.CreateAuditLog(new { }, ProductGroupings, nameof(ProductGroupingService));
                return ProductGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

    }
}
