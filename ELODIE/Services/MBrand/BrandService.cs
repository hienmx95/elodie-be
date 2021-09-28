using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELODIE.Enums;
using ELODIE.Handlers;
using System.Linq;

namespace ELODIE.Services.MBrand
{
    public interface IBrandService : IServiceScoped
    {
        Task<int> Count(BrandFilter BrandFilter);
        Task<List<Brand>> List(BrandFilter BrandFilter);
        Task<Brand> Get(long Id);
        Task<Brand> Create(Brand Brand);
        Task<Brand> Update(Brand Brand);
        Task<Brand> Delete(Brand Brand);
        Task<List<Brand>> BulkDelete(List<Brand> Brands);
        Task<List<Brand>> Import(List<Brand> Brands);
    }

    public class BrandService : BaseService, IBrandService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IBrandValidator BrandValidator;
       //private IRabbitManager RabbitManager;

        public BrandService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IBrandValidator BrandValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.BrandValidator = BrandValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(BrandFilter BrandFilter)
        {
            try
            {
                int result = await UOW.BrandRepository.Count(BrandFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Brand>> List(BrandFilter BrandFilter)
        {
            try
            {
                List<Brand> Brands = await UOW.BrandRepository.List(BrandFilter);
                return Brands;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Brand> Get(long Id)
        {
            Brand Brand = await UOW.BrandRepository.Get(Id);
            if (Brand == null)
                return null;
            return Brand;
        }

        public async Task<Brand> Create(Brand Brand)
        {
            if (!await BrandValidator.Create(Brand))
                return Brand;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.Create(Brand);
                await UOW.Commit();

                List<Brand> Brands = await UOW.BrandRepository.List(new List<long> { Brand.Id });
                Sync(Brands);
                Brand = Brands.FirstOrDefault();
                await Logging.CreateAuditLog(Brand, new { }, nameof(BrandService));
                return Brand;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Brand> Update(Brand Brand)
        {
            if (!await BrandValidator.Update(Brand))
                return Brand;
            try
            {
                var oldData = await UOW.BrandRepository.Get(Brand.Id);

                await UOW.Begin();
                await UOW.BrandRepository.Update(Brand);
                await UOW.Commit();

                List<Brand> Brands = await UOW.BrandRepository.List(new List<long> { Brand.Id });
                Sync(Brands);
                Brand = Brands.FirstOrDefault();
                await Logging.CreateAuditLog(Brand, oldData, nameof(BrandService));
                return Brand;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Brand> Delete(Brand Brand)
        {
            if (!await BrandValidator.Delete(Brand))
                return Brand;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.Delete(Brand);
                await UOW.Commit();
                
                List<Brand> Brands = await UOW.BrandRepository.List(new List<long> { Brand.Id });
                Sync(Brands);
                Brand = Brands.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, Brand, nameof(BrandService));
                return Brand;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Brand>> BulkDelete(List<Brand> Brands)
        {
            if (!await BrandValidator.BulkDelete(Brands))
                return Brands;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.BulkDelete(Brands);
                await UOW.Commit();
                
                List<long> Ids = Brands.Select(x => x.Id).ToList();
                Brands = await UOW.BrandRepository.List(Ids);
                Sync(Brands);
                await Logging.CreateAuditLog(new { }, Brands, nameof(BrandService));
                return Brands;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Brand>> Import(List<Brand> Brands)
        {
            if (!await BrandValidator.Import(Brands))
                return Brands;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.BulkMerge(Brands);
                await UOW.Commit();

                List<long> Ids = Brands.Select(x => x.Id).ToList();
                Brands = await UOW.BrandRepository.List(Ids);
                Sync(Brands);

                await Logging.CreateAuditLog(Brands, new { }, nameof(BrandService));
                return Brands;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private void Sync(List<Brand> Brands)
        {
            //RabbitManager.PublishList(Brands, RoutingKeyEnum.BrandSync);
        }
    }
}
