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
    public interface ICustomerSalesOrderPaymentHistoryRepository
    {
        Task<int> CountAll(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
        Task<int> Count(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
        Task<List<CustomerSalesOrderPaymentHistory>> List(CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter);
        Task<List<CustomerSalesOrderPaymentHistory>> List(List<long> Ids);
        Task<CustomerSalesOrderPaymentHistory> Get(long Id);
        Task<bool> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory);
        Task<bool> BulkMerge(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
        Task<bool> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories);
    }
    public class CustomerSalesOrderPaymentHistoryRepository : ICustomerSalesOrderPaymentHistoryRepository
    {
        private DataContext DataContext;
        public CustomerSalesOrderPaymentHistoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CustomerSalesOrderPaymentHistoryDAO> DynamicFilter(IQueryable<CustomerSalesOrderPaymentHistoryDAO> query, CustomerSalesOrderPaymentHistoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.CustomerSalesOrderId, filter.CustomerSalesOrderId);
            query = query.Where(q => q.PaymentMilestone, filter.PaymentMilestone);
            query = query.Where(q => q.PaymentPercentage, filter.PaymentPercentage);
            query = query.Where(q => q.PaymentAmount, filter.PaymentAmount);
            query = query.Where(q => q.PaymentTypeId, filter.PaymentTypeId);
            query = query.Where(q => q.Description, filter.Description);
            
            return query;
        }

        private IQueryable<CustomerSalesOrderPaymentHistoryDAO> OrFilter(IQueryable<CustomerSalesOrderPaymentHistoryDAO> query, CustomerSalesOrderPaymentHistoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CustomerSalesOrderPaymentHistoryDAO> initQuery = query.Where(q => false);
            foreach (CustomerSalesOrderPaymentHistoryFilter CustomerSalesOrderPaymentHistoryFilter in filter.OrFilter)
            {
                IQueryable<CustomerSalesOrderPaymentHistoryDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.CustomerSalesOrderId, filter.CustomerSalesOrderId);
                queryable = queryable.Where(q => q.PaymentMilestone, filter.PaymentMilestone);
                queryable = queryable.Where(q => q.PaymentPercentage, filter.PaymentPercentage);
                queryable = queryable.Where(q => q.PaymentAmount, filter.PaymentAmount);
                queryable = queryable.Where(q => q.PaymentTypeId, filter.PaymentTypeId);
                queryable = queryable.Where(q => q.Description, filter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CustomerSalesOrderPaymentHistoryDAO> DynamicOrder(IQueryable<CustomerSalesOrderPaymentHistoryDAO> query, CustomerSalesOrderPaymentHistoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSalesOrderPaymentHistoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.CustomerSalesOrder:
                            query = query.OrderBy(q => q.CustomerSalesOrderId);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentMilestone:
                            query = query.OrderBy(q => q.PaymentMilestone);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentPercentage:
                            query = query.OrderBy(q => q.PaymentPercentage);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentAmount:
                            query = query.OrderBy(q => q.PaymentAmount);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentType:
                            query = query.OrderBy(q => q.PaymentTypeId);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.IsPaid:
                            query = query.OrderBy(q => q.IsPaid);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CustomerSalesOrderPaymentHistoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.CustomerSalesOrder:
                            query = query.OrderByDescending(q => q.CustomerSalesOrderId);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentMilestone:
                            query = query.OrderByDescending(q => q.PaymentMilestone);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentPercentage:
                            query = query.OrderByDescending(q => q.PaymentPercentage);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentAmount:
                            query = query.OrderByDescending(q => q.PaymentAmount);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.PaymentType:
                            query = query.OrderByDescending(q => q.PaymentTypeId);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case CustomerSalesOrderPaymentHistoryOrder.IsPaid:
                            query = query.OrderByDescending(q => q.IsPaid);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CustomerSalesOrderPaymentHistory>> DynamicSelect(IQueryable<CustomerSalesOrderPaymentHistoryDAO> query, CustomerSalesOrderPaymentHistoryFilter filter)
        {
            List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await query.Select(q => new CustomerSalesOrderPaymentHistory()
            {
                Id = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.Id) ? q.Id : default(long),
                CustomerSalesOrderId = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.CustomerSalesOrder) ? q.CustomerSalesOrderId : default(long),
                PaymentMilestone = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.PaymentMilestone) ? q.PaymentMilestone : default(string),
                PaymentPercentage = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.PaymentPercentage) ? q.PaymentPercentage : default(decimal?),
                PaymentAmount = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.PaymentAmount) ? q.PaymentAmount : default(decimal?),
                PaymentTypeId = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.PaymentType) ? q.PaymentTypeId : default(long?),
                Description = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.Description) ? q.Description : default(string),
                IsPaid = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.IsPaid) ? q.IsPaid : default(bool?),
                CustomerSalesOrder = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.CustomerSalesOrder) && q.CustomerSalesOrder != null ? new CustomerSalesOrder
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
                PaymentType = filter.Selects.Contains(CustomerSalesOrderPaymentHistorySelect.PaymentType) && q.PaymentType != null ? new PaymentType
                {
                    Id = q.PaymentType.Id,
                    Code = q.PaymentType.Code,
                    Name = q.PaymentType.Name,
                    StatusId = q.PaymentType.StatusId,
                    Used = q.PaymentType.Used,
                    RowId = q.PaymentType.RowId,
                } : null,
            }).ToListAsync();
            return CustomerSalesOrderPaymentHistories;
        }

        public async Task<int> CountAll(CustomerSalesOrderPaymentHistoryFilter filter)
        {
            IQueryable<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistories = DataContext.CustomerSalesOrderPaymentHistory.AsNoTracking();
            CustomerSalesOrderPaymentHistories = DynamicFilter(CustomerSalesOrderPaymentHistories, filter);
            return await CustomerSalesOrderPaymentHistories.CountAsync();
        }

        public async Task<int> Count(CustomerSalesOrderPaymentHistoryFilter filter)
        {
            IQueryable<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistories = DataContext.CustomerSalesOrderPaymentHistory.AsNoTracking();
            CustomerSalesOrderPaymentHistories = DynamicFilter(CustomerSalesOrderPaymentHistories, filter);
            CustomerSalesOrderPaymentHistories = OrFilter(CustomerSalesOrderPaymentHistories, filter);
            return await CustomerSalesOrderPaymentHistories.CountAsync();
        }

        public async Task<List<CustomerSalesOrderPaymentHistory>> List(CustomerSalesOrderPaymentHistoryFilter filter)
        {
            if (filter == null) return new List<CustomerSalesOrderPaymentHistory>();
            IQueryable<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistoryDAOs = DataContext.CustomerSalesOrderPaymentHistory.AsNoTracking();
            CustomerSalesOrderPaymentHistoryDAOs = DynamicFilter(CustomerSalesOrderPaymentHistoryDAOs, filter);
            CustomerSalesOrderPaymentHistoryDAOs = OrFilter(CustomerSalesOrderPaymentHistoryDAOs, filter);
            CustomerSalesOrderPaymentHistoryDAOs = DynamicOrder(CustomerSalesOrderPaymentHistoryDAOs, filter);
            List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await DynamicSelect(CustomerSalesOrderPaymentHistoryDAOs, filter);
            return CustomerSalesOrderPaymentHistories;
        }

        public async Task<List<CustomerSalesOrderPaymentHistory>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.CustomerSalesOrderPaymentHistory
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories = await query.AsNoTracking()
            .Select(x => new CustomerSalesOrderPaymentHistory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                CustomerSalesOrderId = x.CustomerSalesOrderId,
                PaymentMilestone = x.PaymentMilestone,
                PaymentPercentage = x.PaymentPercentage,
                PaymentAmount = x.PaymentAmount,
                PaymentTypeId = x.PaymentTypeId,
                Description = x.Description,
                IsPaid = x.IsPaid,
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
                PaymentType = x.PaymentType == null ? null : new PaymentType
                {
                    Id = x.PaymentType.Id,
                    Code = x.PaymentType.Code,
                    Name = x.PaymentType.Name,
                    StatusId = x.PaymentType.StatusId,
                    Used = x.PaymentType.Used,
                    RowId = x.PaymentType.RowId,
                },
            }).ToListAsync();
            

            return CustomerSalesOrderPaymentHistories;
        }

        public async Task<CustomerSalesOrderPaymentHistory> Get(long Id)
        {
            CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory = await DataContext.CustomerSalesOrderPaymentHistory.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new CustomerSalesOrderPaymentHistory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                CustomerSalesOrderId = x.CustomerSalesOrderId,
                PaymentMilestone = x.PaymentMilestone,
                PaymentPercentage = x.PaymentPercentage,
                PaymentAmount = x.PaymentAmount,
                PaymentTypeId = x.PaymentTypeId,
                Description = x.Description,
                IsPaid = x.IsPaid,
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
                PaymentType = x.PaymentType == null ? null : new PaymentType
                {
                    Id = x.PaymentType.Id,
                    Code = x.PaymentType.Code,
                    Name = x.PaymentType.Name,
                    StatusId = x.PaymentType.StatusId,
                    Used = x.PaymentType.Used,
                    RowId = x.PaymentType.RowId,
                },
            }).FirstOrDefaultAsync();

            if (CustomerSalesOrderPaymentHistory == null)
                return null;

            return CustomerSalesOrderPaymentHistory;
        }
        
        public async Task<bool> Create(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            CustomerSalesOrderPaymentHistoryDAO CustomerSalesOrderPaymentHistoryDAO = new CustomerSalesOrderPaymentHistoryDAO();
            CustomerSalesOrderPaymentHistoryDAO.Id = CustomerSalesOrderPaymentHistory.Id;
            CustomerSalesOrderPaymentHistoryDAO.CustomerSalesOrderId = CustomerSalesOrderPaymentHistory.CustomerSalesOrderId;
            CustomerSalesOrderPaymentHistoryDAO.PaymentMilestone = CustomerSalesOrderPaymentHistory.PaymentMilestone;
            CustomerSalesOrderPaymentHistoryDAO.PaymentPercentage = CustomerSalesOrderPaymentHistory.PaymentPercentage;
            CustomerSalesOrderPaymentHistoryDAO.PaymentAmount = CustomerSalesOrderPaymentHistory.PaymentAmount;
            CustomerSalesOrderPaymentHistoryDAO.PaymentTypeId = CustomerSalesOrderPaymentHistory.PaymentTypeId;
            CustomerSalesOrderPaymentHistoryDAO.Description = CustomerSalesOrderPaymentHistory.Description;
            CustomerSalesOrderPaymentHistoryDAO.IsPaid = CustomerSalesOrderPaymentHistory.IsPaid;
            CustomerSalesOrderPaymentHistoryDAO.CreatedAt = StaticParams.DateTimeNow;
            CustomerSalesOrderPaymentHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.CustomerSalesOrderPaymentHistory.Add(CustomerSalesOrderPaymentHistoryDAO);
            await DataContext.SaveChangesAsync();
            CustomerSalesOrderPaymentHistory.Id = CustomerSalesOrderPaymentHistoryDAO.Id;
            await SaveReference(CustomerSalesOrderPaymentHistory);
            return true;
        }

        public async Task<bool> Update(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            CustomerSalesOrderPaymentHistoryDAO CustomerSalesOrderPaymentHistoryDAO = DataContext.CustomerSalesOrderPaymentHistory.Where(x => x.Id == CustomerSalesOrderPaymentHistory.Id).FirstOrDefault();
            if (CustomerSalesOrderPaymentHistoryDAO == null)
                return false;
            CustomerSalesOrderPaymentHistoryDAO.Id = CustomerSalesOrderPaymentHistory.Id;
            CustomerSalesOrderPaymentHistoryDAO.CustomerSalesOrderId = CustomerSalesOrderPaymentHistory.CustomerSalesOrderId;
            CustomerSalesOrderPaymentHistoryDAO.PaymentMilestone = CustomerSalesOrderPaymentHistory.PaymentMilestone;
            CustomerSalesOrderPaymentHistoryDAO.PaymentPercentage = CustomerSalesOrderPaymentHistory.PaymentPercentage;
            CustomerSalesOrderPaymentHistoryDAO.PaymentAmount = CustomerSalesOrderPaymentHistory.PaymentAmount;
            CustomerSalesOrderPaymentHistoryDAO.PaymentTypeId = CustomerSalesOrderPaymentHistory.PaymentTypeId;
            CustomerSalesOrderPaymentHistoryDAO.Description = CustomerSalesOrderPaymentHistory.Description;
            CustomerSalesOrderPaymentHistoryDAO.IsPaid = CustomerSalesOrderPaymentHistory.IsPaid;
            CustomerSalesOrderPaymentHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(CustomerSalesOrderPaymentHistory);
            return true;
        }

        public async Task<bool> Delete(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
            await DataContext.CustomerSalesOrderPaymentHistory.Where(x => x.Id == CustomerSalesOrderPaymentHistory.Id).UpdateFromQueryAsync(x => new CustomerSalesOrderPaymentHistoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            List<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistoryDAOs = new List<CustomerSalesOrderPaymentHistoryDAO>();
            foreach (CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory in CustomerSalesOrderPaymentHistories)
            {
                CustomerSalesOrderPaymentHistoryDAO CustomerSalesOrderPaymentHistoryDAO = new CustomerSalesOrderPaymentHistoryDAO();
                CustomerSalesOrderPaymentHistoryDAO.Id = CustomerSalesOrderPaymentHistory.Id;
                CustomerSalesOrderPaymentHistoryDAO.CustomerSalesOrderId = CustomerSalesOrderPaymentHistory.CustomerSalesOrderId;
                CustomerSalesOrderPaymentHistoryDAO.PaymentMilestone = CustomerSalesOrderPaymentHistory.PaymentMilestone;
                CustomerSalesOrderPaymentHistoryDAO.PaymentPercentage = CustomerSalesOrderPaymentHistory.PaymentPercentage;
                CustomerSalesOrderPaymentHistoryDAO.PaymentAmount = CustomerSalesOrderPaymentHistory.PaymentAmount;
                CustomerSalesOrderPaymentHistoryDAO.PaymentTypeId = CustomerSalesOrderPaymentHistory.PaymentTypeId;
                CustomerSalesOrderPaymentHistoryDAO.Description = CustomerSalesOrderPaymentHistory.Description;
                CustomerSalesOrderPaymentHistoryDAO.IsPaid = CustomerSalesOrderPaymentHistory.IsPaid;
                CustomerSalesOrderPaymentHistoryDAO.CreatedAt = StaticParams.DateTimeNow;
                CustomerSalesOrderPaymentHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                CustomerSalesOrderPaymentHistoryDAOs.Add(CustomerSalesOrderPaymentHistoryDAO);
            }
            await DataContext.BulkMergeAsync(CustomerSalesOrderPaymentHistoryDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<CustomerSalesOrderPaymentHistory> CustomerSalesOrderPaymentHistories)
        {
            List<long> Ids = CustomerSalesOrderPaymentHistories.Select(x => x.Id).ToList();
            await DataContext.CustomerSalesOrderPaymentHistory
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new CustomerSalesOrderPaymentHistoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(CustomerSalesOrderPaymentHistory CustomerSalesOrderPaymentHistory)
        {
        }
        
    }
}
