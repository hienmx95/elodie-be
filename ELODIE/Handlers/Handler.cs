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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace ELODIE.Handlers
{
    public interface IHandler
    {
        string Name { get; }
        IRabbitManager RabbitManager { get; set; }
        void QueueBind(IModel channel, string queue, string exchange);
        Task Handle(IUOW UOW, string routingKey, string content);
    }

    public abstract class Handler : IHandler
    {
        public abstract string Name { get; }
        public IRabbitManager RabbitManager { get; set; }

        public abstract Task Handle(IUOW UOW, string routingKey, string content);

        public abstract void QueueBind(IModel channel, string queue, string exchange);

       
        protected void Log(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = null,
                AppUser = "RABBITMQ",
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            RabbitManager.PublishSingle(SystemLog, RoutingKeyEnum.SystemLogSend);
        }
    }
}
