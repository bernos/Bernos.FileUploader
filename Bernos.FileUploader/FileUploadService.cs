using System;
using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    public class FileUploadService
    {
        private readonly IStorageProvider _storageProvider;

        public FileUploadService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public UploadedFile UploadFile(FileUploadRequest request)
        {
            var tokens = request.Filename.Split('.');
            var filename = Guid.NewGuid().ToString();

            if (tokens.Length > 1)
            {
                filename += "." + tokens[tokens.Length - 1];
            }

            return _storageProvider.Save(filename, request.Folder, request.ContentType, request.InputStream, request.Metadata);
        }

        public void DeleteFile(string path)
        {
            _storageProvider.Delete(path);
        }

        public UploadedFile GetUploadedFile(string path)
        {
            return _storageProvider.Load(path);
        }
    }
}