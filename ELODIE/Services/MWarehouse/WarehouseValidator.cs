using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;

namespace ELODIE.Services.MWarehouse
{
    public interface IWarehouseValidator : IServiceScoped
    {
        Task Get(Warehouse Warehouse);
        Task<bool> Create(Warehouse Warehouse);
        Task<bool> Update(Warehouse Warehouse);
        Task<bool> Delete(Warehouse Warehouse);
        Task<bool> BulkDelete(List<Warehouse> Warehouses);
        Task<bool> Import(List<Warehouse> Warehouses);
    }

    public class WarehouseValidator : IWarehouseValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private WarehouseMessage WarehouseMessage;

        public WarehouseValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.WarehouseMessage = new WarehouseMessage();
        }

        public async Task Get(Warehouse Warehouse)
        {
        }

        public async Task<bool> Create(Warehouse Warehouse)
        {
            return Warehouse.IsValidated;
        }

        public async Task<bool> Update(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
            }
            return Warehouse.IsValidated;
        }

        public async Task<bool> Delete(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
            }
            return Warehouse.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Warehouse> Warehouses)
        {
            foreach (Warehouse Warehouse in Warehouses)
            {
                await Delete(Warehouse);
            }
            return Warehouses.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Warehouse> Warehouses)
        {
            return true;
        }
        
        public async Task<bool> ValidateId(Warehouse Warehouse)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Warehouse.Id },
                Selects = WarehouseSelect.Id
            };

            int count = await UOW.WarehouseRepository.CountAll(WarehouseFilter);
            if (count == 0)
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Id), WarehouseMessage.Error.IdNotExisted);
            return count == 1;
        }
    }
}
