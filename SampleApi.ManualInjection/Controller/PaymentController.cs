using Microsoft.AspNetCore.Mvc;

namespace SampleApi.ManualInjection.Controller;

public class PaymentController(IPaymentRepositoryFactory _partnerRepositoryFactory)
{
    [HttpGet("/payment/{partnerName}/{paymentId}")]
    public async Task<Payment> GetPaymentAsync([FromRoute] string partnerName, [FromRoute] int paymentId)
    {
        var paymentRepository = await _partnerRepositoryFactory.CreateAsync(partnerName);
        var paymentService = new PaymentService(paymentRepository);
        return await paymentService.ComputeDerivedPaymentAsync(paymentId);
    }
}