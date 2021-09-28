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
    public class UnitOfMeasureGroupingHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(UnitOfMeasureGrouping);

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
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = JsonConvert.DeserializeObject<List<UnitOfMeasureGrouping>>(json);
            List<long> Ids = UnitOfMeasureGroupings.Select(a => a.Id).Distinct().ToList();
            try
            {
                await UOW.UnitOfMeasureGroupingRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(UnitOfMeasureGroupingHandler));
            }
        }
    }
}
