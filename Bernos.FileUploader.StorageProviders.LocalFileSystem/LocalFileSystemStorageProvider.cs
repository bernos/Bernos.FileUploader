using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Bernos.FileUploader.StorageProviders.LocalFileSystem
{
    /// <summary>
    /// An implementation of IStorageProvider that uses the local filesystem to persist
    /// uploaded files. Metadata is also serialized to the local filesystem alongside the
    /// actual uploaded files.
    /// </summary>
    public class LocalFileSystemStorageProvider : IStorageProvider
    {
        private const string ContentTypeMetadataKey = "content-type";

        private readonly LocalFileSystemStorageProviderConfiguration _configuration;
        private readonly IPathProvider _rootPathProvider;

        public LocalFileSystemStorageProvider(LocalFileSystemStorageProviderConfiguration configuration, IPathProvider rootPathProvider)
        {
            _configuration = configuration;
            _rootPathProvider = rootPathProvider;
        }

        public UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            var relativeFolder = EnsurePathIsRelative(folder);
            var destinationFolder = Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, relativeFolder);

            var path = string.IsNullOrEmpty(relativeFolder)
                ? filename
                : string.Format("{0}/{1}", relativeFolder, filename);
            
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            using (var destinationStream = new FileStream(Path.Combine(destinationFolder, filename), FileMode.Create))
            {
                inputStream.CopyTo(destinationStream);
            }

            if (metadata == null)
            {
                metadata = new Dictionary<string, string>();
            }

            // Copy the content type to the metadata before saving
            metadata[ContentTypeMetadataKey] = contentType;

            SaveMetadata(filename, folder, metadata);

            return new LocalFileSystemUploadedFile(_configuration, _rootPathProvider, path, contentType, metadata);
        }

        public UploadedFile Load(string path)
        {
            var filename = Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, path);

            if (!File.Exists(filename))
            {
                return null;
            }
            
            // Load up metadata and use the content type that was saved there when building up
            // the UploadedFile
            var metadata = LoadMetadata(path);

            return new LocalFileSystemUploadedFile(_configuration, _rootPathProvider, path, metadata[ContentTypeMetadataKey], metadata);
        }

        public bool Delete(string path)
        {
            var file = Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, path);

            if (!File.Exists(file))
            {
                return false;
            }

            File.Delete(file);

            return true;
        }
        
        private string EnsurePathIsRelative(string path)
        {
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                return path.Substring(1);
            }

            return path;
        }

        private void SaveMetadata(string filename, string folder, IDictionary<string, string> metadata)
        {
            var relativeFolder = EnsurePathIsRelative(folder);
            var destinationFolder = Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, relativeFolder);
            var formatter = new BinaryFormatter();
            using (var destinationStream = new FileStream(Path.Combine(destinationFolder, filename + ".metadata"), FileMode.Create))
            {
                formatter.Serialize(destinationStream, metadata);    
            }
        }

        private IDictionary<string, string> LoadMetadata(string path)
        {
            var formatter = new BinaryFormatter();
            var filename = Path.Combine(_rootPathProvider.GetRootPath(), _configuration.UploadPath, path + ".metadata");

            using (var inputStream = new FileStream(filename, FileMode.Open))
            {
                return (IDictionary<string,string>)formatter.Deserialize(inputStream);
            }
        } 
    }
}