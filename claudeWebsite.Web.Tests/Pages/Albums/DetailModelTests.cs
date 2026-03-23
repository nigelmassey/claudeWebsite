using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Pages.Albums;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace claudeWebsite.Web.Tests.Pages.Albums;

public class DetailModelTests
{
    private static DetailModel CreatePageModel(Mock<IGalleryApiClient> mockClient)
    {
        var model = new DetailModel(mockClient.Object);
        model.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_PopulatesAlbumAndFilteredPhotos()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var album = new Album("1", "Test", DateTime.Now, new List<string> { "photo1.png" });
        var allPhotos = new List<PhotoEntry>
        {
            new("photo1.png", DateTime.Now),
            new("photo2.png", DateTime.Now)
        };

        mockClient.Setup(c => c.GetAlbumAsync("1")).ReturnsAsync(album);
        mockClient.Setup(c => c.GetAllAsync()).ReturnsAsync(allPhotos);

        var model = CreatePageModel(mockClient);

        var result = await model.OnGetAsync("1");

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Album);
        Assert.Single(model.Photos);
        Assert.Equal("photo1.png", model.Photos[0].Filename);
    }

    [Fact]
    public async Task OnGetAsync_WithInvalidId_ReturnsNotFound()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.GetAlbumAsync("bad")).ReturnsAsync((Album?)null);

        var model = CreatePageModel(mockClient);

        var result = await model.OnGetAsync("bad");

        Assert.IsType<NotFoundResult>(result);
    }
}
