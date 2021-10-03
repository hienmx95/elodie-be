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

namespace ELODIE.Services.MProfession
{
    public interface IProfessionService :  IServiceScoped
    {
        Task<int> Count(ProfessionFilter ProfessionFilter);
        Task<List<Profession>> List(ProfessionFilter ProfessionFilter);
        Task<Profession> Get(long Id);
        Task<Profession> Create(Profession Profession);
        Task<Profession> Update(Profession Profession);
        Task<Profession> Delete(Profession Profession);
        Task<List<Profession>> BulkDelete(List<Profession> Professions);
        Task<List<Profession>> Import(List<Profession> Professions);
        Task<ProfessionFilter> ToFilter(ProfessionFilter ProfessionFilter);
    }

    public class ProfessionService : BaseService, IProfessionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProfessionValidator ProfessionValidator;

        public ProfessionService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IProfessionValidator ProfessionValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProfessionValidator = ProfessionValidator;
        }
        public async Task<int> Count(ProfessionFilter ProfessionFilter)
        {
            try
            {
                int result = await UOW.ProfessionRepository.Count(ProfessionFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return 0;
        }

        public async Task<List<Profession>> List(ProfessionFilter ProfessionFilter)
        {
            try
            {
                List<Profession> Professions = await UOW.ProfessionRepository.List(ProfessionFilter);
                return Professions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;
        }

        public async Task<Profession> Get(long Id)
        {
            Profession Profession = await UOW.ProfessionRepository.Get(Id);
            await ProfessionValidator.Get(Profession);
            if (Profession == null)
                return null;
            return Profession;
        }
        
        public async Task<Profession> Create(Profession Profession)
        {
            if (!await ProfessionValidator.Create(Profession))
                return Profession;

            try
            {
                await UOW.ProfessionRepository.Create(Profession);
                Profession = await UOW.ProfessionRepository.Get(Profession.Id);
                await Logging.CreateAuditLog(Profession, new { }, nameof(ProfessionService));
                return Profession;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;
        }

        public async Task<Profession> Update(Profession Profession)
        {
            if (!await ProfessionValidator.Update(Profession))
                return Profession;
            try
            {
                var oldData = await UOW.ProfessionRepository.Get(Profession.Id);

                await UOW.ProfessionRepository.Update(Profession);

                Profession = await UOW.ProfessionRepository.Get(Profession.Id);
                await Logging.CreateAuditLog(Profession, oldData, nameof(ProfessionService));
                return Profession;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;
        }

        public async Task<Profession> Delete(Profession Profession)
        {
            if (!await ProfessionValidator.Delete(Profession))
                return Profession;

            try
            {
                await UOW.ProfessionRepository.Delete(Profession);
                await Logging.CreateAuditLog(new { }, Profession, nameof(ProfessionService));
                return Profession;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;
        }

        public async Task<List<Profession>> BulkDelete(List<Profession> Professions)
        {
            if (!await ProfessionValidator.BulkDelete(Professions))
                return Professions;

            try
            {
                await UOW.ProfessionRepository.BulkDelete(Professions);
                await Logging.CreateAuditLog(new { }, Professions, nameof(ProfessionService));
                return Professions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;

        }
        
        public async Task<List<Profession>> Import(List<Profession> Professions)
        {
            if (!await ProfessionValidator.Import(Professions))
                return Professions;
            try
            {
                await UOW.ProfessionRepository.BulkMerge(Professions);

                await Logging.CreateAuditLog(Professions, new { }, nameof(ProfessionService));
                return Professions;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ProfessionService));
            }
            return null;
        }     
        
        public async Task<ProfessionFilter> ToFilter(ProfessionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProfessionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProfessionFilter subFilter = new ProfessionFilter();
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
    }
}
