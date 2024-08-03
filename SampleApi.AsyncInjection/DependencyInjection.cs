namespace SampleApi.AsyncInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPartnerContextService, PartnerContextService>();

        services.AddScoped<ICurrentPartnerProvider, CurrentPartnerProvider>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped(serviceProvider
                    => new Lazy<Task<PartnerContext>>(
                        () => serviceProvider.GetRequiredService<ICurrentPartnerProvider>().GetPartnerContextAsync()
                ))
                .AddScoped(serviceProvider
                    => new Lazy<Task<IPaymentRepository>>(
                        async () =>
                        {
                            var deferredPartnerContextTask = serviceProvider.GetRequiredService<Lazy<Task<PartnerContext>>>();
                            var partnerContext = await deferredPartnerContextTask.Value;
                            return new PaymentRepository(partnerContext.DatabaseLocation);
                        }
                ));

        return services;
    }
}
