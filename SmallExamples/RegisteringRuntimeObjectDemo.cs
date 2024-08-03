namespace SmallExamples;

public class RegisteringRuntimeObjectDemo
{

    [Fact]
    public void ExecutionContextInjection()
    {
        ServiceCollection services = new();
        services.AddScoped<IExecutionContextInjector, ExecutionContextInjector>();
        services.AddScoped(serviceProvider => new Lazy<ExecutionContext?>(()
            => serviceProvider.GetRequiredService<IExecutionContextInjector>().GetInjectedExecutionContext()));
        services.AddScoped<ISampleService, SampleService>();

        IServiceProvider serviceProvider = services.BuildServiceProvider(true);

        //typical case: sampleService should be able to access to the injected "runtimeExecutionContext"
        using (var scopedService = serviceProvider.CreateScope())
        {
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleService>();
            var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextInjector>();

            var runtimeExecutionContext = new ExecutionContext(Guid.NewGuid());
            executionContextInjector.Inject(runtimeExecutionContext);

            Assert.Equal(runtimeExecutionContext, sampleService.ExecutionContext);
        }

        //injecting a "null" ExecutionContext, then sampleService should use it as normal execution context
        using (var scopedService = serviceProvider.CreateScope())
        {
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleService>();
            var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextInjector>();

            executionContextInjector.Inject(null);

            Assert.Null(sampleService.ExecutionContext);
        }

        //not injected: sampleService should crash when trying to access to the executionContext which is not yet available
        using (var scopedService = serviceProvider.CreateScope())
        {
            var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleService>();

            //sampleService try to access to the ExecutionContext when it is not yet injected
            Assert.Throws<InvalidOperationException>(() => sampleService.ExecutionContext);
        }
    }
}