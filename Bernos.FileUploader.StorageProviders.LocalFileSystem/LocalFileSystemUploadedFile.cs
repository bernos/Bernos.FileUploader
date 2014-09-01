using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader.StorageProviders.LocalFileSystem
{
    public class LocalFileSystemUploadedFile : UploadedFile
    {
        private readonly LocalFileSystemStorageProviderConfiguration _configuration;
        private readonly IPathProvider _rootPathProvider;
        private readonly string _path;
        private readonly string _contentType;
        private readonly IDictionary<string, string> _metadata;

        public LocalFileSystemUploadedFile(LocalFileSystemStorageProviderConfiguration configuration, IPathProvider rootPathProvider, string path, string contentType, IDictionary<string, string> metadata)
        {
            _configuration = configuration;
            _rootPathProvider = rootPathProvider;
            _path = path;
            _contentType = contentType;
            _metadata = metadata;
        }

        public override Stream AsStream()
        {
            var destination = System.IO.Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, Path);
            return new FileStream(destination, FileMode.Open);
        }

        protected override string GetPath()
        {
            return _path;
        }

        protected override IDictionary<string, string> GetMetadata()
        {
            return _metadata;
        }

        protected override string GetUrl()
        {
            return _configuration.BaseUrl + "/" + Path;
        }

        protected override string GetContentType()
        {
            return _contentType;
        }
    }
}