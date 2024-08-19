namespace TryPureDI.PerResolve;

public interface IDependency;

public class Dependency : IDependency;

public class Service(
    IDependency dep1,
    IDependency dep2,
    (IDependency dep3, IDependency dep4) deps)
{
    public IDependency Dep1 { get; } = dep1;

    public IDependency Dep2 { get; } = dep2;

    public IDependency Dep3 { get; } = deps.dep3;

    public IDependency Dep4 { get; } = deps.dep4;
}

public partial class Composition
{
    static void Setup() =>
        DI.Setup(nameof(Composition))
            // This hint indicates to not generate methods such as Resolve
            //.Hint(Hint.Resolve, "Off")
            .Bind().As(Lifetime.PerResolve).To<Dependency>()
            .Bind().As(Lifetime.Singleton).To<(IDependency dep3, IDependency dep4)>()

            // Composition root
            .Root<Service>(nameof(Service));
};

public class Runner
{
    [Fact]
    public void RunTest()
    {
        var composition = new Composition();

        var service1 = composition.Service;
        Assert.Equal(service1.Dep1, service1.Dep2);
        Assert.Equal(service1.Dep3, service1.Dep4);
        Assert.Equal(service1.Dep1, service1.Dep3);

        var service2 = composition.Service;
        Assert.NotEqual(service2, service1);
        Assert.NotEqual(service2.Dep1, service1.Dep1);
    }
}


