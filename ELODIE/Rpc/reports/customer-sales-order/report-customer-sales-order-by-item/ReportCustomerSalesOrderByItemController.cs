using ELODIE.Common;
using ELODIE.Models;
using ELODIE.Services.MProduct;
using ELODIE.Services.MProductGrouping;
using ELODIE.Services.MProductType;
using Microsoft.AspNetCore.Mvc;
using System;
using ELODIE.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Enums;
using ELODIE.Helpers;
using ELODIE.Services.MCustomerSalesOrder;
using ELODIE.Services.MOrganization;
using Microsoft.EntityFrameworkCore;
using ELODIE.Repositories;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using ELODIE.Rpc.customer_sales_order;
using ELODIE.Services.MAppUser;

namespace ELODIE.Rpc.reports.report_customer_sales_order.report_customer_sales_order_by_item
{
    public class ReportCustomerSalesOrderByItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICustomerSalesOrderService CustomerSalesOrderService;
        private IItemService ItemService;
        private IProductService ProductService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ReportCustomerSalesOrderByItemController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            ICustomerSalesOrderService CustomerSalesOrderService,
            IProductService ProductService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CustomerSalesOrderService = CustomerSalesOrderService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportCustomerSalesOrderByItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportCustomerSalesOrderByItem_OrganizationDTO>> FilterListOrganization()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportCustomerSalesOrderByItem_OrganizationDTO> ReportCustomerSalesOrderByItem_OrganizationDTO = Organizations
                .Select(x => new ReportCustomerSalesOrderByItem_OrganizationDTO(x)).ToList();
            return ReportCustomerSalesOrderByItem_OrganizationDTO;
        }

        [Route(ReportCustomerSalesOrderByItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<ReportCustomerSalesOrderByItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] ReportCustomerSalesOrderByItem_ProductGroupingFilterDTO ReportCustomerSalesOrderByItem_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;
            ProductGroupingFilter.Code = ReportCustomerSalesOrderByItem_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ReportCustomerSalesOrderByItem_ProductGroupingFilterDTO.Name;

            if (ProductGroupingFilter.Id == null) ProductGroupingFilter.Id = new IdFilter();
            ProductGroupingFilter.Id.In = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            List<ProductGrouping> ReportCustomerSalesOrderByItemGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ReportCustomerSalesOrderByItem_ProductGroupingDTO> ReportCustomerSalesOrderByItem_ProductGroupingDTOs = ReportCustomerSalesOrderByItemGroupings
                .Select(x => new ReportCustomerSalesOrderByItem_ProductGroupingDTO(x)).ToList();
            return ReportCustomerSalesOrderByItem_ProductGroupingDTOs;
        }

        [Route(ReportCustomerSalesOrderByItemRoute.FilterListProductType), HttpPost]
        public async Task<List<ReportCustomerSalesOrderByItem_ProductTypeDTO>> FilterListProductType([FromBody] ReportCustomerSalesOrderByItem_ProductTypeFilterDTO ReportCustomerSalesOrderByItem_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = ReportCustomerSalesOrderByItem_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ReportCustomerSalesOrderByItem_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ReportCustomerSalesOrderByItem_ProductTypeFilterDTO.Name;
            ProductTypeFilter.StatusId = ReportCustomerSalesOrderByItem_ProductTypeFilterDTO.StatusId;

            if (ProductTypeFilter.Id == null) ProductTypeFilter.Id = new IdFilter();
            ProductTypeFilter.Id.In = await FilterProductType(ProductTypeService, CurrentContext);
            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ReportCustomerSalesOrderByItem_ProductTypeDTO> ReportCustomerSalesOrderByItem_ProductTypeDTOs = ProductTypes
                .Select(x => new ReportCustomerSalesOrderByItem_ProductTypeDTO(x)).ToList();
            return ReportCustomerSalesOrderByItem_ProductTypeDTOs;
        }

        [Route(ReportCustomerSalesOrderByItemRoute.FilterListItem), HttpPost]
        public async Task<List<ReportCustomerSalesOrderByItem_ItemDTO>> FilterListItem([FromBody] ReportCustomerSalesOrderByItem_ItemFilterDTO ReportCustomerSalesOrderByItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportCustomerSalesOrderByItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportCustomerSalesOrderByItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportCustomerSalesOrderByItem_ItemFilterDTO.Name;
            ItemFilter.StatusId = ReportCustomerSalesOrderByItem_ItemFilterDTO.StatusId;
            ItemFilter.Search = ReportCustomerSalesOrderByItem_ItemFilterDTO.Search;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportCustomerSalesOrderByItem_ItemDTO> ReportCustomerSalesOrderByItem_ItemDTOs = Items
                .Select(x => new ReportCustomerSalesOrderByItem_ItemDTO(x)).ToList();
            return ReportCustomerSalesOrderByItem_ItemDTOs;
        }
        #endregion

        [Route(ReportCustomerSalesOrderByItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.HasValue == false)
            //    return 0;

            DateTime Now = StaticParams.DateTimeNow;
            var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));

            DateTime Start = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                     //LocalStartDay(CurrentContext) :
                     new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone) :
            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone) :
                    ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            //if (End.Subtract(Start).Days > 31)
            //    return 0;

            long? ItemId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ItemId?.Equal;
            //long? ProductTypeId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            //long? ProductGroupingId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            //if (ProductTypeId.HasValue)
            //{
            //    var listId = new List<long> { ProductTypeId.Value };
            //    ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            //}
            //List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            //if (ProductGroupingId.HasValue)
            //{
            //    var listId = new List<long> { ProductGroupingId.Value };
            //    ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            //}
            var query = from t in DataContext.CustomerSalesOrderTransaction
                        join od in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SalesEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        //(ProductTypeIds.Contains(p.ProductTypeId)) &&
                        //(
                        //    sppgm == null ||
                        //    (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        //) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.COMPLETED.Id
                        group t by new {t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            return await query.CountAsync();
        }

        [Route(ReportCustomerSalesOrderByItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO>>> List([FromBody] ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.HasValue == false)
            //    return new List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO>();

            DateTime Now = StaticParams.DateTimeNow;
            var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));

            DateTime Start = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                     //LocalStartDay(CurrentContext) :
                     new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone) :
            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone) :
                    ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            //if (End.Subtract(Start).Days > 31)
            //    return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO> ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs = await ListData(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO, Start, End);
            return ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs;
        }

        [Route(ReportCustomerSalesOrderByItemRoute.Total), HttpPost]
        public async Task<ReportCustomerSalesOrderByItem_TotalDTO> Total([FromBody] ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.HasValue == false)
                return new ReportCustomerSalesOrderByItem_TotalDTO();

            DateTime Start = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                   LocalStartDay(CurrentContext) :
                   ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            //long? ItemId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ItemId?.Equal;
            //long? ProductTypeId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            //long? ProductGroupingId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            if (End.Subtract(Start).Days > 31)
                return new ReportCustomerSalesOrderByItem_TotalDTO();

            ReportCustomerSalesOrderByItem_TotalDTO ReportCustomerSalesOrderByItem_TotalDTO = await TotalData(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO, Start, End);
            return ReportCustomerSalesOrderByItem_TotalDTO;
        }

        [Route(ReportCustomerSalesOrderByItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Skip = 0;
            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Take = int.MaxValue;
            List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO> ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs = await ListData(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO, Start, End);

            ReportCustomerSalesOrderByItem_TotalDTO ReportCustomerSalesOrderByItem_TotalDTO = await TotalData(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO, Start, End);
            long stt = 1;
            foreach (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO in ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs)
            {
                foreach (var Item in ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.ItemDetails)
                {
                    Item.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Customer_Sales_Order_By_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByItems = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs;
            Data.Total = ReportCustomerSalesOrderByItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportCustomerSalesOrderByItem.xlsx");
        }

        private async Task<List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO>> ListData(
            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ItemId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ItemId?.Equal;
            //long? ProductTypeId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            //long? ProductGroupingId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            //List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            //if (ProductTypeId.HasValue)
            //{
            //    var listId = new List<long> { ProductTypeId.Value };
            //    ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            //}
            //List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            //if (ProductGroupingId.HasValue)
            //{
            //    var listId = new List<long> { ProductGroupingId.Value };
            //    ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            //}

            var query = from t in DataContext.CustomerSalesOrderTransaction
                        join od in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SalesEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        //(ProductTypeIds.Contains(p.ProductTypeId)) &&
                        //(
                        //    sppgm == null ||
                        //    (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        //) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.COMPLETED.Id
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.ItemId)
                .Skip(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Skip)
                .Take(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.Take)
                .ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();
            var OrganizationNames = await DataContext.Organization.Where(x => OrgIds.Contains(x.Id)).OrderBy(x => x.Id).Select(x => x.Name).ToListAsync();
            List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO> ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs = new List<ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO>();
            foreach (var OrganizationName in OrganizationNames)
            {
                ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO = new ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO();
                ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.OrganizationName = OrganizationName;
                ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.ItemDetails = new List<ReportCustomerSalesOrderByItem_ItemDetailDTO>();
                ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs.Add(ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO);
            }

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.CustomerSalesOrderTransaction
                                   join od in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals od.Id
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join ind in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   AppUserIds.Contains(od.SalesEmployeeId) &&
                                   od.OrderDate >= Start && od.OrderDate <= End &&
                                   ind.RequestStateId == RequestStateEnum.COMPLETED.Id
                                   select new CustomerSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       CustomerSalesOrderId = t.CustomerSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       Item = new ItemDAO
                                       {
                                           Id = i.Id,
                                           Code = i.Code,
                                           Name = i.Name,
                                       },
                                       Organization = new OrganizationDAO
                                       {
                                           Name = o.Name
                                       },
                                       UnitOfMeasure = new UnitOfMeasureDAO
                                       {
                                           Name = u.Name
                                       }
                                   };
            var CustomerSalesOrderTransactions = await queryTransaction.ToListAsync();

            foreach (var ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO in ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs)
            {
                var Transactions = CustomerSalesOrderTransactions.Where(x => x.Organization.Name == ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.OrganizationName);
                foreach (var Transaction in Transactions)
                {
                    var ItemDetail = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.ItemDetails.Where(x => x.ItemId == Transaction.ItemId).FirstOrDefault();
                    if (ItemDetail == null)
                    {
                        ItemDetail = new ReportCustomerSalesOrderByItem_ItemDetailDTO();
                        ItemDetail.ItemId = Transaction.Item.Id;
                        ItemDetail.ItemCode = Transaction.Item.Code;
                        ItemDetail.ItemName = Transaction.Item.Name;
                        ItemDetail.UnitOfMeasureName = Transaction.UnitOfMeasure.Name;
                        ItemDetail.CustomerSalesOrderIds = new HashSet<long>();
                        ItemDetail.BuyerStoreIds = new HashSet<long>();
                        ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.ItemDetails.Add(ItemDetail);
                    }
                    if (Transaction.TypeId == TransactionTypeEnum.SALES_CONTENT.Id)
                    {
                        ItemDetail.SaleStock += Transaction.Quantity;
                    }
                    if (Transaction.TypeId == TransactionTypeEnum.PROMOTION.Id)
                    {
                        ItemDetail.PromotionStock += Transaction.Quantity;
                    }
                    ItemDetail.Discount += Transaction.Discount ?? 0;
                    ItemDetail.Revenue += Transaction.Revenue ?? 0;
                    ItemDetail.CustomerSalesOrderIds.Add(Transaction.CustomerSalesOrderId);
                }
            }
            //làm tròn số
            foreach (var ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO in ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs)
            {
                foreach (var item in ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTO.ItemDetails)
                {
                    item.Discount = Math.Round(item.Discount, 0);
                    item.Revenue = Math.Round(item.Revenue, 0);
                }
            }

            return ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemDTOs;
        }

        private async Task<ReportCustomerSalesOrderByItem_TotalDTO> TotalData(
            ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ItemId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ItemId?.Equal;
            //long? ProductTypeId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            //long? ProductGroupingId = ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            ReportCustomerSalesOrderByItem_TotalDTO ReportCustomerSalesOrderByItem_TotalDTO = new ReportCustomerSalesOrderByItem_TotalDTO();

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportCustomerSalesOrderByItem_ReportCustomerSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            //List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            //if (ProductTypeId.HasValue)
            //{
            //    var listId = new List<long> { ProductTypeId.Value };
            //    ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            //}
            //List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            //if (ProductGroupingId.HasValue)
            //{
            //    var listId = new List<long> { ProductGroupingId.Value };
            //    ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            //}

            var query = from t in DataContext.CustomerSalesOrderTransaction
                        join od in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SalesEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        //(ProductTypeIds.Contains(p.ProductTypeId)) &&
                        //(
                        //    sppgm == null ||
                        //    (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        //) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.COMPLETED.Id
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query.ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.CustomerSalesOrderTransaction
                                   join od in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals od.Id
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join ind in DataContext.CustomerSalesOrder on t.CustomerSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   od.OrderDate >= Start && od.OrderDate <= End &&
                                   ind.RequestStateId == RequestStateEnum.COMPLETED.Id
                                   select new CustomerSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       CustomerSalesOrderId = t.CustomerSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       Item = new ItemDAO
                                       {
                                           Id = i.Id,
                                           Code = i.Code,
                                           Name = i.Name,
                                       },
                                       Organization = new OrganizationDAO
                                       {
                                           Name = o.Name
                                       },
                                       UnitOfMeasure = new UnitOfMeasureDAO
                                       {
                                           Name = u.Name
                                       }
                                   };
            var CustomerSalesOrderTransactions = await queryTransaction.ToListAsync();

            ReportCustomerSalesOrderByItem_TotalDTO.TotalDiscount = CustomerSalesOrderTransactions
                .Where(x => x.Discount.HasValue)
                .Select(x => x.Discount.Value)
                .DefaultIfEmpty(0)
                .Sum();
            ReportCustomerSalesOrderByItem_TotalDTO.TotalRevenue = CustomerSalesOrderTransactions
                .Where(x => x.Revenue.HasValue)
                .Select(x => x.Revenue.Value)
                .DefaultIfEmpty(0)
                .Sum();

            ReportCustomerSalesOrderByItem_TotalDTO.TotalPromotionStock = CustomerSalesOrderTransactions
                .Where(x => x.TypeId == TransactionTypeEnum.PROMOTION.Id)
                .Select(x => x.Quantity)
                .Sum();
            ReportCustomerSalesOrderByItem_TotalDTO.TotalSalesStock = CustomerSalesOrderTransactions
                .Where(x => x.TypeId == TransactionTypeEnum.SALES_CONTENT.Id)
                .Select(x => x.Quantity)
                .Sum();

            return ReportCustomerSalesOrderByItem_TotalDTO;
        }

    }
}
