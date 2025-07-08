using FileUploadApi.Models;

namespace FileUploadApi.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IWebHostEnvironment _env;

    public FileService(ILogger<FileService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task<FileUploadResult> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("File is null or empty");
            throw new ArgumentException("Invalid File");
        }

        var uploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation($"File Uploaded: {fileName}");

        return new FileUploadResult
        {
            FileName = fileName,
            FilePath = fullPath
        };
    }
}