using Pure.DI;

namespace SmallExamples;

public partial class Composition
{
    private static void Setup()
    {
        DI.Setup()
            .DefaultLifetime(Lifetime.Scoped)
            .Bind<IExecutionContextAsyncInjector>().To<ExecutionContextAsyncInjector>()
            .Bind<Task<ExecutionContext?>>().To((IExecutionContextAsyncInjector e) => e.GetInjectedExecutionContextAsync())
            .Bind<ISampleAsyncService>().To<SampleAsyncService>()
            .Root<ISampleAsyncService>("SampleService")
            .Root<IExecutionContextAsyncInjector>("ExecutionContextInjector");
    }
}

public class RegisteringRuntimeAsyncObjectDemo
{
    [Fact]
    public async Task ExecutionContextAsyncInjection()
    {
        ServiceCollection services = new();
        services.AddScoped<IExecutionContextAsyncInjector, ExecutionContextAsyncInjector>();
        services.AddScoped(serviceProvider => new Lazy<Task<ExecutionContext?>>(()
            => serviceProvider.GetRequiredService<IExecutionContextAsyncInjector>().GetInjectedExecutionContextAsync()));
        services.AddScoped<ISampleAsyncService, SampleAsyncService>();

        IServiceProvider serviceProvider = services.BuildServiceProvider(true);

        //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
        using (var scopedService = serviceProvider.CreateScope())
        {
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleAsyncService>();
            var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextAsyncInjector>();
            await AssertThat_SampleService_can_inject_ExecutionContext_at_runtime(
                sampleService,
                executionContextInjector);
        }
    }

    [Fact]
    public async Task ExecutionContextAsyncInjection_PureDI()
    {
        //root scope
        var composition = new Composition();

        //verify that same scope, same object
        Assert.Equal(composition.ExecutionContextInjector, composition.ExecutionContextInjector);
        Assert.Equal(composition.SampleService, composition.SampleService);

        //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
        await AssertThat_SampleService_can_inject_ExecutionContext_at_runtime(
            composition.SampleService,
            composition.ExecutionContextInjector);
    }

    private static async Task AssertThat_SampleService_can_inject_ExecutionContext_at_runtime(ISampleAsyncService sampleService, IExecutionContextAsyncInjector executionContextInjector)
    {
        var runtimeExecutionContextId = Guid.NewGuid();
        executionContextInjector.Inject(runtimeExecutionContextId);
        var executionContextUsedBySampleService = await sampleService.ExecutionContextTask;
        Assert.Equal(runtimeExecutionContextId, executionContextUsedBySampleService?.Id);

        //Inject other ExecutionContext
        Assert.Throws<InvalidOperationException>(() => executionContextInjector.Inject(Guid.NewGuid()));
    }
}