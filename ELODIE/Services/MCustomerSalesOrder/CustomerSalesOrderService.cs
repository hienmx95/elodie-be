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

namespace ELODIE.Services.MCustomerSalesOrder
{
    public interface ICustomerSalesOrderService :  IServiceScoped
    {
        Task<int> Count(CustomerSalesOrderFilter CustomerSalesOrderFilter);
        Task<List<CustomerSalesOrder>> List(CustomerSalesOrderFilter CustomerSalesOrderFilter);
        Task<CustomerSalesOrder> Get(long Id);
        Task<CustomerSalesOrder> Create(CustomerSalesOrder CustomerSalesOrder);
        Task<CustomerSalesOrder> Update(CustomerSalesOrder CustomerSalesOrder);
        Task<CustomerSalesOrder> Delete(CustomerSalesOrder CustomerSalesOrder);
        Task<List<CustomerSalesOrder>> BulkDelete(List<CustomerSalesOrder> CustomerSalesOrders);
        Task<List<CustomerSalesOrder>> Import(List<CustomerSalesOrder> CustomerSalesOrders);
        Task<CustomerSalesOrderFilter> ToFilter(CustomerSalesOrderFilter CustomerSalesOrderFilter);
    }

    public class CustomerSalesOrderService : BaseService, ICustomerSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerSalesOrderValidator CustomerSalesOrderValidator;

        public CustomerSalesOrderService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerSalesOrderValidator CustomerSalesOrderValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderValidator = CustomerSalesOrderValidator;
        }
        public async Task<int> Count(CustomerSalesOrderFilter CustomerSalesOrderFilter)
        {
            try
            {
                int result = await UOW.CustomerSalesOrderRepository.Count(CustomerSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return 0;
        }

        public async Task<List<CustomerSalesOrder>> List(CustomerSalesOrderFilter CustomerSalesOrderFilter)
        {
            try
            {
                List<CustomerSalesOrder> CustomerSalesOrders = await UOW.CustomerSalesOrderRepository.List(CustomerSalesOrderFilter);
                return CustomerSalesOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;
        }

        public async Task<CustomerSalesOrder> Get(long Id)
        {
            CustomerSalesOrder CustomerSalesOrder = await UOW.CustomerSalesOrderRepository.Get(Id);
            await CustomerSalesOrderValidator.Get(CustomerSalesOrder);
            if (CustomerSalesOrder == null)
                return null;
            return CustomerSalesOrder;
        }
        
        public async Task<CustomerSalesOrder> Create(CustomerSalesOrder CustomerSalesOrder)
        {
            if (!await CustomerSalesOrderValidator.Create(CustomerSalesOrder))
                return CustomerSalesOrder;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var SalesEmployee = await UOW.AppUserRepository.Get(CustomerSalesOrder.SalesEmployeeId);
                CustomerSalesOrder.CreatorId = CurrentContext.UserId;
                CustomerSalesOrder.OrganizationId = SalesEmployee.OrganizationId;
                CustomerSalesOrder.Organization = SalesEmployee.Organization;
                //CustomerSalesOrder.RequestStateId = RequestStateEnum.NEW.Id;
                await UOW.CustomerSalesOrderRepository.Create(CustomerSalesOrder);
                CustomerSalesOrder = await UOW.CustomerSalesOrderRepository.Get(CustomerSalesOrder.Id);
                CustomerSalesOrder.Code = $"ORDER{CustomerSalesOrder.Id}";
                await UOW.CustomerSalesOrderRepository.Update(CustomerSalesOrder);
                await Logging.CreateAuditLog(CustomerSalesOrder, new { }, nameof(CustomerSalesOrderService));
                return CustomerSalesOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;
        }

        public async Task<CustomerSalesOrder> Update(CustomerSalesOrder CustomerSalesOrder)
        {
            if (!await CustomerSalesOrderValidator.Update(CustomerSalesOrder))
                return CustomerSalesOrder;
            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var oldData = await UOW.CustomerSalesOrderRepository.Get(CustomerSalesOrder.Id);
                var AppUser = await UOW.AppUserRepository.Get(CustomerSalesOrder.SalesEmployeeId);
                CustomerSalesOrder.OrganizationId = AppUser.OrganizationId;
                await UOW.CustomerSalesOrderRepository.Update(CustomerSalesOrder);

                CustomerSalesOrder = await UOW.CustomerSalesOrderRepository.Get(CustomerSalesOrder.Id);
                await Logging.CreateAuditLog(CustomerSalesOrder, oldData, nameof(CustomerSalesOrderService));
                return CustomerSalesOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;
        }

        public async Task<CustomerSalesOrder> Delete(CustomerSalesOrder CustomerSalesOrder)
        {
            if (!await CustomerSalesOrderValidator.Delete(CustomerSalesOrder))
                return CustomerSalesOrder;

            try
            {
                await UOW.CustomerSalesOrderRepository.Delete(CustomerSalesOrder);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrder, nameof(CustomerSalesOrderService));
                return CustomerSalesOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;
        }

        public async Task<List<CustomerSalesOrder>> BulkDelete(List<CustomerSalesOrder> CustomerSalesOrders)
        {
            if (!await CustomerSalesOrderValidator.BulkDelete(CustomerSalesOrders))
                return CustomerSalesOrders;

            try
            {
                await UOW.CustomerSalesOrderRepository.BulkDelete(CustomerSalesOrders);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrders, nameof(CustomerSalesOrderService));
                return CustomerSalesOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;

        }
        
        public async Task<List<CustomerSalesOrder>> Import(List<CustomerSalesOrder> CustomerSalesOrders)
        {
            if (!await CustomerSalesOrderValidator.Import(CustomerSalesOrders))
                return CustomerSalesOrders;
            try
            {
                await UOW.CustomerSalesOrderRepository.BulkMerge(CustomerSalesOrders);

                await Logging.CreateAuditLog(CustomerSalesOrders, new { }, nameof(CustomerSalesOrderService));
                return CustomerSalesOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderService));
            }
            return null;
        }     
        
        public async Task<CustomerSalesOrderFilter> ToFilter(CustomerSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerSalesOrderFilter subFilter = new CustomerSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CustomerId))
                        subFilter.CustomerId = FilterBuilder.Merge(subFilter.CustomerId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderSourceId))
                        subFilter.OrderSourceId = FilterBuilder.Merge(subFilter.OrderSourceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestStateId))
                        subFilter.RequestStateId = FilterBuilder.Merge(subFilter.RequestStateId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderPaymentStatusId))
                        subFilter.OrderPaymentStatusId = FilterBuilder.Merge(subFilter.OrderPaymentStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EditedPriceStatusId))
                        subFilter.EditedPriceStatusId = FilterBuilder.Merge(subFilter.EditedPriceStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ShippingName))
                        subFilter.ShippingName = FilterBuilder.Merge(subFilter.ShippingName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = FilterBuilder.Merge(subFilter.OrderDate, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryDate))
                        subFilter.DeliveryDate = FilterBuilder.Merge(subFilter.DeliveryDate, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalesEmployeeId))
                        subFilter.SalesEmployeeId = FilterBuilder.Merge(subFilter.SalesEmployeeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        subFilter.Note = FilterBuilder.Merge(subFilter.Note, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceAddress))
                        subFilter.InvoiceAddress = FilterBuilder.Merge(subFilter.InvoiceAddress, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceNationId))
                        subFilter.InvoiceNationId = FilterBuilder.Merge(subFilter.InvoiceNationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceProvinceId))
                        subFilter.InvoiceProvinceId = FilterBuilder.Merge(subFilter.InvoiceProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceDistrictId))
                        subFilter.InvoiceDistrictId = FilterBuilder.Merge(subFilter.InvoiceDistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceWardId))
                        subFilter.InvoiceWardId = FilterBuilder.Merge(subFilter.InvoiceWardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InvoiceZIPCode))
                        subFilter.InvoiceZIPCode = FilterBuilder.Merge(subFilter.InvoiceZIPCode, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryAddress))
                        subFilter.DeliveryAddress = FilterBuilder.Merge(subFilter.DeliveryAddress, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryNationId))
                        subFilter.DeliveryNationId = FilterBuilder.Merge(subFilter.DeliveryNationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryProvinceId))
                        subFilter.DeliveryProvinceId = FilterBuilder.Merge(subFilter.DeliveryProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryDistrictId))
                        subFilter.DeliveryDistrictId = FilterBuilder.Merge(subFilter.DeliveryDistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryWardId))
                        subFilter.DeliveryWardId = FilterBuilder.Merge(subFilter.DeliveryWardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryZIPCode))
                        subFilter.DeliveryZIPCode = FilterBuilder.Merge(subFilter.DeliveryZIPCode, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SubTotal))
                        subFilter.SubTotal = FilterBuilder.Merge(subFilter.SubTotal, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountPercentage))
                        subFilter.GeneralDiscountPercentage = FilterBuilder.Merge(subFilter.GeneralDiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountAmount))
                        subFilter.GeneralDiscountAmount = FilterBuilder.Merge(subFilter.GeneralDiscountAmount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalTaxOther))
                        subFilter.TotalTaxOther = FilterBuilder.Merge(subFilter.TotalTaxOther, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalTax))
                        subFilter.TotalTax = FilterBuilder.Merge(subFilter.TotalTax, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = FilterBuilder.Merge(subFilter.Total, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CodeGeneratorRuleId))
                        subFilter.CodeGeneratorRuleId = FilterBuilder.Merge(subFilter.CodeGeneratorRuleId, FilterPermissionDefinition.IdFilter);
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
