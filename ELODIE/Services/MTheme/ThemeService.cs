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

namespace ELODIE.Services.MTheme
{
    public interface IThemeService :  IServiceScoped
    {
        Task<int> Count(ThemeFilter ThemeFilter);
        Task<List<Theme>> List(ThemeFilter ThemeFilter);
    }

    public class ThemeService : BaseService, IThemeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ThemeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ThemeFilter ThemeFilter)
        {
            try
            {
                int result = await UOW.ThemeRepository.Count(ThemeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ThemeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ThemeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Theme>> List(ThemeFilter ThemeFilter)
        {
            try
            {
                List<Theme> Themes = await UOW.ThemeRepository.List(ThemeFilter);
                return Themes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ThemeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ThemeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
