using ELODIE.Common;
using Microsoft.AspNetCore.Mvc;
using ELODIE.Services.MAppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ELODIE.Services.MSex;
using ELODIE.Enums;
using ELODIE.Services.MSite;
using System.IO;

namespace ELODIE.Rpc.app_user
{
    public class ProfileRoot
    {
        public const string Login = "rpc/elodie/account/login";
        public const string Logged = "rpc/elodie/account/logged";
        public const string GetForWeb = "rpc/elodie/profile-web/get";
        public const string Get = "rpc/elodie/profile/get";
        public const string GetDraft = "rpc/elodie/profile/get-draft";
        public const string Update = "rpc/elodie/profile/update";
        public const string SaveImage = "rpc/elodie/profile/save-image";
        public const string ChangePassword = "rpc/elodie/profile/change-password";
        public const string ForgotPassword = "rpc/elodie/profile/forgot-password";
        public const string VerifyOtpCode = "rpc/elodie/profile/verify-otp-code";
        public const string RecoveryPassword = "rpc/elodie/profile/recovery-password";
        public const string SingleListSex = "rpc/elodie/profile/single-list-sex";
        public const string SingleListProvince = "rpc/elodie/profile/single-list-province";
        public const string ListSite = "rpc/elodie/profile/list-site";
    }
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private IAppUserService AppUserService;
        private ISexService SexService;
        private ISiteService SiteService;
        private ICurrentContext CurrentContext;
        public ProfileController(
            IAppUserService AppUserService,
            ISexService SexService,
            ISiteService SiteService,
            ICurrentContext CurrentContext
            )
        {
            this.AppUserService = AppUserService;
            this.SexService = SexService;
            this.SiteService = SiteService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(ProfileRoot.Login), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Login([FromBody] AppUser_LoginDTO AppUser_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Username = AppUser_LoginDTO.Username,
                Password = AppUser_LoginDTO.Password,
                BaseLanguage = "vi",
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.Login(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);

            if (AppUser.IsValidated)
            {
                Response.Cookies.Append("Token", AppUser.Token);
                AppUser_AppUserDTO.Token = AppUser.Token;
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.Logged), HttpPost]
        public bool Logged()
        {
            return true;
        }
        [Route(ProfileRoot.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ProfileChangePasswordDTO AppUser_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            this.CurrentContext.UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Password = AppUser_ProfileChangePasswordDTO.OldPassword,
                NewPassword = AppUser_ProfileChangePasswordDTO.NewPassword,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.ChangePassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(ProfileRoot.ForgotPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ForgotPassword([FromBody] AppUser_ForgotPassword AppUser_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_ForgotPassword.Email,
            };
            AppUser.BaseLanguage = CurrentContext.Language;

            AppUser = await AppUserService.ForgotPassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [AllowAnonymous]
        [Route(ProfileRoot.VerifyOtpCode), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> VerifyCode([FromBody] AppUser_VerifyOtpDTO AppUser_VerifyOtpDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_VerifyOtpDTO.Email,
                OtpCode = AppUser_VerifyOtpDTO.OtpCode,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.VerifyOtpCode(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                HttpContext.Response.Cookies.Append("Token", AppUser.Token);
                return AppUser_AppUserDTO;
            }

            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.RecoveryPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPassword([FromBody] AppUser_RecoveryPassword AppUser_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = UserId,
                Password = AppUser_RecoveryPassword.Password,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.RecoveryPassword(AppUser);
            if (AppUser == null)
                return Unauthorized();
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }
        #endregion

        [Route(ProfileRoot.SaveImage), HttpPost]
        public async Task<ActionResult<string>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            CurrentContext.Token = Request.Cookies["Token"];
            string str = await AppUserService.SaveImage(Image);
            return str;
        }

        [Route(ProfileRoot.GetForWeb), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetForWeb()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }

        [Route(ProfileRoot.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetMe()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser_AppUserDTO.AppUserSiteMappings != null)
                foreach (var AppUserSiteMapping in AppUser_AppUserDTO.AppUserSiteMappings)
                {
                    if (AppUserSiteMapping.Site != null)
                    {
                        AppUserSiteMapping.Site.Icon = "";
                        AppUserSiteMapping.Site.Logo = "";
                    }
                }
            return AppUser_AppUserDTO;
        }

        [Route(ProfileRoot.GetDraft), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetDraft()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);

            List<Site> Sites = await SiteService.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SiteSelect.ALL
            });
            AppUser.AppUserSiteMappings = new List<AppUserSiteMapping>();
            foreach (var Site in Sites)
            {
                AppUserSiteMapping AppUserSiteMapping = new AppUserSiteMapping
                {
                    AppUserId = AppUser.Id,
                    SiteId = Site.Id,
                    Enabled = true,
                    Site = new Site
                    {
                        Id = Site.Id,
                        Code = Site.Code,
                        Name = Site.Name,
                    }
                };
                AppUser.AppUserSiteMappings.Add(AppUserSiteMapping);
            }
            return new AppUser_AppUserDTO(AppUser);
        }

        [Route(ProfileRoot.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateMe([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            this.CurrentContext.UserId = ExtractUserId();
            AppUser OldData = await AppUserService.Get(this.CurrentContext.UserId);
            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser.Id = CurrentContext.UserId;
            AppUser.AppUserSiteMappings = OldData.AppUserSiteMappings;
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoot.SingleListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> SingleListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
        {
            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = AppUser_SexFilterDTO.Id;
            SexFilter.Code = AppUser_SexFilterDTO.Code;
            SexFilter.Name = AppUser_SexFilterDTO.Name;
            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser_SexDTO> AppUser_SexDTOs = Sexes
                .Select(x => new AppUser_SexDTO(x)).ToList();
            return AppUser_SexDTOs;
        }

        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }
        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.Address = AppUser_AppUserDTO.Address;
            AppUser.Avatar = AppUser_AppUserDTO.Avatar;
            AppUser.Birthday = AppUser_AppUserDTO.Birthday;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.Department = AppUser_AppUserDTO.Department;
            AppUser.OrganizationId = AppUser_AppUserDTO.OrganizationId;
            AppUser.SexId = AppUser_AppUserDTO.SexId;
            AppUser.StatusId = AppUser_AppUserDTO.StatusId;
            AppUser.Organization = AppUser_AppUserDTO.Organization == null ? null : new Organization
            {
                Id = AppUser_AppUserDTO.Organization.Id,
                Code = AppUser_AppUserDTO.Organization.Code,
                Name = AppUser_AppUserDTO.Organization.Name,
                ParentId = AppUser_AppUserDTO.Organization.ParentId,
                Path = AppUser_AppUserDTO.Organization.Path,
                Level = AppUser_AppUserDTO.Organization.Level,
                StatusId = AppUser_AppUserDTO.Organization.StatusId,
                Phone = AppUser_AppUserDTO.Organization.Phone,
                Address = AppUser_AppUserDTO.Organization.Address,
                Email = AppUser_AppUserDTO.Organization.Email,
            };
            AppUser.Sex = AppUser_AppUserDTO.Sex == null ? null : new Sex
            {
                Id = AppUser_AppUserDTO.Sex.Id,
                Code = AppUser_AppUserDTO.Sex.Code,
                Name = AppUser_AppUserDTO.Sex.Name,
            };
            AppUser.Status = AppUser_AppUserDTO.Status == null ? null : new Status
            {
                Id = AppUser_AppUserDTO.Status.Id,
                Code = AppUser_AppUserDTO.Status.Code,
                Name = AppUser_AppUserDTO.Status.Name,
            };
            AppUser.AppUserRoleMappings = AppUser_AppUserDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    RoleId = x.RoleId,
                    Role = x.Role == null ? null : new Role
                    {
                        Id = x.Role.Id,
                        Code = x.Role.Code,
                        Name = x.Role.Name,
                        StatusId = x.Role.StatusId,
                    },
                }).ToList();
            AppUser.AppUserSiteMappings = AppUser_AppUserDTO.AppUserSiteMappings?
                .Select(x => new AppUserSiteMapping
                {
                    AppUserId = x.AppUserId,
                    SiteId = x.SiteId,
                    Enabled = x.Enabled
                }).ToList();
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        [Route(ProfileRoot.ListSite), HttpPost]
        public async Task<List<AppUser_SiteDTO>> ListSite()
        {
            SiteFilter SiteFilter = new SiteFilter();
            SiteFilter.Skip = 0;
            SiteFilter.Take = int.MaxValue;
            SiteFilter.OrderBy = SiteOrder.Id;
            SiteFilter.OrderType = OrderType.ASC;
            SiteFilter.Selects = SiteSelect.ALL;
            SiteFilter.IsDisplay = true;

            List<Site> Sites = await SiteService.List(SiteFilter);
            List<AppUser_SiteDTO> AppUser_SiteDTOs = Sites
                .Select(x => new AppUser_SiteDTO(x)).ToList();
            return AppUser_SiteDTOs;
        }
    }
}
