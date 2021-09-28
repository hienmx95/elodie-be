using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using Microsoft.AspNetCore.Mvc;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public partial class SupplierController : RpcController
    {
        [Route(SupplierRoute.CountCategory), HttpPost]
        public async Task<long> CountCategory([FromBody] Supplier_CategoryFilterDTO Supplier_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Id = Supplier_CategoryFilterDTO.Id;
            CategoryFilter.Code = Supplier_CategoryFilterDTO.Code;
            CategoryFilter.Name = Supplier_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Supplier_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Supplier_CategoryFilterDTO.Path;
            CategoryFilter.Level = Supplier_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Supplier_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Supplier_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Supplier_CategoryFilterDTO.RowId;

            return await CategoryService.Count(CategoryFilter);
        }

        [Route(SupplierRoute.ListCategory), HttpPost]
        public async Task<List<Supplier_CategoryDTO>> ListCategory([FromBody] Supplier_CategoryFilterDTO Supplier_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = Supplier_CategoryFilterDTO.Skip;
            CategoryFilter.Take = Supplier_CategoryFilterDTO.Take;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = Supplier_CategoryFilterDTO.Id;
            CategoryFilter.Code = Supplier_CategoryFilterDTO.Code;
            CategoryFilter.Name = Supplier_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Supplier_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Supplier_CategoryFilterDTO.Path;
            CategoryFilter.Level = Supplier_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Supplier_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Supplier_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Supplier_CategoryFilterDTO.RowId;

            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Supplier_CategoryDTO> Supplier_CategoryDTOs = Categories
                .Select(x => new Supplier_CategoryDTO(x)).ToList();
            return Supplier_CategoryDTOs;
        }
    }
}

