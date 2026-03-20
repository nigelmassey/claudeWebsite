using System.Text.Json;
using claudeWebsite.Models;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace claudeWebsite.Services;

public class PhotoService
{
    private readonly string _originalsPath;
    private readonly string _thumbnailsPath;
    private readonly string _metadataPath;
    private readonly string _albumsPath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly SemaphoreSlim _albumLock = new(1, 1);
    private List<PhotoEntry> _entries = new();
    private List<Album> _albums = new();

    public PhotoService(IConfiguration config, IWebHostEnvironment env)
    {
        var storagePath = Path.Combine(env.ContentRootPath, config["Gallery:StoragePath"] ?? "PhotoStorage");
        _originalsPath = Path.Combine(storagePath, "originals");
        _thumbnailsPath = Path.Combine(storagePath, "thumbnails");
        _metadataPath = Path.Combine(storagePath, "metadata.json");
        _albumsPath = Path.Combine(storagePath, "albums.json");

        Directory.CreateDirectory(_originalsPath);
        Directory.CreateDirectory(_thumbnailsPath);

        if (File.Exists(_metadataPath))
        {
            var json = File.ReadAllText(_metadataPath);
            _entries = JsonSerializer.Deserialize<List<PhotoEntry>>(json) ?? new();
        }

        if (File.Exists(_albumsPath))
        {
            var json = File.ReadAllText(_albumsPath);
            _albums = JsonSerializer.Deserialize<List<Album>>(json) ?? new();
        }
    }

    public IReadOnlyList<PhotoEntry> GetAll() =>
        _entries.OrderByDescending(e => e.UploadedAt).ToList();

    public async Task<string> AddPhotoAsync(IFormFile file)
    {
        var randomBytes = new byte[3];
        Random.Shared.NextBytes(randomBytes);
        var filename = $"{DateTime.Now:yyyyMMdd-HHmmss}-{Convert.ToHexString(randomBytes).ToLower()}.png";
        var originalPath = Path.Combine(_originalsPath, filename);
        var thumbnailPath = Path.Combine(_thumbnailsPath, filename);

        // Validate PNG format
        using (var stream = file.OpenReadStream())
        {
            var format = await Image.DetectFormatAsync(stream);
            if (format is null || !format.MimeTypes.Contains("image/png"))
                throw new InvalidDataException("Only PNG files are accepted.");
        }

        // Save original
        using (var stream = file.OpenReadStream())
        using (var dest = File.Create(originalPath))
        {
            await stream.CopyToAsync(dest);
        }

        // Generate thumbnail
        using (var image = await Image.LoadAsync(originalPath))
        {
            int side = Math.Min(image.Width, image.Height);
            image.Mutate(x => x
                .Crop(new Rectangle((image.Width - side) / 2, (image.Height - side) / 2, side, side))
                .Resize(300, 300));
            await image.SaveAsPngAsync(thumbnailPath);
        }

        var entry = new PhotoEntry(filename, DateTime.Now);

        await _lock.WaitAsync();
        try
        {
            _entries.Add(entry);
            await File.WriteAllTextAsync(_metadataPath, JsonSerializer.Serialize(_entries));
        }
        finally
        {
            _lock.Release();
        }

        return filename;
    }

    public string GetOriginalPath(string filename) => Path.Combine(_originalsPath, filename);
    public string GetThumbnailPath(string filename) => Path.Combine(_thumbnailsPath, filename);

    public async Task DeletePhotoAsync(string filename)
    {
        var originalPath = Path.Combine(_originalsPath, filename);
        var thumbnailPath = Path.Combine(_thumbnailsPath, filename);

        if (File.Exists(originalPath)) File.Delete(originalPath);
        if (File.Exists(thumbnailPath)) File.Delete(thumbnailPath);

        await _lock.WaitAsync();
        try
        {
            _entries.RemoveAll(e => e.Filename == filename);
            await File.WriteAllTextAsync(_metadataPath, JsonSerializer.Serialize(_entries));
        }
        finally
        {
            _lock.Release();
        }
    }

    // ── Album methods ──

    public async Task<List<Album>> GetAlbumsAsync()
    {
        await _albumLock.WaitAsync();
        try { return _albums.ToList(); }
        finally { _albumLock.Release(); }
    }

    public async Task<Album?> GetAlbumAsync(string id)
    {
        await _albumLock.WaitAsync();
        try { return _albums.FirstOrDefault(a => a.Id == id); }
        finally { _albumLock.Release(); }
    }

    public async Task<Album> CreateAlbumAsync(string name)
    {
        var randomBytes = new byte[3];
        Random.Shared.NextBytes(randomBytes);
        var id = Convert.ToHexString(randomBytes).ToLower();
        var album = new Album(id, name, DateTime.Now, new List<string>());

        await _albumLock.WaitAsync();
        try
        {
            _albums.Add(album);
            await File.WriteAllTextAsync(_albumsPath, JsonSerializer.Serialize(_albums));
        }
        finally { _albumLock.Release(); }

        return album;
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        await _albumLock.WaitAsync();
        try
        {
            var idx = _albums.FindIndex(a => a.Id == album.Id);
            if (idx >= 0) _albums[idx] = album;
            await File.WriteAllTextAsync(_albumsPath, JsonSerializer.Serialize(_albums));
        }
        finally { _albumLock.Release(); }
    }

    public async Task DeleteAlbumAsync(string id)
    {
        await _albumLock.WaitAsync();
        try
        {
            _albums.RemoveAll(a => a.Id == id);
            await File.WriteAllTextAsync(_albumsPath, JsonSerializer.Serialize(_albums));
        }
        finally { _albumLock.Release(); }
    }

    public async Task AddPhotoToAlbumAsync(string albumId, string filename)
    {
        await _albumLock.WaitAsync();
        try
        {
            var idx = _albums.FindIndex(a => a.Id == albumId);
            if (idx >= 0 && !_albums[idx].PhotoFilenames.Contains(filename))
            {
                _albums[idx].PhotoFilenames.Add(filename);
                await File.WriteAllTextAsync(_albumsPath, JsonSerializer.Serialize(_albums));
            }
        }
        finally { _albumLock.Release(); }
    }

    public async Task RemovePhotoFromAlbumAsync(string albumId, string filename)
    {
        await _albumLock.WaitAsync();
        try
        {
            var idx = _albums.FindIndex(a => a.Id == albumId);
            if (idx >= 0)
            {
                _albums[idx].PhotoFilenames.Remove(filename);
                await File.WriteAllTextAsync(_albumsPath, JsonSerializer.Serialize(_albums));
            }
        }
        finally { _albumLock.Release(); }
    }
}
