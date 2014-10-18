using System.Configuration;
using Amazon;
using System;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3StorageProviderConfiguration
    {
        public static S3StorageProviderConfiguration FromAppSettings()
        {
            var bucketName = ConfigurationManager.AppSettings["S3StorageProvider:Bucket"];
            var region = ConfigurationManager.AppSettings["S3StorageProvider:Region"];
            var folder = ConfigurationManager.AppSettings["S3StorageProvider:Folder"];

            var configuration = new S3StorageProviderConfiguration(bucketName, region, folder);

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:AccessKeyId"]))
            {
                configuration.AccessKeyId = ConfigurationManager.AppSettings["S3StorageProvider:AccessKeyId"];
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:AccessKeySecret"]))
            {
                configuration.AccessKeySecret = ConfigurationManager.AppSettings["S3StorageProvider:AccessKeySecret"];
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:BaseUrl"]))
            {
                configuration.BaseUrl = ConfigurationManager.AppSettings["S3StorageProvider:BaseUrl"];
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:StoreObjectsPublicly"]))
            {
                configuration.StoreObjectsPublicly = bool.Parse(ConfigurationManager.AppSettings["S3StorageProvider:StoreObjectsPublicly"]);
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:PresignedUrlTimeoutMinutes"]))
            {
                configuration.PresignedUrlTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["S3StorageProvider:PresignedUrlTimeoutMinutes"]);
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["S3StorageProvider:UseHttp"]))
            {
                configuration.UseHttp = bool.Parse(ConfigurationManager.AppSettings["S3StorageProvider:UseHttp"]);
            }

            return configuration;
        }

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

        private AmazonS3ClientFactory _clientFactory;

        public AmazonS3ClientFactory ClientFactory
        {
            get
            {
                if (_clientFactory == null)
                {
                    _clientFactory = new AmazonS3ClientFactory();
                }
                return _clientFactory;
            }

            set { _clientFactory = value; }
        }

        private TransferUtilityFactory _transferUtilityFactory;

        public TransferUtilityFactory TransferUtilityFactory
        {
            get
            {
                if (_transferUtilityFactory == null)
                {
                    _transferUtilityFactory = new TransferUtilityFactory();
                }
                return _transferUtilityFactory;
            }

            set { _transferUtilityFactory = value; }
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