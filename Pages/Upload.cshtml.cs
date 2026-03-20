using claudeWebsite.Shared.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages;

public class UploadModel : PageModel
{
    private readonly GalleryApiClient _photos;

    public string? ErrorMessage { get; private set; }
    public List<Album> Albums { get; private set; } = new();

    public UploadModel(GalleryApiClient photos)
    {
        _photos = photos;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Albums = await _photos.GetAlbumsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile photo, string? albumId)
    {
        if (photo is null || photo.ContentType != "image/png")
        {
            ErrorMessage = "Only PNG files are accepted.";
            Albums = await _photos.GetAlbumsAsync();
            return Page();
        }

        string filename;
        try
        {
            filename = await _photos.AddPhotoAsync(photo);
        }
        catch (InvalidDataException ex)
        {
            ErrorMessage = ex.Message;
            Albums = await _photos.GetAlbumsAsync();
            return Page();
        }

        if (!string.IsNullOrEmpty(albumId))
        {
            await _photos.AddPhotoToAlbumAsync(albumId, filename);
        }

        return RedirectToPage("/Index");
    }
}
