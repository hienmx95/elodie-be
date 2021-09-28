using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using ELODIE.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Handlers
{
    public class WorkflowDefinitionHandler : Handler
    {
        private string UsedKey => $"ELODIE.{Name}.Used";
        public override string Name => nameof(WorkflowDefinition);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            List<WorkflowDefinition> WorkflowDefinitions = JsonConvert.DeserializeObject<List<WorkflowDefinition>>(json);
            List<long> Ids = WorkflowDefinitions.Select(wf => wf.Id).ToList();
            try
            {
                await UOW.WorkflowDefinitionRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(WorkflowDefinitionHandler));
            }
        }
    }
}
