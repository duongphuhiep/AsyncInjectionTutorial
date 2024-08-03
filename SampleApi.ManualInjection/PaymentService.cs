
namespace SampleApi.ManualInjection;

public interface IPaymentService
{
    Task<Payment> ComputeDerivedPaymentAsync(int paymentId);
}

public class PaymentService(IPaymentRepository _paymentRepository) : IPaymentService
{
    public async Task<Payment> ComputeDerivedPaymentAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetPaymentAsync(paymentId);
        return new Payment { Id = payment.Id, Amount = payment.Amount + 1 };
    }
}
