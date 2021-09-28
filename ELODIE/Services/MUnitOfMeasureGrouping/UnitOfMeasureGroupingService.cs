using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Repositories;
using ELODIE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Handlers;
using ELODIE.Enums;

namespace ELODIE.Services.MUnitOfMeasureGrouping
{
    public interface IUnitOfMeasureGroupingService : IServiceScoped
    {
        Task<int> Count(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter);
        Task<UnitOfMeasureGrouping> Get(long Id);
        Task<UnitOfMeasureGrouping> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<UnitOfMeasureGrouping> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<UnitOfMeasureGrouping> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<List<UnitOfMeasureGrouping>> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
        Task<List<UnitOfMeasureGrouping>> Import(DataFile DataFile);
    }

    public class UnitOfMeasureGroupingService : BaseService, IUnitOfMeasureGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUnitOfMeasureGroupingValidator UnitOfMeasureGroupingValidator;
        //private IRabbitManager RabbitManager;

        public UnitOfMeasureGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUnitOfMeasureGroupingValidator UnitOfMeasureGroupingValidator
           // IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UnitOfMeasureGroupingValidator = UnitOfMeasureGroupingValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter)
        {
            try
            {
                int result = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<UnitOfMeasureGrouping>> List(UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter)
        {
            try
            {
                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(UnitOfMeasureGroupingFilter);
                return UnitOfMeasureGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<UnitOfMeasureGrouping> Get(long Id)
        {
            UnitOfMeasureGrouping UnitOfMeasureGrouping = await UOW.UnitOfMeasureGroupingRepository.Get(Id);
            if (UnitOfMeasureGrouping == null)
                return null;
            return UnitOfMeasureGrouping;
        }

        public async Task<UnitOfMeasureGrouping> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (!await UnitOfMeasureGroupingValidator.Create(UnitOfMeasureGrouping))
                return UnitOfMeasureGrouping;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingRepository.Create(UnitOfMeasureGrouping);
                await UOW.Commit();

                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(new List<long> { UnitOfMeasureGrouping.Id });
                Sync(UnitOfMeasureGroupings);
                UnitOfMeasureGrouping = UnitOfMeasureGroupings.FirstOrDefault();
                await Logging.CreateAuditLog(UnitOfMeasureGrouping, new { }, nameof(UnitOfMeasureGroupingService));
                return await UOW.UnitOfMeasureGroupingRepository.Get(UnitOfMeasureGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<UnitOfMeasureGrouping> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (!await UnitOfMeasureGroupingValidator.Update(UnitOfMeasureGrouping))
                return UnitOfMeasureGrouping;
            try
            {
                var oldData = await UOW.UnitOfMeasureGroupingRepository.Get(UnitOfMeasureGrouping.Id);

                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingRepository.Update(UnitOfMeasureGrouping);
                await UOW.Commit();

                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(new List<long> { UnitOfMeasureGrouping.Id });
                Sync(UnitOfMeasureGroupings);
                UnitOfMeasureGrouping = UnitOfMeasureGroupings.FirstOrDefault();
                await Logging.CreateAuditLog(UnitOfMeasureGrouping, oldData, nameof(UnitOfMeasureGroupingService));
                return UnitOfMeasureGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<UnitOfMeasureGrouping> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (!await UnitOfMeasureGroupingValidator.Delete(UnitOfMeasureGrouping))
                return UnitOfMeasureGrouping;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingRepository.Delete(UnitOfMeasureGrouping);
                await UOW.Commit();
                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(new List<long> { UnitOfMeasureGrouping.Id });
                Sync(UnitOfMeasureGroupings);
                UnitOfMeasureGrouping = UnitOfMeasureGroupings.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, UnitOfMeasureGrouping, nameof(UnitOfMeasureGroupingService));
                return UnitOfMeasureGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<UnitOfMeasureGrouping>> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            if (!await UnitOfMeasureGroupingValidator.BulkDelete(UnitOfMeasureGroupings))
                return UnitOfMeasureGroupings;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingRepository.BulkDelete(UnitOfMeasureGroupings);
                await UOW.Commit();
                List<long> Ids = UnitOfMeasureGroupings.Select(x => x.Id).ToList();
                UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(Ids);
                Sync(UnitOfMeasureGroupings);
                await Logging.CreateAuditLog(new { }, UnitOfMeasureGroupings, nameof(UnitOfMeasureGroupingService));
                return UnitOfMeasureGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<UnitOfMeasureGrouping>> Import(DataFile DataFile)
        {
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = new List<UnitOfMeasureGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return UnitOfMeasureGroupings;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int UnitOfMeasureIdColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    UnitOfMeasureGrouping UnitOfMeasureGrouping = new UnitOfMeasureGrouping();
                    UnitOfMeasureGrouping.Name = NameValue;
                    UnitOfMeasureGroupings.Add(UnitOfMeasureGrouping);
                }
            }

            if (!await UnitOfMeasureGroupingValidator.Import(UnitOfMeasureGroupings))
                return UnitOfMeasureGroupings;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingRepository.BulkMerge(UnitOfMeasureGroupings);
                await UOW.Commit();
                List<long> Ids = UnitOfMeasureGroupings.Select(x => x.Id).ToList();
                UnitOfMeasureGroupings = await UOW.UnitOfMeasureGroupingRepository.List(Ids);
                Sync(UnitOfMeasureGroupings);
                await Logging.CreateAuditLog(UnitOfMeasureGroupings, new { }, nameof(UnitOfMeasureGroupingService));
                return UnitOfMeasureGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private void Sync(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            List<UnitOfMeasure> UnitOfMeasures = UnitOfMeasureGroupings.Select(x => new UnitOfMeasure { Id = x.UnitOfMeasureId }).Distinct().ToList();
            //RabbitManager.PublishList(UnitOfMeasureGroupings, RoutingKeyEnum.UnitOfMeasureGroupingSync);
            //RabbitManager.PublishList(UnitOfMeasures, RoutingKeyEnum.UnitOfMeasureUsed);
        }
    }
}
