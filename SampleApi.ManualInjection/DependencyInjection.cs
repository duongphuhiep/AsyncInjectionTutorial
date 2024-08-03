namespace SampleApi.ManualInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPartnerContextService, PartnerContextService>();
        services.AddSingleton<IPaymentRepositoryFactory, PaymentRepositoryFactory>();
        return services;
    }
}
