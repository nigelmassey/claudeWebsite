using claudeWebsite.Api.Services;
using Microsoft.Extensions.Configuration;

namespace claudeWebsite.Api.Tests.Services;

public class PhotoServicePhotoTests : IDisposable
{
    private readonly string _tempDir;
    private readonly PhotoService _service;

    public PhotoServicePhotoTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"phototest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gallery:StoragePath"] = "PhotoStorage"
            })
            .Build();

        var env = new Moq.Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(_tempDir);

        _service = new PhotoService(config, env.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void GetAll_InitiallyReturnsEmpty()
    {
        var photos = _service.GetAll();
        Assert.Empty(photos);
    }

    [Fact]
    public void GetOriginalPath_ReturnsCorrectPath()
    {
        var path = _service.GetOriginalPath("test.png");
        Assert.EndsWith(Path.Combine("originals", "test.png"), path);
    }

    [Fact]
    public void GetThumbnailPath_ReturnsCorrectPath()
    {
        var path = _service.GetThumbnailPath("test.png");
        Assert.EndsWith(Path.Combine("thumbnails", "test.png"), path);
    }

    [Fact]
    public async Task DeletePhotoAsync_RemovesEntryFromList()
    {
        // Pre-populate metadata to simulate an existing photo
        var storagePath = Path.Combine(_tempDir, "PhotoStorage");
        var metadataPath = Path.Combine(storagePath, "metadata.json");
        await File.WriteAllTextAsync(metadataPath,
            """[{"Filename":"existing.png","UploadedAt":"2024-01-01T00:00:00"}]""");

        // Recreate service to pick up the metadata
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gallery:StoragePath"] = "PhotoStorage"
            })
            .Build();

        var env = new Moq.Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(_tempDir);

        var service = new PhotoService(config, env.Object);
        Assert.Single(service.GetAll());

        await service.DeletePhotoAsync("existing.png");

        Assert.Empty(service.GetAll());
    }
}
