using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELODIE.Services.MRole
{
    public interface IPermissionOperatorService : IServiceScoped
    {
        Task<int> Count(PermissionOperatorFilter PermissionOperatorFilter);
        Task<List<PermissionOperator>> List(PermissionOperatorFilter PermissionOperatorFilter);
    }

    public class PermissionOperatorService : BaseService, IPermissionOperatorService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public PermissionOperatorService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(PermissionOperatorFilter PermissionOperatorFilter)
        {
            try
            {
                int result = await UOW.PermissionOperatorRepository.Count(PermissionOperatorFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PermissionOperatorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionOperatorService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<PermissionOperator>> List(PermissionOperatorFilter PermissionOperatorFilter)
        {
            try
            {
                List<PermissionOperator> PermissionOperators = await UOW.PermissionOperatorRepository.List(PermissionOperatorFilter);
                return PermissionOperators;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PermissionOperatorService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PermissionOperatorService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
