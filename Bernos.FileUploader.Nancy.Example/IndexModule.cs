using System.IO;
using Nancy;
using System.Collections.Generic;

namespace Bernos.FileUploader.Nancy.Example
{

    public class IndexModule : NancyModule
    {
        public IndexModule(FileUploadService uploadService)
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Post["/"] = _ =>
            {
                var results = new List<UploadedFile>();
                
                foreach (var file in this.Request.Files)
                {
                    var metadata = new Dictionary<string, string>
                    {
                        {"name", "brendan" }
                    };
                    
                    var result = uploadService.UploadFile(new FileUploadRequest
                    {
                        Filename = file.Name,
                        Folder = "",
                        InputStream = file.Value,
                        ContentType = file.ContentType,
                        Metadata = metadata
                    });

                    results.Add(result);
                }

                return Response.AsJson(results);
            };

            Get["/uploads/{filepath}"] = _ =>
            {
                string filepath = _.filepath.TryParse<string>("");
                var file = uploadService.GetUploadedFile(filepath);

                if (file == null)
                {
                    return HttpStatusCode.NotFound;
                }

                //return Response.AsRedirect(file.Url);

                var response = new Response();
                response.Contents = s =>
                {
                    using (var input = uploadService.GetUploadedFile(filepath).AsStream())
                    {
                        input.CopyTo(s);
                    }
                };

                response.ContentType = file.Metadata["content-type"];

                return response;
            };

            Delete["/uploads/{filepath}"] = _ =>
            {
                string filepath = _.filepath.TryParse<string>("");

                try
                {
                    uploadService.DeleteFile(filepath);
                    return HttpStatusCode.NoContent;
                }
                catch (FileNotFoundException ex)
                {
                    return HttpStatusCode.NotFound;
                }
            };
        }
    }
}