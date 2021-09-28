using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Helpers;
using ELODIE.Repositories;
using RestSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using ELODIE.Handlers;
using ELODIE.Enums;

namespace ELODIE.Services.MImage
{
    public interface IImageService : IServiceScoped
    {
        Task<List<ELODIE.Entities.Image>> List(ImageFilter ImageFilter);
        Task<ELODIE.Entities.Image> Get(long Id);
        Task<ELODIE.Entities.Image> Create(ELODIE.Entities.Image Image, string path);
        Task<ELODIE.Entities.Image> Create(ELODIE.Entities.Image Image, string path, string thumbnailPath, int width, int height);
        Task<ELODIE.Entities.Image> Delete(ELODIE.Entities.Image Image);
    }

    public class ImageService : BaseService, IImageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        //private IRabbitManager RabbitManager;

        public ImageService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
            //IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            //this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ImageFilter ImageFilter)
        {
            try
            {
                int result = await UOW.ImageRepository.Count(ImageFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ImageService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<ELODIE.Entities.Image>> List(ImageFilter ImageFilter)
        {
            try
            {
                List<ELODIE.Entities.Image> Images = await UOW.ImageRepository.List(ImageFilter);
                return Images;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ImageService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<ELODIE.Entities.Image> Get(long Id)
        {
            ELODIE.Entities.Image Image = await UOW.ImageRepository.Get(Id);
            if (Image == null)
                return null;
            return Image;
        }

        public async Task<ELODIE.Entities.Image> Delete(ELODIE.Entities.Image Image)
        {
            try
            {
                await UOW.Begin();
                await UOW.ImageRepository.Delete(Image);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Image, nameof(ImageService));
                return Image;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ImageService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ELODIE.Entities.Image> Create(ELODIE.Entities.Image Image, string path)
        {
            RestClient restClient = new RestClient(InternalServices.UTILS);
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
                var response = restClient.Execute<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image.Id = response.Data.Id;
                    Image.Url = "/rpc/utils/file/download" + response.Data.Path;
                    Image.RowId = response.Data.RowId;
                }
                await UOW.Begin();
                await UOW.ImageRepository.Create(Image);
                await UOW.Commit();
                //RabbitManager.PublishSingle(Image, RoutingKeyEnum.ImageSync);             
                return Image;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ELODIE.Entities.Image> Create(ELODIE.Entities.Image Image, string path, string thumbnailPath, int width, int height)
        {
            // save original image
            RestClient restClient = new RestClient(InternalServices.UTILS);
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
                var response = restClient.Execute<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image.Id = response.Data.Id;
                    Image.Url = "/rpc/utils/file/download" + response.Data.Path;
                    Image.RowId = response.Data.RowId;
                }
            }
            catch
            {
                return null;
            }

            // save thumbnail image
            MemoryStream output = new MemoryStream();
            MemoryStream input = new MemoryStream(Image.Content);
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(input, out SixLabors.ImageSharp.Formats.IImageFormat format))
            {
                image.Mutate(x => x
                     .Resize(width, height));
                image.Save(output, format); // Automatic encoder selected based on extension.
            }

            restClient = new RestClient(InternalServices.UTILS);
            restRequest = new RestRequest("/rpc/utils/file/upload");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddFile("file", output.ToArray(), $"thumbs_{Image.Name}");
            restRequest.AddParameter("path", thumbnailPath);
            try
            {
                var response = restClient.Execute<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image.ThumbnailUrl = "/rpc/utils/file/download" + response.Data.Path;
                }
            }
            catch
            {
                return null;
            }

            await UOW.Begin();
            await UOW.ImageRepository.Create(Image);
            await UOW.Commit();
            Image.Content = null;
            //RabbitManager.PublishSingle(Image, RoutingKeyEnum.ImageSync);
            return Image;
        }

        public class File
        {
            public long Id { get; set; }
            public string Path { get; set; }
            public Guid RowId { get; set; }
        }
    }
}
