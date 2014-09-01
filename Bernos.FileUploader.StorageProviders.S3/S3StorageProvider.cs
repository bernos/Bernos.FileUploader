﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.Runtime;
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
            _client = new Lazy<AmazonS3Client>(GetClient);
        }

        public UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            // TODO: Add config param to determine whether uploads are public or private.

            var path = string.IsNullOrEmpty(folder) ? filename : folder + "/" + filename;
            var transferUtility = new TransferUtility(_client.Value);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                AutoCloseStream = false,
                BucketName = _configuration.BucketName,
                CannedACL = S3CannedACL.Private,
                ContentType = contentType,
                Key = _configuration.GetKey(path),
                InputStream = inputStream
            };

            foreach (var kvp in metadata)
            {
                uploadRequest.Metadata.Add(kvp.Key, kvp.Value);
            }

            transferUtility.Upload(uploadRequest);

            return BuildUploadedFile(path, contentType, metadata);
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

                return BuildUploadedFile(path, contentType, metadata);
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

        public void Delete(string path)
        {
            throw new System.NotImplementedException();
        }

        private AmazonS3Client GetClient()
        {
            if (!String.IsNullOrEmpty(_configuration.AccessKeyId))
            {
                return new AmazonS3Client(_configuration.AccessKeyId, _configuration.AccessKeySecret, RegionEndpoint.GetBySystemName(_configuration.Region));
            }
            return new AmazonS3Client(RegionEndpoint.GetBySystemName(_configuration.Region));
        }

        private UploadedFile BuildUploadedFile(string path, string contentType, IDictionary<string, string> metadata)
        {
            // TODO: set up url correctly. Add a "Use private" bool to config. If it is set then we will need to calculate the public URL, otherwise we
            // can just use the default url
            
            return new UploadedFile(() =>
            {
                try
                {
                    using (var client = GetClient())
                    {
                        var r = new GetObjectRequest
                        {
                            BucketName = _configuration.BucketName,
                            Key = _configuration.GetKey(path)
                        };

                        var ms = new MemoryStream();

                        using (var response = client.GetObject(r))
                        {
                            response.ResponseStream.CopyTo(ms);
                        }

                        ms.Position = 0;

                        return ms;
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }

                    //status wasn't not found, so throw the exception
                    throw;
                }
            }, path, "blah", contentType, metadata);
        }
    }
}