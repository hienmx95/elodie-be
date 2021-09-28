using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using ELODIE.Entities;
using ELODIE.Services.MNation;
using ELODIE.Services.MStatus;
using System.Text;

namespace ELODIE.Rpc.nation
{
    public partial class NationController : RpcController
    {
        private IStatusService StatusService;
        private INationService NationService;
        private ICurrentContext CurrentContext;
        public NationController(
            IStatusService StatusService,
            INationService NationService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.NationService = NationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Nation_NationFilterDTO Nation_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = ConvertFilterDTOToFilterEntity(Nation_NationFilterDTO);
            NationFilter = await NationService.ToFilter(NationFilter);
            int count = await NationService.Count(NationFilter);
            return count;
        }

        [Route(NationRoute.List), HttpPost]
        public async Task<ActionResult<List<Nation_NationDTO>>> List([FromBody] Nation_NationFilterDTO Nation_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = ConvertFilterDTOToFilterEntity(Nation_NationFilterDTO);
            NationFilter = await NationService.ToFilter(NationFilter);
            List<Nation> Nations = await NationService.List(NationFilter);
            List<Nation_NationDTO> Nation_NationDTOs = Nations
                .Select(c => new Nation_NationDTO(c)).ToList();
            return Nation_NationDTOs;
        }

        [Route(NationRoute.Get), HttpPost]
        public async Task<ActionResult<Nation_NationDTO>> Get([FromBody] Nation_NationDTO Nation_NationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Nation_NationDTO.Id))
                return Forbid();

            Nation Nation = await NationService.Get(Nation_NationDTO.Id);
            return new Nation_NationDTO(Nation);
        }

        [Route(NationRoute.Create), HttpPost]
        public async Task<ActionResult<Nation_NationDTO>> Create([FromBody] Nation_NationDTO Nation_NationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Nation_NationDTO.Id))
                return Forbid();

            Nation Nation = ConvertDTOToEntity(Nation_NationDTO);
            Nation = await NationService.Create(Nation);
            Nation_NationDTO = new Nation_NationDTO(Nation);
            if (Nation.IsValidated)
                return Nation_NationDTO;
            else
                return BadRequest(Nation_NationDTO);
        }

        [Route(NationRoute.Update), HttpPost]
        public async Task<ActionResult<Nation_NationDTO>> Update([FromBody] Nation_NationDTO Nation_NationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Nation_NationDTO.Id))
                return Forbid();

            Nation Nation = ConvertDTOToEntity(Nation_NationDTO);
            Nation = await NationService.Update(Nation);
            Nation_NationDTO = new Nation_NationDTO(Nation);
            if (Nation.IsValidated)
                return Nation_NationDTO;
            else
                return BadRequest(Nation_NationDTO);
        }

        [Route(NationRoute.Delete), HttpPost]
        public async Task<ActionResult<Nation_NationDTO>> Delete([FromBody] Nation_NationDTO Nation_NationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Nation_NationDTO.Id))
                return Forbid();

            Nation Nation = ConvertDTOToEntity(Nation_NationDTO);
            Nation = await NationService.Delete(Nation);
            Nation_NationDTO = new Nation_NationDTO(Nation);
            if (Nation.IsValidated)
                return Nation_NationDTO;
            else
                return BadRequest(Nation_NationDTO);
        }

        [Route(NationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NationFilter NationFilter = new NationFilter();
            NationFilter = await NationService.ToFilter(NationFilter);
            NationFilter.Id = new IdFilter { In = Ids };
            NationFilter.Selects = NationSelect.Id;
            NationFilter.Skip = 0;
            NationFilter.Take = int.MaxValue;

            List<Nation> Nations = await NationService.List(NationFilter);
            Nations = await NationService.BulkDelete(Nations);
            if (Nations.Any(x => !x.IsValidated))
                return BadRequest(Nations.Where(x => !x.IsValidated));
            return true;
        }

        [Route(NationRoute.Import), HttpPost]
        public async Task<ActionResult<List<Nation_NationDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            NationFilter NationFilter = new NationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = NationSelect.ALL
            };
            List<Nation> oldData = await NationService.List(NationFilter);
            List<Nation> Nations = new List<Nation>();
            StringBuilder errorContent = new StringBuilder();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Nation"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StartColumn = 1;
                int StartRow = 2;

                int SttColumnn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string stt = worksheet.Cells[i + StartRow, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(CodeValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã quốc gia");
                    }
                    else if (string.IsNullOrWhiteSpace(CodeValue) && i == worksheet.Dimension.End.Row)
                        break;
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(NameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập tên quốc gia");
                    }

                    Nation Nation = oldData.Where(x => x.Code == CodeValue).FirstOrDefault();
                    if (Nation == null)
                    {
                        Nation = new Nation
                        {
                            Code = CodeValue,
                            Name = NameValue,
                            StatusId = Enums.StatusEnum.ACTIVE.Id,
                            RowId = Guid.NewGuid(),
                            Used = false
                        };
                    }
                    else
                    {
                        Nation.Name = NameValue;
                    }
                    Nations.Add(Nation);
                }
            }
            Nations = await NationService.Import(Nations);
            List<Nation_NationDTO> Nation_NationDTOs = Nations
                .Select(c => new Nation_NationDTO(c)).ToList();
            return Nation_NationDTOs;
        }

        [Route(NationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Nation_NationFilterDTO Nation_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Nation
                var NationFilter = ConvertFilterDTOToFilterEntity(Nation_NationFilterDTO);
                NationFilter.Skip = 0;
                NationFilter.Take = int.MaxValue;
                NationFilter = await NationService.ToFilter(NationFilter);
                List<Nation> Nations = await NationService.List(NationFilter);

                var NationHeaders = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã quốc gia",
                        "Tên quốc gia"
                    }
                };
                List<object[]> NationData = new List<object[]>();
                for (int i = 0; i < Nations.Count; i++)
                {
                    var Nation = Nations[i];
                    NationData.Add(new Object[]
                    {
                        i+1,
                        Nation.Code,
                        Nation.Name
                    });
                }
                excel.GenerateWorksheet("Nation", NationHeaders, NationData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Nation.xlsx");
        }

        [Route(NationRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Nation_NationFilterDTO Nation_NationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            string path = "Templates/Nation_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "NationTemplate.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            NationFilter NationFilter = new NationFilter();
            NationFilter = await NationService.ToFilter(NationFilter);
            if (Id == 0)
            {

            }
            else
            {
                NationFilter.Id = new IdFilter { Equal = Id };
                int count = await NationService.Count(NationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Nation ConvertDTOToEntity(Nation_NationDTO Nation_NationDTO)
        {
            Nation Nation = new Nation();
            Nation.Id = Nation_NationDTO.Id;
            Nation.Code = Nation_NationDTO.Code;
            Nation.Name = Nation_NationDTO.Name;
            Nation.Priority = Nation_NationDTO.Priority;
            Nation.StatusId = Nation_NationDTO.StatusId;
            Nation.Used = Nation_NationDTO.Used;
            Nation.RowId = Nation_NationDTO.RowId;
            Nation.Status = Nation_NationDTO.Status == null ? null : new Status
            {
                Id = Nation_NationDTO.Status.Id,
                Code = Nation_NationDTO.Status.Code,
                Name = Nation_NationDTO.Status.Name,
            };
            Nation.BaseLanguage = CurrentContext.Language;
            return Nation;
        }

        private NationFilter ConvertFilterDTOToFilterEntity(Nation_NationFilterDTO Nation_NationFilterDTO)
        {
            NationFilter NationFilter = new NationFilter();
            NationFilter.Selects = NationSelect.ALL;
            NationFilter.Skip = Nation_NationFilterDTO.Skip;
            NationFilter.Take = Nation_NationFilterDTO.Take;
            NationFilter.OrderBy = Nation_NationFilterDTO.OrderBy;
            NationFilter.OrderType = Nation_NationFilterDTO.OrderType;

            NationFilter.Id = Nation_NationFilterDTO.Id;
            NationFilter.Code = Nation_NationFilterDTO.Code;
            NationFilter.Name = Nation_NationFilterDTO.Name;
            NationFilter.Priority = Nation_NationFilterDTO.Priority;
            NationFilter.StatusId = Nation_NationFilterDTO.StatusId;
            NationFilter.RowId = Nation_NationFilterDTO.RowId;
            NationFilter.CreatedAt = Nation_NationFilterDTO.CreatedAt;
            NationFilter.UpdatedAt = Nation_NationFilterDTO.UpdatedAt;
            return NationFilter;
        }
    }
}

