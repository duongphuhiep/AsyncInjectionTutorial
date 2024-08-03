# Injecting Async Services at Runtime

## TL;DR

* if you need to inject a type `T` on runtime => inject the `Lazy<T>` instead
* if the construction of `T` requires async/await operation => inject the `Lazy<Task<T>>` instead

Before talking about async/await Dependency injection. Firstly, let's learn how to

## Injecting Service at Runtime

### Problem

Your service depends on some kind of short live `ExecutionContext` object which is created at Runtime.

```C#
public record ExecutionContext(Guid Id);
public class SampleService(ExecutionContext context): ISampleService { }
```

You wanted to register the `ExecutionContext` to the Dependency Graph (a.k.a: the `ServiceCollection`). so that the DI framework would be able to resolve the `SampleService`.

### Step 1: implementing `ExecutionContextInjector`

We will have to implement a `IExecutionContextInjector` to allow our application to inject the `ExecutionContext` and to retrieve the injected value at runtime:

```C#
public interface IExecutionContextInjector
{
    void InjectExecutionContext(ExecutionContext? ec);
    ExecutionContext? GetInjectedExecutionContext();
}
```

The `ExecutionContextInjector` stores the "current" injected `ExecutionContext`. It must to be registered as "Scoped" or "Singleton" depending on the life time of the `ExecutionContext`:

```C#
services.AddScoped<IExecutionContextInjector, ExecutionContextInjector>();
```

### Step 2: Use the `Lazy<ExecutionContext>`

`SampleService` should depend on a "deferred computation" of a `ExecutionContext`  that will eventually return the injected `ExecutionContext`:

```C#
public class SampleService(Lazy<ExecutionContext?> _deferredExecutionContext) : ISampleService
{
    public ExecutionContext? ExecutionContext => _deferredExecutionContext.Value;
}
```

Because the DI framework is able resolve a `SampleService` before `ExecutionContext` injection; it is safe to register our `SampleService` to the Dependency Graph:

```C#
services.AddScoped<ISampleService, SampleService>();
```

Naturally, `SampleSerivce` would crash if it attempts to access to an unavailable (not yet injected) `ExecutionContext` .

### Step 3: Registering the `Lazy<ExecutionContext>`

With help of `ExecutionContextInjector` (in step 1):

```C#
services.AddScoped(serviceProvider => new Lazy<ExecutionContext>(
      () => serviceProvider.GetRequiredService<IExecutionContextInjector>().GetInjectedExecutionContext()
));
```

## Usage example

Our application has to ensure that the call to `InjectExecutionContext()` and the call to `GetInjectedExecutionContext()` would happen in the same Service Scope (or in the same Request in term of Web application):

```C#
using (var scopedService = serviceProvider.CreateScope())
{
   var sampleService = scopedService.ServiceProvider.GetRequiredService<ISampleService>();
   var executionContextInjector = scopedService.ServiceProvider.GetRequiredService<IExecutionContextInjector>();

   executionContextInjector.InjectExecutionContext(runtimeExecutionContext);
   Assert.Equal(runtimeExecutionContext, sampleService.ExecutionContext);
}
```

Checkout full codes [here](./SmallExamples/RegisteringRuntimeObjectDemo.cs)

## Inject Async Service at Runtime

Our `IExecutionContextInjector` is becoming more complex. It allows the application to inject a `executionContextId` string, and to (asynchronously) retrieve the `ExecutionContext` object.

```C#
public interface IExecutionContextAsyncInjector
{
    void InjectExecutionContextId(Guid executionContextId);
    Task<ExecutionContext?> GetInjectedExecutionContextAsync();
}
```

this time, our `SampleService` would depends on a `Lazy<Task<ExecutionContext?>>`

```C#
public class SampleService(Lazy<Task<ExecutionContext?>> _deferredExecutionContextTask) : ISampleAsyncService
{
    public Task<ExecutionContext?> ExecutionContextTask => _deferredExecutionContextTask.Value;
}
```

And the following codes will register the deferred `ExecutionContext` task:

```C#
services.AddScoped(serviceProvider => new Lazy<Task<ExecutionContext?>>(()
            => serviceProvider.GetRequiredService<IExecutionContextAsyncInjector>().GetInjectedExecutionContextAsync()));
```

Checkout full codes [here](./SmallExamples/RegisteringRuntimeAsyncObjectDemo.cs)

## Multi-tenant API example

* The [following project](./SampleApi.AsyncInjection/README.md) demontrate usage of a the above pattern in a more realistic Web API.

   [SampleApi.AsyncInjection](./SampleApi.AsyncInjection/)

* The [following project](./SampleApi.ManualInjection/README.md) demontrate the "normal approach" when we don't know the technique.

   [SampleApi.ManualInjection](./SampleApi.ManualInjection/)

## Futher thoughts to be exploited

Until now I said that

* if you need to inject a type `T` on runtime => inject the `Lazy<T>` instead
* if the construction of `T` requires async/await operation => inject the `Lazy<Task<T>>` instead

But what's about injecting `Func<T>`, `Task<T>`...?

* if you need to inject a type `T` on runtime => isn't it better to inject `Func<T>` instead?
* if the construction of `T` requires async/await operation => isn't it better to inject the `Func<Task<T>>` instead?
* what's about inject only a `Task<T>`? which scenario/use case would we do that?

## References

The idea of this "technique" comes from the [lamar documentation](https://jasperfx.github.io/lamar/guide/):

* <https://jasperfx.github.io/lamar/guide/ioc/injecting-at-runtime.html>
* <https://jasperfx.github.io/lamar/guide/ioc/lazy-resolution.html#bi-relational-dependency-workaround>

Note that: the lamar framework is going to be discontinued, I don't recommend using it on new project.
