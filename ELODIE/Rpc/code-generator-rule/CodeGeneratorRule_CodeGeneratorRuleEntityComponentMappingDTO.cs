using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.code_generator_rule
{
    public class CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingDTO : DataDTO
    {
        public long CodeGeneratorRuleId { get; set; }
        public long EntityComponentId { get; set; }
        public long Sequence { get; set; }
        public string Value { get; set; }
        public CodeGeneratorRule_EntityComponentDTO EntityComponent { get; set; }   

        public CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingDTO() {}
        public CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingDTO(CodeGeneratorRuleEntityComponentMapping CodeGeneratorRuleEntityComponentMapping)
        {
            this.CodeGeneratorRuleId = CodeGeneratorRuleEntityComponentMapping.CodeGeneratorRuleId;
            this.EntityComponentId = CodeGeneratorRuleEntityComponentMapping.EntityComponentId;
            this.Sequence = CodeGeneratorRuleEntityComponentMapping.Sequence;
            this.Value = CodeGeneratorRuleEntityComponentMapping.Value;
            this.EntityComponent = CodeGeneratorRuleEntityComponentMapping.EntityComponent == null ? null : new CodeGeneratorRule_EntityComponentDTO(CodeGeneratorRuleEntityComponentMapping.EntityComponent);
            this.Errors = CodeGeneratorRuleEntityComponentMapping.Errors;
        }
    }

    public class CodeGeneratorRule_CodeGeneratorRuleEntityComponentMappingFilterDTO : FilterDTO
    {
        
        public IdFilter CodeGeneratorRuleId { get; set; }
        
        public IdFilter EntityComponentId { get; set; }
        
        public LongFilter Sequence { get; set; }
        
        public CodeGeneratorRuleEntityComponentMappingOrder OrderBy { get; set; }
    }
}