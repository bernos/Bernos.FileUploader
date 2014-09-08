using System.Threading.Tasks;

namespace Bernos.FileUploader
{
    public interface IFileUploadService
    {
        FileUploadResponse UploadFile(FileUploadRequest request);
        Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request);
        bool DeleteFile(string path);
        UploadedFile GetUploadedFile(string path);
    }
}