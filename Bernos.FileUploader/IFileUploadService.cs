using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    public interface IFileUploadService
    {
        UploadedFile UploadFile(FileUploadRequest request);
        Task<UploadedFile> UploadFileAsync(FileUploadRequest request);
        bool DeleteFile(string path);
        UploadedFile GetUploadedFile(string path);
    }
}