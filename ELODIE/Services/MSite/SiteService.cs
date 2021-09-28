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
using ELODIE.Handlers;

namespace ELODIE.Services.MSite
{
    public interface ISiteService : IServiceScoped
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
    }

    public class SiteService : BaseService, ISiteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public SiteService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(SiteFilter SiteFilter)
        {
            try
            {
                int result = await UOW.SiteRepository.Count(SiteFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SiteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Site>> List(SiteFilter SiteFilter)
        {
            try
            {
                List<Site> Sites = await UOW.SiteRepository.List(SiteFilter);
                return Sites;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SiteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SiteService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Site> Get(long Id)
        {
            Site Site = await UOW.SiteRepository.Get(Id);
            if (Site == null)
                return null;
            return Site;
        }
    }
}
