using claudeWebsite.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace claudeWebsite.Api.Tests.Middleware;

public class ApiKeyMiddlewareTests
{
    private static ApiKeyMiddleware CreateMiddleware(string? apiKey, RequestDelegate next)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKey"] = apiKey
            })
            .Build();

        return new ApiKeyMiddleware(next, config);
    }

    [Fact]
    public async Task InvokeAsync_CorrectKey_CallsNext()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware("secret123", _ => { nextCalled = true; return Task.CompletedTask; });

        var context = new DefaultHttpContext();
        context.Request.Headers["X-Api-Key"] = "secret123";

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_NoKeyProvided_Returns401()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware("secret123", _ => { nextCalled = true; return Task.CompletedTask; });

        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WrongKey_Returns401()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware("secret123", _ => { nextCalled = true; return Task.CompletedTask; });

        var context = new DefaultHttpContext();
        context.Request.Headers["X-Api-Key"] = "wrongkey";

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_NoApiKeyConfigured_CallsNext()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(null, _ => { nextCalled = true; return Task.CompletedTask; });

        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }
}
