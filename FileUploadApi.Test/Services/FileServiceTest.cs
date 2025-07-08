using Xunit;
using Moq;
using FileUploadApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FileUploadApi.Test.Services
{
    public class FileServiceTests
    {
        private readonly FileService _fileService;
        private readonly Mock<ILogger<FileService>> _loggerMock = new();
        private readonly Mock<IWebHostEnvironment> _envMock = new();

        public FileServiceTests()
        {
            _envMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "TestUploads"));
            _fileService = new FileService(_loggerMock.Object, _envMock.Object);
        }

        [Fact]
        public async Task UploadAsync_ValidFile_ReturnsUploadResult()
        {
            // Arrange
            var content = "Fake file content";
            var fileName = "test.txt";
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((target, token) =>
                    {
                        stream.CopyTo(target);
                        return Task.CompletedTask;
                    });

            // Act
            var result = await _fileService.UploadAsync(fileMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.EndsWith(".txt", result.FileName);
            Assert.True(File.Exists(result.FilePath));
        }

        [Fact]
        public async Task UploadAsync_EmptyFile_ThrowsArgumentException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileService.UploadAsync(fileMock.Object));
        }
    }
}