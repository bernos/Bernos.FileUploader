using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3StorageProvider : IStorageProvider
    {
        private const string MetadataHeaderPrefix = "x-amz-meta-";

        private readonly Lazy<AmazonS3Client> _client;
        private readonly S3StorageProviderConfiguration _configuration;

        public S3StorageProvider(S3StorageProviderConfiguration configuration)
        {
            _configuration = configuration;
            _client = new Lazy<AmazonS3Client>(() => AmazonS3ClientFactory.CreateClient(_configuration));
        }

        public UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            var path = string.IsNullOrEmpty(folder) ? filename : folder + "/" + filename;
            var transferUtility = new TransferUtility(_client.Value);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                AutoCloseStream = false,
                BucketName = _configuration.BucketName,
                CannedACL = _configuration.StoreObjectsPublicly ? S3CannedACL.PublicRead : S3CannedACL.Private,
                ContentType = contentType,
                Key = _configuration.GetKey(path),
                InputStream = inputStream
            };

            foreach (var kvp in metadata)
            {
                uploadRequest.Metadata.Add(kvp.Key, kvp.Value);
            }

            transferUtility.Upload(uploadRequest);

            return new S3UploadedFile(_configuration, path, contentType, metadata);
        }

        public UploadedFile Load(string path)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _configuration.BucketName,
                Key = _configuration.GetKey(path)
            };

            try
            {
                var response = _client.Value.GetObjectMetadata(request);
                var contentType = response.Headers.ContentType;
                var metadata = response.Metadata.Keys.ToDictionary(key => key.Substring(MetadataHeaderPrefix.Length), key => response.Metadata[key]);
                
                return new S3UploadedFile(_configuration, path, contentType, metadata);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public bool Delete(string path)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _configuration.BucketName,
                Key = _configuration.GetKey(path)
            };

            try
            {
                _client.Value.DeleteObject(request);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }

            return true;
        }
    }
}