using System.Collections.Generic;
using System.IO;

namespace Bernos.FileUploader
{
    /// <summary>
    /// A file upload request, to be handled by our FileUploadService.
    /// </summary>
    public class FileUploadRequest
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream InputStream { get; set; }
        public string Folder { get; set; }
        public IDictionary<string, string> Metadata { get; set; }

        public FileUploadRequest()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}