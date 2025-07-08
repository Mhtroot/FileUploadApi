using FileUploadApi.Models;

namespace FileUploadApi.Services;

public interface IFileService
{
    Task<FileUploadResult> UploadAsync(IFormFile file);
}