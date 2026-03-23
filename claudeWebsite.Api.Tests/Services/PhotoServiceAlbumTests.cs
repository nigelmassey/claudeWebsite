using claudeWebsite.Api.Services;
using claudeWebsite.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace claudeWebsite.Api.Tests.Services;

public class PhotoServiceAlbumTests : IDisposable
{
    private readonly string _tempDir;
    private readonly PhotoService _service;

    public PhotoServiceAlbumTests()
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
    public async Task CreateAlbumAsync_ReturnsAlbumWithCorrectName()
    {
        var album = await _service.CreateAlbumAsync("Test Album");

        Assert.Equal("Test Album", album.Name);
        Assert.NotEmpty(album.Id);
        Assert.NotEmpty(album.PhotoFilenames is not null ? "ok" : "");
    }

    [Fact]
    public async Task GetAlbumsAsync_ReturnsCreatedAlbums()
    {
        await _service.CreateAlbumAsync("Album 1");
        await _service.CreateAlbumAsync("Album 2");

        var albums = await _service.GetAlbumsAsync();

        Assert.Equal(2, albums.Count);
        Assert.Contains(albums, a => a.Name == "Album 1");
        Assert.Contains(albums, a => a.Name == "Album 2");
    }

    [Fact]
    public async Task GetAlbumAsync_WithValidId_ReturnsAlbum()
    {
        var created = await _service.CreateAlbumAsync("Find Me");

        var found = await _service.GetAlbumAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Find Me", found.Name);
    }

    [Fact]
    public async Task GetAlbumAsync_WithInvalidId_ReturnsNull()
    {
        var found = await _service.GetAlbumAsync("nonexistent");

        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAlbumAsync_RemovesAlbum()
    {
        var album = await _service.CreateAlbumAsync("To Delete");

        await _service.DeleteAlbumAsync(album.Id);

        var albums = await _service.GetAlbumsAsync();
        Assert.Empty(albums);
    }

    [Fact]
    public async Task UpdateAlbumAsync_ChangesAlbumName()
    {
        var album = await _service.CreateAlbumAsync("Original");
        var updated = album with { Name = "Updated" };

        await _service.UpdateAlbumAsync(updated);

        var found = await _service.GetAlbumAsync(album.Id);
        Assert.NotNull(found);
        Assert.Equal("Updated", found.Name);
    }

    [Fact]
    public async Task AddPhotoToAlbumAsync_AddsFilename()
    {
        var album = await _service.CreateAlbumAsync("With Photos");

        await _service.AddPhotoToAlbumAsync(album.Id, "photo1.png");

        var found = await _service.GetAlbumAsync(album.Id);
        Assert.NotNull(found);
        Assert.Contains("photo1.png", found.PhotoFilenames);
    }

    [Fact]
    public async Task AddPhotoToAlbumAsync_DoesNotDuplicate()
    {
        var album = await _service.CreateAlbumAsync("No Dupes");

        await _service.AddPhotoToAlbumAsync(album.Id, "photo1.png");
        await _service.AddPhotoToAlbumAsync(album.Id, "photo1.png");

        var found = await _service.GetAlbumAsync(album.Id);
        Assert.NotNull(found);
        Assert.Single(found.PhotoFilenames);
    }

    [Fact]
    public async Task RemovePhotoFromAlbumAsync_RemovesFilename()
    {
        var album = await _service.CreateAlbumAsync("Remove Photo");
        await _service.AddPhotoToAlbumAsync(album.Id, "photo1.png");

        await _service.RemovePhotoFromAlbumAsync(album.Id, "photo1.png");

        var found = await _service.GetAlbumAsync(album.Id);
        Assert.NotNull(found);
        Assert.Empty(found.PhotoFilenames);
    }
}
