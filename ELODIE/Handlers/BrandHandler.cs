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
    public class BrandHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(Brand);

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
                List<Brand> Brands = JsonConvert.DeserializeObject<List<Brand>>(json);
                List<long> Ids = Brands.Select(x => x.Id).Distinct().ToList();
                await UOW.BrandRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(BrandHandler));
            }
        }
    }
}
