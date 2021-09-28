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
using ELODIE.Handlers;
using ELODIE.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using RestSharp;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace ELODIE.Services.MAppUser
{
    public interface IAppUserService : IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public AppUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            List<Site> Sites = await UOW.SiteRepository.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                IsDisplay = true,
                Selects = SiteSelect.ALL,
            });
            if (AppUser.AppUserSiteMappings == null)
                AppUser.AppUserSiteMappings = new List<AppUserSiteMapping>();
            foreach (Site Site in Sites)
            {
                AppUserSiteMapping AppUserSiteMapping = AppUser.AppUserSiteMappings.Where(s => s.SiteId == Site.Id).FirstOrDefault();
                if (AppUserSiteMapping == null)
                {
                    AppUserSiteMapping = new AppUserSiteMapping();
                    AppUserSiteMapping.SiteId = Site.Id;
                    AppUserSiteMapping.Site = Site;
                    AppUserSiteMapping.Enabled = false;
                    AppUser.AppUserSiteMappings.Add(AppUserSiteMapping);
                }
                else
                {
                    AppUserSiteMapping.Site = Site;
                    AppUserSiteMapping.AppUserId = AppUser.Id;
                }
            }
            {
                AppUserSiteMapping AppUserSiteMapping = AppUser.AppUserSiteMappings.Where(x => x.SiteId == SiteEnum.LANDING.Id).FirstOrDefault();
                if (AppUserSiteMapping != null)
                    AppUserSiteMapping.Enabled = true;
            }
            return AppUser;
        }
    }
}
