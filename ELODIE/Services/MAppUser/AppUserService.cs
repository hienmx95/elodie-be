using ELODIE.Common;
using ELODIE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ELODIE.Repositories;
using ELODIE.Entities;
using ELODIE.Handlers;
using ELODIE.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using RestSharp;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace ELODIE.Services.MAppUser
{
    public interface IAppUserService : IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Login(AppUser AppUser);
        Task<AppUser> ChangePassword(AppUser AppUser);
        Task<AppUser> ForgotPassword(AppUser AppUser);
        Task<AppUser> VerifyOtpCode(AppUser AppUser);
        Task<AppUser> RecoveryPassword(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<AppUser> AdminChangePassword(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(List<AppUser> AppUsers);
        Task<string> SaveImage(Image Image);
        AppUserFilter ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        //private IRabbitManager RabbitManager;
        private IConfiguration Configuration;

        public AppUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            //IRabbitManager RabbitManager,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
            //this.RabbitManager = RabbitManager;
            this.Configuration = Configuration;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            List<Site> Sites = await UOW.SiteRepository.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                IsDisplay = true,
                Selects = SiteSelect.ALL,
            });
            if (AppUser.AppUserSiteMappings == null)
                AppUser.AppUserSiteMappings = new List<AppUserSiteMapping>();
            foreach (Site Site in Sites)
            {
                AppUserSiteMapping AppUserSiteMapping = AppUser.AppUserSiteMappings.Where(s => s.SiteId == Site.Id).FirstOrDefault();
                if (AppUserSiteMapping == null)
                {
                    AppUserSiteMapping = new AppUserSiteMapping();
                    AppUserSiteMapping.SiteId = Site.Id;
                    AppUserSiteMapping.Site = Site;
                    AppUserSiteMapping.Enabled = false;
                    AppUser.AppUserSiteMappings.Add(AppUserSiteMapping);
                }
                else
                {
                    AppUserSiteMapping.Site = Site;
                    AppUserSiteMapping.AppUserId = AppUser.Id;
                }
            }
            {
                AppUserSiteMapping AppUserSiteMapping = AppUser.AppUserSiteMappings.Where(x => x.SiteId == SiteEnum.LANDING.Id).FirstOrDefault();
                if (AppUserSiteMapping != null)
                    AppUserSiteMapping.Enabled = true;
            }
            return AppUser;
        }

        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                var Password = GeneratePassword();
                AppUser.Password = HashPassword(Password);

                await UOW.AppUserRepository.Create(AppUser);

                AppUser = await Get(AppUser.Id);
                await Sync(new List<AppUser> { AppUser });

                Mail mail = new Mail
                {
                    Subject = "Create AppUser",
                    Body = $"Your account has been created at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")} Username: {AppUser.Username} Password: {Password}",
                    Recipients = new List<string> { AppUser.Email },
                    RowId = Guid.NewGuid()
                };
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                AppUser.Password = oldData.Password;

                await UOW.AppUserRepository.Update(AppUser);


                AppUser = await Get(AppUser.Id);
                await Sync(new List<AppUser> { AppUser });
                await Logging.CreateAuditLog(AppUser, oldData, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<string> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/avatar/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            RestClient restClient = new RestClient($"{InternalServices.UTILS}");
            RestRequest restRequest = new RestRequest("/rpc/utils/file/upload");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddFile("file", Image.Content, Image.Name);
            restRequest.AddParameter("path", path);
            try
            {
                var response = await restClient.ExecuteAsync<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image.Id = response.Data.Id;
                    Image.Url = "/rpc/utils/file/download" + response.Data.Path;
                    return Image.Url;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public async Task<AppUser> Login(AppUser AppUser)
        {
            if (!await AppUserValidator.Login(AppUser))
                return AppUser;
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            CurrentContext.UserId = AppUser.Id;
            //await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username, AppUser.RowId);

            return AppUser;
        }
        public async Task<AppUser> ChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ChangePassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);

                Mail mail = new Mail
                {
                    Subject = "Change Password AppUser",
                    Body = $"Your password has been changed at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")}",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> ForgotPassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ForgotPassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    OrganizationId = new IdFilter { },
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;

                oldData.OtpCode = GenerateOTPCode();
                oldData.OtpExpired = StaticParams.DateTimeNow.AddHours(1);


                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Otp Code",
                    Body = $"Otp Code recovery password: {newData.OtpCode}",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> RecoveryPassword(AppUser AppUser)
        {
            if (AppUser.Id == 0)
                return null;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                oldData.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Recovery Password",
                    Body = $"Your password has been recovered.",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> AdminChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.AdminChangePassword(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);


                Mail mail = new Mail
                {
                    Subject = "Change Password AppUser",
                    Body = $"Your new password is {AppUser.NewPassword}",
                    Recipients = new List<string> { AppUser.Email },
                    RowId = Guid.NewGuid()
                };
                //RabbitManager.PublishSingle(mail, RoutingKeyEnum.MailSend);
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                AppUser = await Get(AppUser.Id);
                await Sync(new List<AppUser> { AppUser });
                await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {

                await UOW.AppUserRepository.BulkDelete(AppUsers);

                await Logging.CreateAuditLog(new { }, AppUsers, nameof(AppUserService));
                var Ids = AppUsers.Select(x => x.Id).ToList();
                AppUsers = await UOW.AppUserRepository.List(Ids);
                await Sync(AppUsers);
                return AppUsers;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> VerifyOtpCode(AppUser AppUser)
        {
            if (!await AppUserValidator.VerifyOptCode(AppUser))
                return AppUser;
            AppUser appUser = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                OrganizationId = new IdFilter { },
                Email = new StringFilter { Equal = AppUser.Email },
                Selects = AppUserSelect.ALL
            })).FirstOrDefault();
            appUser.Token = CreateToken(appUser.Id, appUser.Username, appUser.RowId, 300);
            return appUser;
        }
        public async Task<List<AppUser>> Import(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                var listAppUserInDB = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.ALL
                }));

                List<Mail> Mails = new List<Mail>();
                foreach (var AppUser in AppUsers)
                {
                    var appUser = listAppUserInDB.Where(a => a.Username == AppUser.Username).FirstOrDefault();
                    if (appUser != null)
                    {
                        AppUser.Id = appUser.Id;
                        AppUser.RowId = appUser.RowId;
                    }
                    else
                    {
                        var Password = GeneratePassword();
                        AppUser.Id = 0;
                        AppUser.Password = HashPassword(Password);
                        AppUser.RowId = Guid.NewGuid();

                        Mail mail = new Mail
                        {
                            Subject = "Create AppUser",
                            Body = $"Your account has been created at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")} Username: {AppUser.Username} Password: {Password}",
                            Recipients = new List<string> { AppUser.Email },
                            RowId = Guid.NewGuid()
                        };
                        Mails.Add(mail);
                    }
                }
                await UOW.AppUserRepository.BulkMerge(AppUsers);

                var Ids = AppUsers.Select(x => x.Id).ToList();
                AppUsers = await UOW.AppUserRepository.List(Ids);
                await Sync(AppUsers);

                //RabbitManager.PublishList(Mails, RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(AppUsers, new { }, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public AppUserFilter ToFilter(AppUserFilter filter)
        {

            return filter;
        }

        private string CreateToken(long id, string userName, Guid rowId, double? expiredTime = null)
        {
            var secretKey = Configuration["Config:SecretKey"];
            if (expiredTime == null)
                expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.PrimarySid, rowId.ToString()),
                }),
                Expires = StaticParams.DateTimeNow.AddSeconds(expiredTime.Value),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(SecurityToken);
            return Token;
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        private string GeneratePassword()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            Random _rand = new Random();
            var bytes = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            var res = new StringBuilder();
            foreach (byte b in bytes)
            {
                switch (_rand.Next(4))
                {
                    case 0:
                        res.Append(lower[b % lower.Count()]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Count()]);
                        break;
                    case 2:
                        res.Append(number[b % number.Count()]);
                        break;
                    case 3:
                        res.Append(special[b % special.Count()]);
                        break;
                }
            }
            return res.ToString();
        }

        private string GenerateOTPCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

        private async Task Sync(List<AppUser> AppUsers)
        {
            List<Organization> Organizations = AppUsers.Select(x => new Organization { Id = x.OrganizationId }).Distinct().ToList();

            //RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserSync);
            //RabbitManager.PublishList(Organizations, RoutingKeyEnum.OrganizationUsed);
        }
    }

    public class File
    {
        public long Id { get; set; }
        public string Path { get; set; }
    }
}
