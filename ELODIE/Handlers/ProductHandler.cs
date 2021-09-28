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
    public class ProductHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(Product);

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
            List<Product> Products = JsonConvert.DeserializeObject<List<Product>>(json);
            List<long> Ids = Products.Select(a => a.Id).Distinct().ToList();
            try
            {
                await UOW.ProductRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(ProductHandler));
            }
        }
    }
}
