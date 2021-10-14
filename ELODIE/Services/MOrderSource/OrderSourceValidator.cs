using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MOrderSource
{
    public interface IOrderSourceValidator : IServiceScoped
    {
        Task Get(OrderSource OrderSource);
        Task<bool> Create(OrderSource OrderSource);
        Task<bool> Update(OrderSource OrderSource);
        Task<bool> Delete(OrderSource OrderSource);
        Task<bool> BulkDelete(List<OrderSource> OrderSources);
        Task<bool> Import(List<OrderSource> OrderSources);
    }

    public class OrderSourceValidator : IOrderSourceValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private OrderSourceMessage OrderSourceMessage;

        public OrderSourceValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.OrderSourceMessage = new OrderSourceMessage();
        }

        public async Task Get(OrderSource OrderSource)
        {
        }

        public async Task<bool> Create(OrderSource OrderSource)
        {
            return OrderSource.IsValidated;
        }

        public async Task<bool> Update(OrderSource OrderSource)
        {
            if (await ValidateId(OrderSource))
            {
            }
            return OrderSource.IsValidated;
        }

        public async Task<bool> Delete(OrderSource OrderSource)
        {
            if (await ValidateId(OrderSource))
            {
            }
            return OrderSource.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<OrderSource> OrderSources)
        {
            foreach (OrderSource OrderSource in OrderSources)
            {
                await Delete(OrderSource);
            }
            return OrderSources.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<OrderSource> OrderSources)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(OrderSource OrderSource)
        {
            OrderSourceFilter OrderSourceFilter = new OrderSourceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = OrderSource.Id },
                Selects = OrderSourceSelect.Id
            };

            int count = await UOW.OrderSourceRepository.CountAll(OrderSourceFilter);
            if (count == 0)
                OrderSource.AddError(nameof(OrderSourceValidator), nameof(OrderSource.Id), OrderSourceMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
