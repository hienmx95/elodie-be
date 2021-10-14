using ELODIE.Common;
using ELODIE.Helpers;
using ELODIE.Entities;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace ELODIE.Repositories
{
    public interface ICustomerSalesOrderContentRepository
    {
        Task<int> CountAll(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
        Task<int> Count(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
        Task<List<CustomerSalesOrderContent>> List(CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter);
        Task<List<CustomerSalesOrderContent>> List(List<long> Ids);
        Task<CustomerSalesOrderContent> Get(long Id);
        Task<bool> Create(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> Update(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> Delete(CustomerSalesOrderContent CustomerSalesOrderContent);
        Task<bool> BulkMerge(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
        Task<bool> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents);
    }
    public class CustomerSalesOrderContentRepository : ICustomerSalesOrderContentRepository
    {
        private DataContext DataContext;
        public CustomerSalesOrderContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CustomerSalesOrderContentDAO> DynamicFilter(IQueryable<CustomerSalesOrderContentDAO> query, CustomerSalesOrderContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.CustomerSalesOrderId, filter.CustomerSalesOrderId);
            query = query.Where(q => q.ItemId, filter.ItemId);
            query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            query = query.Where(q => q.Quantity, filter.Quantity);
            query = query.Where(q => q.RequestedQuantity, filter.RequestedQuantity);
            query = query.Where(q => q.PrimaryUnitOfMeasureId, filter.PrimaryUnitOfMeasureId);
            query = query.Where(q => q.SalePrice, filter.SalePrice);
            query = query.Where(q => q.PrimaryPrice, filter.PrimaryPrice);
            query = query.Where(q => q.DiscountPercentage, filter.DiscountPercentage);
            query = query.Where(q => q.DiscountAmount, filter.DiscountAmount);
            query = query.Where(q => q.GeneralDiscountPercentage, filter.GeneralDiscountPercentage);
            query = query.Where(q => q.GeneralDiscountAmount, filter.GeneralDiscountAmount);
            query = query.Where(q => q.TaxPercentage, filter.TaxPercentage);
            query = query.Where(q => q.TaxAmount, filter.TaxAmount);
            query = query.Where(q => q.TaxPercentageOther, filter.TaxPercentageOther);
            query = query.Where(q => q.TaxAmountOther, filter.TaxAmountOther);
            query = query.Where(q => q.Amount, filter.Amount);
            query = query.Where(q => q.Factor, filter.Factor);
            query = query.Where(q => q.EditedPriceStatusId, filter.EditedPriceStatusId);
            query = query.Where(q => q.TaxTypeId, filter.TaxTypeId);
            
            return query;
        }

        private IQueryable<CustomerSalesOrderContentDAO> OrFilter(IQueryable<CustomerSalesOrderContentDAO> query, CustomerSalesOrderContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CustomerSalesOrderContentDAO> initQuery = query.Where(q => false);
            foreach (CustomerSalesOrderContentFilter CustomerSalesOrderContentFilter in filter.OrFilter)
            {
                IQueryable<CustomerSalesOrderContentDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.CustomerSalesOrderId, filter.CustomerSalesOrderId);
                queryable = queryable.Where(q => q.ItemId, filter.ItemId);
                queryable = queryable.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
                queryable = queryable.Where(q => q.Quantity, filter.Quantity);
                queryable = queryable.Where(q => q.RequestedQuantity, filter.RequestedQuantity);
                queryable = queryable.Where(q => q.PrimaryUnitOfMeasureId, filter.PrimaryUnitOfMeasureId);
                queryable = queryable.Where(q => q.SalePrice, filter.SalePrice);
                queryable = queryable.Where(q => q.PrimaryPrice, filter.PrimaryPrice);
                queryable = queryable.Where(q => q.DiscountPercentage, filter.DiscountPercentage);
                queryable = queryable.Where(q => q.DiscountAmount, filter.DiscountAmount);
                queryable = queryable.Where(q => q.GeneralDiscountPercentage, filter.GeneralDiscountPercentage);
                queryable = queryable.Where(q => q.GeneralDiscountAmount, filter.GeneralDiscountAmount);
                queryable = queryable.Where(q => q.TaxPercentage, filter.TaxPercentage);
                queryable = queryable.Where(q => q.TaxAmount, filter.TaxAmount);
                queryable = queryable.Where(q => q.TaxPercentageOther, filter.TaxPercentageOther);
                queryable = queryable.Where(q => q.TaxAmountOther, filter.TaxAmountOther);
                queryable = queryable.Where(q => q.Amount, filter.Amount);
                queryable = queryable.Where(q => q.Factor, filter.Factor);
                queryable = queryable.Where(q => q.EditedPriceStatusId, filter.EditedPriceStatusId);
                queryable = queryable.Where(q => q.TaxTypeId, filter.TaxTypeId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CustomerSalesOrderContentDAO> DynamicOrder(IQueryable<CustomerSalesOrderContentDAO> query, CustomerSalesOrderContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSalesOrderContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CustomerSalesOrderContentOrder.CustomerSalesOrder:
                            query = query.OrderBy(q => q.CustomerSalesOrderId);
                            break;
                        case CustomerSalesOrderContentOrder.Item:
                            query = query.OrderBy(q => q.ItemId);
                            break;
                        case CustomerSalesOrderContentOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case CustomerSalesOrderContentOrder.Quantity:
                            query = query.OrderBy(q => q.Quantity);
                            break;
                        case CustomerSalesOrderContentOrder.RequestedQuantity:
                            query = query.OrderBy(q => q.RequestedQuantity);
                            break;
                        case CustomerSalesOrderContentOrder.PrimaryUnitOfMeasure:
                            query = query.OrderBy(q => q.PrimaryUnitOfMeasureId);
                            break;
                        case CustomerSalesOrderContentOrder.SalePrice:
                            query = query.OrderBy(q => q.SalePrice);
                            break;
                        case CustomerSalesOrderContentOrder.PrimaryPrice:
                            query = query.OrderBy(q => q.PrimaryPrice);
                            break;
                        case CustomerSalesOrderContentOrder.DiscountPercentage:
                            query = query.OrderBy(q => q.DiscountPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.DiscountAmount:
                            query = query.OrderBy(q => q.DiscountAmount);
                            break;
                        case CustomerSalesOrderContentOrder.GeneralDiscountPercentage:
                            query = query.OrderBy(q => q.GeneralDiscountPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.GeneralDiscountAmount:
                            query = query.OrderBy(q => q.GeneralDiscountAmount);
                            break;
                        case CustomerSalesOrderContentOrder.TaxPercentage:
                            query = query.OrderBy(q => q.TaxPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.TaxAmount:
                            query = query.OrderBy(q => q.TaxAmount);
                            break;
                        case CustomerSalesOrderContentOrder.TaxPercentageOther:
                            query = query.OrderBy(q => q.TaxPercentageOther);
                            break;
                        case CustomerSalesOrderContentOrder.TaxAmountOther:
                            query = query.OrderBy(q => q.TaxAmountOther);
                            break;
                        case CustomerSalesOrderContentOrder.Amount:
                            query = query.OrderBy(q => q.Amount);
                            break;
                        case CustomerSalesOrderContentOrder.Factor:
                            query = query.OrderBy(q => q.Factor);
                            break;
                        case CustomerSalesOrderContentOrder.EditedPriceStatus:
                            query = query.OrderBy(q => q.EditedPriceStatusId);
                            break;
                        case CustomerSalesOrderContentOrder.TaxType:
                            query = query.OrderBy(q => q.TaxTypeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSalesOrderContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CustomerSalesOrderContentOrder.CustomerSalesOrder:
                            query = query.OrderByDescending(q => q.CustomerSalesOrderId);
                            break;
                        case CustomerSalesOrderContentOrder.Item:
                            query = query.OrderByDescending(q => q.ItemId);
                            break;
                        case CustomerSalesOrderContentOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case CustomerSalesOrderContentOrder.Quantity:
                            query = query.OrderByDescending(q => q.Quantity);
                            break;
                        case CustomerSalesOrderContentOrder.RequestedQuantity:
                            query = query.OrderByDescending(q => q.RequestedQuantity);
                            break;
                        case CustomerSalesOrderContentOrder.PrimaryUnitOfMeasure:
                            query = query.OrderByDescending(q => q.PrimaryUnitOfMeasureId);
                            break;
                        case CustomerSalesOrderContentOrder.SalePrice:
                            query = query.OrderByDescending(q => q.SalePrice);
                            break;
                        case CustomerSalesOrderContentOrder.PrimaryPrice:
                            query = query.OrderByDescending(q => q.PrimaryPrice);
                            break;
                        case CustomerSalesOrderContentOrder.DiscountPercentage:
                            query = query.OrderByDescending(q => q.DiscountPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.DiscountAmount:
                            query = query.OrderByDescending(q => q.DiscountAmount);
                            break;
                        case CustomerSalesOrderContentOrder.GeneralDiscountPercentage:
                            query = query.OrderByDescending(q => q.GeneralDiscountPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.GeneralDiscountAmount:
                            query = query.OrderByDescending(q => q.GeneralDiscountAmount);
                            break;
                        case CustomerSalesOrderContentOrder.TaxPercentage:
                            query = query.OrderByDescending(q => q.TaxPercentage);
                            break;
                        case CustomerSalesOrderContentOrder.TaxAmount:
                            query = query.OrderByDescending(q => q.TaxAmount);
                            break;
                        case CustomerSalesOrderContentOrder.TaxPercentageOther:
                            query = query.OrderByDescending(q => q.TaxPercentageOther);
                            break;
                        case CustomerSalesOrderContentOrder.TaxAmountOther:
                            query = query.OrderByDescending(q => q.TaxAmountOther);
                            break;
                        case CustomerSalesOrderContentOrder.Amount:
                            query = query.OrderByDescending(q => q.Amount);
                            break;
                        case CustomerSalesOrderContentOrder.Factor:
                            query = query.OrderByDescending(q => q.Factor);
                            break;
                        case CustomerSalesOrderContentOrder.EditedPriceStatus:
                            query = query.OrderByDescending(q => q.EditedPriceStatusId);
                            break;
                        case CustomerSalesOrderContentOrder.TaxType:
                            query = query.OrderByDescending(q => q.TaxTypeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CustomerSalesOrderContent>> DynamicSelect(IQueryable<CustomerSalesOrderContentDAO> query, CustomerSalesOrderContentFilter filter)
        {
            List<CustomerSalesOrderContent> CustomerSalesOrderContents = await query.Select(q => new CustomerSalesOrderContent()
            {
                Id = filter.Selects.Contains(CustomerSalesOrderContentSelect.Id) ? q.Id : default(long),
                CustomerSalesOrderId = filter.Selects.Contains(CustomerSalesOrderContentSelect.CustomerSalesOrder) ? q.CustomerSalesOrderId : default(long),
                ItemId = filter.Selects.Contains(CustomerSalesOrderContentSelect.Item) ? q.ItemId : default(long),
                UnitOfMeasureId = filter.Selects.Contains(CustomerSalesOrderContentSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                Quantity = filter.Selects.Contains(CustomerSalesOrderContentSelect.Quantity) ? q.Quantity : default(long),
                RequestedQuantity = filter.Selects.Contains(CustomerSalesOrderContentSelect.RequestedQuantity) ? q.RequestedQuantity : default(long),
                PrimaryUnitOfMeasureId = filter.Selects.Contains(CustomerSalesOrderContentSelect.PrimaryUnitOfMeasure) ? q.PrimaryUnitOfMeasureId : default(long),
                SalePrice = filter.Selects.Contains(CustomerSalesOrderContentSelect.SalePrice) ? q.SalePrice : default(decimal),
                PrimaryPrice = filter.Selects.Contains(CustomerSalesOrderContentSelect.PrimaryPrice) ? q.PrimaryPrice : default(decimal),
                DiscountPercentage = filter.Selects.Contains(CustomerSalesOrderContentSelect.DiscountPercentage) ? q.DiscountPercentage : default(decimal?),
                DiscountAmount = filter.Selects.Contains(CustomerSalesOrderContentSelect.DiscountAmount) ? q.DiscountAmount : default(decimal?),
                GeneralDiscountPercentage = filter.Selects.Contains(CustomerSalesOrderContentSelect.GeneralDiscountPercentage) ? q.GeneralDiscountPercentage : default(decimal?),
                GeneralDiscountAmount = filter.Selects.Contains(CustomerSalesOrderContentSelect.GeneralDiscountAmount) ? q.GeneralDiscountAmount : default(decimal?),
                TaxPercentage = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxPercentage) ? q.TaxPercentage : default(decimal?),
                TaxAmount = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxAmount) ? q.TaxAmount : default(decimal?),
                TaxPercentageOther = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxPercentageOther) ? q.TaxPercentageOther : default(decimal?),
                TaxAmountOther = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxAmountOther) ? q.TaxAmountOther : default(decimal?),
                Amount = filter.Selects.Contains(CustomerSalesOrderContentSelect.Amount) ? q.Amount : default(decimal),
                Factor = filter.Selects.Contains(CustomerSalesOrderContentSelect.Factor) ? q.Factor : default(long?),
                EditedPriceStatusId = filter.Selects.Contains(CustomerSalesOrderContentSelect.EditedPriceStatus) ? q.EditedPriceStatusId : default(long),
                TaxTypeId = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxType) ? q.TaxTypeId : default(long),
                CustomerSalesOrder = filter.Selects.Contains(CustomerSalesOrderContentSelect.CustomerSalesOrder) && q.CustomerSalesOrder != null ? new CustomerSalesOrder
                {
                    Id = q.CustomerSalesOrder.Id,
                    Code = q.CustomerSalesOrder.Code,
                    CustomerId = q.CustomerSalesOrder.CustomerId,
                    OrderSourceId = q.CustomerSalesOrder.OrderSourceId,
                    RequestStateId = q.CustomerSalesOrder.RequestStateId,
                    OrderPaymentStatusId = q.CustomerSalesOrder.OrderPaymentStatusId,
                    EditedPriceStatusId = q.CustomerSalesOrder.EditedPriceStatusId,
                    ShippingName = q.CustomerSalesOrder.ShippingName,
                    OrderDate = q.CustomerSalesOrder.OrderDate,
                    DeliveryDate = q.CustomerSalesOrder.DeliveryDate,
                    SalesEmployeeId = q.CustomerSalesOrder.SalesEmployeeId,
                    Note = q.CustomerSalesOrder.Note,
                    InvoiceAddress = q.CustomerSalesOrder.InvoiceAddress,
                    InvoiceNationId = q.CustomerSalesOrder.InvoiceNationId,
                    InvoiceProvinceId = q.CustomerSalesOrder.InvoiceProvinceId,
                    InvoiceDistrictId = q.CustomerSalesOrder.InvoiceDistrictId,
                    InvoiceWardId = q.CustomerSalesOrder.InvoiceWardId,
                    InvoiceZIPCode = q.CustomerSalesOrder.InvoiceZIPCode,
                    DeliveryAddress = q.CustomerSalesOrder.DeliveryAddress,
                    DeliveryNationId = q.CustomerSalesOrder.DeliveryNationId,
                    DeliveryProvinceId = q.CustomerSalesOrder.DeliveryProvinceId,
                    DeliveryDistrictId = q.CustomerSalesOrder.DeliveryDistrictId,
                    DeliveryWardId = q.CustomerSalesOrder.DeliveryWardId,
                    DeliveryZIPCode = q.CustomerSalesOrder.DeliveryZIPCode,
                    SubTotal = q.CustomerSalesOrder.SubTotal,
                    GeneralDiscountPercentage = q.CustomerSalesOrder.GeneralDiscountPercentage,
                    GeneralDiscountAmount = q.CustomerSalesOrder.GeneralDiscountAmount,
                    TotalTaxOther = q.CustomerSalesOrder.TotalTaxOther,
                    TotalTax = q.CustomerSalesOrder.TotalTax,
                    Total = q.CustomerSalesOrder.Total,
                    CreatorId = q.CustomerSalesOrder.CreatorId,
                    OrganizationId = q.CustomerSalesOrder.OrganizationId,
                    RowId = q.CustomerSalesOrder.RowId,
                    CodeGeneratorRuleId = q.CustomerSalesOrder.CodeGeneratorRuleId,
                } : null,
                EditedPriceStatus = filter.Selects.Contains(CustomerSalesOrderContentSelect.EditedPriceStatus) && q.EditedPriceStatus != null ? new EditedPriceStatus
                {
                    Id = q.EditedPriceStatus.Id,
                    Code = q.EditedPriceStatus.Code,
                    Name = q.EditedPriceStatus.Name,
                } : null,
                Item = filter.Selects.Contains(CustomerSalesOrderContentSelect.Item) && q.Item != null ? new Item
                {
                    Id = q.Item.Id,
                    ProductId = q.Item.ProductId,
                    Code = q.Item.Code,
                    ERPCode = q.Item.ERPCode,
                    Name = q.Item.Name,
                    ScanCode = q.Item.ScanCode,
                    SalePrice = q.Item.SalePrice,
                    RetailPrice = q.Item.RetailPrice,
                    StatusId = q.Item.StatusId,
                    Used = q.Item.Used,
                    RowId = q.Item.RowId,
                } : null,
                PrimaryUnitOfMeasure = filter.Selects.Contains(CustomerSalesOrderContentSelect.PrimaryUnitOfMeasure) && q.PrimaryUnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.PrimaryUnitOfMeasure.Id,
                    Code = q.PrimaryUnitOfMeasure.Code,
                    Name = q.PrimaryUnitOfMeasure.Name,
                    Description = q.PrimaryUnitOfMeasure.Description,
                    StatusId = q.PrimaryUnitOfMeasure.StatusId,
                    Used = q.PrimaryUnitOfMeasure.Used,
                    RowId = q.PrimaryUnitOfMeasure.RowId,
                } : null,
                TaxType = filter.Selects.Contains(CustomerSalesOrderContentSelect.TaxType) && q.TaxType != null ? new TaxType
                {
                    Id = q.TaxType.Id,
                    Code = q.TaxType.Code,
                    Name = q.TaxType.Name,
                    Percentage = q.TaxType.Percentage,
                    StatusId = q.TaxType.StatusId,
                    Used = q.TaxType.Used,
                    RowId = q.TaxType.RowId,
                } : null,
                UnitOfMeasure = filter.Selects.Contains(CustomerSalesOrderContentSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                    Used = q.UnitOfMeasure.Used,
                    RowId = q.UnitOfMeasure.RowId,
                } : null,
            }).ToListAsync();
            return CustomerSalesOrderContents;
        }

        public async Task<int> CountAll(CustomerSalesOrderContentFilter filter)
        {
            IQueryable<CustomerSalesOrderContentDAO> CustomerSalesOrderContents = DataContext.CustomerSalesOrderContent.AsNoTracking();
            CustomerSalesOrderContents = DynamicFilter(CustomerSalesOrderContents, filter);
            return await CustomerSalesOrderContents.CountAsync();
        }

        public async Task<int> Count(CustomerSalesOrderContentFilter filter)
        {
            IQueryable<CustomerSalesOrderContentDAO> CustomerSalesOrderContents = DataContext.CustomerSalesOrderContent.AsNoTracking();
            CustomerSalesOrderContents = DynamicFilter(CustomerSalesOrderContents, filter);
            CustomerSalesOrderContents = OrFilter(CustomerSalesOrderContents, filter);
            return await CustomerSalesOrderContents.CountAsync();
        }

        public async Task<List<CustomerSalesOrderContent>> List(CustomerSalesOrderContentFilter filter)
        {
            if (filter == null) return new List<CustomerSalesOrderContent>();
            IQueryable<CustomerSalesOrderContentDAO> CustomerSalesOrderContentDAOs = DataContext.CustomerSalesOrderContent.AsNoTracking();
            CustomerSalesOrderContentDAOs = DynamicFilter(CustomerSalesOrderContentDAOs, filter);
            CustomerSalesOrderContentDAOs = OrFilter(CustomerSalesOrderContentDAOs, filter);
            CustomerSalesOrderContentDAOs = DynamicOrder(CustomerSalesOrderContentDAOs, filter);
            List<CustomerSalesOrderContent> CustomerSalesOrderContents = await DynamicSelect(CustomerSalesOrderContentDAOs, filter);
            return CustomerSalesOrderContents;
        }

        public async Task<List<CustomerSalesOrderContent>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.CustomerSalesOrderContent
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<CustomerSalesOrderContent> CustomerSalesOrderContents = await query.AsNoTracking()
            .Select(x => new CustomerSalesOrderContent()
            {
                Id = x.Id,
                CustomerSalesOrderId = x.CustomerSalesOrderId,
                ItemId = x.ItemId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                RequestedQuantity = x.RequestedQuantity,
                PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                SalePrice = x.SalePrice,
                PrimaryPrice = x.PrimaryPrice,
                DiscountPercentage = x.DiscountPercentage,
                DiscountAmount = x.DiscountAmount,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                TaxPercentage = x.TaxPercentage,
                TaxAmount = x.TaxAmount,
                TaxPercentageOther = x.TaxPercentageOther,
                TaxAmountOther = x.TaxAmountOther,
                Amount = x.Amount,
                Factor = x.Factor,
                EditedPriceStatusId = x.EditedPriceStatusId,
                TaxTypeId = x.TaxTypeId,
                CustomerSalesOrder = x.CustomerSalesOrder == null ? null : new CustomerSalesOrder
                {
                    Id = x.CustomerSalesOrder.Id,
                    Code = x.CustomerSalesOrder.Code,
                    CustomerId = x.CustomerSalesOrder.CustomerId,
                    OrderSourceId = x.CustomerSalesOrder.OrderSourceId,
                    RequestStateId = x.CustomerSalesOrder.RequestStateId,
                    OrderPaymentStatusId = x.CustomerSalesOrder.OrderPaymentStatusId,
                    EditedPriceStatusId = x.CustomerSalesOrder.EditedPriceStatusId,
                    ShippingName = x.CustomerSalesOrder.ShippingName,
                    OrderDate = x.CustomerSalesOrder.OrderDate,
                    DeliveryDate = x.CustomerSalesOrder.DeliveryDate,
                    SalesEmployeeId = x.CustomerSalesOrder.SalesEmployeeId,
                    Note = x.CustomerSalesOrder.Note,
                    InvoiceAddress = x.CustomerSalesOrder.InvoiceAddress,
                    InvoiceNationId = x.CustomerSalesOrder.InvoiceNationId,
                    InvoiceProvinceId = x.CustomerSalesOrder.InvoiceProvinceId,
                    InvoiceDistrictId = x.CustomerSalesOrder.InvoiceDistrictId,
                    InvoiceWardId = x.CustomerSalesOrder.InvoiceWardId,
                    InvoiceZIPCode = x.CustomerSalesOrder.InvoiceZIPCode,
                    DeliveryAddress = x.CustomerSalesOrder.DeliveryAddress,
                    DeliveryNationId = x.CustomerSalesOrder.DeliveryNationId,
                    DeliveryProvinceId = x.CustomerSalesOrder.DeliveryProvinceId,
                    DeliveryDistrictId = x.CustomerSalesOrder.DeliveryDistrictId,
                    DeliveryWardId = x.CustomerSalesOrder.DeliveryWardId,
                    DeliveryZIPCode = x.CustomerSalesOrder.DeliveryZIPCode,
                    SubTotal = x.CustomerSalesOrder.SubTotal,
                    GeneralDiscountPercentage = x.CustomerSalesOrder.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.CustomerSalesOrder.GeneralDiscountAmount,
                    TotalTaxOther = x.CustomerSalesOrder.TotalTaxOther,
                    TotalTax = x.CustomerSalesOrder.TotalTax,
                    Total = x.CustomerSalesOrder.Total,
                    CreatorId = x.CustomerSalesOrder.CreatorId,
                    OrganizationId = x.CustomerSalesOrder.OrganizationId,
                    RowId = x.CustomerSalesOrder.RowId,
                    CodeGeneratorRuleId = x.CustomerSalesOrder.CodeGeneratorRuleId,
                },
                EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                {
                    Id = x.EditedPriceStatus.Id,
                    Code = x.EditedPriceStatus.Code,
                    Name = x.EditedPriceStatus.Name,
                },
                Item = x.Item == null ? null : new Item
                {
                    Id = x.Item.Id,
                    ProductId = x.Item.ProductId,
                    Code = x.Item.Code,
                    ERPCode = x.Item.ERPCode,
                    Name = x.Item.Name,
                    ScanCode = x.Item.ScanCode,
                    SalePrice = x.Item.SalePrice,
                    RetailPrice = x.Item.RetailPrice,
                    StatusId = x.Item.StatusId,
                    Used = x.Item.Used,
                    RowId = x.Item.RowId,
                },
                PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.PrimaryUnitOfMeasure.Id,
                    Code = x.PrimaryUnitOfMeasure.Code,
                    Name = x.PrimaryUnitOfMeasure.Name,
                    Description = x.PrimaryUnitOfMeasure.Description,
                    StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    Used = x.PrimaryUnitOfMeasure.Used,
                    RowId = x.PrimaryUnitOfMeasure.RowId,
                },
                TaxType = x.TaxType == null ? null : new TaxType
                {
                    Id = x.TaxType.Id,
                    Code = x.TaxType.Code,
                    Name = x.TaxType.Name,
                    Percentage = x.TaxType.Percentage,
                    StatusId = x.TaxType.StatusId,
                    Used = x.TaxType.Used,
                    RowId = x.TaxType.RowId,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                    Used = x.UnitOfMeasure.Used,
                    RowId = x.UnitOfMeasure.RowId,
                },
            }).ToListAsync();
            

            return CustomerSalesOrderContents;
        }

        public async Task<CustomerSalesOrderContent> Get(long Id)
        {
            CustomerSalesOrderContent CustomerSalesOrderContent = await DataContext.CustomerSalesOrderContent.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new CustomerSalesOrderContent()
            {
                Id = x.Id,
                CustomerSalesOrderId = x.CustomerSalesOrderId,
                ItemId = x.ItemId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                RequestedQuantity = x.RequestedQuantity,
                PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                SalePrice = x.SalePrice,
                PrimaryPrice = x.PrimaryPrice,
                DiscountPercentage = x.DiscountPercentage,
                DiscountAmount = x.DiscountAmount,
                GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                GeneralDiscountAmount = x.GeneralDiscountAmount,
                TaxPercentage = x.TaxPercentage,
                TaxAmount = x.TaxAmount,
                TaxPercentageOther = x.TaxPercentageOther,
                TaxAmountOther = x.TaxAmountOther,
                Amount = x.Amount,
                Factor = x.Factor,
                EditedPriceStatusId = x.EditedPriceStatusId,
                TaxTypeId = x.TaxTypeId,
                CustomerSalesOrder = x.CustomerSalesOrder == null ? null : new CustomerSalesOrder
                {
                    Id = x.CustomerSalesOrder.Id,
                    Code = x.CustomerSalesOrder.Code,
                    CustomerId = x.CustomerSalesOrder.CustomerId,
                    OrderSourceId = x.CustomerSalesOrder.OrderSourceId,
                    RequestStateId = x.CustomerSalesOrder.RequestStateId,
                    OrderPaymentStatusId = x.CustomerSalesOrder.OrderPaymentStatusId,
                    EditedPriceStatusId = x.CustomerSalesOrder.EditedPriceStatusId,
                    ShippingName = x.CustomerSalesOrder.ShippingName,
                    OrderDate = x.CustomerSalesOrder.OrderDate,
                    DeliveryDate = x.CustomerSalesOrder.DeliveryDate,
                    SalesEmployeeId = x.CustomerSalesOrder.SalesEmployeeId,
                    Note = x.CustomerSalesOrder.Note,
                    InvoiceAddress = x.CustomerSalesOrder.InvoiceAddress,
                    InvoiceNationId = x.CustomerSalesOrder.InvoiceNationId,
                    InvoiceProvinceId = x.CustomerSalesOrder.InvoiceProvinceId,
                    InvoiceDistrictId = x.CustomerSalesOrder.InvoiceDistrictId,
                    InvoiceWardId = x.CustomerSalesOrder.InvoiceWardId,
                    InvoiceZIPCode = x.CustomerSalesOrder.InvoiceZIPCode,
                    DeliveryAddress = x.CustomerSalesOrder.DeliveryAddress,
                    DeliveryNationId = x.CustomerSalesOrder.DeliveryNationId,
                    DeliveryProvinceId = x.CustomerSalesOrder.DeliveryProvinceId,
                    DeliveryDistrictId = x.CustomerSalesOrder.DeliveryDistrictId,
                    DeliveryWardId = x.CustomerSalesOrder.DeliveryWardId,
                    DeliveryZIPCode = x.CustomerSalesOrder.DeliveryZIPCode,
                    SubTotal = x.CustomerSalesOrder.SubTotal,
                    GeneralDiscountPercentage = x.CustomerSalesOrder.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.CustomerSalesOrder.GeneralDiscountAmount,
                    TotalTaxOther = x.CustomerSalesOrder.TotalTaxOther,
                    TotalTax = x.CustomerSalesOrder.TotalTax,
                    Total = x.CustomerSalesOrder.Total,
                    CreatorId = x.CustomerSalesOrder.CreatorId,
                    OrganizationId = x.CustomerSalesOrder.OrganizationId,
                    RowId = x.CustomerSalesOrder.RowId,
                    CodeGeneratorRuleId = x.CustomerSalesOrder.CodeGeneratorRuleId,
                },
                EditedPriceStatus = x.EditedPriceStatus == null ? null : new EditedPriceStatus
                {
                    Id = x.EditedPriceStatus.Id,
                    Code = x.EditedPriceStatus.Code,
                    Name = x.EditedPriceStatus.Name,
                },
                Item = x.Item == null ? null : new Item
                {
                    Id = x.Item.Id,
                    ProductId = x.Item.ProductId,
                    Code = x.Item.Code,
                    ERPCode = x.Item.ERPCode,
                    Name = x.Item.Name,
                    ScanCode = x.Item.ScanCode,
                    SalePrice = x.Item.SalePrice,
                    RetailPrice = x.Item.RetailPrice,
                    StatusId = x.Item.StatusId,
                    Used = x.Item.Used,
                    RowId = x.Item.RowId,
                },
                PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.PrimaryUnitOfMeasure.Id,
                    Code = x.PrimaryUnitOfMeasure.Code,
                    Name = x.PrimaryUnitOfMeasure.Name,
                    Description = x.PrimaryUnitOfMeasure.Description,
                    StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    Used = x.PrimaryUnitOfMeasure.Used,
                    RowId = x.PrimaryUnitOfMeasure.RowId,
                },
                TaxType = x.TaxType == null ? null : new TaxType
                {
                    Id = x.TaxType.Id,
                    Code = x.TaxType.Code,
                    Name = x.TaxType.Name,
                    Percentage = x.TaxType.Percentage,
                    StatusId = x.TaxType.StatusId,
                    Used = x.TaxType.Used,
                    RowId = x.TaxType.RowId,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                    Used = x.UnitOfMeasure.Used,
                    RowId = x.UnitOfMeasure.RowId,
                },
            }).FirstOrDefaultAsync();

            if (CustomerSalesOrderContent == null)
                return null;

            return CustomerSalesOrderContent;
        }
        
        public async Task<bool> Create(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            CustomerSalesOrderContentDAO CustomerSalesOrderContentDAO = new CustomerSalesOrderContentDAO();
            CustomerSalesOrderContentDAO.Id = CustomerSalesOrderContent.Id;
            CustomerSalesOrderContentDAO.CustomerSalesOrderId = CustomerSalesOrderContent.CustomerSalesOrderId;
            CustomerSalesOrderContentDAO.ItemId = CustomerSalesOrderContent.ItemId;
            CustomerSalesOrderContentDAO.UnitOfMeasureId = CustomerSalesOrderContent.UnitOfMeasureId;
            CustomerSalesOrderContentDAO.Quantity = CustomerSalesOrderContent.Quantity;
            CustomerSalesOrderContentDAO.RequestedQuantity = CustomerSalesOrderContent.RequestedQuantity;
            CustomerSalesOrderContentDAO.PrimaryUnitOfMeasureId = CustomerSalesOrderContent.PrimaryUnitOfMeasureId;
            CustomerSalesOrderContentDAO.SalePrice = CustomerSalesOrderContent.SalePrice;
            CustomerSalesOrderContentDAO.PrimaryPrice = CustomerSalesOrderContent.PrimaryPrice;
            CustomerSalesOrderContentDAO.DiscountPercentage = CustomerSalesOrderContent.DiscountPercentage;
            CustomerSalesOrderContentDAO.DiscountAmount = CustomerSalesOrderContent.DiscountAmount;
            CustomerSalesOrderContentDAO.GeneralDiscountPercentage = CustomerSalesOrderContent.GeneralDiscountPercentage;
            CustomerSalesOrderContentDAO.GeneralDiscountAmount = CustomerSalesOrderContent.GeneralDiscountAmount;
            CustomerSalesOrderContentDAO.TaxPercentage = CustomerSalesOrderContent.TaxPercentage;
            CustomerSalesOrderContentDAO.TaxAmount = CustomerSalesOrderContent.TaxAmount;
            CustomerSalesOrderContentDAO.TaxPercentageOther = CustomerSalesOrderContent.TaxPercentageOther;
            CustomerSalesOrderContentDAO.TaxAmountOther = CustomerSalesOrderContent.TaxAmountOther;
            CustomerSalesOrderContentDAO.Amount = CustomerSalesOrderContent.Amount;
            CustomerSalesOrderContentDAO.Factor = CustomerSalesOrderContent.Factor;
            CustomerSalesOrderContentDAO.EditedPriceStatusId = CustomerSalesOrderContent.EditedPriceStatusId;
            CustomerSalesOrderContentDAO.TaxTypeId = CustomerSalesOrderContent.TaxTypeId;
            DataContext.CustomerSalesOrderContent.Add(CustomerSalesOrderContentDAO);
            await DataContext.SaveChangesAsync();
            CustomerSalesOrderContent.Id = CustomerSalesOrderContentDAO.Id;
            await SaveReference(CustomerSalesOrderContent);
            return true;
        }

        public async Task<bool> Update(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            CustomerSalesOrderContentDAO CustomerSalesOrderContentDAO = DataContext.CustomerSalesOrderContent.Where(x => x.Id == CustomerSalesOrderContent.Id).FirstOrDefault();
            if (CustomerSalesOrderContentDAO == null)
                return false;
            CustomerSalesOrderContentDAO.Id = CustomerSalesOrderContent.Id;
            CustomerSalesOrderContentDAO.CustomerSalesOrderId = CustomerSalesOrderContent.CustomerSalesOrderId;
            CustomerSalesOrderContentDAO.ItemId = CustomerSalesOrderContent.ItemId;
            CustomerSalesOrderContentDAO.UnitOfMeasureId = CustomerSalesOrderContent.UnitOfMeasureId;
            CustomerSalesOrderContentDAO.Quantity = CustomerSalesOrderContent.Quantity;
            CustomerSalesOrderContentDAO.RequestedQuantity = CustomerSalesOrderContent.RequestedQuantity;
            CustomerSalesOrderContentDAO.PrimaryUnitOfMeasureId = CustomerSalesOrderContent.PrimaryUnitOfMeasureId;
            CustomerSalesOrderContentDAO.SalePrice = CustomerSalesOrderContent.SalePrice;
            CustomerSalesOrderContentDAO.PrimaryPrice = CustomerSalesOrderContent.PrimaryPrice;
            CustomerSalesOrderContentDAO.DiscountPercentage = CustomerSalesOrderContent.DiscountPercentage;
            CustomerSalesOrderContentDAO.DiscountAmount = CustomerSalesOrderContent.DiscountAmount;
            CustomerSalesOrderContentDAO.GeneralDiscountPercentage = CustomerSalesOrderContent.GeneralDiscountPercentage;
            CustomerSalesOrderContentDAO.GeneralDiscountAmount = CustomerSalesOrderContent.GeneralDiscountAmount;
            CustomerSalesOrderContentDAO.TaxPercentage = CustomerSalesOrderContent.TaxPercentage;
            CustomerSalesOrderContentDAO.TaxAmount = CustomerSalesOrderContent.TaxAmount;
            CustomerSalesOrderContentDAO.TaxPercentageOther = CustomerSalesOrderContent.TaxPercentageOther;
            CustomerSalesOrderContentDAO.TaxAmountOther = CustomerSalesOrderContent.TaxAmountOther;
            CustomerSalesOrderContentDAO.Amount = CustomerSalesOrderContent.Amount;
            CustomerSalesOrderContentDAO.Factor = CustomerSalesOrderContent.Factor;
            CustomerSalesOrderContentDAO.EditedPriceStatusId = CustomerSalesOrderContent.EditedPriceStatusId;
            CustomerSalesOrderContentDAO.TaxTypeId = CustomerSalesOrderContent.TaxTypeId;
            await DataContext.SaveChangesAsync();
            await SaveReference(CustomerSalesOrderContent);
            return true;
        }

        public async Task<bool> Delete(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
            await DataContext.CustomerSalesOrderContent.Where(x => x.Id == CustomerSalesOrderContent.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            List<CustomerSalesOrderContentDAO> CustomerSalesOrderContentDAOs = new List<CustomerSalesOrderContentDAO>();
            foreach (CustomerSalesOrderContent CustomerSalesOrderContent in CustomerSalesOrderContents)
            {
                CustomerSalesOrderContentDAO CustomerSalesOrderContentDAO = new CustomerSalesOrderContentDAO();
                CustomerSalesOrderContentDAO.Id = CustomerSalesOrderContent.Id;
                CustomerSalesOrderContentDAO.CustomerSalesOrderId = CustomerSalesOrderContent.CustomerSalesOrderId;
                CustomerSalesOrderContentDAO.ItemId = CustomerSalesOrderContent.ItemId;
                CustomerSalesOrderContentDAO.UnitOfMeasureId = CustomerSalesOrderContent.UnitOfMeasureId;
                CustomerSalesOrderContentDAO.Quantity = CustomerSalesOrderContent.Quantity;
                CustomerSalesOrderContentDAO.RequestedQuantity = CustomerSalesOrderContent.RequestedQuantity;
                CustomerSalesOrderContentDAO.PrimaryUnitOfMeasureId = CustomerSalesOrderContent.PrimaryUnitOfMeasureId;
                CustomerSalesOrderContentDAO.SalePrice = CustomerSalesOrderContent.SalePrice;
                CustomerSalesOrderContentDAO.PrimaryPrice = CustomerSalesOrderContent.PrimaryPrice;
                CustomerSalesOrderContentDAO.DiscountPercentage = CustomerSalesOrderContent.DiscountPercentage;
                CustomerSalesOrderContentDAO.DiscountAmount = CustomerSalesOrderContent.DiscountAmount;
                CustomerSalesOrderContentDAO.GeneralDiscountPercentage = CustomerSalesOrderContent.GeneralDiscountPercentage;
                CustomerSalesOrderContentDAO.GeneralDiscountAmount = CustomerSalesOrderContent.GeneralDiscountAmount;
                CustomerSalesOrderContentDAO.TaxPercentage = CustomerSalesOrderContent.TaxPercentage;
                CustomerSalesOrderContentDAO.TaxAmount = CustomerSalesOrderContent.TaxAmount;
                CustomerSalesOrderContentDAO.TaxPercentageOther = CustomerSalesOrderContent.TaxPercentageOther;
                CustomerSalesOrderContentDAO.TaxAmountOther = CustomerSalesOrderContent.TaxAmountOther;
                CustomerSalesOrderContentDAO.Amount = CustomerSalesOrderContent.Amount;
                CustomerSalesOrderContentDAO.Factor = CustomerSalesOrderContent.Factor;
                CustomerSalesOrderContentDAO.EditedPriceStatusId = CustomerSalesOrderContent.EditedPriceStatusId;
                CustomerSalesOrderContentDAO.TaxTypeId = CustomerSalesOrderContent.TaxTypeId;
                CustomerSalesOrderContentDAOs.Add(CustomerSalesOrderContentDAO);
            }
            await DataContext.BulkMergeAsync(CustomerSalesOrderContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<CustomerSalesOrderContent> CustomerSalesOrderContents)
        {
            List<long> Ids = CustomerSalesOrderContents.Select(x => x.Id).ToList();
            await DataContext.CustomerSalesOrderContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(CustomerSalesOrderContent CustomerSalesOrderContent)
        {
        }
        
    }
}
