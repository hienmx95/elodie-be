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

namespace ELODIE.Services.MProduct
{
    public interface IVariationGroupingService : IServiceScoped
    {
        Task<int> Count(VariationGroupingFilter VariationGroupingFilter);
        Task<List<VariationGrouping>> List(VariationGroupingFilter VariationGroupingFilter);
        Task<VariationGrouping> Get(long Id);
    }

    public class VariationGroupingService : BaseService, IVariationGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IVariationGroupingValidator VariationGroupingValidator;

        public VariationGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IVariationGroupingValidator VariationGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.VariationGroupingValidator = VariationGroupingValidator;
        }
        public async Task<int> Count(VariationGroupingFilter VariationGroupingFilter)
        {
            try
            {
                int result = await UOW.VariationGroupingRepository.Count(VariationGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<VariationGrouping>> List(VariationGroupingFilter VariationGroupingFilter)
        {
            try
            {
                List<VariationGrouping> VariationGroupings = await UOW.VariationGroupingRepository.List(VariationGroupingFilter);
                return VariationGroupings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<VariationGrouping> Get(long Id)
        {
            VariationGrouping VariationGrouping = await UOW.VariationGroupingRepository.Get(Id);
            if (VariationGrouping == null)
                return null;
            return VariationGrouping;
        }

        public async Task<VariationGrouping> Create(VariationGrouping VariationGrouping)
        {
            if (!await VariationGroupingValidator.Create(VariationGrouping))
                return VariationGrouping;

            try
            {
                await UOW.Begin();
                await UOW.VariationGroupingRepository.Create(VariationGrouping);
                await UOW.Commit();

                await Logging.CreateAuditLog(VariationGrouping, new { }, nameof(VariationGroupingService));
                return await UOW.VariationGroupingRepository.Get(VariationGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<VariationGrouping> Update(VariationGrouping VariationGrouping)
        {
            if (!await VariationGroupingValidator.Update(VariationGrouping))
                return VariationGrouping;
            try
            {
                var oldData = await UOW.VariationGroupingRepository.Get(VariationGrouping.Id);

                await UOW.Begin();
                await UOW.VariationGroupingRepository.Update(VariationGrouping);
                await UOW.Commit();

                var newData = await UOW.VariationGroupingRepository.Get(VariationGrouping.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(VariationGroupingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<VariationGrouping> Delete(VariationGrouping VariationGrouping)
        {
            if (!await VariationGroupingValidator.Delete(VariationGrouping))
                return VariationGrouping;

            try
            {
                await UOW.Begin();
                await UOW.VariationGroupingRepository.Delete(VariationGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, VariationGrouping, nameof(VariationGroupingService));
                return VariationGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<VariationGrouping>> BulkDelete(List<VariationGrouping> VariationGroupings)
        {
            if (!await VariationGroupingValidator.BulkDelete(VariationGroupings))
                return VariationGroupings;

            try
            {
                await UOW.Begin();
                await UOW.VariationGroupingRepository.BulkDelete(VariationGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, VariationGroupings, nameof(VariationGroupingService));
                return VariationGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<VariationGrouping>> Import(DataFile DataFile)
        {
            List<VariationGrouping> VariationGroupings = new List<VariationGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return VariationGroupings;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int ProductIdColumn = 2 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ProductIdValue = worksheet.Cells[i + StartRow, ProductIdColumn].Value?.ToString();
                    VariationGrouping VariationGrouping = new VariationGrouping();
                    VariationGrouping.Name = NameValue;
                    VariationGroupings.Add(VariationGrouping);
                }
            }

            if (!await VariationGroupingValidator.Import(VariationGroupings))
                return VariationGroupings;

            try
            {
                await UOW.Begin();
                await UOW.VariationGroupingRepository.BulkMerge(VariationGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(VariationGroupings, new { }, nameof(VariationGroupingService));
                return VariationGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(VariationGroupingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(VariationGroupingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<DataFile> Export(VariationGroupingFilter VariationGroupingFilter)
        {
            List<VariationGrouping> VariationGroupings = await UOW.VariationGroupingRepository.List(VariationGroupingFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(VariationGrouping);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int ProductIdColumn = 2 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(VariationGrouping.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(VariationGrouping.Name);
                worksheet.Cells[1, ProductIdColumn].Value = nameof(VariationGrouping.ProductId);

                for (int i = 0; i < VariationGroupings.Count; i++)
                {
                    VariationGrouping VariationGrouping = VariationGroupings[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = VariationGrouping.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = VariationGrouping.Name;
                    worksheet.Cells[i + StartRow, ProductIdColumn].Value = VariationGrouping.ProductId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(VariationGrouping),
                Content = MemoryStream,
            };
            return DataFile;
        }
    }
}
