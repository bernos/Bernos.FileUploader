using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Bernos.FileUploader.StorageProviders.LocalFileSystem;

namespace Bernos.FileUploader.WebApi.Example.Controllers
{
    public class UploadsController : ApiController
    {
        private readonly FileUploadService _fileUploadService;

        public UploadsController()
        {
            _fileUploadService = new FileUploadService(new FileUploaderConfiguration
            {
                StorageProvider = new LocalFileSystemStorageProvider(new LocalFileSystemStorageProviderConfiguration(), new DefaultPathProvider(HttpContext.Current.Server.MapPath("~/")))
            });
        }

        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var results = await Request.UploadFilesAsync(_fileUploadService);

            return Request.CreateResponse(HttpStatusCode.Created, results);
        }
    }

}
