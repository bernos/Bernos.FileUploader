using System.Collections;
using System.Collections.Generic;

namespace Bernos.FileUploader
{
    public class FileUploaderConfiguration
    {
        public IStorageProvider StorageProvider { get; set; }
        public long MaxFilesizeBytes { get; set; }

        public ICollection<string> AllowedContentTypes { get; set; }

        public FileUploaderConfiguration()
        {
            MaxFilesizeBytes = 1000000; // 10 Megabytes
            AllowedContentTypes = new List<string>
            {
                "image/jpeg",
                "image/png"
            };
        }
    }
}