using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3StorageProvider : IStorageProvider
    {
        private Lazy<AmazonS3Client> _client;
        private readonly S3StorageProviderConfiguration _configuration;

        public S3StorageProvider(S3StorageProviderConfiguration configuration)
        {
            _configuration = configuration;

            _client = new Lazy<AmazonS3Client>(() =>
            {
                if (!String.IsNullOrEmpty(_configuration.AccessKeyId))
                {
                   return new AmazonS3Client(_configuration.AccessKeyId, _configuration.AccessKeySecret, RegionEndpoint.GetBySystemName(_configuration.Region)); 
                }
                return new AmazonS3Client(RegionEndpoint.GetBySystemName(_configuration.Region))
            });
        }
        public UploadedFile Save(string filename, string folder, string contentType, Stream inputStream, IDictionary<string, string> metadata)
        {
            var key = string.IsNullOrEmpty(_configuration.Folder) ? "" : _configuration.Folder;

            if (!string.IsNullOrEmpty(folder))
            {
                key += "/" + folder;
            }

            key += "/" + filename;

            var request = new PutObjectRequest
            {
                AutoCloseStream = false,
                BucketName = _configuration.BucketName,
                CannedACL = S3CannedACL.Private,
                ContentType = contentType,
                Key = key,
                
                InputStream = inputStream
            };

            foreach (var kvp in metadata)
            {
                request.Metadata.Add(kvp.Key, kvp.Value);
            }

            _client.Value.PutObject(request);

            return new UploadedFile(() =>
            {
                try
                {
                    var r = new GetObjectRequest
                    {
                        BucketName = _configuration.BucketName,
                        Key = key
                    };

                    var ms = new MemoryStream();
                    
                    using (var response = _client.Value.GetObject(r))
                    {
                        response.ResponseStream.CopyTo(ms);
                    }

                    ms.Position = 0;
                    
                    return ms;
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
            }, key, "blah", contentType, metadata);
        }

        public UploadedFile Load(string path)
        {
            var key = string.IsNullOrEmpty(_configuration.Folder)
                ? path
                : _configuration.Folder + "/" + path;

            return new UploadedFile(() =>
            {
                try
                {
                    var r = new GetObjectRequest
                    {
                        BucketName = _configuration.BucketName,
                        Key = key
                    };

                    var ms = new MemoryStream();

                    using (var response = _client.Value.GetObject(r))
                    {
                        response.ResponseStream.CopyTo(ms);
                    }

                    ms.Position = 0;

                    return ms;
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
            }, key, "blah", "asdf", null);
        }

        public void Delete(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}