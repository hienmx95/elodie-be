using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Helpers;
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
    public class AppUserHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(AppUser);

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
                List<AppUser> AppUsers = JsonConvert.DeserializeObject<List<AppUser>>(json);
                await UOW.AppUserRepository.BulkMerge(AppUsers);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(AppUserHandler));
            }
        }
    }
}
