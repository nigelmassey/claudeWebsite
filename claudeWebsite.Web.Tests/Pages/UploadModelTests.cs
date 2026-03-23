using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Pages;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace claudeWebsite.Web.Tests.Pages;

public class UploadModelTests
{
    private static UploadModel CreateModel(Mock<IGalleryApiClient> mockClient)
    {
        var model = new UploadModel(mockClient.Object);
        model.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return model;
    }

    [Fact]
    public async Task OnPostAsync_WithNullPhoto_SetsErrorMessage()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.GetAlbumsAsync()).ReturnsAsync(new List<Album>());
        var model = CreateModel(mockClient);

        var result = await model.OnPostAsync(null!, null);

        Assert.Equal("Only PNG files are accepted.", model.ErrorMessage);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithNonPngPhoto_SetsErrorMessage()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.GetAlbumsAsync()).ReturnsAsync(new List<Album>());
        var model = CreateModel(mockClient);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        var result = await model.OnPostAsync(mockFile.Object, null);

        Assert.Equal("Only PNG files are accepted.", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WithValidPhoto_CallsAddPhotoAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.AddPhotoAsync(It.IsAny<IFormFile>())).ReturnsAsync("new-photo.png");
        var model = CreateModel(mockClient);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("image/png");

        var result = await model.OnPostAsync(mockFile.Object, null);

        mockClient.Verify(c => c.AddPhotoAsync(mockFile.Object), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithAlbumId_CallsAddPhotoToAlbumAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.AddPhotoAsync(It.IsAny<IFormFile>())).ReturnsAsync("new-photo.png");
        var model = CreateModel(mockClient);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("image/png");

        await model.OnPostAsync(mockFile.Object, "album-123");

        mockClient.Verify(c => c.AddPhotoToAlbumAsync("album-123", "new-photo.png"), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrows_SetsErrorMessage()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.AddPhotoAsync(It.IsAny<IFormFile>()))
            .ThrowsAsync(new InvalidDataException("Bad file"));
        mockClient.Setup(c => c.GetAlbumsAsync()).ReturnsAsync(new List<Album>());
        var model = CreateModel(mockClient);

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.ContentType).Returns("image/png");

        var result = await model.OnPostAsync(mockFile.Object, null);

        Assert.Equal("Bad file", model.ErrorMessage);
        Assert.IsType<PageResult>(result);
    }
}
