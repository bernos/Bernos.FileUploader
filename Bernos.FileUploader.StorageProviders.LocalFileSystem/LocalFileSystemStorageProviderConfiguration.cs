namespace Bernos.FileUploader.StorageProviders.LocalFileSystem
{
    public class LocalFileSystemStorageProviderConfiguration
    {
        public string UploadPath { get; set; }
        public string BaseUrl { get; set; }
        
        public LocalFileSystemStorageProviderConfiguration()
        {
            UploadPath = "Content/uploads";
            BaseUrl = "/Content/uploads";
        }
    }
}