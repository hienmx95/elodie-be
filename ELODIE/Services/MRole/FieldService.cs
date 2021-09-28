using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELODIE.Services.MRole
{
    public interface IFieldService : IServiceScoped
    {
        Task<int> Count(FieldFilter FieldFilter);
        Task<List<Field>> List(FieldFilter FieldFilter);
    }

    public class FieldService : BaseService, IFieldService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public FieldService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(FieldFilter FieldFilter)
        {
            try
            {
                int result = await UOW.FieldRepository.Count(FieldFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(FieldService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Field>> List(FieldFilter FieldFilter)
        {
            try
            {
                List<Field> Fields = await UOW.FieldRepository.List(FieldFilter);
                return Fields;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(FieldService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
