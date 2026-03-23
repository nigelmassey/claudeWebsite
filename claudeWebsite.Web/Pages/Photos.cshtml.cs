using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Web.Pages;

public class PhotosModel : PageModel
{
    private readonly IGalleryApiClient _photos;

    public IReadOnlyList<PhotoEntry> Photos { get; private set; } = Array.Empty<PhotoEntry>();

    public PhotosModel(IGalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Photos = await _photos.GetAllAsync();
    }
}
