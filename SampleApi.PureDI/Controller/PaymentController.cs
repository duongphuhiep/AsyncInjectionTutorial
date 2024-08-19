using Microsoft.AspNetCore.Mvc;

namespace SampleApi.AsyncInjection.Controller;

public class PaymentController(IPaymentService _paymentService, ICurrentPartnerProvider _currentPartnerProvider)
{
    [HttpGet("/payment/{partnerName}/{paymentId}")]
    public async Task<Payment> GetPaymentAsync([FromRoute] string partnerName, [FromRoute] int paymentId)
    {
        _currentPartnerProvider.ProvidePartnerName(partnerName);
        return await _paymentService.ComputeDerivedPaymentAsync(paymentId);
    }
}
