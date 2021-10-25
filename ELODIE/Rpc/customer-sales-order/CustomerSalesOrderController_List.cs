using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using ELODIE.Entities;
using ELODIE.Services.MCustomerSalesOrder;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCustomer;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MProvince;
using ELODIE.Services.MWard;
using ELODIE.Services.MEditedPriceStatus;
using ELODIE.Services.MOrderPaymentStatus;
using ELODIE.Services.MOrderSource;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MRequestState;
using ELODIE.Services.MCustomerSalesOrderContent;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MTaxType;
using ELODIE.Services.MCustomerSalesOrderPaymentHistory;
using ELODIE.Services.MPaymentType;
using ELODIE.Enums;

namespace ELODIE.Rpc.customer_sales_order
{
    public partial class CustomerSalesOrderController : RpcController
    {
        [Route(CustomerSalesOrderRoute.ListItem), HttpPost]
        public async Task<List<CustomerSalesOrder_ItemDTO>> ListItem([FromBody] CustomerSalesOrder_ItemFilterDTO CustomerSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = CustomerSalesOrder_ItemFilterDTO.Skip;
            ItemFilter.Take = CustomerSalesOrder_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = CustomerSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = CustomerSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = CustomerSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = CustomerSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = CustomerSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = CustomerSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = CustomerSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = CustomerSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = CustomerSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = CustomerSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter = ItemService.ToFilter(ItemFilter);

            if (CustomerSalesOrder_ItemFilterDTO.SalesEmployeeId == null && CustomerSalesOrder_ItemFilterDTO.SalesEmployeeId.HasValue == false)
            {
                return new List<CustomerSalesOrder_ItemDTO>();
            }
            else
            {
                List<Item> Items = await CustomerSalesOrderService.ListItem(ItemFilter, CustomerSalesOrder_ItemFilterDTO.SalesEmployeeId.Equal);
                List<CustomerSalesOrder_ItemDTO> CustomerSalesOrder_ItemDTOs = Items
                    .Select(x => new CustomerSalesOrder_ItemDTO(x)).ToList();
                return CustomerSalesOrder_ItemDTOs;
            }
        }
        [Route(CustomerSalesOrderRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] CustomerSalesOrder_ItemFilterDTO CustomerSalesOrder_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = CustomerSalesOrder_ItemFilterDTO.Id;
            ItemFilter.Code = CustomerSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = CustomerSalesOrder_ItemFilterDTO.Name;
            ItemFilter.OtherName = CustomerSalesOrder_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = CustomerSalesOrder_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = CustomerSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = CustomerSalesOrder_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = CustomerSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = CustomerSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = CustomerSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            ItemFilter = ItemService.ToFilter(ItemFilter);

            return await ItemService.Count(ItemFilter);
        }
    }
}

