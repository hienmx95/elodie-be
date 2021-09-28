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
using RestSharp;

namespace ELODIE.Services.MSupplier
{
    public interface ISupplierService : IServiceScoped
    {
        Task<int> Count(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(SupplierFilter SupplierFilter);
        Task<Supplier> Get(long Id);
        Task<Supplier> Create(Supplier Supplier);
        Task<Supplier> QuickCreate(Supplier Supplier);
        Task<Supplier> Update(Supplier Supplier);
        Task<Supplier> Delete(Supplier Supplier);
        Task<List<Supplier>> BulkDelete(List<Supplier> Suppliers);
        Task<List<Supplier>> BulkMerge(List<Supplier> Suppliers);
        Task<string> SaveImage(Image Image);
        Task<DataFile> Export(SupplierFilter SupplierFilter);
        Task<SupplierFilter> ToFilter(SupplierFilter SupplierFilter);
    }

    public class SupplierService : BaseService, ISupplierService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISupplierValidator SupplierValidator;
        //private IRabbitManager RabbitManager;

        public SupplierService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISupplierValidator SupplierValidator
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SupplierValidator = SupplierValidator;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(SupplierFilter SupplierFilter)
        {
            try
            {
                int result = await UOW.SupplierRepository.Count(SupplierFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Supplier>> List(SupplierFilter SupplierFilter)
        {
            try
            {
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierFilter);
                return Suppliers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Supplier> Get(long Id)
        {
            Supplier Supplier = await UOW.SupplierRepository.Get(Id);
            if (Supplier == null)
                return null;
            return Supplier;
        }

        public async Task<Supplier> Create(Supplier Supplier)
        {
            if (!await SupplierValidator.Create(Supplier))
                return Supplier;

            try
            {
                Supplier.Id = 0;
                await UOW.Begin();

                var SupplierPrimaryContactor = Supplier.SupplierContactors.FirstOrDefault();
                Supplier.Email = SupplierPrimaryContactor?.Email;

                await UOW.SupplierRepository.Create(Supplier);
                await UOW.Commit();
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(new List<long> { Supplier.Id });
                Sync(Suppliers);
                Supplier = Suppliers.FirstOrDefault();
                await Logging.CreateAuditLog(Supplier, new { }, nameof(SupplierService));
                return Supplier;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Supplier> QuickCreate(Supplier Supplier)
        {
            if (!await SupplierValidator.QuickCreate(Supplier))
                return Supplier;

            try
            {
                Supplier.Id = 0;
                await UOW.Begin();

                var SupplierPrimaryContactor = Supplier.SupplierContactors?.FirstOrDefault();
                Supplier.Email = SupplierPrimaryContactor?.Email;

                await UOW.SupplierRepository.Create(Supplier);
                await UOW.Commit();
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(new List<long> { Supplier.Id });
                Sync(Suppliers);
                Supplier = Suppliers.FirstOrDefault();
                await Logging.CreateAuditLog(Supplier, new { }, nameof(SupplierService));
                return Supplier;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }


        public async Task<Supplier> Update(Supplier Supplier)
        {
            if (!await SupplierValidator.Update(Supplier))
                return Supplier;
            try
            {
                var oldData = await UOW.SupplierRepository.Get(Supplier.Id);

                var SupplierPrimaryContactor = Supplier.SupplierContactors.FirstOrDefault();
                Supplier.Email = SupplierPrimaryContactor?.Email;

                await UOW.Begin();
                await UOW.SupplierRepository.Update(Supplier);
                await UOW.Commit();

                List<Supplier> Suppliers = await UOW.SupplierRepository.List(new List<long> { Supplier.Id });
                Sync(Suppliers);
                Supplier = Suppliers.FirstOrDefault();

                await Logging.CreateAuditLog(Supplier, oldData, nameof(SupplierService));
                return Supplier;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Supplier> Delete(Supplier Supplier)
        {
            if (!await SupplierValidator.Delete(Supplier))
                return Supplier;

            try
            {
                await UOW.Begin();
                await UOW.SupplierRepository.Delete(Supplier);
                await UOW.Commit();
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(new List<long> { Supplier.Id });
                Sync(Suppliers);
                Supplier = Suppliers.FirstOrDefault();
                await Logging.CreateAuditLog(new { }, Supplier, nameof(SupplierService));
                return Supplier;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Supplier>> BulkDelete(List<Supplier> Suppliers)
        {
            if (!await SupplierValidator.BulkDelete(Suppliers))
                return Suppliers;

            try
            {
                await UOW.Begin();
                await UOW.SupplierRepository.BulkDelete(Suppliers);
                await UOW.Commit();
                List<long> Ids = Suppliers.Select(x => x.Id).ToList();
                Suppliers = await UOW.SupplierRepository.List(Ids);
                Sync(Suppliers);

                await Logging.CreateAuditLog(new { }, Suppliers, nameof(SupplierService));
                return Suppliers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Supplier>> BulkMerge(List<Supplier> Suppliers)
        {
            if (!await SupplierValidator.BulkMerge(Suppliers))
                return Suppliers;

            try
            {
                await UOW.Begin();
                List<Supplier> dbSuppliers = await UOW.SupplierRepository.List(new SupplierFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = SupplierSelect.Id | SupplierSelect.Code,
                });
                foreach (Supplier Supplier in Suppliers)
                {
                    long SupplierId = dbSuppliers.Where(x => x.Code == Supplier.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    Supplier.Id = SupplierId;
                }
                await UOW.SupplierRepository.BulkMerge(Suppliers);
                await UOW.Commit();

                List<long> Ids = Suppliers.Select(x => x.Id).ToList();
                Suppliers = await UOW.SupplierRepository.List(Ids);
                Sync(Suppliers);
                await Logging.CreateAuditLog(Suppliers, new { }, nameof(SupplierService));
                return Suppliers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
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

        public async Task<DataFile> Export(SupplierFilter SupplierFilter)
        {
            try
            {
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierFilter);
                MemoryStream MemoryStream = new MemoryStream();
                using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
                {
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                    excelPackage.Workbook.Properties.Title = nameof(Supplier);
                    excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                    int StartColumn = 1;
                    int StartRow = 2;
                    int IdColumn = 0 + StartColumn;
                    int CodeColumn = 1 + StartColumn;
                    int NameColumn = 2 + StartColumn;
                    int TaxCodeColumn = 3 + StartColumn;
                    int StatusIdColumn = 4 + StartColumn;

                    worksheet.Cells[1, IdColumn].Value = nameof(Supplier.Id);
                    worksheet.Cells[1, CodeColumn].Value = nameof(Supplier.Code);
                    worksheet.Cells[1, NameColumn].Value = nameof(Supplier.Name);
                    worksheet.Cells[1, TaxCodeColumn].Value = nameof(Supplier.TaxCode);
                    worksheet.Cells[1, StatusIdColumn].Value = nameof(Supplier.StatusId);

                    for (int i = 0; i < Suppliers.Count; i++)
                    {
                        Supplier Supplier = Suppliers[i];
                        worksheet.Cells[i + StartRow, IdColumn].Value = Supplier.Id;
                        worksheet.Cells[i + StartRow, CodeColumn].Value = Supplier.Code;
                        worksheet.Cells[i + StartRow, NameColumn].Value = Supplier.Name;
                        worksheet.Cells[i + StartRow, TaxCodeColumn].Value = Supplier.TaxCode;
                        worksheet.Cells[i + StartRow, StatusIdColumn].Value = Supplier.StatusId;
                    }
                    excelPackage.Save();
                }

                DataFile DataFile = new DataFile
                {
                    Name = nameof(Supplier),
                    Content = MemoryStream,
                };
                return DataFile;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SupplierService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<SupplierFilter> ToFilter(SupplierFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SupplierFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SupplierFilter subFilter = new SupplierFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxCode))
                        subFilter.TaxCode = FilterBuilder.Merge(subFilter.TaxCode, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = FilterBuilder.Merge(subFilter.Phone, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = FilterBuilder.Merge(subFilter.Email, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Avatar))
                        subFilter.Avatar = FilterBuilder.Merge(subFilter.Avatar, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Address))
                        subFilter.Address = FilterBuilder.Merge(subFilter.Address, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NationId))
                        subFilter.NationId = FilterBuilder.Merge(subFilter.NationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProvinceId))
                        subFilter.ProvinceId = FilterBuilder.Merge(subFilter.ProvinceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DistrictId))
                        subFilter.DistrictId = FilterBuilder.Merge(subFilter.DistrictId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WardId))
                        subFilter.WardId = FilterBuilder.Merge(subFilter.WardId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OwnerName))
                        subFilter.OwnerName = FilterBuilder.Merge(subFilter.OwnerName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PersonInChargeId))
                        subFilter.PersonInChargeId = FilterBuilder.Merge(subFilter.PersonInChargeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = FilterBuilder.Merge(subFilter.Description, FilterPermissionDefinition.StringFilter);
                    //if (FilterPermissionDefinition.Name == nameof(subFilter.RowId))
                    //    subFilter.RowId = FilterBuilder.Merge(subFilter.RowId, FilterPermissionDefinition.GuidFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<Supplier> Suppliers)
        {
            List<AppUser> AppUsers = Suppliers.Where(x => x.PersonInChargeId.HasValue).Select(x => new AppUser { Id = x.PersonInChargeId.Value }).Distinct().ToList();
            List<Nation> Nations = Suppliers.Where(x => x.NationId.HasValue).Select(x => new Nation { Id = x.NationId.Value }).Distinct().ToList();
            List<Province> Provinces = Suppliers.Where(x => x.ProvinceId.HasValue).Select(x => new Province { Id = x.ProvinceId.Value }).Distinct().ToList();
            List<District> Districts = Suppliers.Where(x => x.DistrictId.HasValue).Select(x => new District { Id = x.DistrictId.Value }).Distinct().ToList();
            List<Ward> Wards = Suppliers.Where(x => x.WardId.HasValue).Select(x => new Ward { Id = x.WardId.Value }).Distinct().ToList();
           
            //RabbitManager.PublishList(Suppliers, RoutingKeyEnum.SupplierSync);
            //RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed);
            //RabbitManager.PublishList(Nations, RoutingKeyEnum.NationUsed);
            //RabbitManager.PublishList(Provinces, RoutingKeyEnum.ProvinceUsed);
            //RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictUsed);
            //RabbitManager.PublishList(Wards, RoutingKeyEnum.WardUsed);
        }

    }
    public class File
    {
        public long Id { get; set; }
        public string Path { get; set; }
    }
}
