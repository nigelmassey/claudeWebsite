using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Pages;
using claudeWebsite.Web.Services;
using Moq;

namespace claudeWebsite.Web.Tests.Pages;

public class PhotosModelTests
{
    [Fact]
    public async Task OnGetAsync_PopulatesPhotos()
    {
        var mockClient = new Mock<IGalleryApiClient>();
        var photos = new List<PhotoEntry>
        {
            new("photo1.png", DateTime.Now),
            new("photo2.png", DateTime.Now)
        };
        mockClient.Setup(c => c.GetAllAsync()).ReturnsAsync(photos);

        var model = new PhotosModel(mockClient.Object);

        await model.OnGetAsync();

        Assert.Equal(2, model.Photos.Count);
    }
}
