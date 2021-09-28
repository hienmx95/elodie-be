using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Handlers;
using ELODIE.Helpers;
using ELODIE.Models;
using ELODIE.Repositories;
using ELODIE.Services.MProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace ELODIE.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        private IRabbitManager RabbitManager;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        private readonly IHttpContextAccessor HttpContextAccessor;

        private IUOW UOW;

        public SetupController(DataContext DataContext, IRabbitManager RabbitManager, IItemService ItemService, ICurrentContext CurrentContext, IHttpContextAccessor HttpContextAccessor, IUOW UOW)
        {
            this.DataContext = DataContext;
            this.RabbitManager = RabbitManager;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
            this.HttpContextAccessor = HttpContextAccessor;
            this.UOW = UOW;
        }

        [HttpGet, Route("rpc/ELODIE/setup/category-children-reset")]
        public async Task<ActionResult> CategoryChildrenReset()
        {
            List<Category> AllCategories = await UOW.CategoryRepository.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.ALL
            });

            foreach (Category x in AllCategories)
                await UOW.CategoryRepository.Update(x);

            return Ok();
        }

        [HttpGet, Route("rpc/ELODIE/setup/supplier-reset")]
        public async Task<ActionResult> SupplierReset()
        {
            List<long> SupplierIds = (await UOW.SupplierRepository.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL
            })).Select(x => x.Id).ToList();
            List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierIds);

            foreach (Supplier Supplier in Suppliers)
                await UOW.SupplierRepository.Update(Supplier);

            return Ok();
        }

        #region Publish Data
        [HttpGet, Route("rpc/ELODIE/setup/publish-data")]
        public async Task<ActionResult> PublishData()
        {
            await PublishBrand();
            await PublishCategory();
            await PublishNation();
            await PublishProductGrouping();
            await PublishProductType();
            await PublishProvince();
            await PublishDistrict();
            await PublishSupplier();
            await PublishTaxType();
            await PublishUnitOfMeasure();
            await PublishUnitOfMeasureGrouping();
            await PublishWard();
            await PublishProduct();
            return Ok();
        }

        #region Brand
        public async Task<ActionResult> PublishBrand()
        {
            List<long> BrandIds = await DataContext.Brand.Select(x => x.Id).ToListAsync();
            List<Brand> Brands = await UOW.BrandRepository.List(BrandIds);
            RabbitManager.PublishList(Brands, RoutingKeyEnum.BrandSync);
            return Ok();
        }
        #endregion

        #region Category
        public async Task<ActionResult> PublishCategory()
        {
            List<long> CategoryIds = await DataContext.Category.Select(x => x.Id).ToListAsync();
            List<Category> Categorys = await UOW.CategoryRepository.List(CategoryIds);
            RabbitManager.PublishList(Categorys, RoutingKeyEnum.CategorySync);
            return Ok();
        }
        #endregion

        #region District
        public async Task<ActionResult> PublishDistrict()
        {
            List<long> DistrictIds = DataContext.District.Select(x => x.Id).ToList();
            List<District> Districts = await UOW.DistrictRepository.List(DistrictIds);
            RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictSync);
            return Ok();
        }
        #endregion

        #region Nation
        public async Task<ActionResult> PublishNation()
        {
            List<long> NationIds = DataContext.Nation.Select(x => x.Id).ToList();
            List<Nation> Nations = await UOW.NationRepository.List(NationIds);
            RabbitManager.PublishList(Nations, RoutingKeyEnum.NationSync);
            return Ok();
        }
        #endregion

        #region Product
        public async Task<ActionResult> PublishProduct()
        {
            List<long> ProductIds = DataContext.Product.Select(x => x.Id).ToList();
            List<Product> Products = await UOW.ProductRepository.List(ProductIds);
            RabbitManager.PublishList(Products, RoutingKeyEnum.ProductSync);
            return Ok();
        }
        #endregion

        #region ProductGrouping
        public async Task<ActionResult> PublishProductGrouping()
        {
            List<long> ProductGroupingIds = DataContext.ProductGrouping.Select(x => x.Id).ToList();
            List<ProductGrouping> ProductGroupings = await UOW.ProductGroupingRepository.List(ProductGroupingIds);
            RabbitManager.PublishList(ProductGroupings, RoutingKeyEnum.ProductGroupingSync);
            return Ok();
        }
        #endregion

        #region ProductType
        public async Task<ActionResult> PublishProductType()
        {
            List<long> ProductTypeIds = DataContext.ProductType.Select(x => x.Id).ToList();
            List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(ProductTypeIds);
            RabbitManager.PublishList(ProductTypes, RoutingKeyEnum.ProductTypeSync);
            return Ok();
        }
        #endregion

        #region Supplier
        public async Task<ActionResult> PublishSupplier()
        {
            List<long> SupplierIds = DataContext.Supplier.Select(x => x.Id).ToList();
            List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierIds);
            RabbitManager.PublishList(Suppliers, RoutingKeyEnum.SupplierSync);
            return Ok();
        }
        #endregion

        #region Province
        public async Task<ActionResult> PublishProvince()
        {
            List<long> ProvinceIds = DataContext.Province.Select(x => x.Id).ToList();
            List<Province> Provinces = await UOW.ProvinceRepository.List(ProvinceIds);
            RabbitManager.PublishList(Provinces, RoutingKeyEnum.ProvinceSync);
            return Ok();
        }
        #endregion

        #region TaxType
        public async Task<ActionResult> PublishTaxType()
        {
            List<long> TaxTypeIds = DataContext.TaxType.Select(x => x.Id).ToList();
            List<TaxType> TaxTypes = await UOW.TaxTypeRepository.List(TaxTypeIds);
            RabbitManager.PublishList(TaxTypes, RoutingKeyEnum.TaxTypeSync);
            return Ok();
        }
        #endregion

        #region UnitOfMeasure
        public async Task<ActionResult> PublishUnitOfMeasure()
        {
            List<long> UnitOfMeasureIds = DataContext.UnitOfMeasure.Select(x => x.Id).ToList();
            List<UnitOfMeasure> UnitOfMeasures = await UOW.UnitOfMeasureRepository.List(UnitOfMeasureIds);
            RabbitManager.PublishList(UnitOfMeasures, RoutingKeyEnum.UnitOfMeasureSync);
            return Ok();
        }
        #endregion

        #region UnitOfMeasureGrouping
        public async Task<ActionResult> PublishUnitOfMeasureGrouping()
        {
            List<long> UnitOfMeasureGroupingIds = DataContext.UnitOfMeasureGrouping.Select(x => x.Id).ToList();
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(UnitOfMeasureGroupingIds);
            RabbitManager.PublishList(UnitOfMeasureGroupings, RoutingKeyEnum.UnitOfMeasureGroupingSync);
            return Ok();
        }
        #endregion

        #region Ward
        public async Task<ActionResult> PublishWard()
        {
            List<long> WardIds = DataContext.Ward.Select(x => x.Id).ToList();
            List<Ward> Wards = await UOW.WardRepository.List(WardIds);
            RabbitManager.PublishList(Wards, RoutingKeyEnum.WardSync);
            return Ok();
        }
        #endregion
        #endregion

        [HttpGet, Route("rpc/ELODIE/setup/init-all")]
        public ActionResult InitAll()
        {
            InitEnum();
            InitRoute();
            InitAdmin();

            RestClient portalClient = new RestClient(InternalServices.PORTAL);
            RestRequest initPortalRequest = new RestRequest("/rpc/portal/setup/init");
            initPortalRequest.Method = Method.GET;

            RestClient dmsClient = new RestClient(InternalServices.DMS);
            RestRequest initDMSRequest = new RestRequest("/rpc/dms/setup/init");
            initDMSRequest.Method = Method.GET;

            RestClient amsClient = new RestClient(InternalServices.AMS);
            RestRequest initAMSRequest = new RestRequest("/rpc/ams/setup/init");
            initAMSRequest.Method = Method.GET;

            RestClient cmsClient = new RestClient(InternalServices.CMS_EXPORT);
            RestRequest initCMSRequest = new RestRequest("/rpc/cms-export/setup/init");
            initCMSRequest.Method = Method.GET;

            RestClient crmClient = new RestClient(InternalServices.CRM);
            RestRequest initCRMRequest = new RestRequest("/rpc/crm/setup/init");
            initCRMRequest.Method = Method.GET;
            try
            {
                var r1 = portalClient.Execute(initPortalRequest);
                if (r1.StatusCode != System.Net.HttpStatusCode.OK && r1.StatusCode != System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Lỗi Init Portal");
                var r2 = dmsClient.Execute(initDMSRequest);
                if (r2.StatusCode != System.Net.HttpStatusCode.OK && r2.StatusCode != System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Lỗi Init DMS");
                var r3 = amsClient.Execute(initAMSRequest);
                if (r3.StatusCode != System.Net.HttpStatusCode.OK && r3.StatusCode != System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Lỗi Init AMS");
                var r4 = cmsClient.Execute(initCMSRequest);
                if (r4.StatusCode != System.Net.HttpStatusCode.OK && r4.StatusCode != System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Lỗi Init CMS Export");
                var r5 = crmClient.Execute(initCRMRequest);
                if (r5.StatusCode != System.Net.HttpStatusCode.OK && r5.StatusCode != System.Net.HttpStatusCode.NotFound)
                    return BadRequest("Lỗi Init CRM");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
        }

        [HttpGet, Route("rpc/ELODIE/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            InitRoute();
            InitAdmin();
            return Ok();
        }

        [HttpGet, Route("rpc/ELODIE/setup/init-data")]
        public ActionResult InitData()
        {
            RestClient esClient = new RestClient(InternalServices.ES);
            InitOrganization(esClient);
            InitSite(esClient);
            InitAppUser(esClient);

            RestClient dmsClient = new RestClient(InternalServices.DMS);
            dmsClient.Timeout = 1000000;
            RestRequest initDMSRequest = new RestRequest("/rpc/dms/setup/init-data");
            initDMSRequest.Method = Method.GET;

            RestClient amsClient = new RestClient(InternalServices.AMS);
            amsClient.Timeout = 1000000;
            RestRequest initAMSRequest = new RestRequest("/rpc/ams/setup/init-data");
            initAMSRequest.Method = Method.GET;

            RestClient cmsClient = new RestClient(InternalServices.CMS_EXPORT);
            cmsClient.Timeout = 1000000;
            RestRequest initCMSRequest = new RestRequest("/rpc/cms-export/setup/init-data");
            initCMSRequest.Method = Method.GET;

            RestClient crmClient = new RestClient(InternalServices.CRM);
            crmClient.Timeout = 1000000;
            RestRequest initCRMRequest = new RestRequest("/rpc/crm/setup/init-data");
            initCRMRequest.Method = Method.GET;
            try
            {
                var r2 = dmsClient.Execute(initDMSRequest);
                if (r2.StatusCode != System.Net.HttpStatusCode.OK && r2.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    if (r2.ErrorException != null)
                        throw r2.ErrorException;
                    else
                        return BadRequest($"DMS: {r2.StatusCode}");
                }
                var r3 = amsClient.Execute(initAMSRequest);
                if (r3.StatusCode != System.Net.HttpStatusCode.OK && r3.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    if (r3.ErrorException != null)
                        throw r3.ErrorException;
                    else
                        return BadRequest($"AMS: {r3.StatusCode}");
                }
                var r4 = cmsClient.Execute(initCMSRequest);
                if (r4.StatusCode != System.Net.HttpStatusCode.OK && r4.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    if (r4.ErrorException != null)
                        throw r4.ErrorException;
                    else
                        return BadRequest($"CMS Export: {r4.StatusCode}");
                }
                var r5 = crmClient.Execute(initCRMRequest);
                if (r5.StatusCode != System.Net.HttpStatusCode.OK && r5.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    if (r5.ErrorException != null)
                        throw r5.ErrorException;
                    else
                        return BadRequest($"Portal: {r5.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
        }

        private void InitOrganization(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/organization/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Organization>> RestResponse = RestClient.Post<List<Organization>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<OrganizationDAO> OrganizationInDB = DataContext.Organization.AsNoTracking().ToList();
                List<Organization> Organizations = RestResponse.Data;
                foreach (Organization Organization in Organizations)
                {
                    OrganizationDAO OrganizationDAO = OrganizationInDB.Where(x => x.Id == Organization.Id).FirstOrDefault();
                    if (OrganizationDAO == null)
                    {
                        OrganizationDAO = new OrganizationDAO
                        {
                            Id = Organization.Id,
                        };
                        OrganizationInDB.Add(OrganizationDAO);
                    }
                    OrganizationDAO.Code = Organization.Code;
                    OrganizationDAO.Name = Organization.Name;
                    OrganizationDAO.Path = Organization.Path;
                    OrganizationDAO.Level = Organization.Level;
                    OrganizationDAO.Address = Organization.Address;
                    OrganizationDAO.Email = Organization.Email;
                    OrganizationDAO.ParentId = Organization.ParentId;
                    OrganizationDAO.StatusId = Organization.StatusId;
                    OrganizationDAO.CreatedAt = Organization.CreatedAt;
                    OrganizationDAO.UpdatedAt = Organization.UpdatedAt;
                    OrganizationDAO.DeletedAt = Organization.DeletedAt;
                    OrganizationDAO.RowId = Organization.RowId;

                }
                DataContext.Organization.BulkMerge(OrganizationInDB);
            }

        }

        private void InitSite(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/site/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Site>> RestResponse = RestClient.Post<List<Site>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<SiteDAO> SiteInDB = DataContext.Site.AsNoTracking().ToList();
                List<Site> Sites = RestResponse.Data;
                foreach (Site Site in Sites)
                {
                    SiteDAO SiteDAO = SiteInDB.Where(x => x.Id == Site.Id).FirstOrDefault();
                    if (SiteDAO == null)
                    {
                        SiteDAO = new SiteDAO
                        {
                            Id = Site.Id,
                        };
                        SiteInDB.Add(SiteDAO);
                    }
                    SiteDAO.Code = Site.Code;
                    SiteDAO.Name = Site.Name;
                    SiteDAO.Description = Site.Description;
                    SiteDAO.Icon = Site.Icon;
                    SiteDAO.Logo = Site.Logo;
                    SiteDAO.IsDisplay = Site.IsDisplay;
                    SiteDAO.ThemeId = Site.ThemeId;
                    SiteDAO.RowId = Site.RowId;
                }
                DataContext.Site.BulkMerge(SiteInDB);
            }
        }

        private void InitAppUser(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/app-user/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<AppUser>> RestResponse = RestClient.Post<List<AppUser>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<AppUserDAO> AppUserInDB = DataContext.AppUser.AsNoTracking().ToList();
                List<AppUserSiteMappingDAO> AppUserSiteMappingInDB = DataContext.AppUserSiteMapping.AsNoTracking().ToList();
                List<AppUser> AppUsers = RestResponse.Data;
                foreach (AppUser AppUser in AppUsers)
                {
                    AppUserDAO AppUserDAO = AppUserInDB.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                    if (AppUserDAO == null)
                    {
                        AppUserDAO = new AppUserDAO
                        {
                            Id = AppUser.Id,

                        };
                        AppUserInDB.Add(AppUserDAO);
                    }
                    AppUserDAO.Username = AppUser.Username;
                    AppUserDAO.DisplayName = AppUser.DisplayName;
                    AppUserDAO.Address = AppUser.Address;
                    AppUserDAO.Email = AppUser.Email;
                    AppUserDAO.Phone = AppUser.Phone;
                    AppUserDAO.Department = AppUser.Department;
                    AppUserDAO.OrganizationId = AppUser.OrganizationId;
                    AppUserDAO.SexId = AppUser.SexId;
                    AppUserDAO.StatusId = AppUser.StatusId;
                    AppUserDAO.CreatedAt = AppUser.CreatedAt;
                    AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                    AppUserDAO.DeletedAt = AppUser.DeletedAt;
                    AppUserDAO.Avatar = AppUser.Avatar;
                    AppUserDAO.Birthday = AppUser.Birthday;
                    AppUserDAO.RowId = AppUser.RowId;

                    if (AppUser.AppUserSiteMappings != null)
                    {
                        foreach (AppUserSiteMapping AppUserSiteMapping in AppUser.AppUserSiteMappings)
                        {
                            AppUserSiteMappingDAO AppUserSiteMappingDAO = AppUserSiteMappingInDB.Where(x => x.AppUserId == AppUser.Id && x.SiteId == AppUserSiteMapping.SiteId).FirstOrDefault();
                            if (AppUserSiteMappingDAO == null)
                            {
                                AppUserSiteMappingDAO = new AppUserSiteMappingDAO
                                {
                                    AppUserId = AppUser.Id,
                                    SiteId = AppUserSiteMapping.SiteId
                                };
                                AppUserSiteMappingInDB.Add(AppUserSiteMappingDAO);
                            }
                        }

                    }
                }
                DataContext.AppUser.BulkMerge(AppUserInDB);
                DataContext.AppUserSiteMapping.BulkMerge(AppUserSiteMappingInDB);
            }
        }

        private ActionResult InitEnum()
        {
            InitStatusEnum();
            InitSexEnum();
            InitColorEnum();
            InitUsedVariationEnum();
            InitEntityComponentEnum();
            InitEntityTypeEnum();
            InitPermissionEnum();
            InitWorkflowEnum();
            DataContext.SaveChanges();
            return Ok();
        }

        private ActionResult InitRoute()
        {
            List<Type> routeTypes = typeof(SetupController).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass && x.Name != "Root")
               .ToList();

            InitMenu(routeTypes);
            InitPage(routeTypes);
            InitField(routeTypes);
            InitAction(routeTypes);

            DataContext.ActionPageMapping.Where(ap => ap.Action.IsDeleted || ap.Page.IsDeleted).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(ap => ap.Action.IsDeleted).DeleteFromQuery();
            DataContext.Action.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted).DeleteFromQuery();
            DataContext.PermissionContent.Where(f => f.Field.IsDeleted == true || f.Field.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
            return Ok();
        }

        private ActionResult InitAdmin()
        {
            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "ADMIN")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "ADMIN",
                    Code = "ADMIN",
                    StatusId = StatusEnum.ACTIVE.Id,
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {
                return Ok();
            }

            AppUserRoleMappingDAO AppUserRoleMappingDAO = DataContext.AppUserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.AppUserId == AppUser.Id)
                .FirstOrDefault();
            if (AppUserRoleMappingDAO == null)
            {
                AppUserRoleMappingDAO = new AppUserRoleMappingDAO
                {
                    AppUserId = AppUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.AppUserRoleMapping.Add(AppUserRoleMappingDAO);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking()
                .Include(v => v.Actions)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking()
                .Include(p => p.PermissionActionMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = Admin + "_" + Menu.Name,
                        Name = Admin + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = Admin.Id,
                        StatusId = StatusEnum.ACTIVE.Id,
                        PermissionActionMappings = new List<PermissionActionMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    permission.StatusId = StatusEnum.ACTIVE.Id;
                    if (permission.PermissionActionMappings == null)
                        permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
                }
                foreach (ActionDAO action in Menu.Actions)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
                        .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
                    if (PermissionActionMappingDAO == null)
                    {
                        PermissionActionMappingDAO = new PermissionActionMappingDAO
                        {
                            ActionId = action.Id
                        };
                        permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
                    }
                }

            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var action in p.PermissionActionMappings)
                {
                    action.PermissionId = p.Id;
                }
            });

            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);
            return Ok();
        }

        private void InitPermissionEnum()
        {
            List<FieldTypeDAO> FieldTypeDAOs = FieldTypeEnum.List.Select(item => new FieldTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.FieldType.BulkSynchronize(FieldTypeDAOs);
            List<PermissionOperatorDAO> PermissionOperatorDAOs = new List<PermissionOperatorDAO>();
            List<PermissionOperatorDAO> ID = PermissionOperatorEnum.PermissionOperatorEnumForID.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.ID.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(ID);
            List<PermissionOperatorDAO> STRING = PermissionOperatorEnum.PermissionOperatorEnumForSTRING.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(STRING);

            List<PermissionOperatorDAO> LONG = PermissionOperatorEnum.PermissionOperatorEnumForLONG.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(LONG);

            List<PermissionOperatorDAO> DECIMAL = PermissionOperatorEnum.PermissionOperatorEnumForDECIMAL.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DECIMAL);

            List<PermissionOperatorDAO> DATE = PermissionOperatorEnum.PermissionOperatorEnumForDATE.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DATE);

            DataContext.PermissionOperator.BulkSynchronize(PermissionOperatorDAOs);
        }
        private void InitMenu(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            Menus.ForEach(m => m.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name && m.Name != "Root").FirstOrDefault();
                if (Menu == null)
                {
                    Menu = new MenuDAO
                    {
                        Code = type.Name,
                        Name = type.Name,
                        IsDeleted = false,
                    };
                    Menus.Add(Menu);
                }
                else
                {
                    Menu.IsDeleted = false;
                }
            }
            DataContext.BulkMerge(Menus);
        }

        private void InitPage(List<Type> routeTypes)
        {
            List<PageDAO> pages = DataContext.Page.AsNoTracking().OrderBy(p => p.Path).ToList();
            pages.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                var values = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
                foreach (string value in values)
                {
                    PageDAO page = pages.Where(p => p.Path == value).FirstOrDefault();
                    if (page == null)
                    {
                        page = new PageDAO
                        {
                            Name = value,
                            Path = value,
                            IsDeleted = false,
                        };
                        pages.Add(page);
                    }
                    else
                    {
                        page.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(pages);
        }
        private void InitField(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<FieldDAO> fields = DataContext.Field.AsNoTracking().ToList();
            fields.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, long>))
                .Select(x => (Dictionary<string, long>)x.GetValue(x))
                .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    FieldDAO field = fields
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (field == null)
                    {
                        field = new FieldDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            FieldTypeId = pair.Value,
                            IsDeleted = false,
                        };
                        fields.Add(field);
                    }
                    else
                    {
                        field.FieldTypeId = pair.Value;
                        field.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(fields);
        }
        private void InitAction(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<ActionDAO> actions = DataContext.Action.AsNoTracking().ToList();
            actions.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                    {
                        action = new ActionDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            IsDeleted = false,
                        };
                        actions.Add(action);
                    }
                    else
                    {
                        action.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(actions);

            actions = DataContext.Action.Where(a => a.IsDeleted == false).AsNoTracking().ToList();
            List<PageDAO> PageDAOs = DataContext.Page.AsNoTracking().ToList();
            List<ActionPageMappingDAO> ActionPageMappingDAOs = new List<ActionPageMappingDAO>();
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;

                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                        continue;
                    IEnumerable<string> pages = pair.Value;
                    foreach (string page in pages)
                    {
                        PageDAO PageDAO = PageDAOs.Where(p => p.Path == page).FirstOrDefault();
                        if (PageDAO != null)
                        {
                            if (!ActionPageMappingDAOs.Any(ap => ap.ActionId == action.Id && ap.PageId == PageDAO.Id))
                            {
                                ActionPageMappingDAOs.Add(new ActionPageMappingDAO
                                {
                                    ActionId = action.Id,
                                    PageId = PageDAO.Id
                                });
                            }
                        }
                    }
                }
            }
            ActionPageMappingDAOs = ActionPageMappingDAOs.Distinct().ToList();
            DataContext.ActionPageMapping.DeleteFromQuery();
            DataContext.BulkInsert(ActionPageMappingDAOs);
        }

        private void InitStatusEnum()
        {
            List<StatusDAO> StatusDAOs = DataContext.Status.ToList();
            if (!StatusDAOs.Any(pt => pt.Id == StatusEnum.ACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.ACTIVE.Id,
                    Code = StatusEnum.ACTIVE.Code,
                    Name = StatusEnum.ACTIVE.Name,
                });
            }

            if (!StatusDAOs.Any(pt => pt.Id == StatusEnum.INACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.INACTIVE.Id,
                    Code = StatusEnum.INACTIVE.Code,
                    Name = StatusEnum.INACTIVE.Name,
                });
            }

            List<Status> Statuses = DataContext.Status.Select(x => new Status
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(Statuses, RoutingKeyEnum.StatusSync);
        }

        private void InitSexEnum()
        {
            List<SexDAO> SexDAOs = SexEnum.SexEnumList.Select(x => new SexDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.Sex.BulkSynchronize(SexDAOs);
            List<Sex> Sexes = SexDAOs.Select(x => new Sex
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(Sexes, RoutingKeyEnum.SexSync);
        }

        private void InitColorEnum()
        {
            List<ColorDAO> ColorDAOs = ColorEnum.ColorEnumList.Select(x => new ColorDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.Color.BulkSynchronize(ColorDAOs);
            List<Color> Colores = ColorDAOs.Select(x => new Color
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(Colores, RoutingKeyEnum.ColorSync);
        }

        private void InitUsedVariationEnum()
        {
            List<UsedVariationDAO> UsedVariationDAOs = UsedVariationEnum.UsedVariationEnumList.Select(x => new UsedVariationDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.UsedVariation.BulkSynchronize(UsedVariationDAOs);
            List<UsedVariation> UsedVariationes = UsedVariationDAOs.Select(x => new UsedVariation
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(UsedVariationes, RoutingKeyEnum.UsedVariationSync);
        }

        private void InitWorkflowEnum()
        {
            List<WorkflowTypeDAO> WorkflowTypeEnumList = WorkflowTypeEnum.WorkflowTypeEnumList.Select(item => new WorkflowTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowType.BulkSynchronize(WorkflowTypeEnumList);
            List<WorkflowStateDAO> WorkflowStateEnumList = WorkflowStateEnum.WorkflowStateEnumList.Select(item => new WorkflowStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowState.BulkSynchronize(WorkflowStateEnumList);
            List<RequestStateDAO> RequestStateEnumList = RequestStateEnum.RequestStateEnumList.Select(item => new RequestStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.RequestState.BulkSynchronize(RequestStateEnumList);
            List<WorkflowParameterTypeDAO> WorkflowParameterTypeDAOs = WorkflowParameterTypeEnum.List.Select(item => new WorkflowParameterTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowParameterType.BulkSynchronize(WorkflowParameterTypeDAOs);

            List<WorkflowOperatorDAO> WorkflowOperatorDAOs = new List<WorkflowOperatorDAO>();
            List<WorkflowOperatorDAO> ID = WorkflowOperatorEnum.WorkflowOperatorEnumForID.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = WorkflowParameterTypeEnum.ID.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(ID);

            List<WorkflowOperatorDAO> STRING = WorkflowOperatorEnum.WorkflowOperatorEnumForSTRING.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(STRING);

            List<WorkflowOperatorDAO> LONG = WorkflowOperatorEnum.WorkflowOperatorEnumForLONG.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(LONG);

            List<WorkflowOperatorDAO> DECIMAL = WorkflowOperatorEnum.WorkflowOperatorEnumForDECIMAL.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DECIMAL);

            List<WorkflowOperatorDAO> DATE = WorkflowOperatorEnum.WorkflowOperatorEnumForDATE.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DATE);

            DataContext.WorkflowOperator.BulkSynchronize(WorkflowOperatorDAOs);

            List<WorkflowParameterDAO> WorkflowParameterDAOs = new List<WorkflowParameterDAO>();

            DataContext.WorkflowParameter.BulkMerge(WorkflowParameterDAOs);
        }

        private void InitEntityComponentEnum()
        {
            List<EntityComponentDAO> EntityComponentDAOs = EntityComponentEnum.EntityComponentEnumList.Select(item => new EntityComponentDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EntityComponent.BulkSynchronize(EntityComponentDAOs);
            List<EntityComponent> EntityComponentes = EntityComponentDAOs.Select(x => new EntityComponent
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(EntityComponentes, RoutingKeyEnum.EntityComponentSync);
        }

        private void InitEntityTypeEnum()
        {
            List<EntityTypeDAO> EntityTypeDAOs = EntityTypeEnum.EntityTypeEnumList.Select(item => new EntityTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EntityType.BulkSynchronize(EntityTypeDAOs);
            List<EntityType> EntityTypees = EntityTypeDAOs.Select(x => new EntityType
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            RabbitManager.PublishList(EntityTypees, RoutingKeyEnum.EntityTypeSync);
        }

        #region tool up ảnh
        //[HttpPost, Route("rpc/ELODIE/setup/save-image")]
        //public async Task SaveImage()
        //{

        //    var HttpContext = HttpContextAccessor.HttpContext;
        //    CurrentContext.Token = HttpContext.Request.Cookies["Token"]; // add Token

        //    string FolderPath = @"D:\Image";
        //    string[] Subdirectories = Directory.GetDirectories(FolderPath);
        //    List<string> ItemCodes = new List<string>();
        //    List<FileItemInfo> FileItemInfos = new List<FileItemInfo>();
        //    foreach (string SubDir in Subdirectories)
        //    {
        //        string ItemCode = Path.GetFileName(SubDir).ToString();
        //        ItemCodes.Add(ItemCode);
        //        foreach(string file in Directory.EnumerateFiles(SubDir))
        //        {
        //            FileInfo FileInfo = new FileInfo(file);
        //            FileItemInfos.Add(new FileItemInfo { ItemCode = ItemCode, FileInfo = FileInfo });
        //        }
        //    } // loop qua tat cac sub directory, ten cua subdir la code cua item

        //    var Items = await DataContext.Item.ToListAsync(); // lay ra item co ten giong ten subFolder
        //    List<ItemImageMappingDAO> ItemImageMappingDAOs = new List<ItemImageMappingDAO>();

        //    foreach (FileItemInfo FileItemInfo in FileItemInfos)
        //    {
        //        var contents = await System.IO.File.ReadAllBytesAsync(FileItemInfo.FileInfo.FullName);
        //        Image Image = new Image()
        //        {
        //            Name = FileItemInfo.FileInfo.Name,
        //            Content = contents
        //        };
        //        var Item = Items.Where(x => x.Code.Trim().ToLower() == FileItemInfo.ItemCode.Trim().ToLower()).FirstOrDefault();
        //        if (Item != null)
        //        {
        //            Image = await ItemService.SaveImage(Image);

        //            ItemImageMappingDAO ItemImageMappingDAO = new ItemImageMappingDAO()
        //            {
        //                ItemId = Item.Id,
        //                ImageId = Image.Id
        //            };
        //            ItemImageMappingDAOs.Add(ItemImageMappingDAO);

        //        }
        //    }
        //    await DataContext.ItemImageMapping.BulkMergeAsync(ItemImageMappingDAOs);
        //}

        //public class FileItemInfo
        //{
        //    public string ItemCode { get; set; }
        //    public FileInfo FileInfo { get; set; }
        //}
        #endregion
    }
}