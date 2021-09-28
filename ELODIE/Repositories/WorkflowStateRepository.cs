using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IWorkflowStateRepository
    {
        Task<int> Count(WorkflowStateFilter WorkflowStateFilter);
        Task<List<WorkflowState>> List(WorkflowStateFilter WorkflowStateFilter);
        Task<WorkflowState> Get(long Id);
    }
    public class WorkflowStateRepository : IWorkflowStateRepository
    {
        private DataContext DataContext;
        public WorkflowStateRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowStateDAO> DynamicFilter(IQueryable<WorkflowStateDAO> query, WorkflowStateFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowStateDAO> OrFilter(IQueryable<WorkflowStateDAO> query, WorkflowStateFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowStateDAO> initQuery = query.Where(q => false);
            foreach (WorkflowStateFilter WorkflowStateFilter in filter.OrFilter)
            {
                IQueryable<WorkflowStateDAO> queryable = query;
                if (WorkflowStateFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowStateFilter.Id);
                if (WorkflowStateFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowStateFilter.Code);
                if (WorkflowStateFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowStateFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowStateDAO> DynamicOrder(IQueryable<WorkflowStateDAO> query, WorkflowStateFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStateOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowStateOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WorkflowStateOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStateOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowStateOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WorkflowStateOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowState>> DynamicSelect(IQueryable<WorkflowStateDAO> query, WorkflowStateFilter filter)
        {
            List<WorkflowState> WorkflowStates = await query.Select(q => new WorkflowState()
            {
                Id = filter.Selects.Contains(WorkflowStateSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(WorkflowStateSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WorkflowStateSelect.Name) ? q.Name : default(string),
            }).AsNoTracking().ToListAsync();
            return WorkflowStates;
        }

        public async Task<int> Count(WorkflowStateFilter filter)
        {
            IQueryable<WorkflowStateDAO> WorkflowStates = DataContext.WorkflowState;
            WorkflowStates = DynamicFilter(WorkflowStates, filter);
            return await WorkflowStates.CountAsync();
        }

        public async Task<List<WorkflowState>> List(WorkflowStateFilter filter)
        {
            if (filter == null) return new List<WorkflowState>();
            IQueryable<WorkflowStateDAO> WorkflowStateDAOs = DataContext.WorkflowState;
            WorkflowStateDAOs = DynamicFilter(WorkflowStateDAOs, filter);
            WorkflowStateDAOs = DynamicOrder(WorkflowStateDAOs, filter);
            List<WorkflowState> WorkflowStates = await DynamicSelect(WorkflowStateDAOs, filter);
            return WorkflowStates;
        }

        public async Task<WorkflowState> Get(long Id)
        {
            WorkflowState WorkflowState = await DataContext.WorkflowState.Where(x => x.Id == Id).Select(x => new WorkflowState()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (WorkflowState == null)
                return null;

            return WorkflowState;
        }
    }
}
