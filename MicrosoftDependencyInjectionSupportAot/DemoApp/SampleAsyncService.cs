namespace SmallExamples;

public interface ISampleAsyncService
{
    Task<ExecutionContext?> ExecutionContextTask { get; }
}

public class SampleAsyncService(Lazy<Task<ExecutionContext?>> _deferredExecutionContextTask) : ISampleAsyncService
{
    public Task<ExecutionContext?> ExecutionContextTask => _deferredExecutionContextTask.Value;
}
