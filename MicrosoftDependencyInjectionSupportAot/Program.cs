using Microsoft.Extensions.DependencyInjection;
using SmallExamples;

ServiceCollection services = new();
services.AddScoped<IExecutionContextAsyncInjector, ExecutionContextAsyncInjector>();
services.AddScoped(serviceProvider => new Lazy<Task<SmallExamples.ExecutionContext?>>(()
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
    if (runtimeExecutionContextId == executionContextUsedBySampleService?.Id)
    {
        Console.WriteLine("Success!");
    }
    else
    {
        Console.WriteLine("Failed!");
    }
}
