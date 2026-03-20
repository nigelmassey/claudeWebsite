using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using claudeWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    // Trust the Docker internal network defined in docker-compose
    options.KnownNetworks.Add(new IPNetwork(
        System.Net.IPAddress.Parse("172.28.0.0"), 16));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Upload");
    options.Conventions.AuthorizePage("/Admin");
    options.Conventions.AuthorizePage("/Albums/Create");
    options.Conventions.AuthorizePage("/Albums/Edit");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddSingleton<PhotoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseForwardedHeaders();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/photos/original/{filename}", (string filename, PhotoService photos) =>
{
    var path = photos.GetOriginalPath(filename);
    return File.Exists(path) ? Results.File(path, "image/png") : Results.NotFound();
});

app.MapGet("/photos/thumb/{filename}", (string filename, PhotoService photos) =>
{
    var path = photos.GetThumbnailPath(filename);
    return File.Exists(path) ? Results.File(path, "image/png") : Results.NotFound();
});

app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/Index");
}).RequireAuthorization();

app.Run();
