using System;
using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    public class FileUploadService
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