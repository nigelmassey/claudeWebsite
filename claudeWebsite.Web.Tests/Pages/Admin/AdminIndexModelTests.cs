using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace claudeWebsite.Web.Tests.Pages.Admin;

public class AdminIndexModelTests
{
    private static claudeWebsite.Web.Pages.Admin.IndexModel CreateModel(Mock<IGalleryApiClient> mockClient)
    {
        var model = new claudeWebsite.Web.Pages.Admin.IndexModel(mockClient.Object);
        model.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_PopulatesPhotosAndAlbums()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var photos = new List<PhotoEntry> { new("photo1.png", DateTime.Now) };
        var albums = new List<Album> { new("1", "Album", DateTime.Now, new List<string>()) };

        mockClient.Setup(c => c.GetAllAsync()).ReturnsAsync(photos);
        mockClient.Setup(c => c.GetAlbumsAsync()).ReturnsAsync(albums);

        var model = CreateModel(mockClient);

        await model.OnGetAsync();

        Assert.Single(model.Photos);
        Assert.Single(model.Albums);
    }

    [Fact]
    public async Task OnPostDeletePhotoAsync_CallsDeletePhotoAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var model = CreateModel(mockClient);

        var result = await model.OnPostDeletePhotoAsync("photo1.png");

        mockClient.Verify(c => c.DeletePhotoAsync("photo1.png"), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteAlbumAsync_CallsDeleteAlbumAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var model = CreateModel(mockClient);

        var result = await model.OnPostDeleteAlbumAsync("album-1");

        mockClient.Verify(c => c.DeleteAlbumAsync("album-1"), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }
}
