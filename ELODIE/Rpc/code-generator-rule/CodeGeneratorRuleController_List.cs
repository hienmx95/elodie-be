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
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MEntityType;
using ELODIE.Services.MStatus;
using ELODIE.Services.MEntityComponent;
using ELODIE.Enums;

namespace ELODIE.Rpc.code_generator_rule
{
    public partial class CodeGeneratorRuleController : RpcController
    {
        [Route(CodeGeneratorRuleRoute.FilterListEntityType), HttpPost]
        public async Task<List<CodeGeneratorRule_EntityTypeDTO>> FilterListEntityType([FromBody] CodeGeneratorRule_EntityTypeFilterDTO CodeGeneratorRule_EntityTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EntityTypeFilter EntityTypeFilter = new EntityTypeFilter();
            EntityTypeFilter.Skip = 0;
            EntityTypeFilter.Take = int.MaxValue;
            EntityTypeFilter.OrderBy = EntityTypeOrder.Id;
            EntityTypeFilter.OrderType = OrderType.ASC;
            EntityTypeFilter.Selects = EntityTypeSelect.ALL;
            EntityTypeFilter.Id = CodeGeneratorRule_EntityTypeFilterDTO.Id;
            EntityTypeFilter.Code = CodeGeneratorRule_EntityTypeFilterDTO.Code;
            EntityTypeFilter.Name = CodeGeneratorRule_EntityTypeFilterDTO.Name;

            List<EntityType> EntityTypes = await EntityTypeService.List(EntityTypeFilter);
            List<CodeGeneratorRule_EntityTypeDTO> CodeGeneratorRule_EntityTypeDTOs = EntityTypes
                .Select(x => new CodeGeneratorRule_EntityTypeDTO(x)).ToList();
            return CodeGeneratorRule_EntityTypeDTOs;
        }
        [Route(CodeGeneratorRuleRoute.FilterListStatus), HttpPost]
        public async Task<List<CodeGeneratorRule_StatusDTO>> FilterListStatus([FromBody] CodeGeneratorRule_StatusFilterDTO CodeGeneratorRule_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = CodeGeneratorRule_StatusFilterDTO.Id;
            StatusFilter.Code = CodeGeneratorRule_StatusFilterDTO.Code;
            StatusFilter.Name = CodeGeneratorRule_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<CodeGeneratorRule_StatusDTO> CodeGeneratorRule_StatusDTOs = Statuses
                .Select(x => new CodeGeneratorRule_StatusDTO(x)).ToList();
            return CodeGeneratorRule_StatusDTOs;
        }
        [Route(CodeGeneratorRuleRoute.FilterListEntityComponent), HttpPost]
        public async Task<List<CodeGeneratorRule_EntityComponentDTO>> FilterListEntityComponent([FromBody] CodeGeneratorRule_EntityComponentFilterDTO CodeGeneratorRule_EntityComponentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EntityComponentFilter EntityComponentFilter = new EntityComponentFilter();
            EntityComponentFilter.Skip = 0;
            EntityComponentFilter.Take = int.MaxValue;
            EntityComponentFilter.OrderBy = EntityComponentOrder.Id;
            EntityComponentFilter.OrderType = OrderType.ASC;
            EntityComponentFilter.Selects = EntityComponentSelect.ALL;

            List<EntityComponent> EntityComponents = await EntityComponentService.List(EntityComponentFilter);
            List<CodeGeneratorRule_EntityComponentDTO> CodeGeneratorRule_EntityComponentDTOs = EntityComponents
                .Select(x => new CodeGeneratorRule_EntityComponentDTO(x)).ToList();
            return CodeGeneratorRule_EntityComponentDTOs;
        }

        [Route(CodeGeneratorRuleRoute.SingleListEntityType), HttpPost]
        public async Task<List<CodeGeneratorRule_EntityTypeDTO>> SingleListEntityType([FromBody] CodeGeneratorRule_EntityTypeFilterDTO CodeGeneratorRule_EntityTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EntityTypeFilter EntityTypeFilter = new EntityTypeFilter();
            EntityTypeFilter.Skip = 0;
            EntityTypeFilter.Take = int.MaxValue;
            EntityTypeFilter.OrderBy = EntityTypeOrder.Id;
            EntityTypeFilter.OrderType = OrderType.ASC;
            EntityTypeFilter.Selects = EntityTypeSelect.ALL;
            EntityTypeFilter.Id = CodeGeneratorRule_EntityTypeFilterDTO.Id;
            EntityTypeFilter.Code = CodeGeneratorRule_EntityTypeFilterDTO.Code;
            EntityTypeFilter.Name = CodeGeneratorRule_EntityTypeFilterDTO.Name;

            List<EntityType> EntityTypes = await EntityTypeService.List(EntityTypeFilter);
            List<CodeGeneratorRule_EntityTypeDTO> CodeGeneratorRule_EntityTypeDTOs = EntityTypes
                .Select(x => new CodeGeneratorRule_EntityTypeDTO(x)).ToList();
            return CodeGeneratorRule_EntityTypeDTOs;
        }
        [Route(CodeGeneratorRuleRoute.SingleListStatus), HttpPost]
        public async Task<List<CodeGeneratorRule_StatusDTO>> SingleListStatus([FromBody] CodeGeneratorRule_StatusFilterDTO CodeGeneratorRule_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = CodeGeneratorRule_StatusFilterDTO.Id;
            StatusFilter.Code = CodeGeneratorRule_StatusFilterDTO.Code;
            StatusFilter.Name = CodeGeneratorRule_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<CodeGeneratorRule_StatusDTO> CodeGeneratorRule_StatusDTOs = Statuses
                .Select(x => new CodeGeneratorRule_StatusDTO(x)).ToList();
            return CodeGeneratorRule_StatusDTOs;
        }
        [Route(CodeGeneratorRuleRoute.SingleListEntityComponent), HttpPost]
        public async Task<List<CodeGeneratorRule_EntityComponentDTO>> SingleListEntityComponent([FromBody] CodeGeneratorRule_EntityComponentFilterDTO CodeGeneratorRule_EntityComponentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            EntityComponentFilter EntityComponentFilter = new EntityComponentFilter();
            EntityComponentFilter.Skip = 0;
            EntityComponentFilter.Take = int.MaxValue;
            EntityComponentFilter.Take = 20;
            EntityComponentFilter.OrderBy = EntityComponentOrder.Id;
            EntityComponentFilter.OrderType = OrderType.ASC;
            EntityComponentFilter.Selects = EntityComponentSelect.ALL;
            EntityComponentFilter.Id = CodeGeneratorRule_EntityComponentFilterDTO.Id;
            EntityComponentFilter.Code = CodeGeneratorRule_EntityComponentFilterDTO.Code;
            EntityComponentFilter.Name = CodeGeneratorRule_EntityComponentFilterDTO.Name;
            if (CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId != null && CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.HasValue)
            {
                if(CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.Value == EntityTypeEnum.SALES_ORDER.Id)
                {
                    EntityComponentFilter.Id.In = EntityComponentEnum.OrderEnumList.Select(x => x.Id).ToList();
                }
                else if(CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.Value == EntityTypeEnum.PRODUCT.Id)
                {
                    EntityComponentFilter.Id.In = EntityComponentEnum.ProductEnumList.Select(x => x.Id).ToList();
                }
                else if (CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.Value == EntityTypeEnum.STORE.Id)
                {
                    EntityComponentFilter.Id.In = EntityComponentEnum.StoreEnumList.Select(x => x.Id).ToList();
                }
                else if (CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.Value == EntityTypeEnum.CUSTOMER.Id)
                {
                    EntityComponentFilter.Id.In = EntityComponentEnum.CustomerEnumList.Select(x => x.Id).ToList();
                }
                else if (CodeGeneratorRule_EntityComponentFilterDTO.EntityTypeId.Equal.Value == EntityTypeEnum.OPPORTUNITY.Id)
                {
                    EntityComponentFilter.Id.In = EntityComponentEnum.OpportunityEnumList.Select(x => x.Id).ToList();
                }
            }
            List<EntityComponent> EntityComponents = await EntityComponentService.List(EntityComponentFilter);
            List<CodeGeneratorRule_EntityComponentDTO> CodeGeneratorRule_EntityComponentDTOs = EntityComponents
                .Select(x => new CodeGeneratorRule_EntityComponentDTO(x)).ToList();
            return CodeGeneratorRule_EntityComponentDTOs;
        }
    }
}

