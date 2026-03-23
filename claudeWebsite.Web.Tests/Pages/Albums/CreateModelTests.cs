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

public class CreateModelTests
{
    private static CreateModel CreatePageModel(Mock<IGalleryApiClient> mockClient)
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var model = new CreateModel(mockClient.Object);
        model.PageContext = new PageContext(actionContext);
        return model;
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyName_AddsModelError()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var model = CreatePageModel(mockClient);
        model.Name = "";

        var result = await model.OnPostAsync();

        Assert.False(model.ModelState.IsValid);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithValidName_CallsCreateAlbumAsync()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        mockClient.Setup(c => c.CreateAlbumAsync("My Album"))
            .ReturnsAsync(new Album("1", "My Album", DateTime.Now, new List<string>()));
        var model = CreatePageModel(mockClient);
        model.Name = "My Album";

        var result = await model.OnPostAsync();

        mockClient.Verify(c => c.CreateAlbumAsync("My Album"), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }
}
