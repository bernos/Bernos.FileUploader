using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;

namespace Bernos.FileUploader.Nancy
{
    public static class RequestExtensions
    {
        public static IEnumerable<FileUploadRequest> GetFileUploadRequests(this Request request)
        {
            return request.Files.Select(CreateFileUploadRequest);
        }

        public static IEnumerable<UploadedFile> UploadFiles(this Request request, IFileUploadService fileUploadService)
        {
            return UploadFiles(request, fileUploadService, String.Empty);
        }
        public static IEnumerable<UploadedFile> UploadFiles(this Request request, IFileUploadService fileUploadService,
            string folder)
        {
            return UploadFiles(request, fileUploadService, (file, fileUploadRequest) =>
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    fileUploadRequest.Folder = folder;
                }
            });
        }

        public static IEnumerable<UploadedFile> UploadFiles(this Request request, IFileUploadService fileUploadService, Action<HttpFile, FileUploadRequest> fileUploadRequestBuilder)
        {
            var uploadedFiles = new List<UploadedFile>();

            foreach (var file in request.Files)
            {
                var fileUploadRequest = CreateFileUploadRequest(file);

                if (fileUploadRequestBuilder != null)
                {
                    fileUploadRequestBuilder(file, fileUploadRequest);
                }

                uploadedFiles.Add(fileUploadService.UploadFile(fileUploadRequest));
            }

            return uploadedFiles;
        }

        public static Task<IEnumerable<UploadedFile>> UploadFilesAsync(this Request request, IFileUploadService fileUploadService)
        {
            return UploadFilesAsync(request, fileUploadService, String.Empty);
        }

        public static Task<IEnumerable<UploadedFile>> UploadFilesAsync(this Request request, IFileUploadService fileUploadService,
            string folder)
        {
            return UploadFilesAsync(request, fileUploadService, (file, fileUploadRequest) =>
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    fileUploadRequest.Folder = folder;
                }
            });
        }

        public static async Task<IEnumerable<UploadedFile>> UploadFilesAsync(this Request request, IFileUploadService fileUploadService, Action<HttpFile, FileUploadRequest> fileUploadRequestBuilder)
        {
            var tasks = new List<Task<UploadedFile>>();

            foreach (var file in request.Files)
            {
                var fileUploadRequest = CreateFileUploadRequest(file);

                if (fileUploadRequestBuilder != null)
                {
                    fileUploadRequestBuilder(file, fileUploadRequest);
                }

                tasks.Add(fileUploadService.UploadFileAsync(fileUploadRequest));
            }

            return await Task.WhenAll(tasks);
        }

        private static FileUploadRequest CreateFileUploadRequest(HttpFile file)
        {
            return new FileUploadRequest
            {
                Filename = file.Name,
                Folder = "",
                InputStream = file.Value,
                ContentType = file.ContentType,
                Metadata = new Dictionary<string, string>()
            };
        }
    }
}