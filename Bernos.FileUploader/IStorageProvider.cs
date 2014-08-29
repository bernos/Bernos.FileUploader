using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    /// <summary>
    /// Storage providers are responsible for saving, loading and deleting uploaded files
    /// </summary>
    public interface IStorageProvider
    {
        UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata);
        UploadedFile Load(string path);
        void Delete(string path);
    }
}