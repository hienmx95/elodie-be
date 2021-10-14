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

namespace ELODIE.Services.MCustomerSalesOrderContent
{
    public interface ICustomerSalesOrderContentService :  IServiceScoped
    {
        Task<int> Count(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
        Task<List<CustomerSalesOrderContent>> List(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
        Task<CustomerSalesOrderContent> Get(long Id);
        Task<CustomerSalesOrderContent> Create(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<CustomerSalesOrderContent> Update(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<CustomerSalesOrderContent> Delete(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<List<CustomerSalesOrderContent>> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
        Task<List<CustomerSalesOrderContent>> Import(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
        Task<CustomerSalesOrderContentFilter> ToFilter(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
    }

    public class CustomerSalesOrderContentService : BaseService, ICustomerSalesOrderContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICustomerSalesOrderContentValidator CustomerSalesOrderContentValidator;

        public CustomerSalesOrderContentService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICustomerSalesOrderContentValidator CustomerSalesOrderContentValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CustomerSalesOrderContentValidator = CustomerSalesOrderContentValidator;
        }
        public async Task<int> Count(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter)
        {
            try
            {
                int result = await UOW.CustomerSalesOrderContentRepository.Count(CustomerSalesOrderContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return 0;
        }

        public async Task<List<CustomerSalesOrderContent>> List(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter)
        {
            try
            {
                List<CustomerSalesOrderContent> CustomerSalesOrderContents = await UOW.CustomerSalesOrderContentRepository.List(CustomerSalesOrderContentFilter);
                return CustomerSalesOrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderContent> Get(long Id)
        {
            CustomerSalesOrderContent CustomerSalesOrderContent = await UOW.CustomerSalesOrderContentRepository.Get(Id);
            await CustomerSalesOrderContentValidator.Get(CustomerSalesOrderContent);
            if (CustomerSalesOrderContent == null)
                return null;
            return CustomerSalesOrderContent;
        }
        
        public async Task<CustomerSalesOrderContent> Create(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            if (!await CustomerSalesOrderContentValidator.Create(CustomerSalesOrderContent))
                return CustomerSalesOrderContent;

            try
            {
                await UOW.CustomerSalesOrderContentRepository.Create(CustomerSalesOrderContent);
                CustomerSalesOrderContent = await UOW.CustomerSalesOrderContentRepository.Get(CustomerSalesOrderContent.Id);
                await Logging.CreateAuditLog(CustomerSalesOrderContent, new { }, nameof(CustomerSalesOrderContentService));
                return CustomerSalesOrderContent;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderContent> Update(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            if (!await CustomerSalesOrderContentValidator.Update(CustomerSalesOrderContent))
                return CustomerSalesOrderContent;
            try
            {
                var oldData = await UOW.CustomerSalesOrderContentRepository.Get(CustomerSalesOrderContent.Id);

                await UOW.CustomerSalesOrderContentRepository.Update(CustomerSalesOrderContent);

                CustomerSalesOrderContent = await UOW.CustomerSalesOrderContentRepository.Get(CustomerSalesOrderContent.Id);
                await Logging.CreateAuditLog(CustomerSalesOrderContent, oldData, nameof(CustomerSalesOrderContentService));
                return CustomerSalesOrderContent;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;
        }

        public async Task<CustomerSalesOrderContent> Delete(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            if (!await CustomerSalesOrderContentValidator.Delete(CustomerSalesOrderContent))
                return CustomerSalesOrderContent;

            try
            {
                await UOW.CustomerSalesOrderContentRepository.Delete(CustomerSalesOrderContent);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrderContent, nameof(CustomerSalesOrderContentService));
                return CustomerSalesOrderContent;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;
        }

        public async Task<List<CustomerSalesOrderContent>> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            if (!await CustomerSalesOrderContentValidator.BulkDelete(CustomerSalesOrderContents))
                return CustomerSalesOrderContents;

            try
            {
                await UOW.CustomerSalesOrderContentRepository.BulkDelete(CustomerSalesOrderContents);
                await Logging.CreateAuditLog(new { }, CustomerSalesOrderContents, nameof(CustomerSalesOrderContentService));
                return CustomerSalesOrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;

        }
        
        public async Task<List<CustomerSalesOrderContent>> Import(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            if (!await CustomerSalesOrderContentValidator.Import(CustomerSalesOrderContents))
                return CustomerSalesOrderContents;
            try
            {
                await UOW.CustomerSalesOrderContentRepository.BulkMerge(CustomerSalesOrderContents);

                await Logging.CreateAuditLog(CustomerSalesOrderContents, new { }, nameof(CustomerSalesOrderContentService));
                return CustomerSalesOrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CustomerSalesOrderContentService));
            }
            return null;
        }     
        
        public async Task<CustomerSalesOrderContentFilter> ToFilter(CustomerSalesOrderContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CustomerSalesOrderContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CustomerSalesOrderContentFilter subFilter = new CustomerSalesOrderContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CustomerSalesOrderId))
                        subFilter.CustomerSalesOrderId = FilterBuilder.Merge(subFilter.CustomerSalesOrderId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = FilterBuilder.Merge(subFilter.ItemId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = FilterBuilder.Merge(subFilter.UnitOfMeasureId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Quantity))
                        subFilter.Quantity = FilterBuilder.Merge(subFilter.Quantity, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestedQuantity))
                        subFilter.RequestedQuantity = FilterBuilder.Merge(subFilter.RequestedQuantity, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PrimaryUnitOfMeasureId))
                        subFilter.PrimaryUnitOfMeasureId = FilterBuilder.Merge(subFilter.PrimaryUnitOfMeasureId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = FilterBuilder.Merge(subFilter.SalePrice, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PrimaryPrice))
                        subFilter.PrimaryPrice = FilterBuilder.Merge(subFilter.PrimaryPrice, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        subFilter.DiscountPercentage = FilterBuilder.Merge(subFilter.DiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountAmount))
                        subFilter.DiscountAmount = FilterBuilder.Merge(subFilter.DiscountAmount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountPercentage))
                        subFilter.GeneralDiscountPercentage = FilterBuilder.Merge(subFilter.GeneralDiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountAmount))
                        subFilter.GeneralDiscountAmount = FilterBuilder.Merge(subFilter.GeneralDiscountAmount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxPercentage))
                        subFilter.TaxPercentage = FilterBuilder.Merge(subFilter.TaxPercentage, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxAmount))
                        subFilter.TaxAmount = FilterBuilder.Merge(subFilter.TaxAmount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxPercentageOther))
                        subFilter.TaxPercentageOther = FilterBuilder.Merge(subFilter.TaxPercentageOther, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxAmountOther))
                        subFilter.TaxAmountOther = FilterBuilder.Merge(subFilter.TaxAmountOther, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Amount))
                        subFilter.Amount = FilterBuilder.Merge(subFilter.Amount, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Factor))
                        subFilter.Factor = FilterBuilder.Merge(subFilter.Factor, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EditedPriceStatusId))
                        subFilter.EditedPriceStatusId = FilterBuilder.Merge(subFilter.EditedPriceStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxTypeId))
                        subFilter.TaxTypeId = FilterBuilder.Merge(subFilter.TaxTypeId, FilterPermissionDefinition.IdFilter);
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
