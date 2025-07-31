
namespace SampleApi.AsyncInjection;

public interface IPaymentService
{
    Task<Payment> ComputeDerivedPaymentAsync(int paymentId);
}

public class PaymentService(Lazy<Task<IPaymentRepository>> _deferredPaymentRepositoryTask) : IPaymentService
{
    public async Task<Payment> ComputeDerivedPaymentAsync(int paymentId)
    {
        var paymentRepository = await _deferredPaymentRepositoryTask.Value;
        var payment = await paymentRepository.GetPaymentAsync(paymentId);
        return new Payment { Id = payment.Id, Amount = payment.Amount + 1 };
    }
}
