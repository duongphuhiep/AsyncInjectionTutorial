namespace SmallExamples;

public interface ISampleService
{
    ExecutionContext? ExecutionContext { get; }
}
public class SampleService(Lazy<ExecutionContext?> _deferredExecutionContext) : ISampleService
{
    public ExecutionContext? ExecutionContext => _deferredExecutionContext.Value;
}
