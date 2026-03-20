using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

[Authorize]
public class EditModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public Album? Album { get; private set; }
    public IReadOnlyList<PhotoEntry> AllPhotos { get; private set; } = Array.Empty<PhotoEntry>();

    [BindProperty]
    public string Name { get; set; } = "";

    [BindProperty]
    public List<string> SelectedFilenames { get; set; } = new();

    public EditModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Album = await _photos.GetAlbumAsync(id);
        if (Album is null) return NotFound();

        Name = Album.Name;
        SelectedFilenames = Album.PhotoFilenames.ToList();
        AllPhotos = await _photos.GetAllAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        Album = await _photos.GetAlbumAsync(id);
        if (Album is null) return NotFound();

        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(nameof(Name), "Album name is required.");
            AllPhotos = await _photos.GetAllAsync();
            return Page();
        }

        var updated = Album with { Name = Name.Trim(), PhotoFilenames = SelectedFilenames };
        await _photos.UpdateAlbumAsync(updated);
        return RedirectToPage("/Admin/Index");
    }
}
