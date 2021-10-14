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

namespace ELODIE.Services.MOrderSource
{
    public interface IOrderSourceService :  IServiceScoped
    {
        Task<int> Count(OrderSourceFilter OrderSourceFilter);
        Task<List<OrderSource>> List(OrderSourceFilter OrderSourceFilter);
        Task<OrderSource> Get(long Id);
        Task<OrderSource> Create(OrderSource OrderSource);
        Task<OrderSource> Update(OrderSource OrderSource);
        Task<OrderSource> Delete(OrderSource OrderSource);
        Task<List<OrderSource>> BulkDelete(List<OrderSource> OrderSources);
        Task<List<OrderSource>> Import(List<OrderSource> OrderSources);
        Task<OrderSourceFilter> ToFilter(OrderSourceFilter OrderSourceFilter);
    }

    public class OrderSourceService : BaseService, IOrderSourceService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IOrderSourceValidator OrderSourceValidator;

        public OrderSourceService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IOrderSourceValidator OrderSourceValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.OrderSourceValidator = OrderSourceValidator;
        }
        public async Task<int> Count(OrderSourceFilter OrderSourceFilter)
        {
            try
            {
                int result = await UOW.OrderSourceRepository.Count(OrderSourceFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return 0;
        }

        public async Task<List<OrderSource>> List(OrderSourceFilter OrderSourceFilter)
        {
            try
            {
                List<OrderSource> OrderSources = await UOW.OrderSourceRepository.List(OrderSourceFilter);
                return OrderSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;
        }

        public async Task<OrderSource> Get(long Id)
        {
            OrderSource OrderSource = await UOW.OrderSourceRepository.Get(Id);
            await OrderSourceValidator.Get(OrderSource);
            if (OrderSource == null)
                return null;
            return OrderSource;
        }
        
        public async Task<OrderSource> Create(OrderSource OrderSource)
        {
            if (!await OrderSourceValidator.Create(OrderSource))
                return OrderSource;

            try
            {
                await UOW.OrderSourceRepository.Create(OrderSource);
                OrderSource = await UOW.OrderSourceRepository.Get(OrderSource.Id);
                await Logging.CreateAuditLog(OrderSource, new { }, nameof(OrderSourceService));
                return OrderSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;
        }

        public async Task<OrderSource> Update(OrderSource OrderSource)
        {
            if (!await OrderSourceValidator.Update(OrderSource))
                return OrderSource;
            try
            {
                var oldData = await UOW.OrderSourceRepository.Get(OrderSource.Id);

                await UOW.OrderSourceRepository.Update(OrderSource);

                OrderSource = await UOW.OrderSourceRepository.Get(OrderSource.Id);
                await Logging.CreateAuditLog(OrderSource, oldData, nameof(OrderSourceService));
                return OrderSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;
        }

        public async Task<OrderSource> Delete(OrderSource OrderSource)
        {
            if (!await OrderSourceValidator.Delete(OrderSource))
                return OrderSource;

            try
            {
                await UOW.OrderSourceRepository.Delete(OrderSource);
                await Logging.CreateAuditLog(new { }, OrderSource, nameof(OrderSourceService));
                return OrderSource;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;
        }

        public async Task<List<OrderSource>> BulkDelete(List<OrderSource> OrderSources)
        {
            if (!await OrderSourceValidator.BulkDelete(OrderSources))
                return OrderSources;

            try
            {
                await UOW.OrderSourceRepository.BulkDelete(OrderSources);
                await Logging.CreateAuditLog(new { }, OrderSources, nameof(OrderSourceService));
                return OrderSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;

        }
        
        public async Task<List<OrderSource>> Import(List<OrderSource> OrderSources)
        {
            if (!await OrderSourceValidator.Import(OrderSources))
                return OrderSources;
            try
            {
                await UOW.OrderSourceRepository.BulkMerge(OrderSources);

                await Logging.CreateAuditLog(OrderSources, new { }, nameof(OrderSourceService));
                return OrderSources;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderSourceService));
            }
            return null;
        }     
        
        public async Task<OrderSourceFilter> ToFilter(OrderSourceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<OrderSourceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                OrderSourceFilter subFilter = new OrderSourceFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Priority))
                        subFilter.Priority = FilterBuilder.Merge(subFilter.Priority, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
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
