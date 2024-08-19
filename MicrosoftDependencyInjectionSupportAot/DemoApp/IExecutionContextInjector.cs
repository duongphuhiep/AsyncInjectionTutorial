namespace SmallExamples;

public interface IExecutionContextInjector
{
    void Inject(ExecutionContext? executionContext);
    ExecutionContext? GetInjectedExecutionContext();
}
