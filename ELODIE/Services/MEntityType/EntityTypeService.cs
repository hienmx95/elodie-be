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

namespace ELODIE.Services.MEntityType
{
    public interface IEntityTypeService :  IServiceScoped
    {
        Task<int> Count(EntityTypeFilter EntityTypeFilter);
        Task<List<EntityType>> List(EntityTypeFilter EntityTypeFilter);
    }

    public class EntityTypeService : BaseService, IEntityTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public EntityTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(EntityTypeFilter EntityTypeFilter)
        {
            try
            {
                int result = await UOW.EntityTypeRepository.Count(EntityTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(EntityTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(EntityTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<EntityType>> List(EntityTypeFilter EntityTypeFilter)
        {
            try
            {
                List<EntityType> EntityTypes = await UOW.EntityTypeRepository.List(EntityTypeFilter);
                return EntityTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(EntityTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(EntityTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
