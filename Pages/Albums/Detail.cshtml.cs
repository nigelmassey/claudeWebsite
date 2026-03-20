using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

public class DetailModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public Album? Album { get; private set; }
    public List<PhotoEntry> Photos { get; private set; } = new();

    public DetailModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Album = await _photos.GetAlbumAsync(id);
        if (Album is null) return NotFound();

        var allPhotos = await _photos.GetAllAsync();
        Photos = allPhotos.Where(p => Album.PhotoFilenames.Contains(p.Filename)).ToList();
        return Page();
    }
}
