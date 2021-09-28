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

namespace ELODIE.Services.MNation
{
    public interface INationService :  IServiceScoped
    {
        Task<int> Count(NationFilter NationFilter);
        Task<List<Nation>> List(NationFilter NationFilter);
        Task<Nation> Get(long Id);
        Task<Nation> Create(Nation Nation);
        Task<Nation> Update(Nation Nation);
        Task<Nation> Delete(Nation Nation);
        Task<List<Nation>> BulkDelete(List<Nation> Nations);
        Task<List<Nation>> Import(List<Nation> Nations);
        Task<NationFilter> ToFilter(NationFilter NationFilter);
    }

    public class NationService : BaseService, INationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INationValidator NationValidator;
        //private IRabbitManager RabbitManager;

        public NationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            INationValidator NationValidator,
            ILogging Logging
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NationValidator = NationValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(NationFilter NationFilter)
        {
            try
            {
                int result = await UOW.NationRepository.Count(NationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return 0;
        }

        public async Task<List<Nation>> List(NationFilter NationFilter)
        {
            try
            {
                List<Nation> Nations = await UOW.NationRepository.List(NationFilter);
                return Nations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;
        }
        
        public async Task<Nation> Get(long Id)
        {
            Nation Nation = await UOW.NationRepository.Get(Id);
            if (Nation == null)
                return null;
            return Nation;
        }
        public async Task<Nation> Create(Nation Nation)
        {
            if (!await NationValidator.Create(Nation))
                return Nation;

            try
            {
                await UOW.NationRepository.Create(Nation);
                var Nations = await UOW.NationRepository.List(new List<long> { Nation.Id });
                Sync(Nations);
                await Logging.CreateAuditLog(Nation, new { }, nameof(NationService));
                return Nation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;
        }

        public async Task<Nation> Update(Nation Nation)
        {
            if (!await NationValidator.Update(Nation))
                return Nation;
            try
            {
                var oldData = await UOW.NationRepository.Get(Nation.Id);

                await UOW.NationRepository.Update(Nation);

                var Nations = await UOW.NationRepository.List(new List<long> { Nation.Id });
                Sync(Nations);
                await Logging.CreateAuditLog(Nation, new { }, nameof(NationService));
                return Nation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;
        }

        public async Task<Nation> Delete(Nation Nation)
        {
            if (!await NationValidator.Delete(Nation))
                return Nation;

            try
            {
                await UOW.NationRepository.Delete(Nation);
                var Nations = await UOW.NationRepository.List(new List<long> { Nation.Id });
                Sync(Nations);
                await Logging.CreateAuditLog(Nation, new { }, nameof(NationService));
                return Nation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;
        }

        public async Task<List<Nation>> BulkDelete(List<Nation> Nations)
        {
            if (!await NationValidator.BulkDelete(Nations))
                return Nations;

            try
            {
                await UOW.NationRepository.BulkDelete(Nations);
                var Ids = Nations.Select(x => x.Id).ToList();
                Nations = await UOW.NationRepository.List(Ids);
                Sync(Nations);
                await Logging.CreateAuditLog(new { }, Nations, nameof(NationService));
                return Nations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;

        }
        
        public async Task<List<Nation>> Import(List<Nation> Nations)
        {
            if (!await NationValidator.Import(Nations))
                return Nations;
            try
            {
                await UOW.NationRepository.BulkMerge(Nations);
                var Ids = Nations.Select(x => x.Id).ToList();
                Nations = await UOW.NationRepository.List(Ids);
                Sync(Nations);
                await Logging.CreateAuditLog(Nations, new { }, nameof(NationService));
                return Nations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(NationService));
            }
            return null;
        }     
        
        public async Task<NationFilter> ToFilter(NationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<NationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                NationFilter subFilter = new NationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<Nation> Nations)
        {
            //RabbitManager.PublishList(Nations, RoutingKeyEnum.NationSync);
        }
    }
}
