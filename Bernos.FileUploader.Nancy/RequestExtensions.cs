using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Bernos.FileUploader.Nancy
{
    public static class RequestExtensions
    {
        public static IEnumerable<FileUploadRequest> GetFileUploadRequests(this Request request)
        {
            return request.Files.Select(f => new FileUploadRequest
            {
                Filename = f.Name,
                Folder = "",
                InputStream = f.Value,
                ContentType = f.ContentType,
                Metadata = new Dictionary<string, string>()
            });
        } 
    }
}