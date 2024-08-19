using Microsoft.Extensions.DependencyInjection;

namespace TryPureDI.InvalidScope;

public interface IRequest;

class Request : IRequest;

public interface IService;
class Service(IRequest request) : IService;


public partial class Composition
{
    static void Setup() => DI.Setup()
            .Bind().As(Lifetime.Scoped).To<Request>()
            .Bind().As(Lifetime.Singleton).To<Service>()
            .Root<IService>(nameof(Service));
}

public class Runner
{
    [Fact]
    public void RunTest()
    {
        var composition = new Composition();
        Assert.Equal(composition.Service, composition.Service);
    }

    [Fact]
    public void RunTest_MSDI()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IRequest, Request>();
        serviceCollection.AddSingleton<IService, Service>();

        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        //Singleton cannot depend on scoped
        Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<IService>());
    }
}