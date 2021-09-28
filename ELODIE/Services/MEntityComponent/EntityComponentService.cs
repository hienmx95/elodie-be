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

namespace ELODIE.Services.MEntityComponent
{
    public interface IEntityComponentService :  IServiceScoped
    {
        Task<int> Count(EntityComponentFilter EntityComponentFilter);
        Task<List<EntityComponent>> List(EntityComponentFilter EntityComponentFilter);
    }

    public class EntityComponentService : BaseService, IEntityComponentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public EntityComponentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(EntityComponentFilter EntityComponentFilter)
        {
            try
            {
                int result = await UOW.EntityComponentRepository.Count(EntityComponentFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(EntityComponentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(EntityComponentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<EntityComponent>> List(EntityComponentFilter EntityComponentFilter)
        {
            try
            {
                List<EntityComponent> EntityComponents = await UOW.EntityComponentRepository.List(EntityComponentFilter);
                return EntityComponents;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(EntityComponentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(EntityComponentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
