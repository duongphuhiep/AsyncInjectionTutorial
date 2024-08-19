namespace TryPureDI.PerScope;

public interface IDependency
{
    bool IsDisposed { get; }
}

class Dependency : IDependency, IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose() => IsDisposed = true;
}

public interface IService
{
    IDependency Dependency { get; }
}

class Service(IDependency dependency) : IService
{
    public IDependency Dependency => dependency;
}

// Implements a session
public class Session(Composition composition) : Composition(composition)
{
    public Composition InnerComposition => composition;
}

public class Program(Func<Session> sessionFactory)
{
    public Session CreateSession() => sessionFactory();
}

public partial class Composition
{
    static void Setup() =>
        DI.Setup()
            // This hint indicates to not generate methods such as Resolve
            .Hint(Hint.Resolve, "Off")
            .Bind().As(Lifetime.Scoped).To<Dependency>()
            .Bind().To<Service>()
            .Bind().As(Lifetime.Singleton).To<Program>()

            // Session composition root
            .Root<IService>("SessionRoot")

            // Program composition root
            .Root<Program>("ProgramRoot");
}

public class Runner
{
    [Fact]
    public void RunTest()
    {
        var rootComposition = new Composition();
        var program = rootComposition.ProgramRoot;

        // Creates session #1
        var session1 = program.CreateSession();
        var dependency1 = session1.SessionRoot.Dependency;
        var dependency12 = session1.SessionRoot.Dependency;

        // Checks the identity of scoped instances in the same session
        Assert.Equal(dependency1, dependency12);

        // Creates session #2
        var session2 = program.CreateSession();
        var dependency2 = session2.SessionRoot.Dependency;

        Assert.Equal(session1.ProgramRoot, session2.ProgramRoot);
        Assert.Equal(session1.InnerComposition, session2.InnerComposition);
        Assert.Equal(session1.InnerComposition, rootComposition);

        // Checks that the scoped instances are not identical in different sessions
        Assert.NotEqual(dependency1, dependency2);

        // Disposes of session #1
        session1.Dispose();
        // Checks that the scoped instance is finalized
        Assert.True(dependency1.IsDisposed);

        // Disposes of session #2
        session2.Dispose();
        // Checks that the scoped instance is finalized
        Assert.True(dependency2.IsDisposed);
    }
}