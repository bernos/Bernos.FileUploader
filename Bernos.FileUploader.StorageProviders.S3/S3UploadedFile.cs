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

        private string _cachedPresignedUrl;
        private DateTime _cachedPresignedUrlTimeout;

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
            // If we have a configured base url, then construct the URL from that
            if (!string.IsNullOrEmpty(_configuration.BaseUrl))
            {
                return _configuration.BaseUrl.Trim('/') + "/" + Path.Trim('/');
            }

            // If objects are stored privately, get a temporary presigned url from S3
            if (!_configuration.StoreObjectsPublicly)
            {
                // If the cached value expires in more than 10 secs, then use that
                if (!string.IsNullOrEmpty(_cachedPresignedUrl) && _cachedPresignedUrlTimeout.AddSeconds(-10) > DateTime.UtcNow)
                {
                    return _cachedPresignedUrl;
                }

                _cachedPresignedUrlTimeout = DateTime.UtcNow.AddMinutes(_configuration.PresignedUrlTimeoutMinutes);

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _configuration.BucketName,
                    Key = _configuration.GetKey(Path.Trim('/')),
                    Expires = _cachedPresignedUrlTimeout
                };

                _cachedPresignedUrl = _client.Value.GetPreSignedURL(request);

                return _cachedPresignedUrl;
            }

            // Otherwise just calculate the public S3 url
            var url = _configuration.UseHttp ? "http" : "https";

            url += "://s3.amazonaws.com/" + _configuration.BucketName;

            if (!string.IsNullOrEmpty(_configuration.Folder))
            {
                url += "/" + _configuration.Folder.Trim('/');
            }

            return url + "/" + Path.Trim('/');
        }

        protected override string GetContentType()
        {
            return _contentType;
        }
    }
}