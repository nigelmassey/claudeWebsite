using System.Net.Http.Headers;
using claudeWebsite.Shared.Models;

namespace claudeWebsite.Services;

public class GalleryApiClient
{
    private readonly HttpClient _http;

    public GalleryApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<PhotoEntry>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<PhotoEntry>>("/api/photos") ?? new();

    public async Task<string> AddPhotoAsync(IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "photo", file.FileName);

        var response = await _http.PostAsync("/api/photos", content);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException(message);
        }

        var entry = await response.Content.ReadFromJsonAsync<PhotoEntry>();
        return entry!.Filename;
    }

    public async Task DeletePhotoAsync(string filename) =>
        await _http.DeleteAsync($"/api/photos/{filename}");

    public async Task<List<Album>> GetAlbumsAsync() =>
        await _http.GetFromJsonAsync<List<Album>>("/api/albums") ?? new();

    public async Task<Album?> GetAlbumAsync(string id)
    {
        var response = await _http.GetAsync($"/api/albums/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Album>();
    }

    public async Task<Album> CreateAlbumAsync(string name)
    {
        var response = await _http.PostAsJsonAsync("/api/albums", new { name });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Album>())!;
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        var response = await _http.PutAsJsonAsync($"/api/albums/{album.Id}", album);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAlbumAsync(string id) =>
        await _http.DeleteAsync($"/api/albums/{id}");

    public async Task AddPhotoToAlbumAsync(string albumId, string filename) =>
        await _http.PostAsync($"/api/albums/{albumId}/photos/{filename}", null);

    public async Task RemovePhotoFromAlbumAsync(string albumId, string filename) =>
        await _http.DeleteAsync($"/api/albums/{albumId}/photos/{filename}");
}
