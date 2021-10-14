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

namespace ELODIE.Services.MOrderPaymentStatus
{
    public interface IOrderPaymentStatusService :  IServiceScoped
    {
        Task<int> Count(OrderPaymentStatusFilter OrderPaymentStatusFilter);
        Task<List<OrderPaymentStatus>> List(OrderPaymentStatusFilter OrderPaymentStatusFilter);
    }

    public class OrderPaymentStatusService : BaseService, IOrderPaymentStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public OrderPaymentStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(OrderPaymentStatusFilter OrderPaymentStatusFilter)
        {
            try
            {
                int result = await UOW.OrderPaymentStatusRepository.Count(OrderPaymentStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderPaymentStatusService));
            }
            return 0;
        }

        public async Task<List<OrderPaymentStatus>> List(OrderPaymentStatusFilter OrderPaymentStatusFilter)
        {
            try
            {
                List<OrderPaymentStatus> OrderPaymentStatuses = await UOW.OrderPaymentStatusRepository.List(OrderPaymentStatusFilter);
                return OrderPaymentStatuses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(OrderPaymentStatusService));
            }
            return null;
        }

    }
}
