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

namespace ELODIE.Services.MPaymentType
{
    public interface IPaymentTypeService :  IServiceScoped
    {
        Task<int> Count(PaymentTypeFilter PaymentTypeFilter);
        Task<List<PaymentType>> List(PaymentTypeFilter PaymentTypeFilter);
        Task<PaymentType> Get(long Id);
        Task<PaymentType> Create(PaymentType PaymentType);
        Task<PaymentType> Update(PaymentType PaymentType);
        Task<PaymentType> Delete(PaymentType PaymentType);
        Task<List<PaymentType>> BulkDelete(List<PaymentType> PaymentTypes);
        Task<List<PaymentType>> Import(List<PaymentType> PaymentTypes);
        Task<PaymentTypeFilter> ToFilter(PaymentTypeFilter PaymentTypeFilter);
    }

    public class PaymentTypeService : BaseService, IPaymentTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPaymentTypeValidator PaymentTypeValidator;

        public PaymentTypeService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IPaymentTypeValidator PaymentTypeValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PaymentTypeValidator = PaymentTypeValidator;
        }
        public async Task<int> Count(PaymentTypeFilter PaymentTypeFilter)
        {
            try
            {
                int result = await UOW.PaymentTypeRepository.Count(PaymentTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return 0;
        }

        public async Task<List<PaymentType>> List(PaymentTypeFilter PaymentTypeFilter)
        {
            try
            {
                List<PaymentType> PaymentTypes = await UOW.PaymentTypeRepository.List(PaymentTypeFilter);
                return PaymentTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;
        }

        public async Task<PaymentType> Get(long Id)
        {
            PaymentType PaymentType = await UOW.PaymentTypeRepository.Get(Id);
            await PaymentTypeValidator.Get(PaymentType);
            if (PaymentType == null)
                return null;
            return PaymentType;
        }
        
        public async Task<PaymentType> Create(PaymentType PaymentType)
        {
            if (!await PaymentTypeValidator.Create(PaymentType))
                return PaymentType;

            try
            {
                await UOW.PaymentTypeRepository.Create(PaymentType);
                PaymentType = await UOW.PaymentTypeRepository.Get(PaymentType.Id);
                await Logging.CreateAuditLog(PaymentType, new { }, nameof(PaymentTypeService));
                return PaymentType;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;
        }

        public async Task<PaymentType> Update(PaymentType PaymentType)
        {
            if (!await PaymentTypeValidator.Update(PaymentType))
                return PaymentType;
            try
            {
                var oldData = await UOW.PaymentTypeRepository.Get(PaymentType.Id);

                await UOW.PaymentTypeRepository.Update(PaymentType);

                PaymentType = await UOW.PaymentTypeRepository.Get(PaymentType.Id);
                await Logging.CreateAuditLog(PaymentType, oldData, nameof(PaymentTypeService));
                return PaymentType;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;
        }

        public async Task<PaymentType> Delete(PaymentType PaymentType)
        {
            if (!await PaymentTypeValidator.Delete(PaymentType))
                return PaymentType;

            try
            {
                await UOW.PaymentTypeRepository.Delete(PaymentType);
                await Logging.CreateAuditLog(new { }, PaymentType, nameof(PaymentTypeService));
                return PaymentType;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;
        }

        public async Task<List<PaymentType>> BulkDelete(List<PaymentType> PaymentTypes)
        {
            if (!await PaymentTypeValidator.BulkDelete(PaymentTypes))
                return PaymentTypes;

            try
            {
                await UOW.PaymentTypeRepository.BulkDelete(PaymentTypes);
                await Logging.CreateAuditLog(new { }, PaymentTypes, nameof(PaymentTypeService));
                return PaymentTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;

        }
        
        public async Task<List<PaymentType>> Import(List<PaymentType> PaymentTypes)
        {
            if (!await PaymentTypeValidator.Import(PaymentTypes))
                return PaymentTypes;
            try
            {
                await UOW.PaymentTypeRepository.BulkMerge(PaymentTypes);

                await Logging.CreateAuditLog(PaymentTypes, new { }, nameof(PaymentTypeService));
                return PaymentTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PaymentTypeService));
            }
            return null;
        }     
        
        public async Task<PaymentTypeFilter> ToFilter(PaymentTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PaymentTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PaymentTypeFilter subFilter = new PaymentTypeFilter();
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
