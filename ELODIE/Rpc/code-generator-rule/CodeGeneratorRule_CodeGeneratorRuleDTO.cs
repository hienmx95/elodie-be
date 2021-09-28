using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.code_generator_rule
{
    public class CodeGeneratorRule_CodeGeneratorRuleDTO : DataDTO
    {
        public long Id { get; set; }
        public long EntityTypeId { get; set; }
        public long AutoNumberLenth { get; set; }
        public long StatusId { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public CodeGeneratorRule_EntityTypeDTO EntityType { get; set; }
        public CodeGeneratorRule_StatusDTO Status { get; set; }
        public List<CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingDTO> CodeGeneratorRuleEntityComponentMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CodeGeneratorRule_CodeGeneratorRuleDTO() {}
        public CodeGeneratorRule_CodeGeneratorRuleDTO(CodeGeneratorRule CodeGeneratorRule)
        {
            this.Id = CodeGeneratorRule.Id;
            this.EntityTypeId = CodeGeneratorRule.EntityTypeId;
            this.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
            this.StatusId = CodeGeneratorRule.StatusId;
            this.RowId = CodeGeneratorRule.RowId;
            this.Used = CodeGeneratorRule.Used;
            this.EntityType = CodeGeneratorRule.EntityType == null ? null : new CodeGeneratorRule_EntityTypeDTO(CodeGeneratorRule.EntityType);
            this.Status = CodeGeneratorRule.Status == null ? null : new CodeGeneratorRule_StatusDTO(CodeGeneratorRule.Status);
            this.CodeGeneratorRuleEntityComponentMappings = CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings?.Select(x => new CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingDTO(x)).ToList();
            this.CreatedAt = CodeGeneratorRule.CreatedAt;
            this.UpdatedAt = CodeGeneratorRule.UpdatedAt;
            this.Errors = CodeGeneratorRule.Errors;
        }
    }

    public class CodeGeneratorRule_CodeGeneratorRuleFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter EntityTypeId { get; set; }
        public IdFilter EntityComponentId { get; set; }
        public LongFilter AutoNumberLenth { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public CodeGeneratorRuleOrder OrderBy { get; set; }
    }
}
