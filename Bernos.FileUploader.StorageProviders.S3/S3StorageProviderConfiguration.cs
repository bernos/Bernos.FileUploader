using Amazon;
using System;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3StorageProviderConfiguration
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BucketName { get; set; }
        public string Folder { get; set; }
        public string Region { get; set; }
        public string BaseUrl { get; set; }
        public bool StoreObjectsPublicly { get; set; }
        public int PresignedUrlTimeoutMinutes { get; set; }
        public bool UseHttp { get; set; }
        public string GetKey(string path)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return path;
            }

            return Folder + "/" + path;
        }

        public S3StorageProviderConfiguration(string bucketName, string region) : this(bucketName, region, "") { }

        public S3StorageProviderConfiguration(string bucketName, string region, string folder)
        {
            var theRegion = RegionEndpoint.GetBySystemName(region);

            if (theRegion.DisplayName.ToLower() == "unknown")
            {
                throw new ArgumentException("Please specify a valid AWS region", "region");
            }

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentException("Please specify a valid AWS bucket name", "bucket");
            }

            if (!string.IsNullOrEmpty(folder)) {
                Folder = folder;
            }

            BucketName = bucketName;
            Region = region;
            PresignedUrlTimeoutMinutes = 30;
        }
    }
}