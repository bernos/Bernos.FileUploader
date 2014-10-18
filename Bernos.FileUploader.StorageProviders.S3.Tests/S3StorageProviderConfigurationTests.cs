using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bernos.FileUploader.StorageProviders.S3.Tests
{
    public class S3StorageProviderConfigurationTests
    {
        [Fact]
        public void Should_Throw_For_Invalid_Region()
        {
            Assert.Throws<ArgumentException>(() => new S3StorageProviderConfiguration("bucket", "invalid-region", ""));
        }

        [Fact]
        public void Should_Construct_For_Valid_Region()
        {
            Assert.DoesNotThrow(() => new S3StorageProviderConfiguration("bucket", "ap-southeast-1", ""));
        }

        [Fact]
        public void Should_Throw_For_Invalid_BucketName()
        {
            Assert.Throws<ArgumentException>(() => new S3StorageProviderConfiguration("", "ap-southeast-1", ""));
        }

        [Fact]
        public void Should_Load_From_App_Settings()
        {
            var configuration = S3StorageProviderConfiguration.FromAppSettings();

            Assert.Equal(ConfigurationManager.AppSettings["S3StorageProvider:Bucket"], configuration.BucketName);
        }
    }
}
