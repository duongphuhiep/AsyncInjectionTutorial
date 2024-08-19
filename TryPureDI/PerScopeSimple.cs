using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace TryPureDI.PerScopeSimple;

public class MySingleton : ITestOutputHelperAccessor
{
    public ITestOutputHelper? OutputHelper { get; set; }
    public MySingleton()
    {
        OutputHelper?.WriteLine($"ctor MySingleton object");
    }
}

public interface IMyScoped
{
    MySingleton MySingleton { get; }
}

class MyScoped : IMyScoped
{
    private readonly MySingleton mySingleton;

    public MyScoped(MySingleton mySingleton, ITestOutputHelperAccessor outputHelperAccessor)
    {
        this.mySingleton = mySingleton;
        outputHelperAccessor.OutputHelper?.WriteLine($"ctor MyScoped object");
    }

    public MySingleton MySingleton => mySingleton;

}

public interface IMyTransient
{
    IMyScoped MyScoped { get; }
}

class MyTransient : IMyTransient
{
    private readonly IMyScoped myScoped;

    public MyTransient(IMyScoped myScoped, ITestOutputHelperAccessor outputHelperAccessor)
    {
        this.myScoped = myScoped;
        outputHelperAccessor.OutputHelper?.WriteLine($"ctor MyTransient object");
    }

    public IMyScoped MyScoped => myScoped;
};

public partial class Composition
{
    static void Setup() => DI.Setup()
            .Bind().To<MySingleton>()
            .Bind().As(Lifetime.Singleton).To<MySingleton>()
            .Bind().As(Lifetime.Scoped).To<MyScoped>()
            .Bind().As(Lifetime.Transient).To<MyTransient>()
            .Root<IMyTransient>(nameof(MyTransient))
            .Root<ITestOutputHelperAccessor>("TestOutputHelperAccessor");

}

public class Runner(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void RunTest()
    {
        //root
        var compositionRoot = new Composition();
        compositionRoot.TestOutputHelperAccessor.OutputHelper = testOutputHelper;
        Assert.NotEqual(compositionRoot.MyTransient, compositionRoot.MyTransient);
        Assert.Equal(compositionRoot.MyTransient.MyScoped, compositionRoot.MyTransient.MyScoped);

        //scoped1
        var composition1 = new Composition(compositionRoot);
        Assert.NotEqual(composition1.MyTransient, composition1.MyTransient);
        Assert.Equal(composition1.MyTransient.MyScoped, composition1.MyTransient.MyScoped);
        Assert.NotEqual(composition1.MyTransient.MyScoped, compositionRoot.MyTransient.MyScoped);
        Assert.Equal(composition1.MyTransient.MyScoped.MySingleton, compositionRoot.MyTransient.MyScoped.MySingleton);

        //scoped2
        var composition2 = new Composition(compositionRoot);
        Assert.NotEqual(composition2.MyTransient, composition2.MyTransient);
        Assert.Equal(composition2.MyTransient.MyScoped, composition2.MyTransient.MyScoped);
        Assert.NotEqual(composition2.MyTransient.MyScoped, composition1.MyTransient.MyScoped);
        Assert.Equal(composition2.MyTransient.MyScoped.MySingleton, composition1.MyTransient.MyScoped.MySingleton);
    }

    [Fact]
    public void RunTest_MSDI()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<MySingleton>();
        serviceCollection.AddSingleton<ITestOutputHelperAccessor, MySingleton>();
        serviceCollection.AddScoped<IMyScoped, MyScoped>();
        serviceCollection.AddTransient<IMyTransient, MyTransient>();

        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var scope1 = serviceProvider.CreateScope();
        var service11 = scope1.ServiceProvider.GetRequiredService<IMyTransient>();
        var service12 = scope1.ServiceProvider.GetRequiredService<IMyTransient>();

        Assert.NotEqual(service11, service12);
        Assert.Equal(service11.MyScoped, service12.MyScoped);

        var scope2 = serviceProvider.CreateScope();
        var service21 = scope2.ServiceProvider.GetRequiredService<IMyTransient>();
        Assert.NotEqual(service11.MyScoped, service21.MyScoped);
        Assert.Equal(service11.MyScoped.MySingleton, service21.MyScoped.MySingleton);
    }
}


