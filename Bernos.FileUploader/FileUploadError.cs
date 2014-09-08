using System;

namespace Bernos.FileUploader
{
    public class FileUploadError
    {
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        public FileUploadError(Exception e)
        {
            Message = e.Message;
            StackTrace = e.StackTrace;
        }
    }
}