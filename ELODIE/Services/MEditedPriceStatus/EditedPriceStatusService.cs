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

namespace ELODIE.Services.MEditedPriceStatus
{
    public interface IEditedPriceStatusService :  IServiceScoped
    {
        Task<int> Count(EditedPriceStatusFilter EditedPriceStatusFilter);
        Task<List<EditedPriceStatus>> List(EditedPriceStatusFilter EditedPriceStatusFilter);
    }

    public class EditedPriceStatusService : BaseService, IEditedPriceStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public EditedPriceStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(EditedPriceStatusFilter EditedPriceStatusFilter)
        {
            try
            {
                int result = await UOW.EditedPriceStatusRepository.Count(EditedPriceStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(EditedPriceStatusService));
            }
            return 0;
        }

        public async Task<List<EditedPriceStatus>> List(EditedPriceStatusFilter EditedPriceStatusFilter)
        {
            try
            {
                List<EditedPriceStatus> EditedPriceStatuses = await UOW.EditedPriceStatusRepository.List(EditedPriceStatusFilter);
                return EditedPriceStatuses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(EditedPriceStatusService));
            }
            return null;
        }

    }
}
