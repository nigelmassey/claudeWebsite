using claudeWebsite.Shared.Models;

namespace claudeWebsite.Web.Services;

public interface IGalleryApiClient
{
    Task<IReadOnlyList<PhotoEntry>> GetAllAsync();
    Task<string> AddPhotoAsync(IFormFile file);
    Task DeletePhotoAsync(string filename);
    Task<List<Album>> GetAlbumsAsync();
    Task<Album?> GetAlbumAsync(string id);
    Task<Album> CreateAlbumAsync(string name);
    Task UpdateAlbumAsync(Album album);
    Task DeleteAlbumAsync(string id);
    Task AddPhotoToAlbumAsync(string albumId, string filename);
    Task RemovePhotoFromAlbumAsync(string albumId, string filename);
}
