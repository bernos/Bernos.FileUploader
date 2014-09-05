using System;
using System.Collections.Generic;

namespace Bernos.FileUploader
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FileUploaderConfiguration _configuration;

        public FileUploadService(FileUploaderConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UploadedFile UploadFile(FileUploadRequest request)
        {
            var tokens = request.Filename.Split('.');
            var filename = Guid.NewGuid().ToString();

            if (tokens.Length > 1)
            {
                filename += "." + tokens[tokens.Length - 1];
            }

            if (request.Metadata == null)
            {
                request.Metadata = new Dictionary<string, string>();
            }

            request.Metadata.Add("filename", request.Filename);

            return _configuration.StorageProvider.Save(filename, request.Folder, request.ContentType, request.InputStream, request.Metadata);
        }

        public bool DeleteFile(string path)
        {
            return _configuration.StorageProvider.Delete(path);
        }

        public UploadedFile GetUploadedFile(string path)
        {
            return _configuration.StorageProvider.Load(path);
        }
    }
}