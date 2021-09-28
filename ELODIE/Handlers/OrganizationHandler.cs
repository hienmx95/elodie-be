using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Models;
using ELODIE.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Handlers
{
    public class OrganizationHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(Organization);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<Organization> Organizations = JsonConvert.DeserializeObject<List<Organization>>(json);
                await UOW.OrganizationRepository.BulkMerge(Organizations);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(OrganizationHandler));
            }
        }
    }
}
