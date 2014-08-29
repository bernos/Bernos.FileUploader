using System;
using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    public class UploadedFile
    {
        private readonly Func<Stream> _content;
        private readonly string _path;
        private readonly IDictionary<string, string> _metadata;
        private readonly string _url;
        private readonly string _contentType;

        public Stream AsStream()
        {
            return _content();
        }

        public string Path
        {
            get { return _path; }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metadata; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string ContentType
        {
            get { return _contentType; }
        }

        public UploadedFile(Func<Stream> content, string path, string url, string contentType, IDictionary<string,string> metadata)
        {
            _url = url;
            _content = content;
            _path = path;
            _contentType = contentType;
            _metadata = metadata;
        }
    }
}