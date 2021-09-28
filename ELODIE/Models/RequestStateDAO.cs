using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class RequestStateDAO
    {
        public RequestStateDAO()
        {
            RequestWorkflowDefinitionMappings = new HashSet<RequestWorkflowDefinitionMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RequestWorkflowDefinitionMappingDAO> RequestWorkflowDefinitionMappings { get; set; }
    }
}
