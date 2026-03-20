using claudeWebsite.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

public class DetailModel : PageModel
{
    private readonly PhotoService _photos;

    public Album? Album { get; private set; }
    public List<PhotoEntry> Photos { get; private set; } = new();

    public DetailModel(PhotoService photos)
    {
        _photos = photos;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Album = await _photos.GetAlbumAsync(id);
        if (Album is null) return NotFound();

        var allPhotos = _photos.GetAll();
        Photos = allPhotos.Where(p => Album.PhotoFilenames.Contains(p.Filename)).ToList();
        return Page();
    }
}
