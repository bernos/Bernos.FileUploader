using System;

namespace Bernos.FileUploader
{
    public class FileUploadResponse
    {
        public UploadedFile File { get; private set; }
        public FileUploadError Error { get; private set; }

        public FileUploadResponse(UploadedFile file)
        {
            File = file;
        }

        public FileUploadResponse(Exception e)
        {
            Error = new FileUploadError(e);
        }
    }
}