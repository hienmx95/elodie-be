using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Helpers;
using ELODIE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Services.MOrganization
{
    public interface IOrganizationService : IServiceScoped
    {
        Task<int> Count(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(OrganizationFilter OrganizationFilter);
        Task<Organization> Get(long Id);
        Task<Organization> Create(Organization Organization);
        Task<Organization> Update(Organization Organization);
        Task<Organization> Delete(Organization Organization);
        OrganizationFilter ToFilter(OrganizationFilter OrganizationFilter);
        Task<Organization> UpdateIsDisplay(Organization Organization);
    }

    public class OrganizationService : BaseService, IOrganizationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IOrganizationValidator OrganizationValidator;
        public OrganizationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IOrganizationValidator OrganizationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.OrganizationValidator = OrganizationValidator;
        }
        public async Task<int> Count(OrganizationFilter OrganizationFilter)
        {
            try
            {
                int result = await UOW.OrganizationRepository.Count(OrganizationFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(OrganizationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(OrganizationService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Organization>> List(OrganizationFilter OrganizationFilter)
        {
            try
            {
                List<Organization> Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
                foreach (Organization Organization in Organizations)
                {
                    if (Organization.ParentId.HasValue)
                    {
                        Organization Parent = Organizations.Where(o => o.Id == Organization.ParentId.Value).FirstOrDefault();
                        if (Parent == null)
                            Organization.ParentId = null;
                    }
                }

                return Organizations;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(OrganizationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(OrganizationService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Organization> Get(long Id)
        {
            Organization Organization = await UOW.OrganizationRepository.Get(Id);
            if (Organization == null)
                return null;
            return Organization;
        }

        public async Task<Organization> Create(Organization Organization)
        {
            if (!await OrganizationValidator.Create(Organization))
                return Organization;

            try
            {
                await UOW.OrganizationRepository.Create(Organization);
                Organization = await UOW.OrganizationRepository.Get(Organization.Id);
                await Logging.CreateAuditLog(Organization, new { }, nameof(OrganizationService));
                return Organization;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrganizationService));
            }
            return null;
        }

        public async Task<Organization> Update(Organization Organization)
        {
            if (!await OrganizationValidator.Update(Organization))
                return Organization;
            try
            {
                var oldData = await UOW.OrganizationRepository.Get(Organization.Id);

                await UOW.OrganizationRepository.Update(Organization);

                Organization = await UOW.OrganizationRepository.Get(Organization.Id);
                await Logging.CreateAuditLog(Organization, oldData, nameof(OrganizationService));
                return Organization;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrganizationService));
            }
            return null;
        }

        public async Task<Organization> Delete(Organization Organization)
        {
            if (!await OrganizationValidator.Delete(Organization))
                return Organization;

            try
            {
                await UOW.OrganizationRepository.Delete(Organization);
                await Logging.CreateAuditLog(new { }, Organization, nameof(OrganizationService));
                return Organization;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrganizationService));
            }
            return null;
        }

        public async Task<Organization> UpdateIsDisplay(Organization Organization)
        {
            try
            {
                var oldData = await UOW.OrganizationRepository.Get(Organization.Id);
                await UOW.OrganizationRepository.UpdateIsDisplay(Organization);
                await Logging.CreateAuditLog(Organization, oldData, nameof(OrganizationService));
                return Organization;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(OrganizationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(OrganizationService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public OrganizationFilter ToFilter(OrganizationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<OrganizationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                OrganizationFilter subFilter = new OrganizationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterPermissionDefinition.StringFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterPermissionDefinition.StringFilter;
                }
            }
            return filter;
        }
    }
}
