using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELODIE.Entities;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MSex;
using ELODIE.Services.MStatus;
using ELODIE.Services.MRole;
using OfficeOpenXml;
using ELODIE.Enums;
using System.IO;
using ELODIE.Services.MSite;
using System.Text;

namespace ELODIE.Rpc.app_user
{
    public class AppUserController : RpcController
    {
        private IOrganizationService OrganizationService;
        private ISexService SexService;
        private IStatusService StatusService;
        private IRoleService RoleService;
        private IAppUserService AppUserService;
        private ISiteService SiteService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IOrganizationService OrganizationService,
            ISexService SexService,
            IStatusService StatusService,
            IRoleService RoleService,
            IAppUserService AppUserService,
            ISiteService SiteService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.SexService = SexService;
            this.StatusService = StatusService;
            this.RoleService = RoleService;
            this.AppUserService = AppUserService;
            this.SiteService = SiteService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            return new AppUser_AppUserDTO(AppUser);
        }

        [Route(AppUserRoute.SaveImage), HttpPost]
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
            return await AppUserService.SaveImage(Image);
        }

        [Route(AppUserRoute.Create), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Create([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Update([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ChangePasswordDTO AppUser_ChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Id = AppUser_ChangePasswordDTO.Id,
                NewPassword = AppUser_ChangePasswordDTO.NewPassword,
            };
            AppUser = await AppUserService.AdminChangePassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Delete), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Delete([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Delete(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Import), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            #region MDM
            List<Sex> Sexes = await SexService.List(new SexFilter
            {
                Skip = 0,
                Take = 10,
                Selects = SexSelect.ALL
            });
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Code | OrganizationSelect.Name |
                OrganizationSelect.Path,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC,
            });
            Organization Root = Organizations.FirstOrDefault();
            List<Site> Sites = await SiteService.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SiteSelect.Id | SiteSelect.Code | SiteSelect.Name
            });
            List<Status> Statuses = await StatusService.List(new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.Id | StatusSelect.Code | StatusSelect.Name
            });
            #endregion
            List<AppUser> AppUsers = new List<AppUser>();
            StringBuilder errorContent = new StringBuilder();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Tài khoản"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StartColumn = 1;
                int StartRow = 2;

                int SttColumnn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int DisplayNameColumn = 2 + StartColumn;
                int AddressColumn = 3 + StartColumn;
                int PhoneColumn = 4 + StartColumn;
                int EmailColumn = 5 + StartColumn;
                int SexColumn = 6 + StartColumn;
                int BirthColumn = 7 + StartColumn;
                int OrganizationCodeColumn = 8 + StartColumn;
                int SiteCodeColumn = 9 + StartColumn;
                int StatusNameColumn = 10 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string stt = worksheet.Cells[i, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(UsernameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập mã nhân viên");
                    }
                    else if (string.IsNullOrWhiteSpace(UsernameValue) && i == worksheet.Dimension.End.Row)
                        break;
                    string DisplayNameValue = worksheet.Cells[i, DisplayNameColumn].Value?.ToString();
                    string AddressNameValue = worksheet.Cells[i, AddressColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i, PhoneColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i, EmailColumn].Value?.ToString();
                    string SexValue = worksheet.Cells[i, SexColumn].Value?.ToString();
                    string BirthValue = worksheet.Cells[i, BirthColumn].Value?.ToString();
                    string OrganizationCodeValue = worksheet.Cells[i, OrganizationCodeColumn].Value?.ToString();
                    string SiteCodeValue = worksheet.Cells[i, SiteCodeColumn].Value?.ToString();
                    string StatusNameValue = worksheet.Cells[i, StatusNameColumn].Value?.ToString();

                    AppUser AppUser = new AppUser();

                    AppUser.Username = UsernameValue;
                    AppUser.DisplayName = DisplayNameValue;
                    AppUser.Address = AddressNameValue;
                    AppUser.Phone = PhoneValue;
                    AppUser.Email = EmailValue;
                    Sex sex = Sexes.Where(s => s.Code == SexValue).FirstOrDefault();
                    AppUser.SexId = sex == null ? 0 : sex.Id;
                    AppUser.Sex = sex;
                   
                    if (DateTime.TryParse(BirthValue, out DateTime Birthday))
                    {
                        AppUser.Birthday = Birthday;
                    }

                    if (string.IsNullOrWhiteSpace(OrganizationCodeValue))
                    {
                        AppUser.OrganizationId = Root.Id;
                    }
                    else
                    {
                        Organization Organization = Organizations.Where(x => x.Code.Equals(OrganizationCodeValue)).FirstOrDefault();
                        AppUser.OrganizationId = 0;
                        if (Organization != null)
                            AppUser.OrganizationId = Organization.Id;
                    }

                    if (!string.IsNullOrWhiteSpace(SiteCodeValue))
                    {
                        var siteCodes = SiteCodeValue.Split(';');
                        foreach (var code in siteCodes)
                        {
                            var Site = Sites.Where(x => x.Code.ToLower() == code.ToLower()).FirstOrDefault();
                            if (Site == null)
                            {
                                errorContent.AppendLine($"Lỗi dòng thứ {i}: Mã phân hệ không tồn tại");
                            }
                            else
                            {
                                AppUserSiteMapping AppUserSiteMapping = new AppUserSiteMapping
                                {
                                    SiteId = Site.Id
                                };
                                if (AppUser.AppUserSiteMappings == null)
                                {
                                    AppUser.AppUserSiteMappings = Sites.Select(x => new AppUserSiteMapping
                                    {
                                        SiteId = x.Id,
                                        Enabled = false
                                    }).ToList();
                                }
                                var appUserSiteMapping = AppUser.AppUserSiteMappings.Where(x => x.SiteId == Site.Id).FirstOrDefault();
                                appUserSiteMapping.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập mã phân hệ");
                    }

                    Status Status = Statuses.Where(s => s.Name.ToLower() == StatusNameValue.ToLower()).FirstOrDefault();
                    if (Status != null)
                        AppUser.StatusId = Status.Id;
                    AppUser.BaseLanguage = CurrentContext.Language;
                    AppUsers.Add(AppUser);
                }
                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }
            AppUsers = await AppUserService.Import(AppUsers);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(a => new AppUser_AppUserDTO(a)).ToList();
            for (int i = 0; i < AppUsers.Count; i++)
            {
                if (!AppUsers[i].IsValidated)
                {
                    errorContent.Append($"Lỗi dòng thứ {i + 2}:");
                    foreach (var Error in AppUsers[i].Errors)
                    {
                        errorContent.Append($" {Error.Value},");
                    }
                    errorContent.AppendLine("");
                }
            }
            if (AppUsers.Any(s => !s.IsValidated))
                return BadRequest(errorContent.ToString());
            return Ok(AppUser_AppUserDTOs);
        }

        [Route(AppUserRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name
            });
            List<Sex> Sexes = await SexService.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.ALL
            });
            List<Site> Sites = await SiteService.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SiteSelect.Code | SiteSelect.Name
            });
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Appuser sheet
                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { "STT", "Tên đăng nhập","Tên hiển thị","Địa chỉ","Điện thoại","Email","Giới tính","Ngày sinh","Đơn vị quản lý", "Phân hệ", "Trạng thái"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var appUser = AppUsers[i];
                    var site = appUser.AppUserSiteMappings == null ? "" : string.Join(';', appUser.AppUserSiteMappings.Where(x => x.Enabled == true).Select(x => x.Site?.Name).ToList());
                    data.Add(new Object[]
                    {
                        i+1,
                        appUser.Username,
                        appUser.DisplayName,
                        appUser.Address,
                        appUser.Phone,
                        appUser.Email,
                        appUser.Sex.Name,
                        appUser.Birthday?.ToString("dd-MM-yyyy"),
                        appUser.Organization.Code,
                        site,
                        appUser.Status.Name,
                    });
                }
                excel.GenerateWorksheet("Tài khoản", AppUserHeaders, data);
                #endregion

                #region Org sheet
                data.Clear();
                var OrganizationHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Organization in Organizations)
                {
                    data.Add(new object[]
                    {
                        Organization.Code,
                        Organization.Name,
                    });
                }
                excel.GenerateWorksheet("Đơn vị quản lý", OrganizationHeader, data);
                #endregion

                #region Sex sheet
                data.Clear();
                var SexHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Sex in Sexes)
                {
                    data.Add(new object[]
                    {
                        Sex.Code,
                        Sex.Name,
                    });
                }
                excel.GenerateWorksheet("Giới tính", SexHeader, data);
                #endregion

                #region Site sheet
                data.Clear();
                var SiteHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Site in Sites)
                {
                    data.Add(new object[]
                    {
                        Site.Code,
                        Site.Name,
                    });
                }
                excel.GenerateWorksheet("Phân hệ", SiteHeader, data);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "AppUser_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(AppUserRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate()
        {
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                OrderBy = OrganizationOrder.Path,
                OrderType = OrderType.ASC,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name
            });
            List<Sex> Sexes = await SexService.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                OrderBy = SexOrder.Code,
                OrderType = OrderType.ASC,
                Selects = SexSelect.ALL
            });
            List<Status> Statuses = await StatusService.List(new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.Code | StatusSelect.Name
            });
            List<Site> Sites = await SiteService.List(new SiteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SiteSelect.Code | SiteSelect.Name
            });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/AppUser_Export.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                #region sheet Organization 
                var worksheet_Organization = xlPackage.Workbook.Worksheets["Đơn vị quản lý"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Organization = 2;
                int numberCell_Organization = 1;
                for (var i = 0; i < Organizations.Count; i++)
                {
                    Organization Organization = Organizations[i];
                    worksheet_Organization.Cells[startRow_Organization + i, numberCell_Organization].Value = Organization.Code;
                    worksheet_Organization.Cells[startRow_Organization + i, numberCell_Organization + 1].Value = Organization.Name;
                }
                #endregion

                #region sheet Sex 
                var worksheet_Sex = xlPackage.Workbook.Worksheets["Giới tính"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Sex = 2;
                int numberCell_Sex = 1;
                for (var i = 0; i < Sexes.Count; i++)
                {
                    Sex Sex = Sexes[i];
                    worksheet_Sex.Cells[startRow_Sex + i, numberCell_Sex].Value = Sex.Code;
                    worksheet_Sex.Cells[startRow_Sex + i, numberCell_Sex + 1].Value = Sex.Name;
                }
                #endregion

                #region sheet Site 
                var worksheet_Site = xlPackage.Workbook.Worksheets["Phân hệ"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Site = 2;
                int numberCell_Site = 1;
                for (var i = 0; i < Sites.Count; i++)
                {
                    Site Site = Sites[i];
                    worksheet_Site.Cells[startRow_Site + i, numberCell_Site].Value = Site.Code;
                    worksheet_Site.Cells[startRow_Site + i, numberCell_Site + 1].Value = Site.Name;
                }
                #endregion

                xlPackage.SaveAs(MemoryStream);
            }
            return File(MemoryStream.ToArray(), "application/octet-stream", "Template_AppUser.xlsx");
        }

        [Route(AppUserRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            AppUserFilter.Id = new IdFilter { In = Ids };
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.RowId;
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            AppUsers = await AppUserService.BulkDelete(AppUsers);
            if (AppUsers.Any(x => !x.IsValidated))
                return BadRequest(AppUsers.Where(x => !x.IsValidated));
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            if (Id == 0)
            {

            }
            else
            {
                AppUserFilter.Id = new IdFilter { Equal = Id };
                AppUserFilter.OrganizationId = new IdFilter { };
                int count = await AppUserService.Count(AppUserFilter);
                if (count == 0)
                    return false;
            }
            return true;
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

        private AppUserFilter ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Skip = AppUser_AppUserFilterDTO.Skip;
            AppUserFilter.Take = AppUser_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUser_AppUserFilterDTO.OrderBy;
            AppUserFilter.OrderType = AppUser_AppUserFilterDTO.OrderType;

            AppUserFilter.Id = AppUser_AppUserFilterDTO.Id;
            AppUserFilter.Username = AppUser_AppUserFilterDTO.Username;
            AppUserFilter.Password = AppUser_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = AppUser_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = AppUser_AppUserFilterDTO.Address;
            AppUserFilter.Email = AppUser_AppUserFilterDTO.Email;
            AppUserFilter.Phone = AppUser_AppUserFilterDTO.Phone;
            AppUserFilter.Department = AppUser_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = AppUser_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = AppUser_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = AppUser_AppUserFilterDTO.StatusId;
            return AppUserFilter;
        }

        [Route(AppUserRoute.FilterListOrganization), HttpPost]
        public async Task<List<AppUser_OrganizationDTO>> FilterListOrganization([FromBody] AppUser_OrganizationFilterDTO AppUser_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = AppUser_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = AppUser_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = AppUser_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = AppUser_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = AppUser_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = AppUser_OrganizationFilterDTO.Level;
            OrganizationFilter.Phone = AppUser_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = AppUser_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = AppUser_OrganizationFilterDTO.Email;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<AppUser_OrganizationDTO> AppUser_OrganizationDTOs = Organizations
                .Select(x => new AppUser_OrganizationDTO(x)).ToList();
            return AppUser_OrganizationDTOs;
        }

        [Route(AppUserRoute.FilterListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> FilterListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
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
        [Route(AppUserRoute.FilterListStatus), HttpPost]
        public async Task<List<AppUser_StatusDTO>> FilterListStatus([FromBody] AppUser_StatusFilterDTO AppUser_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = AppUser_StatusFilterDTO.Id;
            StatusFilter.Code = AppUser_StatusFilterDTO.Code;
            StatusFilter.Name = AppUser_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<AppUser_StatusDTO> AppUser_StatusDTOs = Statuses
                .Select(x => new AppUser_StatusDTO(x)).ToList();
            return AppUser_StatusDTOs;
        }
        [Route(AppUserRoute.FilterListRole), HttpPost]
        public async Task<List<AppUser_RoleDTO>> FilterListRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser_RoleDTO> AppUser_RoleDTOs = Roles
                .Select(x => new AppUser_RoleDTO(x)).ToList();
            return AppUser_RoleDTOs;
        }

        [Route(AppUserRoute.SingleListOrganization), HttpPost]
        public async Task<List<AppUser_OrganizationDTO>> SingleListOrganization([FromBody] AppUser_OrganizationFilterDTO AppUser_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = AppUser_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = AppUser_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = AppUser_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = AppUser_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = AppUser_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = AppUser_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = AppUser_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = AppUser_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = AppUser_OrganizationFilterDTO.Email;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<AppUser_OrganizationDTO> AppUser_OrganizationDTOs = Organizations
                .Select(x => new AppUser_OrganizationDTO(x)).ToList();
            return AppUser_OrganizationDTOs;
        }

        [Route(AppUserRoute.SingleListSex), HttpPost]
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
        [Route(AppUserRoute.SingleListStatus), HttpPost]
        public async Task<List<AppUser_StatusDTO>> SingleListStatus([FromBody] AppUser_StatusFilterDTO AppUser_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = AppUser_StatusFilterDTO.Id;
            StatusFilter.Code = AppUser_StatusFilterDTO.Code;
            StatusFilter.Name = AppUser_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<AppUser_StatusDTO> AppUser_StatusDTOs = Statuses
                .Select(x => new AppUser_StatusDTO(x)).ToList();
            return AppUser_StatusDTOs;
        }
        [Route(AppUserRoute.SingleListRole), HttpPost]
        public async Task<List<AppUser_RoleDTO>> SingleListRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser_RoleDTO> AppUser_RoleDTOs = Roles
                .Select(x => new AppUser_RoleDTO(x)).ToList();
            return AppUser_RoleDTOs;
        }

        [Route(AppUserRoute.CountRole), HttpPost]
        public async Task<long> CountRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = AppUser_RoleFilterDTO.StatusId;

            return await RoleService.Count(RoleFilter);
        }

        [Route(AppUserRoute.ListRole), HttpPost]
        public async Task<List<AppUser_RoleDTO>> ListRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = AppUser_RoleFilterDTO.Skip;
            RoleFilter.Take = AppUser_RoleFilterDTO.Take;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = AppUser_RoleFilterDTO.StatusId;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser_RoleDTO> AppUser_RoleDTOs = Roles
                .Select(x => new AppUser_RoleDTO(x)).ToList();
            return AppUser_RoleDTOs;
        }

        [Route(AppUserRoute.ListSite), HttpPost]
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

