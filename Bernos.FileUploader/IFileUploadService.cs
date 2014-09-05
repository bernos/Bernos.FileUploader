namespace Bernos.FileUploader
{
    public interface IFileUploadService
    {
        UploadedFile UploadFile(FileUploadRequest request);
        bool DeleteFile(string path);
        UploadedFile GetUploadedFile(string path);
    }
}