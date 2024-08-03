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

        //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
        using (var scopedService = serviceProvider.CreateScope())
        {
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleAsyncService>();
            var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextAsyncInjector>();

            var runtimeExecutionContextId = Guid.NewGuid();
            executionContextInjector.Inject(runtimeExecutionContextId);
            var executionContextUsedBySampleService = await sampleService.ExecutionContextTask;
            Assert.Equal(runtimeExecutionContextId, executionContextUsedBySampleService?.Id);
        }
    }
}