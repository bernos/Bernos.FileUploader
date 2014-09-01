using System;
using Amazon;
using Amazon.S3;

namespace Bernos.FileUploader.StorageProviders.S3
{
    internal static class AmazonS3ClientFactory
    {
        public static AmazonS3Client CreateClient(S3StorageProviderConfiguration configuration)
        {
            if (!String.IsNullOrEmpty(configuration.AccessKeyId))
            {
                return new AmazonS3Client(configuration.AccessKeyId, configuration.AccessKeySecret, RegionEndpoint.GetBySystemName(configuration.Region));
            }
            return new AmazonS3Client(RegionEndpoint.GetBySystemName(configuration.Region));
        }
    }
}