namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3StorageProviderConfiguration
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BucketName { get; set; }
        public string Folder { get; set; }
        public string Region { get; set; }

        public string GetKey(string path)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return path;
            }

            return Folder + "/" + path;
        }
    }
}