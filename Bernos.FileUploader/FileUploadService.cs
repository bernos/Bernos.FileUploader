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
                ValidateFileUploadRequest(request);

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

        protected virtual void ValidateFileUploadRequest(FileUploadRequest request)
        {
            if (request.InputStream.Length > _configuration.MaxFilesizeBytes)
            {
                throw new Exception(string.Format("Uploaded file size exceded configured maximum size of {0} bytes.", _configuration.MaxFilesizeBytes));
            }

            if (_configuration.AllowedContentTypes.All(c => c != request.ContentType))
            {
                throw new Exception(string.Format("File upload service is not configurated to allow uploads of type {0}.", request.ContentType));
            }
        }

        private string BuildFilename(FileUploadRequest request)
        {
            var tokens = request.Filename.Split('.');
            var filename = Guid.NewGuid().ToString();

            if (tokens.Length > 1 && _configuration.RetainFileExtensions)
            {
                filename += "." + tokens[tokens.Length - 1];
            }

            return filename;
        }


    }
}