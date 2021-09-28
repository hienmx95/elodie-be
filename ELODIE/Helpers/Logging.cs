using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Handlers;
using ELODIE.Repositories;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ELODIE.Helpers
{
    public interface ILogging : IServiceScoped
    {
        Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "");
        Task CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "");
    }
    public class Logging : ILogging
    {
        private ICurrentContext CurrentContext;
        //private IRabbitManager RabbitManager;
        private IUOW UOW;
        public Logging(
            ICurrentContext CurrentContext,
            //IRabbitManager RabbitManager,
            IUOW UOW)
        {
            this.CurrentContext = CurrentContext;
            //this.RabbitManager = RabbitManager;
            this.UOW = UOW;
        }
        public async Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "")
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            AuditLog AuditLog = new AuditLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = AppUser.DisplayName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                OldData = JsonConvert.SerializeObject(oldData),
                NewData = JsonConvert.SerializeObject(newData),
                Time = StaticParams.DateTimeNow,
                RowId = Guid.NewGuid(),
            };
            //RabbitManager.PublishSingle(AuditLog, RoutingKeyEnum.AuditLogSend);
            return true;
        }
        public async Task CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = AppUser.DisplayName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };
            //RabbitManager.PublishSingle(SystemLog, RoutingKeyEnum.SystemLogSend);
            throw new MessageException(ex);
        }
    }
}
