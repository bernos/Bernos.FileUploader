using System;

namespace Bernos.FileUploader
{
    public class FileUploadResponse
    {
        public UploadedFile File { get; private set; }
        public Exception Error { get; private set; }

        public FileUploadResponse(UploadedFile file)
        {
            File = file;
        }

        public FileUploadResponse(Exception e)
        {
            Error = e;
        }
    }
}