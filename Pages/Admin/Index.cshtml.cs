using claudeWebsite.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Admin;

[Authorize]
public class IndexModel : PageModel
{
    private readonly PhotoService _photos;

    public IReadOnlyList<PhotoEntry> Photos { get; private set; } = Array.Empty<PhotoEntry>();
    public List<Album> Albums { get; private set; } = new();

    public IndexModel(PhotoService photos)
    {
        _photos = photos;
    }

    public async Task OnGetAsync()
    {
        Photos = _photos.GetAll();
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
