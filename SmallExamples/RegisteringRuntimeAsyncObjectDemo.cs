using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace SmallExamples;

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

        for (int i = 0; i < 10; i++)
        {
            //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
            using var scopedService = serviceProvider.CreateScope();
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleAsyncService>();
            var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextAsyncInjector>();

            await AssertThat_SampleService_can_inject_ExecutionContext_at_runtime(
                sampleService,
                executionContextInjector);
        }
    }

    [Fact]
    public async Task ExecutionContextAsyncInjection_SimpleInjector()
    {
        Container container = new();
        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        container.Register<IExecutionContextAsyncInjector, ExecutionContextAsyncInjector>(Lifestyle.Scoped);
        container.Register<Lazy<Task<ExecutionContext?>>>(() => new Lazy<Task<ExecutionContext?>>(()
            => container.GetInstance<IExecutionContextAsyncInjector>().GetInjectedExecutionContextAsync()), Lifestyle.Scoped);
        container.Register<ISampleAsyncService, SampleAsyncService>(Lifestyle.Scoped);
        container.Verify();

        for (int i = 0; i < 10; i++)
        {
            //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
            await using var scopedService = AsyncScopedLifestyle.BeginScope(container);
            var sampleService = container.GetRequiredService<ISampleAsyncService>();
            var executionContextInjector = container.GetRequiredService<IExecutionContextAsyncInjector>();

            await AssertThat_SampleService_can_inject_ExecutionContext_at_runtime(
                sampleService,
                executionContextInjector);
        }
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