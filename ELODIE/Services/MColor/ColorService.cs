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

namespace ELODIE.Services.MColor
{
    public interface IColorService :  IServiceScoped
    {
        Task<int> Count(ColorFilter ColorFilter);
        Task<List<Color>> List(ColorFilter ColorFilter);
    }

    public class ColorService : BaseService, IColorService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ColorService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ColorFilter ColorFilter)
        {
            try
            {
                int result = await UOW.ColorRepository.Count(ColorFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ColorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ColorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Color>> List(ColorFilter ColorFilter)
        {
            try
            {
                List<Color> Colors = await UOW.ColorRepository.List(ColorFilter);
                return Colors;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ColorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ColorService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
