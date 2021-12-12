using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MOrganization;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Enums;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using ELODIE.Services.MProvince;
using ELODIE.Services.MCustomer;

namespace ELODIE.Rpc.dashboards.order
{
    public class DashboardOrderController : RpcController
    {
        private const long TODAY = 1;
        private const long THIS_WEEK = 2;
        private const long THIS_MONTH = 3;
        private const long LAST_MONTH = 4;
        private const long THIS_QUARTER = 5;
        private const long LAST_QUATER = 6;
        private const long YEAR = 7;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private ICustomerService CustomerService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        public DashboardOrderController(
            DataContext DataContext,
            IAppUserService AppUserService,
            ICustomerService CustomerService,
            ICurrentContext CurrentContext,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService
            )
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.CustomerService = CustomerService;
            this.CurrentContext = CurrentContext;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
        }

        #region Filter List
        [Route(DashboardOrderRoute.FilterListTime), HttpPost]
        public List<DashboardOrder_EnumList> FilterListTime()
        {
            List<DashboardOrder_EnumList> Dashborad_EnumLists = new List<DashboardOrder_EnumList>();
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = TODAY, Name = "Hôm nay" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = THIS_QUARTER, Name = "Quý này" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = LAST_QUATER, Name = "Quý trước" });
            Dashborad_EnumLists.Add(new DashboardOrder_EnumList { Id = YEAR, Name = "Năm" });
            return Dashborad_EnumLists;
        }

        [Route(DashboardOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<DashboardOrder_AppUserDTO>> FilterListAppUser([FromBody] DashboardOrder_AppUserFilterDTO DashboardOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DashboardOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = DashboardOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = DashboardOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DashboardOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DashboardOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = DashboardOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DashboardOrder_AppUserFilterDTO.Phone;
            AppUserFilter.Department = DashboardOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DashboardOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DashboardOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = DashboardOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = DashboardOrder_AppUserFilterDTO.Birthday;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DashboardOrder_AppUserDTO> DashboardOrder_AppUserDTOs = AppUsers
                .Select(x => new DashboardOrder_AppUserDTO(x)).ToList();
            return DashboardOrder_AppUserDTOs;
        }

        
        [Route(DashboardOrderRoute.FilterListOrganization), HttpPost]
        public async Task<List<DashboardOrder_OrganizationDTO>> FilterListOrganization([FromBody] DashboardOrder_OrganizationFilterDTO DashboardOrder_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DashboardOrder_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DashboardOrder_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DashboardOrder_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DashboardOrder_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DashboardOrder_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DashboardOrder_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = DashboardOrder_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = DashboardOrder_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = DashboardOrder_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = DashboardOrder_OrganizationFilterDTO.Email;
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DashboardOrder_OrganizationDTO> DashboardOrder_OrganizationDTOs = Organizations
                .Select(x => new DashboardOrder_OrganizationDTO(x)).ToList();
            return DashboardOrder_OrganizationDTOs;
        }

        [Route(DashboardOrderRoute.FilterListProvince), HttpPost]
        public async Task<List<DashboardOrder_ProvinceDTO>> FilterListProvince([FromBody] DashboardOrder_ProvinceFilterDTO DashboardOrder_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = DashboardOrder_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = DashboardOrder_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = DashboardOrder_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<DashboardOrder_ProvinceDTO> DashboardOrder_ProvinceDTOs = Provinces
                .Select(x => new DashboardOrder_ProvinceDTO(x)).ToList();
            return DashboardOrder_ProvinceDTOs;
        }

        #endregion

        [Route(DashboardOrderRoute.TotalRevenue), HttpPost]
        public async Task<decimal> TotalRevenue([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId) &&
                             o.RequestStateId == RequestStateEnum.COMPLETED.Id
                             && o.DeletedAt == null
                             select o;

            var Orders = await queryOrder.ToListAsync();
            var Revenue = Orders.Select(x => x.Total).Sum();
            return Revenue;
        }

        [Route(DashboardOrderRoute.OrderCounter), HttpPost]
        public async Task<long> OrderCounter([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId) 
                             && o.DeletedAt == null
                             //&& o.RequestStateId == RequestStateEnum.COMPLETED.Id
                             select o;

            return await queryOrder.Select(x => x.Id).Distinct().CountAsync();
        }

        [Route(DashboardOrderRoute.InstagramGad8Counter), HttpPost]
        public async Task<long> InstagramGad8Counter([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
           
            var queryOrder = from o in DataContext.InstagramSupplier
                             where o.Code.Equals("gad8")
                             select o;

            return await queryOrder.Select(x => x.CountView).FirstOrDefaultAsync();
        }

        [Route(DashboardOrderRoute.CompletedOrderCounter), HttpPost]
        public async Task<long> CompletedOrderCounter([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId) &&
                             o.RequestStateId == RequestStateEnum.COMPLETED.Id
                             && o.DeletedAt == null
                             select o;

            return await queryOrder.Select(x => x.Id).Distinct().CountAsync();
        }

        [Route(DashboardOrderRoute.ProcessingOrderCounter), HttpPost]
        public async Task<long> ProcessingOrderCounter([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId) &&
                             o.RequestStateId == RequestStateEnum.PROCESSING.Id
                             && o.DeletedAt == null
                             select o;

            return await queryOrder.Select(x => x.Id).Distinct().CountAsync();
        }


        [Route(DashboardOrderRoute.RejectedOrderCounter), HttpPost]
        public async Task<long> RejectedOrderCounter([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId) &&
                             o.RequestStateId == RequestStateEnum.REJECTED.Id
                             && o.DeletedAt == null
                             select o;

            return await queryOrder.Select(x => x.Id).Distinct().CountAsync();
        }

        [Route(DashboardOrderRoute.RevenueBySource), HttpPost]
        public async Task<DashboardOrder_RevenueBySourceDTO> RevenueBySource([FromBody] DashboardOrder_RevenueBySourceFilterDTO DashboardOrder_RevenueBySourceFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueBySourceFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueBySourceFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueBySourceFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueBySourceFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueBySourceFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from o in DataContext.CustomerSalesOrder
                        where Start <= o.OrderDate && o.OrderDate <= End &&
                        AppUserIds.Contains(o.SalesEmployeeId) &&
                        (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(o.OrganizationId) &&
                        o.RequestStateId == RequestStateEnum.COMPLETED.Id
                        && o.DeletedAt == null
                        select o;

            var CustomerSalesOrders = await query.ToListAsync();
            var OrderSourceIds = CustomerSalesOrders.Select(x => x.OrderSourceId).Distinct().ToList();
            var OrderSources = await DataContext.OrderSource.Where(x => OrderSourceIds.Contains(x.Id)).OrderBy(x => x.Name).ToListAsync();
            var DashboardOrder_RevenueBySourceContentDTOs = OrderSources.Select(x => new DashboardOrder_RevenueBySourceContentDTO
            {
                OrderSourceId = x.Id,
                OrderSourceName = x.Name,
            }).ToList();
            foreach (var OrderSource in DashboardOrder_RevenueBySourceContentDTOs)
            {
                var subOrder = CustomerSalesOrders.Where(x => x.OrderSourceId == OrderSource.OrderSourceId).ToList();
                OrderSource.Revenue = subOrder.Select(x => x.Total).Sum();
            }
            DashboardOrder_RevenueBySourceDTO DashboardOrder_RevenueBySourceDTO = new DashboardOrder_RevenueBySourceDTO
            {
                Total = DashboardOrder_RevenueBySourceContentDTOs.Sum(x => x.Revenue),
                Contents = DashboardOrder_RevenueBySourceContentDTOs
            };
            foreach (var Content in DashboardOrder_RevenueBySourceDTO.Contents)
            {
                Content.Rate = DashboardOrder_RevenueBySourceDTO.Total == 0 ? 0 :
                    Math.Round((decimal)(Content.Revenue / DashboardOrder_RevenueBySourceDTO.Total) * 100);
            }
            return DashboardOrder_RevenueBySourceDTO;
        }

        [Route(DashboardOrderRoute.RevenueByTime), HttpPost]
        public async Task<DashboardOrder_RevenueByTimeDTO> RevenueByTime([FromBody] DashboardOrder_RevenueByTimeFilterDTO DashboardOrder_RevenueByTimeFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueByTimeFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueByTimeFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueByTimeFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueByTimeFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueByTimeFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId)
                             select o;

            var Orders = await queryOrder.ToListAsync();
            var ApprovedOrders = Orders.Where(x => x.RequestStateId == RequestStateEnum.COMPLETED.Id).ToList();
            var RejectedOrders = Orders.Where(x => x.RequestStateId == RequestStateEnum.REJECTED.Id).ToList();

            if (DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.HasValue == false
                || DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                DashboardOrder_RevenueByTimeDTO DashboardOrder_RevenueByTimeDTO = new DashboardOrder_RevenueByTimeDTO();
                DashboardOrder_RevenueByTimeDTO.Month = new List<DashboardOrder_RevenueByTimeByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardOrder_RevenueByTimeByMonthDTO DashboardOrder_RevenueByTimeByMonthDTO = new DashboardOrder_RevenueByTimeByMonthDTO
                    {
                        Day = i,
                        Win = 0,
                        Lost = 0
                    };
                    DashboardOrder_RevenueByTimeDTO.Month.Add(DashboardOrder_RevenueByTimeByMonthDTO);
                }

                foreach (var RevenueByTimeByMonth in DashboardOrder_RevenueByTimeDTO.Month)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, (int)RevenueByTimeByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

                    RevenueByTimeByMonth.Win = ApprovedOrders.Select(x => x.Total).Sum();
                    RevenueByTimeByMonth.Lost = RejectedOrders.Select(x => x.Total).Sum();
                }

                return DashboardOrder_RevenueByTimeDTO;
            }
            else if (DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                DashboardOrder_RevenueByTimeDTO DashboardOrder_RevenueByTimeDTO = new DashboardOrder_RevenueByTimeDTO();
                DashboardOrder_RevenueByTimeDTO.Month = new List<DashboardOrder_RevenueByTimeByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardOrder_RevenueByTimeByMonthDTO DashboardOrder_RevenueByTimeByMonthDTO = new DashboardOrder_RevenueByTimeByMonthDTO
                    {
                        Day = i,
                        Win = 0,
                        Lost = 0
                    };
                    DashboardOrder_RevenueByTimeDTO.Month.Add(DashboardOrder_RevenueByTimeByMonthDTO);
                }

                foreach (var RevenueByTimeByMonth in DashboardOrder_RevenueByTimeDTO.Month)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).AddMonths(-1).Month, (int)RevenueByTimeByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

                    RevenueByTimeByMonth.Win = ApprovedOrders.Select(x => x.Total).Sum();
                    RevenueByTimeByMonth.Lost = RejectedOrders.Select(x => x.Total).Sum();
                }

                return DashboardOrder_RevenueByTimeDTO;
            }
            else if (DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                DashboardOrder_RevenueByTimeDTO DashboardOrder_RevenueByTimeDTO = new DashboardOrder_RevenueByTimeDTO();
                DashboardOrder_RevenueByTimeDTO.Quarter = new List<DashboardOrder_RevenueByTimeByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardOrder_RevenueByTimeByQuarterDTO DashboardOrder_RevenueByTimeByQuarterDTO = new DashboardOrder_RevenueByTimeByQuarterDTO
                    {
                        Month = i,
                        Win = 0,
                        Lost = 0
                    };
                    DashboardOrder_RevenueByTimeDTO.Quarter.Add(DashboardOrder_RevenueByTimeByQuarterDTO);
                }

                foreach (var RevenueByTimeByQuarter in DashboardOrder_RevenueByTimeDTO.Quarter)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueByTimeByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

                    RevenueByTimeByQuarter.Win = ApprovedOrders.Select(x => x.Total).Sum();
                    RevenueByTimeByQuarter.Lost = RejectedOrders.Select(x => x.Total).Sum();
                }

                return DashboardOrder_RevenueByTimeDTO;
            }
            else if (DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                DashboardOrder_RevenueByTimeDTO DashboardOrder_RevenueByTimeDTO = new DashboardOrder_RevenueByTimeDTO();
                DashboardOrder_RevenueByTimeDTO.Quarter = new List<DashboardOrder_RevenueByTimeByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardOrder_RevenueByTimeByQuarterDTO DashboardOrder_RevenueByTimeByQuarterDTO = new DashboardOrder_RevenueByTimeByQuarterDTO
                    {
                        Month = i,
                        Win = 0,
                        Lost = 0
                    };
                    DashboardOrder_RevenueByTimeDTO.Quarter.Add(DashboardOrder_RevenueByTimeByQuarterDTO);
                }

                foreach (var RevenueByTimeByQuarter in DashboardOrder_RevenueByTimeDTO.Quarter)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueByTimeByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

                    RevenueByTimeByQuarter.Win = ApprovedOrders.Select(x => x.Total).Sum();
                    RevenueByTimeByQuarter.Lost = RejectedOrders.Select(x => x.Total).Sum();
                }

                return DashboardOrder_RevenueByTimeDTO;
            }
            else if (DashboardOrder_RevenueByTimeFilterDTO.Time.Equal.Value == YEAR)
            {
                DashboardOrder_RevenueByTimeDTO DashboardOrder_RevenueByTimeDTO = new DashboardOrder_RevenueByTimeDTO();
                DashboardOrder_RevenueByTimeDTO.Year = new List<DashboardOrder_RevenueByTimeByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardOrder_RevenueByTimeByYearDTO DashboardOrder_RevenueByTimeByYearDTO = new DashboardOrder_RevenueByTimeByYearDTO
                    {
                        Month = i,
                        Win = 0,
                        Lost = 0
                    };
                    DashboardOrder_RevenueByTimeDTO.Year.Add(DashboardOrder_RevenueByTimeByYearDTO);
                }

                foreach (var RevenueByTimeByYear in DashboardOrder_RevenueByTimeDTO.Year)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueByTimeByYear.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

                    RevenueByTimeByYear.Win = ApprovedOrders.Select(x => x.Total).Sum();
                    RevenueByTimeByYear.Lost = RejectedOrders.Select(x => x.Total).Sum();
                }

                return DashboardOrder_RevenueByTimeDTO;
            }
            return new DashboardOrder_RevenueByTimeDTO();
        }

        [Route(DashboardOrderRoute.RevenueByStatus), HttpPost]
        public async Task<DashboardOrder_RevenueByStatusDTO> RevenueByStatus([FromBody] DashboardOrder_RevenueByStatusFilterDTO DashboardOrder_RevenueByStatusFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardOrder_RevenueByStatusFilterDTO.Time);

            var ProvinceId = DashboardOrder_RevenueByStatusFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardOrder_RevenueByStatusFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardOrder_RevenueByStatusFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardOrder_RevenueByStatusFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var queryOrder = from o in DataContext.CustomerSalesOrder
                             where Start <= o.OrderDate && o.OrderDate <= End &&
                             AppUserIds.Contains(o.SalesEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || o.SalesEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(o.OrganizationId)
                             select o;

            var Orders = await queryOrder.ToListAsync();
            var DashboardOrder_RevenueByStatusContentDTO = RequestStateEnum.RequestStateEnumList.Select(x => new DashboardOrder_RevenueByStatusContentDTO
            {
                RequestStateId = x.Id,
                RequestStateName = x.Name,
            }).ToList();
            foreach (var RequestState in DashboardOrder_RevenueByStatusContentDTO)
            {
                var subOrder = Orders.Where(x => x.RequestStateId == RequestState.RequestStateId).ToList();
                RequestState.Revenue = subOrder.Select(x => x.Total).Sum();
            }
            DashboardOrder_RevenueByStatusDTO DashboardOrder_RevenueByStatusDTO = new DashboardOrder_RevenueByStatusDTO
            {
                Total = DashboardOrder_RevenueByStatusContentDTO.Sum(x => x.Revenue),
                Contents = DashboardOrder_RevenueByStatusContentDTO
            };
            foreach (var Content in DashboardOrder_RevenueByStatusDTO.Contents)
            {
                Content.Rate = DashboardOrder_RevenueByStatusDTO.Total == 0 ? 0 :
                    Math.Round((decimal)(Content.Revenue / DashboardOrder_RevenueByStatusDTO.Total) * 100);
            }
            return DashboardOrder_RevenueByStatusDTO;
        }


        private Tuple<DateTime, DateTime> ConvertTime(IdFilter Time)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            DateTime Now = StaticParams.DateTimeNow;
            if (Time.Equal.HasValue == false)
            {
                Time.Equal = 0;
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == TODAY)
            {
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.AddHours(CurrentContext.TimeZone).DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = LocalStartDay(CurrentContext).AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(-3).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == YEAR)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddYears(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            return Tuple.Create(Start, End);
        }
    }
}
