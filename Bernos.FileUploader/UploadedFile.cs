using System;
using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    public abstract class UploadedFile
    {
        public abstract Stream AsStream();

        public string Path
        {
            get { return GetPath(); }
        }

        public IDictionary<string, string> Metadata
        {
            get { return GetMetadata(); }
        }

        public string Url
        {
            get { return GetUrl(); }
        }

        public string ContentType
        {
            get { return GetContentType(); }
        }

        protected abstract string GetPath();
        protected abstract IDictionary<string, string> GetMetadata();
        protected abstract string GetUrl();
        protected abstract string GetContentType();
    }
}