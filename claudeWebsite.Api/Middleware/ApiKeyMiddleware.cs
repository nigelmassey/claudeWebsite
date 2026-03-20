namespace claudeWebsite.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _apiKey = config["ApiKey"];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!string.IsNullOrEmpty(_apiKey))
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var provided) || provided != _apiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }

        await _next(context);
    }
}
