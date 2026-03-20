using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Admin;

[Authorize]
public class IndexModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public IReadOnlyList<PhotoEntry> Photos { get; private set; } = Array.Empty<PhotoEntry>();
    public List<Album> Albums { get; private set; } = new();

    public IndexModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Photos = await _photos.GetAllAsync();
        Albums = await _photos.GetAlbumsAsync();
    }

    public async Task<IActionResult> OnPostDeletePhotoAsync(string filename)
    {
        await _photos.DeletePhotoAsync(filename);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAlbumAsync(string id)
    {
        await _photos.DeleteAlbumAsync(id);
        return RedirectToPage();
    }
}
