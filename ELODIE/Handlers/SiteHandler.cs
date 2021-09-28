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
    public class SiteHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Site);

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
            List<Site> Sites = JsonConvert.DeserializeObject<List<Site>>(json);
            try
            {
                await UOW.SiteRepository.BulkMerge(Sites);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(SiteHandler));
            }
        }
    }
}
