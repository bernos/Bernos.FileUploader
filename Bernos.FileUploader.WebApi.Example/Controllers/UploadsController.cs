using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bernos.FileUploader.WebApi.Example.Controllers
{
    public class UploadsController : ApiController
    {
        private readonly FileUploadService _fileUploadService;

        public async Task<HttpResponseMessage> Post()
        {

            var streamProvider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            var results = new Collection<UploadedFile>();


            foreach (var content in streamProvider.Contents)
            {
                using (var stream = await content.ReadAsStreamAsync())
                {
                    var result = _fileUploadService.UploadFile(new FileUploadRequest
                    {
                        Filename = content.Headers.ContentDisposition.FileName.Trim('"'),
                        Folder = "",
                        InputStream = stream,
                        ContentType = content.Headers.ContentType.MediaType
                    });

                    results.Add(result);
                }
            }

            var response = Request.CreateResponse<ICollection<UploadedFile>>(HttpStatusCode.Created, results);

            return response;
            /*
             var result = uploadService.UploadFile(new FileUploadRequest
                    {
                        Filename = file.Name,
                        Folder = "",
                        InputStream = file.Value,
                        ContentType = file.ContentType,
                        Metadata = metadata
                    });
             
             */
        }
    }
}
