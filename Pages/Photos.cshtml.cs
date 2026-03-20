using claudeWebsite.Models;
using claudeWebsite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages;

public class PhotosModel : PageModel
{
    private readonly PhotoService _photos;

    public IReadOnlyList<PhotoEntry> Photos { get; private set; } = Array.Empty<PhotoEntry>();

    public PhotosModel(PhotoService photos)
    {
        _photos = photos;
    }

    public void OnGet()
    {
        Photos = _photos.GetAll();
    }
}
