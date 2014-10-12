using System;

namespace Bernos.FileUploader
{
    public class FileUploadResponse
    {
        public UploadedFile File { get; private set; }
        public FileUploadException Error { get; private set; }

        public FileUploadResponse(UploadedFile file)
        {
            File = file;
        }

        public FileUploadResponse(FileUploadException e)
        {
            Error = e;
        }
    }
}