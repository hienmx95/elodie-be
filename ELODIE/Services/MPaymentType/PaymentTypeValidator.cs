using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MPaymentType
{
    public interface IPaymentTypeValidator : IServiceScoped
    {
        Task Get(PaymentType PaymentType);
        Task<bool> Create(PaymentType PaymentType);
        Task<bool> Update(PaymentType PaymentType);
        Task<bool> Delete(PaymentType PaymentType);
        Task<bool> BulkDelete(List<PaymentType> PaymentTypes);
        Task<bool> Import(List<PaymentType> PaymentTypes);
    }

    public class PaymentTypeValidator : IPaymentTypeValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private PaymentTypeMessage PaymentTypeMessage;

        public PaymentTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.PaymentTypeMessage = new PaymentTypeMessage();
        }

        public async Task Get(PaymentType PaymentType)
        {
        }

        public async Task<bool> Create(PaymentType PaymentType)
        {
            return PaymentType.IsValidated;
        }

        public async Task<bool> Update(PaymentType PaymentType)
        {
            if (await ValidateId(PaymentType))
            {
            }
            return PaymentType.IsValidated;
        }

        public async Task<bool> Delete(PaymentType PaymentType)
        {
            if (await ValidateId(PaymentType))
            {
            }
            return PaymentType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PaymentType> PaymentTypes)
        {
            foreach (PaymentType PaymentType in PaymentTypes)
            {
                await Delete(PaymentType);
            }
            return PaymentTypes.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<PaymentType> PaymentTypes)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(PaymentType PaymentType)
        {
            PaymentTypeFilter PaymentTypeFilter = new PaymentTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PaymentType.Id },
                Selects = PaymentTypeSelect.Id
            };

            int count = await UOW.PaymentTypeRepository.CountAll(PaymentTypeFilter);
            if (count == 0)
                PaymentType.AddError(nameof(PaymentTypeValidator), nameof(PaymentType.Id), PaymentTypeMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
