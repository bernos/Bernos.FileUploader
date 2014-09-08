using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    /// <summary>
    /// Storage providers are responsible for saving, loading and deleting uploaded files
    /// </summary>
    public interface IStorageProvider
    {
        Task<UploadedFile> SaveAsync(string filename, string folder, string contentType, Stream inputStream,
            IDictionary<string, string> metadata);
        UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata);
        UploadedFile Load(string path);
        bool Delete(string path);
    }
}