using claudeWebsite.Shared.Models;

namespace claudeWebsite.Api.Services;

public interface IAlbumService
{
    Task<List<Album>> GetAlbumsAsync();
    Task<Album?> GetAlbumAsync(string id);
    Task<Album> CreateAlbumAsync(string name);
    Task UpdateAlbumAsync(Album album);
    Task DeleteAlbumAsync(string id);
    Task AddPhotoToAlbumAsync(string albumId, string filename);
    Task RemovePhotoFromAlbumAsync(string albumId, string filename);
}
