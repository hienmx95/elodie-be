using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE;
using ELODIE.Repositories;

namespace ELODIE.Services.MItemHistory
{
    public interface IItemHistoryValidator : IServiceScoped
    {
        Task<bool> Create(ItemHistory ItemHistory);
        Task<bool> Update(ItemHistory ItemHistory);
        Task<bool> Delete(ItemHistory ItemHistory);
        Task<bool> BulkDelete(List<ItemHistory> ItemHistories);
        Task<bool> Import(List<ItemHistory> ItemHistories);
    }

    public class ItemHistoryValidator : IItemHistoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemHistoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ItemHistory ItemHistory)
        {
            ItemHistoryFilter ItemHistoryFilter = new ItemHistoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ItemHistory.Id },
                Selects = ItemHistorySelect.Id
            };

            int count = await UOW.ItemHistoryRepository.Count(ItemHistoryFilter);
            if (count == 0)
                ItemHistory.AddError(nameof(ItemHistoryValidator), nameof(ItemHistory.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ItemHistory ItemHistory)
        {
            return ItemHistory.IsValidated;
        }

        public async Task<bool> Update(ItemHistory ItemHistory)
        {
            if (await ValidateId(ItemHistory))
            {
            }
            return ItemHistory.IsValidated;
        }

        public async Task<bool> Delete(ItemHistory ItemHistory)
        {
            if (await ValidateId(ItemHistory))
            {
            }
            return ItemHistory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ItemHistory> ItemHistories)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ItemHistory> ItemHistories)
        {
            return true;
        }
    }
}
