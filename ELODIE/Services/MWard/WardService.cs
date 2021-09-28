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

namespace ELODIE.Services.MWard
{
    public interface IWardService : IServiceScoped
    {
        Task<int> Count(WardFilter WardFilter);
        Task<List<Ward>> List(WardFilter WardFilter);
        Task<Ward> Get(long Id);
        Task<Ward> Create(Ward Ward);
        Task<Ward> Update(Ward Ward);
        Task<Ward> Delete(Ward Ward);
        Task<List<Ward>> BulkDelete(List<Ward> Wards);
        WardFilter ToFilter(WardFilter WardFilter);
    }

    public class WardService : BaseService, IWardService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWardValidator WardValidator;
        //private IRabbitManager RabbitManager;
        public WardService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWardValidator WardValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WardValidator = WardValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(WardFilter WardFilter)
        {
            try
            {
                int result = await UOW.WardRepository.Count(WardFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Ward>> List(WardFilter WardFilter)
        {
            try
            {
                List<Ward> Wards = await UOW.WardRepository.List(WardFilter);
                return Wards;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Ward> Get(long Id)
        {
            Ward Ward = await UOW.WardRepository.Get(Id);
            if (Ward == null)
                return null;
            return Ward;
        }

        public async Task<Ward> Create(Ward Ward)
        {
            if (!await WardValidator.Create(Ward))
                return Ward;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.Create(Ward);
                await UOW.Commit();

                List<Ward> Wards = await UOW.WardRepository.List(new List<long> { Ward.Id });
                Sync(Wards);
                Ward = Wards.FirstOrDefault();
                await Logging.CreateAuditLog(Ward, new { }, nameof(WardService));
                return Ward;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Ward> Update(Ward Ward)
        {
            if (!await WardValidator.Update(Ward))
                return Ward;
            try
            {
                var oldData = await UOW.WardRepository.Get(Ward.Id);

                await UOW.Begin();
                await UOW.WardRepository.Update(Ward);
                await UOW.Commit();

                List<Ward> Wards = await UOW.WardRepository.List(new List<long> { Ward.Id });
                Sync(Wards);
                Ward = Wards.FirstOrDefault();
                await Logging.CreateAuditLog(Ward, oldData, nameof(WardService));
                return Ward;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Ward> Delete(Ward Ward)
        {
            if (!await WardValidator.Delete(Ward))
                return Ward;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.Delete(Ward);
                await UOW.Commit();

                List<Ward> Wards = await UOW.WardRepository.List(new List<long> { Ward.Id });
                Sync(Wards);
                Ward = Wards.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, Ward, nameof(WardService));
                return Ward;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Ward>> BulkDelete(List<Ward> Wards)
        {
            if (!await WardValidator.BulkDelete(Wards))
                return Wards;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.BulkDelete(Wards);
                await UOW.Commit();

                List<long> Ids = Wards.Select(x => x.Id).ToList();
                Wards = await UOW.WardRepository.List(Ids);
                Sync(Wards);

                await Logging.CreateAuditLog(new { }, Wards, nameof(WardService));
                return Wards;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WardService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public WardFilter ToFilter(WardFilter filter)
        {
           
            return filter;
        }

        private void Sync(List<Ward> Wards)
        {
            List<District> Districts = Wards.Select(x => new District { Id = x.DistrictId }).Distinct().ToList();
            //RabbitManager.PublishList(Wards, RoutingKeyEnum.WardSync);
            //RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictUsed);
        }
    }
}
