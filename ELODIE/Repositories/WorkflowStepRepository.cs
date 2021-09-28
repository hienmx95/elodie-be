using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using ELODIE.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IWorkflowStepRepository
    {
        Task<int> Count(WorkflowStepFilter WorkflowStepFilter);
        Task<List<WorkflowStep>> List(WorkflowStepFilter WorkflowStepFilter);
        Task<WorkflowStep> Get(long Id);
        Task<bool> Create(WorkflowStep WorkflowStep);
        Task<bool> Update(WorkflowStep WorkflowStep);
        Task<bool> Delete(WorkflowStep WorkflowStep);
        Task<bool> BulkMerge(List<WorkflowStep> WorkflowSteps);
        Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps);
    }
    public class WorkflowStepRepository : IWorkflowStepRepository
    {
        private DataContext DataContext;
        public WorkflowStepRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowStepDAO> DynamicFilter(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.WorkflowDefinitionId != null)
                query = query.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.RoleId != null)
                query = query.Where(q => q.RoleId, filter.RoleId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowStepDAO> OrFilter(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowStepDAO> initQuery = query.Where(q => false);
            foreach (WorkflowStepFilter WorkflowStepFilter in filter.OrFilter)
            {
                IQueryable<WorkflowStepDAO> queryable = query;
                if (WorkflowStepFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowStepFilter.Id);
                if (WorkflowStepFilter.WorkflowDefinitionId != null)
                    queryable = queryable.Where(q => q.WorkflowDefinitionId, WorkflowStepFilter.WorkflowDefinitionId);
                if (WorkflowStepFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowStepFilter.Code);
                if (WorkflowStepFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowStepFilter.Name);
                if (WorkflowStepFilter.RoleId != null)
                    queryable = queryable.Where(q => q.RoleId, WorkflowStepFilter.RoleId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowStepDAO> DynamicOrder(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStepOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowStepOrder.WorkflowDefinition:
                            query = query.OrderBy(q => q.WorkflowDefinition.Name);
                            break;
                        case WorkflowStepOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WorkflowStepOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WorkflowStepOrder.Role:
                            query = query.OrderBy(q => q.Role.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStepOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowStepOrder.WorkflowDefinition:
                            query = query.OrderByDescending(q => q.WorkflowDefinition.Name);
                            break;
                        case WorkflowStepOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WorkflowStepOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WorkflowStepOrder.Role:
                            query = query.OrderByDescending(q => q.Role.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowStep>> DynamicSelect(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            List<WorkflowStep> WorkflowSteps = await query.Select(q => new WorkflowStep()
            {
                Id = filter.Selects.Contains(WorkflowStepSelect.Id) ? q.Id : default(long),
                WorkflowDefinitionId = filter.Selects.Contains(WorkflowStepSelect.WorkflowDefinition) ? q.WorkflowDefinitionId : default(long),
                Code = filter.Selects.Contains(WorkflowStepSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WorkflowStepSelect.Name) ? q.Name : default(string),
                RoleId = filter.Selects.Contains(WorkflowStepSelect.Role) ? q.RoleId : default(long),
                StatusId = filter.Selects.Contains(WorkflowStepSelect.Status) ? q.StatusId : default(long),
                Role = filter.Selects.Contains(WorkflowStepSelect.Role) && q.Role != null ? new Role
                {
                    Id = q.Role.Id,
                    Code = q.Role.Code,
                    Name = q.Role.Name,
                    StatusId = q.Role.StatusId,
                } : null,
                Status = filter.Selects.Contains(WorkflowStepSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                WorkflowDefinition = filter.Selects.Contains(WorkflowStepSelect.WorkflowDefinition) && q.WorkflowDefinition != null ? new WorkflowDefinition
                {
                    Id = q.WorkflowDefinition.Id,
                    Name = q.WorkflowDefinition.Name,
                    WorkflowTypeId = q.WorkflowDefinition.WorkflowTypeId,
                    StartDate = q.WorkflowDefinition.StartDate,
                    EndDate = q.WorkflowDefinition.EndDate,
                    StatusId = q.WorkflowDefinition.StatusId,
                } : null,
                Used = q.WorkflowDefinition.Used,
            }).ToListAsync();
            return WorkflowSteps;
        }

        public async Task<int> Count(WorkflowStepFilter filter)
        {
            IQueryable<WorkflowStepDAO> WorkflowSteps = DataContext.WorkflowStep.AsNoTracking();
            WorkflowSteps = DynamicFilter(WorkflowSteps, filter);
            return await WorkflowSteps.CountAsync();
        }

        public async Task<List<WorkflowStep>> List(WorkflowStepFilter filter)
        {
            if (filter == null) return new List<WorkflowStep>();
            IQueryable<WorkflowStepDAO> WorkflowStepDAOs = DataContext.WorkflowStep.AsNoTracking();
            WorkflowStepDAOs = DynamicFilter(WorkflowStepDAOs, filter);
            WorkflowStepDAOs = DynamicOrder(WorkflowStepDAOs, filter);
            List<WorkflowStep> WorkflowSteps = await DynamicSelect(WorkflowStepDAOs, filter);
            return WorkflowSteps;
        }

        public async Task<WorkflowStep> Get(long Id)
        {
            WorkflowStep WorkflowStep = await DataContext.WorkflowStep.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new WorkflowStep()
            {
                Id = x.Id,
                WorkflowDefinitionId = x.WorkflowDefinitionId,
                Code = x.Code,
                Name = x.Name,
                RoleId = x.RoleId,
                StatusId = x.StatusId,
                SubjectMailForReject = x.SubjectMailForReject,
                BodyMailForReject = x.BodyMailForReject,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Role = x.Role == null ? null : new Role
                {
                    Id = x.Role.Id,
                    Code = x.Role.Code,
                    Name = x.Role.Name,
                    StatusId = x.Role.StatusId,
                },
                WorkflowDefinition = x.WorkflowDefinition == null ? null : new WorkflowDefinition
                {
                    Id = x.WorkflowDefinition.Id,
                    Code = x.WorkflowDefinition.Code,
                    Name = x.WorkflowDefinition.Name,
                    WorkflowTypeId = x.WorkflowDefinition.WorkflowTypeId,
                    StartDate = x.WorkflowDefinition.StartDate,
                    EndDate = x.WorkflowDefinition.EndDate,
                    StatusId = x.WorkflowDefinition.StatusId,
                    UpdatedAt = x.WorkflowDefinition.UpdatedAt,
                },
                Used = x.WorkflowDefinition.Used,
            }).FirstOrDefaultAsync();

            if (WorkflowStep == null)
                return null;

            return WorkflowStep;
        }
        public async Task<bool> Create(WorkflowStep WorkflowStep)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = await DataContext.WorkflowDefinition
                .Where(x => x.Id == WorkflowStep.WorkflowDefinitionId)
                .FirstOrDefaultAsync();
            if (WorkflowDefinitionDAO == null)
                return false;
            WorkflowDefinitionDAO.ModifierId = WorkflowStep.ModifierId;
            await DataContext.SaveChangesAsync();
            if (WorkflowDefinitionDAO.Used == false)
            {
                WorkflowStepDAO WorkflowStepDAO = new WorkflowStepDAO();
                WorkflowStepDAO.Id = WorkflowStep.Id;
                WorkflowStepDAO.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
                WorkflowStepDAO.Code = WorkflowStep.Code;
                WorkflowStepDAO.Name = WorkflowStep.Name;
                WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
                WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
                WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
                WorkflowStepDAO.UpdatedAt = StaticParams.DateTimeNow;
                WorkflowStepDAO.StatusId = WorkflowStep.StatusId;
                DataContext.WorkflowStep.Add(WorkflowStepDAO);
                await DataContext.SaveChangesAsync();
                WorkflowStep.Id = WorkflowStepDAO.Id;
            }
            return true;
        }

        public async Task<bool> Update(WorkflowStep WorkflowStep)
        {
            WorkflowStepDAO WorkflowStepDAO = await DataContext.WorkflowStep
                .Where(x => x.Id == WorkflowStep.Id)
                .FirstOrDefaultAsync();
            if (WorkflowStepDAO == null)
                return false;
            WorkflowDefinitionDAO WorkflowDefinitionDAO = await DataContext.WorkflowDefinition
                .Where(x => x.Id == WorkflowStepDAO.WorkflowDefinitionId)
                .FirstOrDefaultAsync();
            if (WorkflowDefinitionDAO == null)
                return false;
            WorkflowDefinitionDAO.ModifierId = WorkflowStep.ModifierId;
            await DataContext.SaveChangesAsync();

            WorkflowStepDAO.Code = WorkflowStep.Code;
            WorkflowStepDAO.Name = WorkflowStep.Name;
            WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
            WorkflowStepDAO.StatusId = WorkflowStep.StatusId;
            WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
            WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
            WorkflowStepDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(WorkflowStep WorkflowStep)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = await DataContext.WorkflowDefinition
                .Where(x => x.Id == WorkflowStep.WorkflowDefinitionId)
                .FirstOrDefaultAsync();
            if (WorkflowDefinitionDAO == null)
                return false;
            if (WorkflowDefinitionDAO.Used == false)
            {
                WorkflowDefinitionDAO.ModifierId = WorkflowStep.ModifierId;
                await DataContext.SaveChangesAsync();

                await DataContext.WorkflowDirection.Where(x => x.FromStepId == WorkflowStep.Id).DeleteFromQueryAsync();
                await DataContext.WorkflowDirection.Where(x => x.ToStepId == WorkflowStep.Id).DeleteFromQueryAsync();
                await DataContext.WorkflowStep.Where(x => x.Id == WorkflowStep.Id).DeleteFromQueryAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> BulkMerge(List<WorkflowStep> WorkflowSteps)
        {
            List<WorkflowStepDAO> WorkflowStepDAOs = new List<WorkflowStepDAO>();
            foreach (WorkflowStep WorkflowStep in WorkflowSteps)
            {
                WorkflowStepDAO WorkflowStepDAO = new WorkflowStepDAO();
                WorkflowStepDAO.Id = WorkflowStep.Id;
                WorkflowStepDAO.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
                WorkflowStepDAO.Code = WorkflowStep.Code;
                WorkflowStepDAO.Name = WorkflowStep.Name;
                WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
                WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
                WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
                WorkflowStepDAO.UpdatedAt = StaticParams.DateTimeNow;
                WorkflowStepDAOs.Add(WorkflowStepDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowStepDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps)
        {
            List<long> Ids = WorkflowSteps.Select(x => x.Id).ToList();
            await DataContext.WorkflowDirection.Where(x => Ids.Contains(x.FromStepId)).DeleteFromQueryAsync();
            await DataContext.WorkflowDirection.Where(x => Ids.Contains(x.ToStepId)).DeleteFromQueryAsync();
            await DataContext.WorkflowStep
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

    }
}
