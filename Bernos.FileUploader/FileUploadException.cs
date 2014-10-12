using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    public enum FileUploadErrorCode
    {
        NoContent,
        IllegalContentType,
        IllegalFileSize
    }

    [Serializable]
    public class FileUploadException : Exception
    {
        private readonly FileUploadErrorCode _errorCode;

        public FileUploadErrorCode ErrorCode
        {
            get { return _errorCode; }
        }

        public override string Message
        {
            get
            {
                switch (ErrorCode)
                {
                    case FileUploadErrorCode.IllegalContentType :
                        return "Content type not allowed for upload.";

                    case FileUploadErrorCode.IllegalFileSize :
                        return "File exceeds maximum allowable upload size.";

                    case FileUploadErrorCode.NoContent :
                        return "Uploaded file is empty.";
                }

                return string.Empty;
            }
        }

        public FileUploadException(FileUploadErrorCode errorCode)
        {
            _errorCode = errorCode;
        }
        
        public FileUploadException(FileUploadErrorCode errorCode, Exception inner) : base(string.Empty, inner)
        {
            _errorCode = errorCode;
        }

        protected FileUploadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
