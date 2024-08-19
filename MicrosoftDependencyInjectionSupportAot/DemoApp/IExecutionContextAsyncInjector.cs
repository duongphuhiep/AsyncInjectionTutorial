namespace SmallExamples;

public interface IExecutionContextAsyncInjector
{
    void Inject(Guid executionContextId);
    Task<ExecutionContext?> GetInjectedExecutionContextAsync();
}
