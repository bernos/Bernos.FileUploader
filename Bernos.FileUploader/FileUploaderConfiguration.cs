namespace Bernos.FileUploader
{
    public class FileUploaderConfiguration
    {
        public IStorageProvider StorageProvider { get; set; }
        public long MaxFilesizeBytes { get; set; }

        public FileUploaderConfiguration()
        {
            MaxFilesizeBytes = 1000000; // 10 Megabytes
        }
    }
}