namespace claudeWebsite.Services;

public class ApiKeyHandler : DelegatingHandler
{
    private readonly string? _apiKey;

    public ApiKeyHandler(IConfiguration config)
    {
        _apiKey = config["Api:ApiKey"];
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_apiKey))
            request.Headers.Add("X-Api-Key", _apiKey);

        return base.SendAsync(request, cancellationToken);
    }
}
