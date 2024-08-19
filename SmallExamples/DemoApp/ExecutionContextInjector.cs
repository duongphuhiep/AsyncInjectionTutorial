namespace SmallExamples;

public class ExecutionContextInjector : IExecutionContextInjector
{
    private ExecutionContext? _executionContext;

    private bool _isInjected = false;

    public ExecutionContext? GetInjectedExecutionContext()
    {
        if (!_isInjected)
        {
            throw new InvalidOperationException("ExecutionContext has not been injected");
        }
        return _executionContext;
    }
    public void Inject(ExecutionContext? executionContext)
    {
        if (_isInjected)
        {
            throw new InvalidOperationException("ExecutionContext has been injected. You can only do it once per scope (request)");
        }
        _executionContext = executionContext;
        _isInjected = true;
    }
}