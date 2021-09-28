using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Helpers;
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
    public class ItemHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(Item);

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
            try
            {
                List<Item> Items = JsonConvert.DeserializeObject<List<Item>>(json);
                List<long> Ids = Items.Select(a => a.Id).Distinct().ToList();
                await UOW.ItemRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(ItemHandler));
            }
        }
    }
}
