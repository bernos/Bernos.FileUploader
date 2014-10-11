using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bernos.FileUploader.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        public static async Task<IEnumerable<FileUploadResponse>> UploadFilesAsync(this HttpRequestMessage request, IFileUploadService fileUploadService)
        {
            return await UploadFilesAsync(request, fileUploadService, string.Empty);
        }

        public static async Task<IEnumerable<FileUploadResponse>> UploadFilesAsync(this HttpRequestMessage request,
            IFileUploadService fileUploadService, string folder)
        {
            return await UploadFilesAsync(request, fileUploadService, (content, fileUploadRequest) =>
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    fileUploadRequest.Folder = folder;
                }
            });
        }

        public static async Task<IEnumerable<FileUploadResponse>> UploadFilesAsync(this HttpRequestMessage request, IFileUploadService fileUploadService, Action<HttpContent, FileUploadRequest> fileUploadRequestBuilder)
        {
            var streamProvider = new MultipartMemoryStreamProvider();

            await request.Content.ReadAsMultipartAsync(streamProvider);

            var tasks = streamProvider.Contents.Select(content => Task.Run(async () =>
            {
                using (var stream = await content.ReadAsStreamAsync())
                {
                    var fileUploadRequest = CreateFileUploadRequest(content, stream);

                    if (fileUploadRequestBuilder != null)
                    {
                        fileUploadRequestBuilder(content, fileUploadRequest);
                    }

                    return await fileUploadService.UploadFileAsync(fileUploadRequest);
                }
            })).ToList();

            return await Task.WhenAll(tasks);
        }

        private static FileUploadRequest CreateFileUploadRequest(HttpContent content, Stream stream)
        {
            return new FileUploadRequest
            {
                Filename = content.Headers.ContentDisposition.FileName.Trim('"'),
                Folder = "",
                InputStream = stream,
                ContentType = content.Headers.ContentType.MediaType,
                Metadata = new Dictionary<string, string>()
            };
        }
    }
}