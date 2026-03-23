using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Pages.Albums;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace claudeWebsite.Web.Tests.Pages.Albums;

public class EditModelTests
{
    private static EditModel CreatePageModel(Mock<IGalleryApiClient> mockClient)
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var model = new EditModel(mockClient.Object);
        model.PageContext = new PageContext(actionContext);
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_PopulatesAlbumAndPhotos()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var album = new Album("1", "Test", DateTime.Now, new List<string> { "photo1.png" });
        var photos = new List<PhotoEntry> { new("photo1.png", DateTime.Now) };

        mockClient.Setup(c => c.GetAlbumAsync("1")).ReturnsAsync(album);
        mockClient.Setup(c => c.GetAllAsync()).ReturnsAsync(photos);

        var model = CreatePageModel(mockClient);

        var result = await model.OnGetAsync("1");

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Album);
        Assert.Equal("Test", model.Name);
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

    [Fact]
    public async Task OnPostAsync_WithValidData_CallsUpdateAlbumAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var album = new Album("1", "Old Name", DateTime.Now, new List<string>());
        mockClient.Setup(c => c.GetAlbumAsync("1")).ReturnsAsync(album);

        var model = CreatePageModel(mockClient);
        model.Name = "New Name";
        model.SelectedFilenames = new List<string> { "photo1.png" };

        var result = await model.OnPostAsync("1");

        mockClient.Verify(c => c.UpdateAlbumAsync(It.Is<Album>(a =>
            a.Name == "New Name" && a.PhotoFilenames.Contains("photo1.png"))), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }
}
