﻿using System.IO;
using Nancy;
using System.Collections.Generic;

namespace Bernos.FileUploader.Nancy.Example
{

    public class IndexModule : NancyModule
    {
        public IndexModule(IFileUploadService uploadService)
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Post["/", true] = async (_, ct) =>
            {
                /*
                var results = new List<UploadedFile>();
                
                foreach (var fileUploadRequest in this.Request.GetFileUploadRequests())
                {
                    fileUploadRequest.Metadata.Add("name", "blah");
                    fileUploadRequest.Folder = "/something";

                    var result = await uploadService.UploadFileAsync(fileUploadRequest);

                    results.Add(result);
                }
                
                return Response.AsJson(results);
                */

                return Response.AsJson(await Request.UploadFilesAsync(uploadService, "/a-folder"));
            };

            Get["/uploads/{filepath}"] = _ =>
            {
                string filepath = _.filepath.TryParse<string>("");
                var file = uploadService.GetUploadedFile(filepath);

                if (file == null)
                {
                    return HttpStatusCode.NotFound;
                }

                return Response.AsJson(file);

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

                if (uploadService.DeleteFile(filepath))
                {
                    return HttpStatusCode.NoContent;
                }

                return HttpStatusCode.NotFound;
            };
        }
    }
}