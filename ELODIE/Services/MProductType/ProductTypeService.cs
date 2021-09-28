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

namespace ELODIE.Services.MProductType
{
    public interface IProductTypeService : IServiceScoped
    {
        Task<int> Count(ProductTypeFilter ProductTypeFilter);
        Task<List<ProductType>> List(ProductTypeFilter ProductTypeFilter);
        Task<ProductType> Get(long Id);
        Task<ProductType> Create(ProductType ProductType);
        Task<ProductType> Update(ProductType ProductType);
        Task<ProductType> Delete(ProductType ProductType);
        Task<List<ProductType>> BulkDelete(List<ProductType> ProductTypes);
        Task<List<ProductType>> Import(DataFile DataFile);
        Task<DataFile> Export(ProductTypeFilter ProductTypeFilter);
    }

    public class ProductTypeService : BaseService, IProductTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductTypeValidator ProductTypeValidator;
        //private IRabbitManager RabbitManager;

        public ProductTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductTypeValidator ProductTypeValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductTypeValidator = ProductTypeValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ProductTypeFilter ProductTypeFilter)
        {
            try
            {
                int result = await UOW.ProductTypeRepository.Count(ProductTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProductType>> List(ProductTypeFilter ProductTypeFilter)
        {
            try
            {
                List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(ProductTypeFilter);
                return ProductTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<ProductType> Get(long Id)
        {
            ProductType ProductType = await UOW.ProductTypeRepository.Get(Id);
            if (ProductType == null)
                return null;
            return ProductType;
        }

        public async Task<ProductType> Create(ProductType ProductType)
        {
            if (!await ProductTypeValidator.Create(ProductType))
                return ProductType;

            try
            {
                await UOW.Begin();
                await UOW.ProductTypeRepository.Create(ProductType);
                await UOW.Commit();
                List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(new List<long> { ProductType.Id });
                Sync(ProductTypes);
                ProductType = ProductTypes.FirstOrDefault();
                await Logging.CreateAuditLog(ProductType, new { }, nameof(ProductTypeService));
                return await UOW.ProductTypeRepository.Get(ProductType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<ProductType> Update(ProductType ProductType)
        {
            if (!await ProductTypeValidator.Update(ProductType))
                return ProductType;
            try
            {
                var oldData = await UOW.ProductTypeRepository.Get(ProductType.Id);

                await UOW.Begin();
                await UOW.ProductTypeRepository.Update(ProductType);
                await UOW.Commit();

                List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(new List<long> { ProductType.Id });
                Sync(ProductTypes);
                ProductType = ProductTypes.FirstOrDefault();
                await Logging.CreateAuditLog(ProductType, oldData, nameof(ProductTypeService));
                return ProductType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<ProductType> Delete(ProductType ProductType)
        {
            if (!await ProductTypeValidator.Delete(ProductType))
                return ProductType;

            try
            {
                await UOW.Begin();
                await UOW.ProductTypeRepository.Delete(ProductType);
                await UOW.Commit();
                List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(new List<long> { ProductType.Id });
                Sync(ProductTypes);
                ProductType = ProductTypes.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, ProductType, nameof(ProductTypeService));
                return ProductType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProductType>> BulkDelete(List<ProductType> ProductTypes)
        {
            if (!await ProductTypeValidator.BulkDelete(ProductTypes))
                return ProductTypes;

            try
            {
                await UOW.Begin();
                await UOW.ProductTypeRepository.BulkDelete(ProductTypes);
                await UOW.Commit();

                List<long> Ids = ProductTypes.Select(x => x.Id).ToList();
                ProductTypes = await UOW.ProductTypeRepository.List(Ids);
                Sync(ProductTypes);
                await Logging.CreateAuditLog(new { }, ProductTypes, nameof(ProductTypeService));
                return ProductTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProductType>> Import(DataFile DataFile)
        {
            List<ProductType> ProductTypes = new List<ProductType>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return ProductTypes;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    ProductType ProductType = new ProductType();
                    ProductType.Code = CodeValue;
                    ProductType.Name = NameValue;
                    ProductType.Description = DescriptionValue;
                    ProductTypes.Add(ProductType);
                }
            }

            if (!await ProductTypeValidator.Import(ProductTypes))
                return ProductTypes;

            try
            {
                await UOW.Begin();
                await UOW.ProductTypeRepository.BulkMerge(ProductTypes);
                await UOW.Commit();

                List<long> Ids = ProductTypes.Select(x => x.Id).ToList();
                ProductTypes = await UOW.ProductTypeRepository.List(Ids);
                Sync(ProductTypes);

                await Logging.CreateAuditLog(ProductTypes, new { }, nameof(ProductTypeService));
                return ProductTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<DataFile> Export(ProductTypeFilter ProductTypeFilter)
        {
            List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(ProductTypeFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(ProductType);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(ProductType.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(ProductType.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(ProductType.Name);
                worksheet.Cells[1, DescriptionColumn].Value = nameof(ProductType.Description);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(ProductType.StatusId);

                for (int i = 0; i < ProductTypes.Count; i++)
                {
                    ProductType ProductType = ProductTypes[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = ProductType.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = ProductType.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = ProductType.Name;
                    worksheet.Cells[i + StartRow, DescriptionColumn].Value = ProductType.Description;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = ProductType.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(ProductType),
                Content = MemoryStream,
            };
            return DataFile;
        }

        private void Sync(List<ProductType> ProductTypes)
        {
            //RabbitManager.PublishList(ProductTypes, RoutingKeyEnum.ProductTypeSync);
        }
    }
}
