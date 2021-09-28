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
        private IEntityTypeService EntityTypeService;
        private IStatusService StatusService;
        private IEntityComponentService EntityComponentService;
        private ICodeGeneratorRuleService CodeGeneratorRuleService;
        private ICurrentContext CurrentContext;
        public CodeGeneratorRuleController(
            IEntityTypeService EntityTypeService,
            IStatusService StatusService,
            IEntityComponentService EntityComponentService,
            ICodeGeneratorRuleService CodeGeneratorRuleService,
            ICurrentContext CurrentContext
        )
        {
            this.EntityTypeService = EntityTypeService;
            this.StatusService = StatusService;
            this.EntityComponentService = EntityComponentService;
            this.CodeGeneratorRuleService = CodeGeneratorRuleService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CodeGeneratorRuleRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] CodeGeneratorRule_CodeGeneratorRuleFilterDTO CodeGeneratorRule_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = ConvertFilterDTOToFilterEntity(CodeGeneratorRule_CodeGeneratorRuleFilterDTO);
            CodeGeneratorRuleFilter = await CodeGeneratorRuleService.ToFilter(CodeGeneratorRuleFilter);
            int count = await CodeGeneratorRuleService.Count(CodeGeneratorRuleFilter);
            return count;
        }

        [Route(CodeGeneratorRuleRoute.List), HttpPost]
        public async Task<ActionResult<List<CodeGeneratorRule_CodeGeneratorRuleDTO>>> List([FromBody] CodeGeneratorRule_CodeGeneratorRuleFilterDTO CodeGeneratorRule_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = ConvertFilterDTOToFilterEntity(CodeGeneratorRule_CodeGeneratorRuleFilterDTO);
            CodeGeneratorRuleFilter = await CodeGeneratorRuleService.ToFilter(CodeGeneratorRuleFilter);
            List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);
            List<CodeGeneratorRule_CodeGeneratorRuleDTO> CodeGeneratorRule_CodeGeneratorRuleDTOs = CodeGeneratorRules
                .Select(c => new CodeGeneratorRule_CodeGeneratorRuleDTO(c)).ToList();
            return CodeGeneratorRule_CodeGeneratorRuleDTOs;
        }

        [Route(CodeGeneratorRuleRoute.Get), HttpPost]
        public async Task<ActionResult<CodeGeneratorRule_CodeGeneratorRuleDTO>> Get([FromBody]CodeGeneratorRule_CodeGeneratorRuleDTO CodeGeneratorRule_CodeGeneratorRuleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CodeGeneratorRule_CodeGeneratorRuleDTO.Id))
                return Forbid();

            CodeGeneratorRule CodeGeneratorRule = await CodeGeneratorRuleService.Get(CodeGeneratorRule_CodeGeneratorRuleDTO.Id);
            return new CodeGeneratorRule_CodeGeneratorRuleDTO(CodeGeneratorRule);
        }

        [Route(CodeGeneratorRuleRoute.Create), HttpPost]
        public async Task<ActionResult<CodeGeneratorRule_CodeGeneratorRuleDTO>> Create([FromBody] CodeGeneratorRule_CodeGeneratorRuleDTO CodeGeneratorRule_CodeGeneratorRuleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(CodeGeneratorRule_CodeGeneratorRuleDTO.Id))
                return Forbid();

            CodeGeneratorRule CodeGeneratorRule = ConvertDTOToEntity(CodeGeneratorRule_CodeGeneratorRuleDTO);
            CodeGeneratorRule = await CodeGeneratorRuleService.Create(CodeGeneratorRule);
            CodeGeneratorRule_CodeGeneratorRuleDTO = new CodeGeneratorRule_CodeGeneratorRuleDTO(CodeGeneratorRule);
            if (CodeGeneratorRule.IsValidated)
                return CodeGeneratorRule_CodeGeneratorRuleDTO;
            else
                return BadRequest(CodeGeneratorRule_CodeGeneratorRuleDTO);
        }

        [Route(CodeGeneratorRuleRoute.Update), HttpPost]
        public async Task<ActionResult<CodeGeneratorRule_CodeGeneratorRuleDTO>> Update([FromBody] CodeGeneratorRule_CodeGeneratorRuleDTO CodeGeneratorRule_CodeGeneratorRuleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CodeGeneratorRule_CodeGeneratorRuleDTO.Id))
                return Forbid();

            CodeGeneratorRule CodeGeneratorRule = ConvertDTOToEntity(CodeGeneratorRule_CodeGeneratorRuleDTO);
            CodeGeneratorRule = await CodeGeneratorRuleService.Update(CodeGeneratorRule);
            CodeGeneratorRule_CodeGeneratorRuleDTO = new CodeGeneratorRule_CodeGeneratorRuleDTO(CodeGeneratorRule);
            if (CodeGeneratorRule.IsValidated)
                return CodeGeneratorRule_CodeGeneratorRuleDTO;
            else
                return BadRequest(CodeGeneratorRule_CodeGeneratorRuleDTO);
        }

        [Route(CodeGeneratorRuleRoute.Delete), HttpPost]
        public async Task<ActionResult<CodeGeneratorRule_CodeGeneratorRuleDTO>> Delete([FromBody] CodeGeneratorRule_CodeGeneratorRuleDTO CodeGeneratorRule_CodeGeneratorRuleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CodeGeneratorRule_CodeGeneratorRuleDTO.Id))
                return Forbid();

            CodeGeneratorRule CodeGeneratorRule = ConvertDTOToEntity(CodeGeneratorRule_CodeGeneratorRuleDTO);
            CodeGeneratorRule = await CodeGeneratorRuleService.Delete(CodeGeneratorRule);
            CodeGeneratorRule_CodeGeneratorRuleDTO = new CodeGeneratorRule_CodeGeneratorRuleDTO(CodeGeneratorRule);
            if (CodeGeneratorRule.IsValidated)
                return CodeGeneratorRule_CodeGeneratorRuleDTO;
            else
                return BadRequest(CodeGeneratorRule_CodeGeneratorRuleDTO);
        }

        [Route(CodeGeneratorRuleRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
            CodeGeneratorRuleFilter = await CodeGeneratorRuleService.ToFilter(CodeGeneratorRuleFilter);
            CodeGeneratorRuleFilter.Id = new IdFilter { In = Ids };
            CodeGeneratorRuleFilter.Selects = CodeGeneratorRuleSelect.Id;
            CodeGeneratorRuleFilter.Skip = 0;
            CodeGeneratorRuleFilter.Take = int.MaxValue;

            List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);
            CodeGeneratorRules = await CodeGeneratorRuleService.BulkDelete(CodeGeneratorRules);
            if (CodeGeneratorRules.Any(x => !x.IsValidated))
                return BadRequest(CodeGeneratorRules.Where(x => !x.IsValidated));
            return true;
        }

        [Route(CodeGeneratorRuleRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            EntityTypeFilter EntityTypeFilter = new EntityTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = EntityTypeSelect.ALL
            };
            List<EntityType> EntityTypes = await EntityTypeService.List(EntityTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<CodeGeneratorRule> CodeGeneratorRules = new List<CodeGeneratorRule>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(CodeGeneratorRules);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int EntityTypeIdColumn = 1 + StartColumn;
                int AutoNumberLenthColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int RowIdColumn = 7 + StartColumn;
                int UsedColumn = 8 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string EntityTypeIdValue = worksheet.Cells[i + StartRow, EntityTypeIdColumn].Value?.ToString();
                    string AutoNumberLenthValue = worksheet.Cells[i + StartRow, AutoNumberLenthColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    CodeGeneratorRule CodeGeneratorRule = new CodeGeneratorRule();
                    CodeGeneratorRule.AutoNumberLenth = long.TryParse(AutoNumberLenthValue, out long AutoNumberLenth) ? AutoNumberLenth : 0;
                    EntityType EntityType = EntityTypes.Where(x => x.Id.ToString() == EntityTypeIdValue).FirstOrDefault();
                    CodeGeneratorRule.EntityTypeId = EntityType == null ? 0 : EntityType.Id;
                    CodeGeneratorRule.EntityType = EntityType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    CodeGeneratorRule.StatusId = Status == null ? 0 : Status.Id;
                    CodeGeneratorRule.Status = Status;
                    
                    CodeGeneratorRules.Add(CodeGeneratorRule);
                }
            }
            CodeGeneratorRules = await CodeGeneratorRuleService.Import(CodeGeneratorRules);
            if (CodeGeneratorRules.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < CodeGeneratorRules.Count; i++)
                {
                    CodeGeneratorRule CodeGeneratorRule = CodeGeneratorRules[i];
                    if (!CodeGeneratorRule.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.Id)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.Id)];
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.EntityTypeId)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.EntityTypeId)];
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.AutoNumberLenth)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.AutoNumberLenth)];
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.StatusId)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.StatusId)];
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.RowId)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.RowId)];
                        if (CodeGeneratorRule.Errors.ContainsKey(nameof(CodeGeneratorRule.Used)))
                            Error += CodeGeneratorRule.Errors[nameof(CodeGeneratorRule.Used)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CodeGeneratorRuleRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] CodeGeneratorRule_CodeGeneratorRuleFilterDTO CodeGeneratorRule_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region CodeGeneratorRule
                var CodeGeneratorRuleFilter = ConvertFilterDTOToFilterEntity(CodeGeneratorRule_CodeGeneratorRuleFilterDTO);
                CodeGeneratorRuleFilter.Skip = 0;
                CodeGeneratorRuleFilter.Take = int.MaxValue;
                CodeGeneratorRuleFilter = await CodeGeneratorRuleService.ToFilter(CodeGeneratorRuleFilter);
                List<CodeGeneratorRule> CodeGeneratorRules = await CodeGeneratorRuleService.List(CodeGeneratorRuleFilter);

                var CodeGeneratorRuleHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "EntityTypeId",
                        "AutoNumberLenth",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CodeGeneratorRuleData = new List<object[]>();
                for (int i = 0; i < CodeGeneratorRules.Count; i++)
                {
                    var CodeGeneratorRule = CodeGeneratorRules[i];
                    CodeGeneratorRuleData.Add(new Object[]
                    {
                        CodeGeneratorRule.Id,
                        CodeGeneratorRule.EntityTypeId,
                        CodeGeneratorRule.AutoNumberLenth,
                        CodeGeneratorRule.StatusId,
                        CodeGeneratorRule.RowId,
                        CodeGeneratorRule.Used,
                    });
                }
                excel.GenerateWorksheet("CodeGeneratorRule", CodeGeneratorRuleHeaders, CodeGeneratorRuleData);
                #endregion
                
                #region EntityType
                var EntityTypeFilter = new EntityTypeFilter();
                EntityTypeFilter.Selects = EntityTypeSelect.ALL;
                EntityTypeFilter.OrderBy = EntityTypeOrder.Id;
                EntityTypeFilter.OrderType = OrderType.ASC;
                EntityTypeFilter.Skip = 0;
                EntityTypeFilter.Take = int.MaxValue;
                List<EntityType> EntityTypes = await EntityTypeService.List(EntityTypeFilter);

                var EntityTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EntityTypeData = new List<object[]>();
                for (int i = 0; i < EntityTypes.Count; i++)
                {
                    var EntityType = EntityTypes[i];
                    EntityTypeData.Add(new Object[]
                    {
                        EntityType.Id,
                        EntityType.Code,
                        EntityType.Name,
                    });
                }
                excel.GenerateWorksheet("EntityType", EntityTypeHeaders, EntityTypeData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                #region EntityComponent
                var EntityComponentFilter = new EntityComponentFilter();
                EntityComponentFilter.Selects = EntityComponentSelect.ALL;
                EntityComponentFilter.OrderBy = EntityComponentOrder.Id;
                EntityComponentFilter.OrderType = OrderType.ASC;
                EntityComponentFilter.Skip = 0;
                EntityComponentFilter.Take = int.MaxValue;
                List<EntityComponent> EntityComponents = await EntityComponentService.List(EntityComponentFilter);

                var EntityComponentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EntityComponentData = new List<object[]>();
                for (int i = 0; i < EntityComponents.Count; i++)
                {
                    var EntityComponent = EntityComponents[i];
                    EntityComponentData.Add(new Object[]
                    {
                        EntityComponent.Id,
                        EntityComponent.Code,
                        EntityComponent.Name,
                    });
                }
                excel.GenerateWorksheet("EntityComponent", EntityComponentHeaders, EntityComponentData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "CodeGeneratorRule.xlsx");
        }

        [Route(CodeGeneratorRuleRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] CodeGeneratorRule_CodeGeneratorRuleFilterDTO CodeGeneratorRule_CodeGeneratorRuleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region CodeGeneratorRule
                var CodeGeneratorRuleHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "EntityTypeId",
                        "AutoNumberLenth",
                        "StatusId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CodeGeneratorRuleData = new List<object[]>();
                excel.GenerateWorksheet("CodeGeneratorRule", CodeGeneratorRuleHeaders, CodeGeneratorRuleData);
                #endregion
                
                #region EntityType
                var EntityTypeFilter = new EntityTypeFilter();
                EntityTypeFilter.Selects = EntityTypeSelect.ALL;
                EntityTypeFilter.OrderBy = EntityTypeOrder.Id;
                EntityTypeFilter.OrderType = OrderType.ASC;
                EntityTypeFilter.Skip = 0;
                EntityTypeFilter.Take = int.MaxValue;
                List<EntityType> EntityTypes = await EntityTypeService.List(EntityTypeFilter);

                var EntityTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EntityTypeData = new List<object[]>();
                for (int i = 0; i < EntityTypes.Count; i++)
                {
                    var EntityType = EntityTypes[i];
                    EntityTypeData.Add(new Object[]
                    {
                        EntityType.Id,
                        EntityType.Code,
                        EntityType.Name,
                    });
                }
                excel.GenerateWorksheet("EntityType", EntityTypeHeaders, EntityTypeData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                #region EntityComponent
                var EntityComponentFilter = new EntityComponentFilter();
                EntityComponentFilter.Selects = EntityComponentSelect.ALL;
                EntityComponentFilter.OrderBy = EntityComponentOrder.Id;
                EntityComponentFilter.OrderType = OrderType.ASC;
                EntityComponentFilter.Skip = 0;
                EntityComponentFilter.Take = int.MaxValue;
                List<EntityComponent> EntityComponents = await EntityComponentService.List(EntityComponentFilter);

                var EntityComponentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EntityComponentData = new List<object[]>();
                for (int i = 0; i < EntityComponents.Count; i++)
                {
                    var EntityComponent = EntityComponents[i];
                    EntityComponentData.Add(new Object[]
                    {
                        EntityComponent.Id,
                        EntityComponent.Code,
                        EntityComponent.Name,
                    });
                }
                excel.GenerateWorksheet("EntityComponent", EntityComponentHeaders, EntityComponentData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "CodeGeneratorRule.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
            CodeGeneratorRuleFilter = await CodeGeneratorRuleService.ToFilter(CodeGeneratorRuleFilter);
            if (Id == 0)
            {

            }
            else
            {
                CodeGeneratorRuleFilter.Id = new IdFilter { Equal = Id };
                int count = await CodeGeneratorRuleService.Count(CodeGeneratorRuleFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private CodeGeneratorRule ConvertDTOToEntity(CodeGeneratorRule_CodeGeneratorRuleDTO CodeGeneratorRule_CodeGeneratorRuleDTO)
        {
            CodeGeneratorRule CodeGeneratorRule = new CodeGeneratorRule();
            CodeGeneratorRule.Id = CodeGeneratorRule_CodeGeneratorRuleDTO.Id;
            CodeGeneratorRule.EntityTypeId = CodeGeneratorRule_CodeGeneratorRuleDTO.EntityTypeId;
            CodeGeneratorRule.AutoNumberLenth = CodeGeneratorRule_CodeGeneratorRuleDTO.AutoNumberLenth;
            CodeGeneratorRule.StatusId = CodeGeneratorRule_CodeGeneratorRuleDTO.StatusId;
            CodeGeneratorRule.RowId = CodeGeneratorRule_CodeGeneratorRuleDTO.RowId;
            CodeGeneratorRule.Used = CodeGeneratorRule_CodeGeneratorRuleDTO.Used;
            CodeGeneratorRule.EntityType = CodeGeneratorRule_CodeGeneratorRuleDTO.EntityType == null ? null : new EntityType
            {
                Id = CodeGeneratorRule_CodeGeneratorRuleDTO.EntityType.Id,
                Code = CodeGeneratorRule_CodeGeneratorRuleDTO.EntityType.Code,
                Name = CodeGeneratorRule_CodeGeneratorRuleDTO.EntityType.Name,
            };
            CodeGeneratorRule.Status = CodeGeneratorRule_CodeGeneratorRuleDTO.Status == null ? null : new Status
            {
                Id = CodeGeneratorRule_CodeGeneratorRuleDTO.Status.Id,
                Code = CodeGeneratorRule_CodeGeneratorRuleDTO.Status.Code,
                Name = CodeGeneratorRule_CodeGeneratorRuleDTO.Status.Name,
            };
            CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRule_CodeGeneratorRuleDTO.CodeGeneratorRuleEntityComponentMappings?
                .Select(x => new CodeGeneratorRuleEntityComponentMapping
                {
                    EntityComponentId = x.EntityComponentId,
                    Sequence = x.Sequence,
                    Value = x.Value,
                    EntityComponent = x.EntityComponent == null ? null : new EntityComponent
                    {
                        Id = x.EntityComponent.Id,
                        Code = x.EntityComponent.Code,
                        Name = x.EntityComponent.Name,
                    },
                }).ToList();
            CodeGeneratorRule.BaseLanguage = CurrentContext.Language;
            return CodeGeneratorRule;
        }

        private CodeGeneratorRuleFilter ConvertFilterDTOToFilterEntity(CodeGeneratorRule_CodeGeneratorRuleFilterDTO CodeGeneratorRule_CodeGeneratorRuleFilterDTO)
        {
            CodeGeneratorRuleFilter CodeGeneratorRuleFilter = new CodeGeneratorRuleFilter();
            CodeGeneratorRuleFilter.Selects = CodeGeneratorRuleSelect.ALL;
            CodeGeneratorRuleFilter.Skip = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.Skip;
            CodeGeneratorRuleFilter.Take = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.Take;
            CodeGeneratorRuleFilter.OrderBy = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.OrderBy;
            CodeGeneratorRuleFilter.OrderType = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.OrderType;

            CodeGeneratorRuleFilter.Id = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.Id;
            CodeGeneratorRuleFilter.EntityTypeId = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.EntityTypeId;
            CodeGeneratorRuleFilter.EntityComponentId = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.EntityComponentId;
            CodeGeneratorRuleFilter.AutoNumberLenth = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.AutoNumberLenth;
            CodeGeneratorRuleFilter.StatusId = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.StatusId;
            CodeGeneratorRuleFilter.RowId = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.RowId;
            CodeGeneratorRuleFilter.CreatedAt = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.CreatedAt;
            CodeGeneratorRuleFilter.UpdatedAt = CodeGeneratorRule_CodeGeneratorRuleFilterDTO.UpdatedAt;
            return CodeGeneratorRuleFilter;
        }
    }
}

