using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CodeGeneratedDAO
    {
        public long Id { get; set; }
        public long CodeGeneratorRuleId { get; set; }
        public long Code { get; set; }
        public bool Used { get; set; }

        public virtual CodeGeneratorRuleDAO CodeGeneratorRule { get; set; }
    }
}
