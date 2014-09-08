using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FileUploaderConfiguration _configuration;

        public FileUploadService(FileUploaderConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FileUploadResponse UploadFile(FileUploadRequest request)
        {
            request.Metadata.Add("filename", request.Filename);

            try
            {
                var uploadedFile = _configuration.StorageProvider.Save(BuildFilename(request), request.Folder,
                    request.ContentType, request.InputStream, request.Metadata);

                return new FileUploadResponse(uploadedFile);
            }
            catch(Exception e)
            {
                return new FileUploadResponse(e);
            }
        }

        public async Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request)
        {
            request.Metadata.Add("filename", request.Filename);

            try
            {
                var uploadedFile = await _configuration.StorageProvider.SaveAsync(BuildFilename(request), request.Folder, request.ContentType, request.InputStream, request.Metadata);

                return new FileUploadResponse(uploadedFile);
            }
            catch (Exception e)
            {
                return new FileUploadResponse(e);
            }
        }

        public bool DeleteFile(string path)
        {
            return _configuration.StorageProvider.Delete(path);
        }

        public UploadedFile GetUploadedFile(string path)
        {
            return _configuration.StorageProvider.Load(path);
        }

        private string BuildFilename(FileUploadRequest request)
        {
            var tokens = request.Filename.Split('.');
            var filename = Guid.NewGuid().ToString();

            if (tokens.Length > 1)
            {
                filename += "." + tokens[tokens.Length - 1];
            }

            return filename;
        }
    }
}