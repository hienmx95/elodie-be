using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
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
    public class RoleHandler : Handler
    {
        private string UsedKey => $"ELODIE.{Name}.Used";
        public override string Name => nameof(Role);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"ELODIE.{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            List<Role> Roles = JsonConvert.DeserializeObject<List<Role>>(json);
            List<long> Ids = Roles.Select(r => r.Id).ToList();
            try
            {
                await UOW.RoleRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(ProvinceHandler));
            }
        }
    }
}
