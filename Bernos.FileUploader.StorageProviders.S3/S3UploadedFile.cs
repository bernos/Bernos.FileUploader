using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class S3UploadedFile : UploadedFile
    {
        private readonly S3StorageProviderConfiguration _configuration;
        private readonly string _path;
        private readonly string _contentType;
        private readonly Lazy<AmazonS3Client> _client;
        private readonly IDictionary<string, string> _metadata; 
        
        public S3UploadedFile(S3StorageProviderConfiguration configuration, string path, string contentType, IDictionary<string,string> metadata)
        {
            _configuration = configuration;
            _path = path;
            _contentType = contentType;
            _metadata = metadata;
            _client = new Lazy<AmazonS3Client>(() => AmazonS3ClientFactory.CreateClient(_configuration));
        }

        public override Stream AsStream()
        {
            try
            {
                var r = new GetObjectRequest
                {
                    BucketName = _configuration.BucketName,
                    Key = _configuration.GetKey(Path)
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
        }

        protected override string GetPath()
        {
            return _path;
        }

        protected override IDictionary<string, string> GetMetadata()
        {
            return _metadata;
        }

        protected override string GetUrl()
        {
            if (string.IsNullOrEmpty(_configuration.BaseUrl))
            {
                var url = "https://s3.amazonaws.com/" + _configuration.BucketName;

                if (!string.IsNullOrEmpty(_configuration.Folder))
                {
                    url += "/" + _configuration.Folder;
                }

                return url + "/" + Path;
            }

            return _configuration.BaseUrl + "/" + Path;
        }

        protected override string GetContentType()
        {
            return _contentType;
        }
    }
}