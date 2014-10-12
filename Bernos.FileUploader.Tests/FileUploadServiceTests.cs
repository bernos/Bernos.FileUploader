using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using NSubstitute;
using Xunit;

namespace Bernos.FileUploader.Tests
{
    public class FileUploadServiceTests
    {
        [Fact]
        public void Constructor_Should_Throw_For_Invalid_Configuration()
        {
            Assert.Throws<ArgumentException>(() => new FileUploadService(new FileUploaderConfiguration()));
        }

        [Fact]
        public void Should_Return_Error_For_Disallowed_Content_Type()
        {
            var configuration = new FileUploaderConfiguration
            {
                StorageProvider = Substitute.For<IStorageProvider>()
            };

            var service = new FileUploadService(configuration);
            var request = new FileUploadRequest
            {
                ContentType = "arse",
                InputStream = new MemoryStream()
            };

            var result = service.UploadFile(request);

            Assert.True(result.Error != null && result.Error.ErrorCode == FileUploadErrorCode.IllegalContentType);
        }

        [Fact]
        public void Should_Return_Error_For_Null_Stream()
        {
            var configuration = new FileUploaderConfiguration
            {
                StorageProvider = Substitute.For<IStorageProvider>(),
                AllowedContentTypes = new Collection<string>
                {
                    "image/jpeg"
                }
            };

            var service = new FileUploadService(configuration);
            var request = new FileUploadRequest
            {
                ContentType = "image/jpeg"
            };

            var result = service.UploadFile(request);

            Assert.True(result.Error != null && result.Error.ErrorCode == FileUploadErrorCode.NoContent);
        }

        [Fact]
        public void Should_Return_Error_If_File_Too_Large()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                writer.Flush();

                stream.Position = 0;

                var configuration = new FileUploaderConfiguration
                {
                    StorageProvider = Substitute.For<IStorageProvider>(),
                    MaxFilesizeBytes = 1,
                    AllowedContentTypes = new Collection<string>
                    {
                        "image/jpeg"
                    }
                };

                var service = new FileUploadService(configuration);
                var request = new FileUploadRequest
                {
                    ContentType = "image/jpeg",
                    InputStream = stream
                };

                var result = service.UploadFile(request);

                Assert.True(result.Error != null && result.Error.ErrorCode == FileUploadErrorCode.IllegalFileSize);
            }
        }
    }
}