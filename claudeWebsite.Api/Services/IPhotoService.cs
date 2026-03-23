using claudeWebsite.Shared.Models;

namespace claudeWebsite.Api.Services;

public interface IPhotoService
{
    IReadOnlyList<PhotoEntry> GetAll();
    Task<string> AddPhotoAsync(IFormFile file);
    Task DeletePhotoAsync(string filename);
    string GetOriginalPath(string filename);
    string GetThumbnailPath(string filename);
}
