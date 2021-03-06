﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            _client = new Lazy<AmazonS3Client>(() => _configuration.ClientFactory.CreateClient(_configuration));
        }

        private TransferUtilityUploadRequest PrepareUploadRequest(string path, string contentType,
            Stream inputStream, IEnumerable<KeyValuePair<string, string>> metadata)
        {   
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

            return uploadRequest;
        }

        public async Task<UploadedFile> SaveAsync(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            var path = BuildUploadPath(folder, filename);
            var transferUtility = _configuration.TransferUtilityFactory.CreateTransferUtility(_client.Value);
            var uploadRequest = PrepareUploadRequest(path, contentType, inputStream, metadata);
            
            await transferUtility.UploadAsync(uploadRequest);

            return new S3UploadedFile(_configuration, path, contentType, metadata);
        }

        public UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            var path = BuildUploadPath(folder, filename);
            var transferUtility = _configuration.TransferUtilityFactory.CreateTransferUtility(_client.Value);
            var uploadRequest = PrepareUploadRequest(path, contentType, inputStream, metadata);

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

        private string BuildUploadPath(string folder, string filename)
        {
            return string.IsNullOrEmpty(folder) ? filename : folder.Trim('/') + "/" + filename;
        }
    }
}