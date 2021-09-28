using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Handlers;
using ELODIE.Enums;

namespace ELODIE.Services.MDistrict
{
    public interface IDistrictService : IServiceScoped
    {
        Task<int> Count(DistrictFilter DistrictFilter);
        Task<List<District>> List(DistrictFilter DistrictFilter);
        Task<District> Get(long Id);
        Task<District> Create(District District);
        Task<District> Update(District District);
        Task<District> Delete(District District);
        Task<List<District>> BulkDelete(List<District> Districts);
        Task<DataFile> Export(DistrictFilter DistrictFilter);
        DistrictFilter ToFilter(DistrictFilter DistrictFilter);
    }

    public class DistrictService : BaseService, IDistrictService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDistrictValidator DistrictValidator;
        //private IRabbitManager RabbitManager;


        public DistrictService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDistrictValidator DistrictValidator
            //IRabbitManager rabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DistrictValidator = DistrictValidator;
            //this.RabbitManager = rabbitManager;
        }
        public async Task<int> Count(DistrictFilter DistrictFilter)
        {
            try
            {
                int result = await UOW.DistrictRepository.Count(DistrictFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<District>> List(DistrictFilter DistrictFilter)
        {
            try
            {
                List<District> Districts = await UOW.DistrictRepository.List(DistrictFilter);
                return Districts;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<District> Get(long Id)
        {
            District District = await UOW.DistrictRepository.Get(Id);
            if (District == null)
                return null;
            return District;
        }

        public async Task<District> Create(District District)
        {
            if (!await DistrictValidator.Create(District))
                return District;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.Create(District);
                await UOW.Commit();

                List<District> Districts = await UOW.DistrictRepository.List(new List<long> { District.Id });
                Sync(Districts);
                District = Districts.FirstOrDefault();

                await Logging.CreateAuditLog(District, new { }, nameof(DistrictService));
                return District;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<District> Update(District District)
        {
            if (!await DistrictValidator.Update(District))
                return District;
            try
            {
                var oldData = await UOW.DistrictRepository.Get(District.Id);

                await UOW.Begin();
                await UOW.DistrictRepository.Update(District);
                await UOW.Commit();

                List<District> Districts = await UOW.DistrictRepository.List(new List<long> { District.Id });
                Sync(Districts);
                District = Districts.FirstOrDefault();
                await Logging.CreateAuditLog(District, oldData, nameof(DistrictService));
                return District;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<District> Delete(District District)
        {
            if (!await DistrictValidator.Delete(District))
                return District;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.Delete(District);
                await UOW.Commit();

                List<District> Districts = await UOW.DistrictRepository.List(new List<long> { District.Id });
                Sync(Districts);
                District = Districts.FirstOrDefault();

                await Logging.CreateAuditLog(new { }, District, nameof(DistrictService));
                return District;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<District>> BulkDelete(List<District> Districts)
        {
            if (!await DistrictValidator.BulkDelete(Districts))
                return Districts;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.BulkDelete(Districts);
                await UOW.Commit();

                List<long> Ids = Districts.Select(x => x.Id).ToList();
                Districts = await UOW.DistrictRepository.List(Ids);
                Sync(Districts);
                await Logging.CreateAuditLog(new { }, Districts, nameof(DistrictService));

                return Districts;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DistrictService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DataFile> Export(DistrictFilter DistrictFilter)
        {
            List<District> Districts = await UOW.DistrictRepository.List(DistrictFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(District);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int ProvinceIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(District.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(District.Name);
                worksheet.Cells[1, PriorityColumn].Value = nameof(District.Priority);
                worksheet.Cells[1, ProvinceIdColumn].Value = nameof(District.ProvinceId);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(District.StatusId);

                for (int i = 0; i < Districts.Count; i++)
                {
                    District District = Districts[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = District.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = District.Name;
                    worksheet.Cells[i + StartRow, PriorityColumn].Value = District.Priority;
                    worksheet.Cells[i + StartRow, ProvinceIdColumn].Value = District.ProvinceId;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = District.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(District),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public DistrictFilter ToFilter(DistrictFilter filter)
        {

            return filter;
        }

        private void Sync(List<District> Districts)
        {
            List<Province> Provinces = Districts.Select(x => new Province { Id = x.ProvinceId }).Distinct().ToList();
            //RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictSync);
            //RabbitManager.PublishList(Provinces, RoutingKeyEnum.ProvinceUsed);
        }
    }
}
