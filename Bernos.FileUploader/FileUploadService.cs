using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FileUploaderConfiguration _configuration;

        public FileUploadService(FileUploaderConfiguration configuration)
        {
            if (configuration.StorageProvider == null)
            {
                throw new ArgumentException("No storage provider has been configured.", "configuration")
                    ;
            }
            _configuration = configuration;
        }

        public FileUploadResponse UploadFile(FileUploadRequest request)
        {
            request.Metadata.Add("filename", request.Filename);

            try
            {
                ValidateFileUploadRequest(request);

                var uploadedFile = _configuration.StorageProvider.Save(BuildFilename(request), request.Folder,
                    request.ContentType, request.InputStream, request.Metadata);

                return new FileUploadResponse(uploadedFile);
            }
            catch(FileUploadException e)
            {
                return new FileUploadResponse(e);
            }
        }

        public async Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request)
        {
            try
            {
                ValidateFileUploadRequest(request);

                var uploadedFile = await _configuration.StorageProvider.SaveAsync(BuildFilename(request), request.Folder, request.ContentType, request.InputStream, request.Metadata);

                return new FileUploadResponse(uploadedFile);
            }
            catch (FileUploadException e)
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

        protected virtual void ValidateFileUploadRequest(FileUploadRequest request)
        {
            if (request.InputStream == null)
            {
                throw new FileUploadException(FileUploadErrorCode.NoContent);
            }

            if (request.InputStream.Length > _configuration.MaxFilesizeBytes)
            {
                throw new FileUploadException(FileUploadErrorCode.IllegalFileSize);
            }

            if (_configuration.AllowedContentTypes.All(c => c != request.ContentType))
            {
                throw new FileUploadException(FileUploadErrorCode.IllegalContentType);
            }
        }

        private string BuildFilename(FileUploadRequest request)
        {
            return string.IsNullOrEmpty(request.Filename) ? Guid.NewGuid().ToString() : request.Filename;
        }
    }
}