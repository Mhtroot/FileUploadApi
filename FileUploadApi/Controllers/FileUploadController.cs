using FileUploadApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileUploadController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var result = await _fileService.UploadAsync(file);
        return Ok(result);
    }
}