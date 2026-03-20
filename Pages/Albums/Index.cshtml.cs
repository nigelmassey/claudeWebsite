using claudeWebsite.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

public class IndexModel : PageModel
{
    private readonly PhotoService _photos;

    public List<Album> Albums { get; private set; } = new();

    public IndexModel(PhotoService photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Albums = await _photos.GetAlbumsAsync();
    }
}
