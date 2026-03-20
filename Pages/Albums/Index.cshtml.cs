using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

public class IndexModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public List<Album> Albums { get; private set; } = new();

    public IndexModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Albums = await _photos.GetAlbumsAsync();
    }
}
