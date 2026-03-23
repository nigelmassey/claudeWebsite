using claudeWebsite.Api.Middleware;
using claudeWebsite.Api.Services;
using claudeWebsite.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PhotoService>();
builder.Services.AddSingleton<IPhotoService>(sp => sp.GetRequiredService<PhotoService>());
builder.Services.AddSingleton<IAlbumService>(sp => sp.GetRequiredService<PhotoService>());

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

// ── Photos ──

app.MapGet("/api/photos", (IPhotoService photos) =>
    Results.Ok(photos.GetAll()));

app.MapPost("/api/photos", async (IFormFile photo, IPhotoService photos) =>
{
    try
    {
        var filename = await photos.AddPhotoAsync(photo);
        return Results.Created($"/api/photos/{filename}", new PhotoEntry(filename, DateTime.UtcNow));
    }
    catch (InvalidDataException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/photos/{filename}", async (string filename, IPhotoService photos) =>
{
    await photos.DeletePhotoAsync(filename);
    return Results.NoContent();
});

// ── Albums ──

app.MapGet("/api/albums", async (IAlbumService albums) =>
    Results.Ok(await albums.GetAlbumsAsync()));

app.MapGet("/api/albums/{id}", async (string id, IAlbumService albums) =>
{
    var album = await albums.GetAlbumAsync(id);
    return album is not null ? Results.Ok(album) : Results.NotFound();
});

app.MapPost("/api/albums", async (CreateAlbumRequest req, IAlbumService albums) =>
{
    var album = await albums.CreateAlbumAsync(req.Name);
    return Results.Created($"/api/albums/{album.Id}", album);
});

app.MapPut("/api/albums/{id}", async (string id, Album album, IAlbumService albums) =>
{
    await albums.UpdateAlbumAsync(album);
    return Results.NoContent();
});

app.MapDelete("/api/albums/{id}", async (string id, IAlbumService albums) =>
{
    await albums.DeleteAlbumAsync(id);
    return Results.NoContent();
});

// ── Album-Photo associations ──

app.MapPost("/api/albums/{albumId}/photos/{filename}", async (string albumId, string filename, IAlbumService albums) =>
{
    await albums.AddPhotoToAlbumAsync(albumId, filename);
    return Results.NoContent();
});

app.MapDelete("/api/albums/{albumId}/photos/{filename}", async (string albumId, string filename, IAlbumService albums) =>
{
    await albums.RemovePhotoFromAlbumAsync(albumId, filename);
    return Results.NoContent();
});

app.Run();

record CreateAlbumRequest(string Name);
