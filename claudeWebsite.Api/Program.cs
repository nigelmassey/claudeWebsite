using claudeWebsite.Api.Middleware;
using claudeWebsite.Api.Services;
using claudeWebsite.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PhotoService>();

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

// ── Photos ──

app.MapGet("/api/photos", (PhotoService photos) =>
    Results.Ok(photos.GetAll()));

app.MapPost("/api/photos", async (IFormFile photo, PhotoService photos) =>
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

app.MapDelete("/api/photos/{filename}", async (string filename, PhotoService photos) =>
{
    await photos.DeletePhotoAsync(filename);
    return Results.NoContent();
});

// ── Albums ──

app.MapGet("/api/albums", async (PhotoService photos) =>
    Results.Ok(await photos.GetAlbumsAsync()));

app.MapGet("/api/albums/{id}", async (string id, PhotoService photos) =>
{
    var album = await photos.GetAlbumAsync(id);
    return album is not null ? Results.Ok(album) : Results.NotFound();
});

app.MapPost("/api/albums", async (CreateAlbumRequest req, PhotoService photos) =>
{
    var album = await photos.CreateAlbumAsync(req.Name);
    return Results.Created($"/api/albums/{album.Id}", album);
});

app.MapPut("/api/albums/{id}", async (string id, Album album, PhotoService photos) =>
{
    await photos.UpdateAlbumAsync(album);
    return Results.NoContent();
});

app.MapDelete("/api/albums/{id}", async (string id, PhotoService photos) =>
{
    await photos.DeleteAlbumAsync(id);
    return Results.NoContent();
});

// ── Album-Photo associations ──

app.MapPost("/api/albums/{albumId}/photos/{filename}", async (string albumId, string filename, PhotoService photos) =>
{
    await photos.AddPhotoToAlbumAsync(albumId, filename);
    return Results.NoContent();
});

app.MapDelete("/api/albums/{albumId}/photos/{filename}", async (string albumId, string filename, PhotoService photos) =>
{
    await photos.RemovePhotoFromAlbumAsync(albumId, filename);
    return Results.NoContent();
});

app.Run();

record CreateAlbumRequest(string Name);
