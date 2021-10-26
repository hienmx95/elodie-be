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
    public interface IInventoryRepository
    {
        Task<int> CountAll(InventoryFilter InventoryFilter);
        Task<int> Count(InventoryFilter InventoryFilter);
        Task<List<Inventory>> List(InventoryFilter InventoryFilter);
        Task<List<Inventory>> List(List<long> Ids);
        Task<Inventory> Get(long Id);
        Task<Inventory> GetByWarehouseAndItem(long WarehouseId, long ItemId);
        Task<bool> Create(Inventory Inventory);
        Task<bool> Update(Inventory Inventory);
        Task<bool> Delete(Inventory Inventory);
        Task<bool> BulkMerge(List<Inventory> Inventories);
        Task<bool> BulkDelete(List<Inventory> Inventories);
    }
    public class InventoryRepository : IInventoryRepository
    {
        private DataContext DataContext;
        public InventoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<InventoryDAO> DynamicFilter(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.WarehouseId, filter.WarehouseId);
            query = query.Where(q => q.ItemId, filter.ItemId);
            query = query.Where(q => q.AlternateUnitOfMeasureId, filter.AlternateUnitOfMeasureId);
            query = query.Where(q => q.AlternateQuantity, filter.AlternateQuantity);
            query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            query = query.Where(q => q.Quantity, filter.Quantity);
            query = query.Where(q => q.PendingQuantity, filter.PendingQuantity);
            
            return query;
        }

        private IQueryable<InventoryDAO> OrFilter(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<InventoryDAO> initQuery = query.Where(q => false);
            foreach (InventoryFilter InventoryFilter in filter.OrFilter)
            {
                IQueryable<InventoryDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.WarehouseId, filter.WarehouseId);
                queryable = queryable.Where(q => q.ItemId, filter.ItemId);
                queryable = queryable.Where(q => q.AlternateUnitOfMeasureId, filter.AlternateUnitOfMeasureId);
                queryable = queryable.Where(q => q.AlternateQuantity, filter.AlternateQuantity);
                queryable = queryable.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
                queryable = queryable.Where(q => q.Quantity, filter.Quantity);
                queryable = queryable.Where(q => q.PendingQuantity, filter.PendingQuantity);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<InventoryDAO> DynamicOrder(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case InventoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case InventoryOrder.Warehouse:
                            query = query.OrderBy(q => q.WarehouseId);
                            break;
                        case InventoryOrder.Item:
                            query = query.OrderBy(q => q.ItemId);
                            break;
                        case InventoryOrder.AlternateUnitOfMeasure:
                            query = query.OrderBy(q => q.AlternateUnitOfMeasureId);
                            break;
                        case InventoryOrder.AlternateQuantity:
                            query = query.OrderBy(q => q.AlternateQuantity);
                            break;
                        case InventoryOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case InventoryOrder.Quantity:
                            query = query.OrderBy(q => q.Quantity);
                            break;
                        case InventoryOrder.PendingQuantity:
                            query = query.OrderBy(q => q.PendingQuantity);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case InventoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case InventoryOrder.Warehouse:
                            query = query.OrderByDescending(q => q.WarehouseId);
                            break;
                        case InventoryOrder.Item:
                            query = query.OrderByDescending(q => q.ItemId);
                            break;
                        case InventoryOrder.AlternateUnitOfMeasure:
                            query = query.OrderByDescending(q => q.AlternateUnitOfMeasureId);
                            break;
                        case InventoryOrder.AlternateQuantity:
                            query = query.OrderByDescending(q => q.AlternateQuantity);
                            break;
                        case InventoryOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case InventoryOrder.Quantity:
                            query = query.OrderByDescending(q => q.Quantity);
                            break;
                        case InventoryOrder.PendingQuantity:
                            query = query.OrderByDescending(q => q.PendingQuantity);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Inventory>> DynamicSelect(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            List<Inventory> Inventories = await query.Select(q => new Inventory()
            {
                Id = filter.Selects.Contains(InventorySelect.Id) ? q.Id : default(long),
                WarehouseId = filter.Selects.Contains(InventorySelect.Warehouse) ? q.WarehouseId : default(long),
                ItemId = filter.Selects.Contains(InventorySelect.Item) ? q.ItemId : default(long),
                AlternateUnitOfMeasureId = filter.Selects.Contains(InventorySelect.AlternateUnitOfMeasure) ? q.AlternateUnitOfMeasureId : default(long),
                AlternateQuantity = filter.Selects.Contains(InventorySelect.AlternateQuantity) ? q.AlternateQuantity : default(decimal),
                UnitOfMeasureId = filter.Selects.Contains(InventorySelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                Quantity = filter.Selects.Contains(InventorySelect.Quantity) ? q.Quantity : default(long),
                PendingQuantity = filter.Selects.Contains(InventorySelect.PendingQuantity) ? q.PendingQuantity : default(long),
                AlternateUnitOfMeasure = filter.Selects.Contains(InventorySelect.AlternateUnitOfMeasure) && q.AlternateUnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.AlternateUnitOfMeasure.Id,
                    Code = q.AlternateUnitOfMeasure.Code,
                    Name = q.AlternateUnitOfMeasure.Name,
                    Description = q.AlternateUnitOfMeasure.Description,
                    StatusId = q.AlternateUnitOfMeasure.StatusId,
                    Used = q.AlternateUnitOfMeasure.Used,
                    RowId = q.AlternateUnitOfMeasure.RowId,
                } : null,
                Item = filter.Selects.Contains(InventorySelect.Item) && q.Item != null ? new Item
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
                UnitOfMeasure = filter.Selects.Contains(InventorySelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                    Used = q.UnitOfMeasure.Used,
                    RowId = q.UnitOfMeasure.RowId,
                } : null,
                Warehouse = filter.Selects.Contains(InventorySelect.Warehouse) && q.Warehouse != null ? new Warehouse
                {
                    Id = q.Warehouse.Id,
                    Code = q.Warehouse.Code,
                    Name = q.Warehouse.Name,
                    Address = q.Warehouse.Address,
                    OrganizationId = q.Warehouse.OrganizationId,
                    ProvinceId = q.Warehouse.ProvinceId,
                    DistrictId = q.Warehouse.DistrictId,
                    WardId = q.Warehouse.WardId,
                    StatusId = q.Warehouse.StatusId,
                    RowId = q.Warehouse.RowId,
                } : null,
            }).ToListAsync();
            return Inventories;
        }

        public async Task<int> CountAll(InventoryFilter filter)
        {
            IQueryable<InventoryDAO> Inventories = DataContext.Inventory.AsNoTracking();
            Inventories = DynamicFilter(Inventories, filter);
            return await Inventories.CountAsync();
        }

        public async Task<int> Count(InventoryFilter filter)
        {
            IQueryable<InventoryDAO> Inventories = DataContext.Inventory.AsNoTracking();
            Inventories = DynamicFilter(Inventories, filter);
            Inventories = OrFilter(Inventories, filter);
            return await Inventories.CountAsync();
        }

        public async Task<List<Inventory>> List(InventoryFilter filter)
        {
            if (filter == null) return new List<Inventory>();
            IQueryable<InventoryDAO> InventoryDAOs = DataContext.Inventory.AsNoTracking();
            InventoryDAOs = DynamicFilter(InventoryDAOs, filter);
            InventoryDAOs = OrFilter(InventoryDAOs, filter);
            InventoryDAOs = DynamicOrder(InventoryDAOs, filter);
            List<Inventory> Inventories = await DynamicSelect(InventoryDAOs, filter);
            return Inventories;
        }

        public async Task<List<Inventory>> List(List<long> Ids)
        {
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var query = from x in DataContext.Inventory
                        join tt in tempTableQuery.Query on x.Id equals tt.Column1
                        select x;
            List<Inventory> Inventories = await query.AsNoTracking()
            .Select(x => new Inventory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                WarehouseId = x.WarehouseId,
                ItemId = x.ItemId,
                AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                AlternateQuantity = x.AlternateQuantity,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                PendingQuantity = x.PendingQuantity,
                AlternateUnitOfMeasure = x.AlternateUnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.AlternateUnitOfMeasure.Id,
                    Code = x.AlternateUnitOfMeasure.Code,
                    Name = x.AlternateUnitOfMeasure.Name,
                    Description = x.AlternateUnitOfMeasure.Description,
                    StatusId = x.AlternateUnitOfMeasure.StatusId,
                    Used = x.AlternateUnitOfMeasure.Used,
                    RowId = x.AlternateUnitOfMeasure.RowId,
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
                Warehouse = x.Warehouse == null ? null : new Warehouse
                {
                    Id = x.Warehouse.Id,
                    Code = x.Warehouse.Code,
                    Name = x.Warehouse.Name,
                    Address = x.Warehouse.Address,
                    OrganizationId = x.Warehouse.OrganizationId,
                    ProvinceId = x.Warehouse.ProvinceId,
                    DistrictId = x.Warehouse.DistrictId,
                    WardId = x.Warehouse.WardId,
                    StatusId = x.Warehouse.StatusId,
                    RowId = x.Warehouse.RowId,
                },
            }).ToListAsync();
            

            return Inventories;
        }

        public async Task<Inventory> Get(long Id)
        {
            Inventory Inventory = await DataContext.Inventory.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Inventory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                WarehouseId = x.WarehouseId,
                ItemId = x.ItemId,
                AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                AlternateQuantity = x.AlternateQuantity,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                PendingQuantity = x.PendingQuantity,
                AlternateUnitOfMeasure = x.AlternateUnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.AlternateUnitOfMeasure.Id,
                    Code = x.AlternateUnitOfMeasure.Code,
                    Name = x.AlternateUnitOfMeasure.Name,
                    Description = x.AlternateUnitOfMeasure.Description,
                    StatusId = x.AlternateUnitOfMeasure.StatusId,
                    Used = x.AlternateUnitOfMeasure.Used,
                    RowId = x.AlternateUnitOfMeasure.RowId,
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
                Warehouse = x.Warehouse == null ? null : new Warehouse
                {
                    Id = x.Warehouse.Id,
                    Code = x.Warehouse.Code,
                    Name = x.Warehouse.Name,
                    Address = x.Warehouse.Address,
                    OrganizationId = x.Warehouse.OrganizationId,
                    ProvinceId = x.Warehouse.ProvinceId,
                    DistrictId = x.Warehouse.DistrictId,
                    WardId = x.Warehouse.WardId,
                    StatusId = x.Warehouse.StatusId,
                    RowId = x.Warehouse.RowId,
                },
            }).FirstOrDefaultAsync();

            if (Inventory == null)
                return null;

            return Inventory;
        }

        public async Task<Inventory> GetByWarehouseAndItem(long WarehouseId, long ItemId)
        {
            Inventory Inventory = await DataContext.Inventory.AsNoTracking()
            .Where(x => x.WarehouseId == WarehouseId)
            .Where(x => x.ItemId == ItemId)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Inventory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                WarehouseId = x.WarehouseId,
                ItemId = x.ItemId,
                AlternateUnitOfMeasureId = x.AlternateUnitOfMeasureId,
                AlternateQuantity = x.AlternateQuantity,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                PendingQuantity = x.PendingQuantity,
                AlternateUnitOfMeasure = x.AlternateUnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.AlternateUnitOfMeasure.Id,
                    Code = x.AlternateUnitOfMeasure.Code,
                    Name = x.AlternateUnitOfMeasure.Name,
                    Description = x.AlternateUnitOfMeasure.Description,
                    StatusId = x.AlternateUnitOfMeasure.StatusId,
                    Used = x.AlternateUnitOfMeasure.Used,
                    RowId = x.AlternateUnitOfMeasure.RowId,
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
                Warehouse = x.Warehouse == null ? null : new Warehouse
                {
                    Id = x.Warehouse.Id,
                    Code = x.Warehouse.Code,
                    Name = x.Warehouse.Name,
                    Address = x.Warehouse.Address,
                    OrganizationId = x.Warehouse.OrganizationId,
                    ProvinceId = x.Warehouse.ProvinceId,
                    DistrictId = x.Warehouse.DistrictId,
                    WardId = x.Warehouse.WardId,
                    StatusId = x.Warehouse.StatusId,
                    RowId = x.Warehouse.RowId,
                },
            }).FirstOrDefaultAsync();

            if (Inventory == null)
                return null;

            return Inventory;
        }

        public async Task<bool> Create(Inventory Inventory)
        {
            InventoryDAO InventoryDAO = new InventoryDAO();
            InventoryDAO.Id = Inventory.Id;
            InventoryDAO.WarehouseId = Inventory.WarehouseId;
            InventoryDAO.ItemId = Inventory.ItemId;
            InventoryDAO.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
            InventoryDAO.AlternateQuantity = Inventory.AlternateQuantity;
            InventoryDAO.UnitOfMeasureId = Inventory.UnitOfMeasureId;
            InventoryDAO.Quantity = Inventory.Quantity;
            InventoryDAO.PendingQuantity = Inventory.PendingQuantity;
            InventoryDAO.CreatedAt = StaticParams.DateTimeNow;
            InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Inventory.Add(InventoryDAO);
            await DataContext.SaveChangesAsync();
            Inventory.Id = InventoryDAO.Id;
            await SaveReference(Inventory);
            return true;
        }

        public async Task<bool> Update(Inventory Inventory)
        {
            InventoryDAO InventoryDAO = DataContext.Inventory.Where(x => x.Id == Inventory.Id).FirstOrDefault();
            if (InventoryDAO == null)
                return false;
            InventoryDAO.Id = Inventory.Id;
            InventoryDAO.WarehouseId = Inventory.WarehouseId;
            InventoryDAO.ItemId = Inventory.ItemId;
            InventoryDAO.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
            InventoryDAO.AlternateQuantity = Inventory.AlternateQuantity;
            InventoryDAO.UnitOfMeasureId = Inventory.UnitOfMeasureId;
            InventoryDAO.Quantity = Inventory.Quantity;
            InventoryDAO.PendingQuantity = Inventory.PendingQuantity;
            InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Inventory);
            return true;
        }

        public async Task<bool> Delete(Inventory Inventory)
        {
            await DataContext.Inventory.Where(x => x.Id == Inventory.Id).UpdateFromQueryAsync(x => new InventoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Inventory> Inventories)
        {
            List<InventoryDAO> InventoryDAOs = new List<InventoryDAO>();
            foreach (Inventory Inventory in Inventories)
            {
                InventoryDAO InventoryDAO = new InventoryDAO();
                InventoryDAO.Id = Inventory.Id;
                InventoryDAO.WarehouseId = Inventory.WarehouseId;
                InventoryDAO.ItemId = Inventory.ItemId;
                InventoryDAO.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
                InventoryDAO.AlternateQuantity = Inventory.AlternateQuantity;
                InventoryDAO.UnitOfMeasureId = Inventory.UnitOfMeasureId;
                InventoryDAO.Quantity = Inventory.Quantity;
                InventoryDAO.PendingQuantity = Inventory.PendingQuantity;
                InventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                InventoryDAOs.Add(InventoryDAO);
            }
            await DataContext.BulkMergeAsync(InventoryDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Inventory> Inventories)
        {
            List<long> Ids = Inventories.Select(x => x.Id).ToList();
            await DataContext.Inventory
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new InventoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Inventory Inventory)
        {
        }
        
    }
}
