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

namespace ELODIE.Services.MRequestState
{
    public interface IRequestStateService :  IServiceScoped
    {
        Task<int> Count(RequestStateFilter RequestStateFilter);
        Task<List<RequestState>> List(RequestStateFilter RequestStateFilter);
    }

    public class RequestStateService : BaseService, IRequestStateService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public RequestStateService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(RequestStateFilter RequestStateFilter)
        {
            try
            {
                int result = await UOW.RequestStateRepository.Count(RequestStateFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(RequestStateService));
            }
            return 0;
        }

        public async Task<List<RequestState>> List(RequestStateFilter RequestStateFilter)
        {
            try
            {
                List<RequestState> RequestStates = await UOW.RequestStateRepository.List(RequestStateFilter);
                return RequestStates;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(RequestStateService));
            }
            return null;
        }

    }
}
