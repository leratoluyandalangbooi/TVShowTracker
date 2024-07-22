public class RateLimitHandler : DelegatingHandler
{
    private readonly TMDbOptions _options;
    private readonly SemaphoreSlim _semaphore;

    public RateLimitHandler(IOptions<TMDbOptions> options)
    {
        _options = options.Value;
        _semaphore = new SemaphoreSlim(_options.RequestsPerSecond, _options.RequestsPerSecond);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            _ = Task.Delay(1000, cancellationToken).ContinueWith(_ => _semaphore.Release(), cancellationToken);
        }
    }
}