namespace Common;


public interface IPaymentRepository
{
    Task<Payment> GetPaymentAsync(int paymentId);
}

public class PaymentRepository(string databaseLocation) : IPaymentRepository
{
    public async Task<Payment> GetPaymentAsync(int paymentId)
    {
        //Simulate call to the database using the provided database location
        await Task.Delay(1);
        return InMemoryDatabase.Payments[databaseLocation].First(p => p.Id == paymentId);
    }
}
