using Pure.DI;
using Pure.DI.MS;
using SampleApi.AsyncInjection.Controller;
using static Pure.DI.Lifetime;

namespace SampleApi.AsyncInjection;

internal partial class Composition : ServiceProviderFactory<Composition>
{
    void Setup() => DI.Setup()
        .DependsOn(Base)
        // Specifies not to attempt to resolve types whose fully qualified name
        // begins with Microsoft.Extensions., Microsoft.AspNetCore.
        // since ServiceProvider will be used to retrieve them.
        .Hint(
            Hint.OnCannotResolveContractTypeNameRegularExpression,
            @"^Microsoft\.(Extensions|AspNetCore)\..+$")
        .Bind().As(Singleton).To<PartnerContextService>()
        .DefaultLifetime(Scoped)
        .Bind().To<CurrentPartnerProvider>()
        .Bind().To<PaymentService>()
        .Bind<Task<PartnerContext>>().To((ICurrentPartnerProvider currentPartnerProvider) =>
            currentPartnerProvider.GetPartnerContextAsync())
        .Bind<Task<IPaymentRepository>>().To((Task<PartnerContext> partnerContextTask) => CreatePaymentRepository(partnerContextTask))
        .Root<PaymentController>();

    public async Task<IPaymentRepository> CreatePaymentRepository(Task<PartnerContext> partnerContextTask)
    {
        var partnerContext = await partnerContextTask;
        return new PaymentRepository(partnerContext.DatabaseLocation);
    }
}