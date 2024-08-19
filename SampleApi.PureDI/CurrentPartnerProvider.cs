namespace SampleApi.AsyncInjection;

public interface ICurrentPartnerProvider
{
    void ProvidePartnerName(string partnerName);
    Task<PartnerContext> GetPartnerContextAsync();
}

public class CurrentPartnerProvider(IPartnerContextService _partnerContextService) : ICurrentPartnerProvider
{
    /// <summary>
    /// Protect from calling <see cref="ProvidePartnerName(string)"/> concurently.
    /// </summary>
    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

    private string? _partnerName;

    /// <summary>
    /// Cached value for the request
    /// </summary>
    private PartnerContext? _partnerContext;

    public void ProvidePartnerName(string partnerName)
    {
        semaphoreSlim.Wait();
        try
        {
            if (!string.IsNullOrEmpty(_partnerName))
            {
                throw new InvalidOperationException("Partner name is already provided. You can only do it once per request");
            }
            _partnerName = partnerName;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// The same request can have multiple calls to this method. The result is cached per request basis.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<PartnerContext> GetPartnerContextAsync()
    {
        if (string.IsNullOrEmpty(_partnerName))
        {
            throw new InvalidOperationException("No partner name is provided");
        }

        if (_partnerContext?.Name == _partnerName)
        {
            return _partnerContext;
        }
        _partnerContext = await _partnerContextService.GetPartnerContextAsync(_partnerName);
        return _partnerContext;
    }
}
