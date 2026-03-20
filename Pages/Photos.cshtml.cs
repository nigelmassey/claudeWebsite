using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages;

public class PhotosModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public IReadOnlyList<PhotoEntry> Photos { get; private set; } = Array.Empty<PhotoEntry>();

    public PhotosModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Photos = await _photos.GetAllAsync();
    }
}
