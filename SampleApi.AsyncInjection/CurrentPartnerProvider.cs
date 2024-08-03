namespace SampleApi.AsyncInjection;

public interface ICurrentPartnerProvider
{
    void ProvidePartnerName(string partnerName);
    Task<PartnerContext> GetPartnerContextAsync();
}

public class CurrentPartnerProvider(IPartnerContextService _partnerContextService) : ICurrentPartnerProvider
{
    private string _partnerName;

    /// <summary>
    /// Cached value for the request
    /// </summary>
    private PartnerContext? _partnerContext;

    public void ProvidePartnerName(string partnerName)
    {
        _partnerName = partnerName;
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
