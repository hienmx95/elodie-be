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
using ELODIE.Handlers;

namespace ELODIE.Services.MItemHistory
{
    public interface IItemHistoryService :  IServiceScoped
    {
        Task<int> Count(ItemHistoryFilter ItemHistoryFilter);
        Task<List<ItemHistory>> List(ItemHistoryFilter ItemHistoryFilter);
        Task<ItemHistory> Get(long Id);
        Task<ItemHistory> Create(ItemHistory ItemHistory);
        Task<ItemHistory> Update(ItemHistory ItemHistory);
        Task<ItemHistory> Delete(ItemHistory ItemHistory);
        Task<List<ItemHistory>> BulkDelete(List<ItemHistory> ItemHistories);
        Task<List<ItemHistory>> Import(List<ItemHistory> ItemHistories);
        ItemHistoryFilter ToFilter(ItemHistoryFilter ItemHistoryFilter);
    }

    public class ItemHistoryService : BaseService, IItemHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IItemHistoryValidator ItemHistoryValidator;

        public ItemHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IItemHistoryValidator ItemHistoryValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ItemHistoryValidator = ItemHistoryValidator;
        }
        public async Task<int> Count(ItemHistoryFilter ItemHistoryFilter)
        {
            try
            {
                int result = await UOW.ItemHistoryRepository.Count(ItemHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<ItemHistory>> List(ItemHistoryFilter ItemHistoryFilter)
        {
            try
            {
                List<ItemHistory> ItemHistorys = await UOW.ItemHistoryRepository.List(ItemHistoryFilter);
                return ItemHistorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<ItemHistory> Get(long Id)
        {
            ItemHistory ItemHistory = await UOW.ItemHistoryRepository.Get(Id);
            if (ItemHistory == null)
                return null;
            return ItemHistory;
        }
       
        public async Task<ItemHistory> Create(ItemHistory ItemHistory)
        {
            if (!await ItemHistoryValidator.Create(ItemHistory))
                return ItemHistory;

            try
            {
                await UOW.Begin();
                await UOW.ItemHistoryRepository.Create(ItemHistory);
                await UOW.Commit();
                await Logging.CreateAuditLog(ItemHistory, new { }, nameof(ItemHistoryService));
                return await UOW.ItemHistoryRepository.Get(ItemHistory.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ItemHistory> Update(ItemHistory ItemHistory)
        {
            if (!await ItemHistoryValidator.Update(ItemHistory))
                return ItemHistory;
            try
            {
                var oldData = await UOW.ItemHistoryRepository.Get(ItemHistory.Id);

                await UOW.Begin();
                await UOW.ItemHistoryRepository.Update(ItemHistory);
                await UOW.Commit();

                var newData = await UOW.ItemHistoryRepository.Get(ItemHistory.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ItemHistoryService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ItemHistory> Delete(ItemHistory ItemHistory)
        {
            if (!await ItemHistoryValidator.Delete(ItemHistory))
                return ItemHistory;

            try
            {
                await UOW.Begin();
                await UOW.ItemHistoryRepository.Delete(ItemHistory);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemHistory, nameof(ItemHistoryService));
                return ItemHistory;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<ItemHistory>> BulkDelete(List<ItemHistory> ItemHistories)
        {
            if (!await ItemHistoryValidator.BulkDelete(ItemHistories))
                return ItemHistories;

            try
            {
                await UOW.Begin();
                await UOW.ItemHistoryRepository.BulkDelete(ItemHistories);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemHistories, nameof(ItemHistoryService));
                return ItemHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<ItemHistory>> Import(List<ItemHistory> ItemHistories)
        {
            if (!await ItemHistoryValidator.Import(ItemHistories))
                return ItemHistories;
            try
            {
                await UOW.Begin();
                await UOW.ItemHistoryRepository.BulkMerge(ItemHistories);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemHistories, new { }, nameof(ItemHistoryService));
                return ItemHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public ItemHistoryFilter ToFilter(ItemHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemHistoryFilter subFilter = new ItemHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
