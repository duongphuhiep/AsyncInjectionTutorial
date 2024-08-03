
namespace SmallExamples;

public class ExecutionContextAsyncInjector : IExecutionContextAsyncInjector
{
    private Guid _executionContextId;

    private bool _isInjected = false;


    public async Task<ExecutionContext?> GetInjectedExecutionContextAsync()
    {
        if (!_isInjected)
        {
            throw new InvalidOperationException("ExecutionContext has not been injected");
        }
        //Simulate I/O Operation
        await Task.Delay(1);
        return new ExecutionContext(_executionContextId);
    }

    public void Inject(Guid executionContextId)
    {
        _executionContextId = executionContextId;
        _isInjected = true;
    }
}