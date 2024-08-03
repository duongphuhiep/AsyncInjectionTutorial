namespace SampleApi.ManualInjection;

public interface IPaymentRepositoryFactory
{
    Task<IPaymentRepository> CreateAsync(string partnerName);
}

public class PaymentRepositoryFactory(IPartnerContextService _partnerContextService) : IPaymentRepositoryFactory
{
    public async Task<IPaymentRepository> CreateAsync(string partnerName)
    {
        var partnerContext = await _partnerContextService.GetPartnerContextAsync(partnerName);
        return new PaymentRepository(partnerContext.DatabaseLocation);
    }
}

