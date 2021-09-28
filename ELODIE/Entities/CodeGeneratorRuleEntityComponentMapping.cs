using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class CodeGeneratorRuleEntityComponentMapping : DataEntity,  IEquatable<CodeGeneratorRuleEntityComponentMapping>
    {
        public long CodeGeneratorRuleId { get; set; }
        public long EntityComponentId { get; set; }
        public long Sequence { get; set; }
        public string Value { get; set; }
        public CodeGeneratorRule CodeGeneratorRule { get; set; }
        public EntityComponent EntityComponent { get; set; }
        
        public bool Equals(CodeGeneratorRuleEntityComponentMapping other)
        {
            return other != null &&
               EntityComponentId == other.EntityComponentId &&
               Sequence == other.Sequence &&
               Value == other.Value;
        }
        public override int GetHashCode()
        {
            int HashCode = 0;
            if (Value == null)
            {
                HashCode = EntityComponentId.GetHashCode() ^ Sequence.GetHashCode();
            }
            else
            {
                HashCode = EntityComponentId.GetHashCode() ^ Sequence.GetHashCode() ^ Value.GetHashCode();
            }    
            return HashCode;
        }
    }

    public class CodeGeneratorRuleEntityComponentMappingFilter : FilterEntity
    {
        public IdFilter CodeGeneratorRuleId { get; set; }
        public IdFilter EntityComponentId { get; set; }
        public LongFilter Sequence { get; set; }
        public List<CodeGeneratorRuleEntityComponentMappingFilter> OrFilter { get; set; }
        public CodeGeneratorRuleEntityComponentMappingOrder OrderBy {get; set;}
        public CodeGeneratorRuleEntityComponentMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CodeGeneratorRuleEntityComponentMappingOrder
    {
        CodeGeneratorRule = 0,
        EntityComponent = 1,
        Sequence = 2,
    }

    [Flags]
    public enum CodeGeneratorRuleEntityComponentMappingSelect:long
    {
        ALL = E.ALL,
        CodeGeneratorRule = E._0,
        EntityComponent = E._1,
        Sequence = E._2,
    }
}
