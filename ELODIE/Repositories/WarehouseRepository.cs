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
    public interface IWarehouseRepository
    {
        Task<int> CountAll(WarehouseFilter WarehouseFilter);
        Task<int> Count(WarehouseFilter WarehouseFilter);
        Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter);
        Task<List<Warehouse>> List(List<long> Ids);
        Task<Warehouse> Get(long Id);
        Task<bool> Create(Warehouse Warehouse);
        Task<bool> Update(Warehouse Warehouse);
        Task<bool> Delete(Warehouse Warehouse);
        Task<bool> BulkMerge(List<Warehouse> Warehouses);
        Task<bool> BulkDelete(List<Warehouse> Warehouses);
    }
    public class WarehouseRepository : IWarehouseRepository
    {
        private DataContext DataContext;
        public WarehouseRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WarehouseDAO> DynamicFilter(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Address, filter.Address);
            query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            query = query.Where(q => q.DistrictId, filter.DistrictId);
            query = query.Where(q => q.WardId, filter.WardId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.RowId, filter.RowId);
            
            return query;
        }

        private IQueryable<WarehouseDAO> OrFilter(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WarehouseDAO> initQuery = query.Where(q => false);
            foreach (WarehouseFilter WarehouseFilter in filter.OrFilter)
            {
                IQueryable<WarehouseDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Code, filter.Code);
                queryable = queryable.Where(q => q.Name, filter.Name);
                queryable = queryable.Where(q => q.Address, filter.Address);
                queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                queryable = queryable.Where(q => q.WardId, filter.WardId);
                queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<WarehouseDAO> DynamicOrder(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WarehouseOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WarehouseOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WarehouseOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WarehouseOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case WarehouseOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case WarehouseOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case WarehouseOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case WarehouseOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case WarehouseOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case WarehouseOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WarehouseOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WarehouseOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WarehouseOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WarehouseOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case WarehouseOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case WarehouseOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case WarehouseOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case WarehouseOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case WarehouseOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case WarehouseOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Warehouse>> DynamicSelect(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            List<Warehouse> Warehouses = await query.Select(q => new Warehouse()
            {
                Id = filter.Selects.Contains(WarehouseSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(WarehouseSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WarehouseSelect.Name) ? q.Name : default(string),
                Address = filter.Selects.Contains(WarehouseSelect.Address) ? q.Address : default(string),
                OrganizationId = filter.Selects.Contains(WarehouseSelect.Organization) ? q.OrganizationId : default(long),
                ProvinceId = filter.Selects.Contains(WarehouseSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(WarehouseSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(WarehouseSelect.Ward) ? q.WardId : default(long?),
                StatusId = filter.Selects.Contains(WarehouseSelect.Status) ? q.StatusId : default(long),
                RowId = filter.Selects.Contains(WarehouseSelect.Row) ? q.RowId : default(Guid),
                District = filter.Selects.Contains(WarehouseSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                    RowId = q.District.RowId,
                    Used = q.District.Used,
                } : null,
                Organization = filter.Selects.Contains(WarehouseSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Email = q.Organization.Email,
                    Address = q.Organization.Address,
                    RowId = q.Organization.RowId,
                    Used = q.Organization.Used,
                    IsDisplay = q.Organization.IsDisplay,
                } : null,
                Province = filter.Selects.Contains(WarehouseSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                    RowId = q.Province.RowId,
                    Used = q.Province.Used,
                } : null,
                Status = filter.Selects.Contains(WarehouseSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Ward = filter.Selects.Contains(WarehouseSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                    RowId = q.Ward.RowId,
                    Used = q.Ward.Used,
                } : null,
            }).ToListAsync();
            return Warehouses;
        }

        public async Task<int> CountAll(WarehouseFilter filter)
        {
            IQueryable<WarehouseDAO> Warehouses = DataContext.Warehouse.AsNoTracking();
            Warehouses = DynamicFilter(Warehouses, filter);
            return await Warehouses.CountAsync();
        }

        public async Task<int> Count(WarehouseFilter filter)
        {
            IQueryable<WarehouseDAO> Warehouses = DataContext.Warehouse.AsNoTracking();
            Warehouses = DynamicFilter(Warehouses, filter);
            Warehouses = OrFilter(Warehouses, filter);
            return await Warehouses.CountAsync();
        }

        public async Task<List<Warehouse>> List(WarehouseFilter filter)
        {
            if (filter == null) return new List<Warehouse>();
            IQueryable<WarehouseDAO> WarehouseDAOs = DataContext.Warehouse.AsNoTracking();
            WarehouseDAOs = DynamicFilter(WarehouseDAOs, filter);
            WarehouseDAOs = OrFilter(WarehouseDAOs, filter);
            WarehouseDAOs = DynamicOrder(WarehouseDAOs, filter);
            List<Warehouse> Warehouses = await DynamicSelect(WarehouseDAOs, filter);
            return Warehouses;
        }

        public async Task<List<Warehouse>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.Warehouse
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<Warehouse> Warehouses = await query.AsNoTracking()
            .Select(x => new Warehouse()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                OrganizationId = x.OrganizationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
                    Used = x.District.Used,
                },
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                    Phone = x.Organization.Phone,
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                    RowId = x.Organization.RowId,
                    Used = x.Organization.Used,
                    IsDisplay = x.Organization.IsDisplay,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                    Used = x.Province.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                    Used = x.Ward.Used,
                },
            }).ToListAsync();
            
            var Inventory = from s in DataContext.Inventory
                                   join tt in tempTableQuery.Query on s.WarehouseId equals tt.Column1
                                   select s;
            List<Inventory> Inventories = await Inventory
                .Where(x => x.DeletedAt == null)
                .Select(x => new Inventory
                {
                    WarehouseId = x.WarehouseId,
                    ItemId = x.ItemId,
                    AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                    AlternateQuantity = x.AlternateQuantity,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    AlternateUnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.AlternateUnitOfMeasure.Id,
                        Code = x.AlternateUnitOfMeasure.Code,
                        Name = x.AlternateUnitOfMeasure.Name,
                        Description = x.AlternateUnitOfMeasure.Description,
                        StatusId = x.AlternateUnitOfMeasure.StatusId,
                        Used = x.AlternateUnitOfMeasure.Used,
                        RowId = x.AlternateUnitOfMeasure.RowId,
                    },
                    Item = new Item
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
                    UnitOfMeasure = new UnitOfMeasure
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

            foreach(Warehouse Warehouse in Warehouses)
            {
                Warehouse.Inventories = Inventories
                    .Where(x => x.WarehouseId == Warehouse.Id)
                    .ToList();
            }


            return Warehouses;
        }

        public async Task<Warehouse> Get(long Id)
        {
            Warehouse Warehouse = await DataContext.Warehouse.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Warehouse()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Address = x.Address,
                OrganizationId = x.OrganizationId,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                District = x.District == null ? null : new District
                {
                    Id = x.District.Id,
                    Code = x.District.Code,
                    Name = x.District.Name,
                    Priority = x.District.Priority,
                    ProvinceId = x.District.ProvinceId,
                    StatusId = x.District.StatusId,
                    RowId = x.District.RowId,
                    Used = x.District.Used,
                },
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                    Phone = x.Organization.Phone,
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                    RowId = x.Organization.RowId,
                    Used = x.Organization.Used,
                    IsDisplay = x.Organization.IsDisplay,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                    RowId = x.Province.RowId,
                    Used = x.Province.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Ward = x.Ward == null ? null : new Ward
                {
                    Id = x.Ward.Id,
                    Code = x.Ward.Code,
                    Name = x.Ward.Name,
                    Priority = x.Ward.Priority,
                    DistrictId = x.Ward.DistrictId,
                    StatusId = x.Ward.StatusId,
                    RowId = x.Ward.RowId,
                    Used = x.Ward.Used,
                },
            }).FirstOrDefaultAsync();

            if (Warehouse == null)
                return null;
            Warehouse.Inventories = await DataContext.Inventory.AsNoTracking()
                .Where(x => x.WarehouseId == Warehouse.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new Inventory
                {
                    WarehouseId = x.WarehouseId,
                    ItemId = x.ItemId,
                    AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                    AlternateQuantity = x.AlternateQuantity,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    AlternateUnitOfMeasure = new UnitOfMeasure
                    {
                        Id = x.AlternateUnitOfMeasure.Id,
                        Code = x.AlternateUnitOfMeasure.Code,
                        Name = x.AlternateUnitOfMeasure.Name,
                        Description = x.AlternateUnitOfMeasure.Description,
                        StatusId = x.AlternateUnitOfMeasure.StatusId,
                        Used = x.AlternateUnitOfMeasure.Used,
                        RowId = x.AlternateUnitOfMeasure.RowId,
                    },
                    Item = new Item
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
                    UnitOfMeasure = new UnitOfMeasure
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

            return Warehouse;
        }
        
        public async Task<bool> Create(Warehouse Warehouse)
        {
            WarehouseDAO WarehouseDAO = new WarehouseDAO();
            WarehouseDAO.Id = Warehouse.Id;
            WarehouseDAO.Code = Warehouse.Code;
            WarehouseDAO.Name = Warehouse.Name;
            WarehouseDAO.Address = Warehouse.Address;
            WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
            WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
            WarehouseDAO.DistrictId = Warehouse.DistrictId;
            WarehouseDAO.WardId = Warehouse.WardId;
            WarehouseDAO.StatusId = Warehouse.StatusId;
            WarehouseDAO.RowId = Warehouse.RowId;
            WarehouseDAO.RowId = Guid.NewGuid();
            WarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
            WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Warehouse.Add(WarehouseDAO);
            await DataContext.SaveChangesAsync();
            Warehouse.Id = WarehouseDAO.Id;
            await SaveReference(Warehouse);
            return true;
        }

        public async Task<bool> Update(Warehouse Warehouse)
        {
            WarehouseDAO WarehouseDAO = DataContext.Warehouse.Where(x => x.Id == Warehouse.Id).FirstOrDefault();
            if (WarehouseDAO == null)
                return false;
            WarehouseDAO.Id = Warehouse.Id;
            WarehouseDAO.Code = Warehouse.Code;
            WarehouseDAO.Name = Warehouse.Name;
            WarehouseDAO.Address = Warehouse.Address;
            WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
            WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
            WarehouseDAO.DistrictId = Warehouse.DistrictId;
            WarehouseDAO.WardId = Warehouse.WardId;
            WarehouseDAO.StatusId = Warehouse.StatusId;
            WarehouseDAO.RowId = Warehouse.RowId;
            WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Warehouse);
            return true;
        }

        public async Task<bool> Delete(Warehouse Warehouse)
        {
            await DataContext.Warehouse.Where(x => x.Id == Warehouse.Id).UpdateFromQueryAsync(x => new WarehouseDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Warehouse> Warehouses)
        {
            List<WarehouseDAO> WarehouseDAOs = new List<WarehouseDAO>();
            foreach (Warehouse Warehouse in Warehouses)
            {
                WarehouseDAO WarehouseDAO = new WarehouseDAO();
                WarehouseDAO.Id = Warehouse.Id;
                WarehouseDAO.Code = Warehouse.Code;
                WarehouseDAO.Name = Warehouse.Name;
                WarehouseDAO.Address = Warehouse.Address;
                WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
                WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
                WarehouseDAO.DistrictId = Warehouse.DistrictId;
                WarehouseDAO.WardId = Warehouse.WardId;
                WarehouseDAO.StatusId = Warehouse.StatusId;
                WarehouseDAO.RowId = Warehouse.RowId;
                WarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
                WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
                WarehouseDAOs.Add(WarehouseDAO);
            }
            await DataContext.BulkMergeAsync(WarehouseDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Warehouse> Warehouses)
        {
            List<long> Ids = Warehouses.Select(x => x.Id).ToList();
            await DataContext.Warehouse
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new WarehouseDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Warehouse Warehouse)
        {
            List<InventoryDAO> InventoryDAOs = await DataContext.Inventory
                .Where(x => x.WarehouseId == Warehouse.Id).ToListAsync();
            InventoryDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Warehouse.Inventories != null)
            {
                foreach (Inventory Inventory in Warehouse.Inventories)
                {
                    InventoryDAO InventoryDAO = InventoryDAOs
                        .Where(x => x.ItemId == Inventory.ItemId && x.WarehouseId == Inventory.WarehouseId).FirstOrDefault();
                    if (InventoryDAO == null)
                    {
                        InventoryDAO = new InventoryDAO();
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
                        InventoryDAO.AlternateQuantity = Inventory.AlternateQuantity;
                        InventoryDAO.UnitOfMeasureId = Inventory.UnitOfMeasureId;
                        InventoryDAO.Quantity = Inventory.Quantity;
                        InventoryDAOs.Add(InventoryDAO);
                        InventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                    }
                    else
                    {
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
                        InventoryDAO.AlternateQuantity = Inventory.AlternateQuantity;
                        InventoryDAO.UnitOfMeasureId = Inventory.UnitOfMeasureId;
                        InventoryDAO.Quantity = Inventory.Quantity;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                    }
                }
                await DataContext.Inventory.BulkMergeAsync(InventoryDAOs);
            }
        }
        
    }
}
