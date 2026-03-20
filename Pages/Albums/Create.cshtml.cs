using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages.Albums;

[Authorize]
public class CreateModel : PageModel
{
    private readonly GalleryApiClient _photos;

    [BindProperty]
    public string Name { get; set; } = "";

    public CreateModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(nameof(Name), "Album name is required.");
            return Page();
        }

        var album = await _photos.CreateAlbumAsync(Name.Trim());
        return RedirectToPage("/Admin/Index");
    }
}
