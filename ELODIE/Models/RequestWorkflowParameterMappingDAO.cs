﻿using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class RequestWorkflowParameterMappingDAO
    {
        public long WorkflowParameterId { get; set; }
        public Guid RequestId { get; set; }
        public string Value { get; set; }

        public virtual WorkflowParameterDAO WorkflowParameter { get; set; }
    }
}
