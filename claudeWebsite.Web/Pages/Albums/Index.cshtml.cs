using claudeWebsite.Shared.Models;
using claudeWebsite.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Web.Pages.Albums;

public class IndexModel : PageModel
{
    private readonly IGalleryApiClient _photos;

    public List<Album> Albums { get; private set; } = new();

    public IndexModel(IGalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Albums = await _photos.GetAlbumsAsync();
    }
}
