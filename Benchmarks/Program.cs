using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Injecter;
using Microsoft.Extensions.DependencyInjection;

BenchmarkRunner.Run(typeof(Program).Assembly);

[MemoryDiagnoser]
public class InjectionBenchmarks
{
    private IInjecter _baseline = default!;
    private IInjecter _current = default!;
    private ServiceProvider _serviceProvider;

    public InjectionBenchmarks()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<Injecter.Injecter>()
            .AddSingleton<BaselineInjecter>()
            .AddSingleton<IScopeStore, ScopeStore>()
            .AddSingleton(new InjecterOptions { UseCaching = true })
            .AddTransient<ServiceA>()
            .AddTransient<ServiceB>()
            .AddTransient<ServiceC>()
            .BuildServiceProvider();

        _baseline = _serviceProvider.GetRequiredService<BaselineInjecter>();
        _current = _serviceProvider.GetRequiredService<Injecter.Injecter>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider.Dispose();

        _baseline = null!;
        _current = null!;
        _serviceProvider = null!;
    }

    [Benchmark]
    public (ServiceA, ServiceB, ServiceC) BaseLine()
    {
        var target = new Target();
        _baseline.InjectIntoType(target, true);

        return target.GetServices();
    }

    [Benchmark]
    public (ServiceA, ServiceB, ServiceC) Current()
    {
        var target = new Target();
        _current.InjectIntoType(target, true);

        return target.GetServices();
    }
}

public sealed class ServiceA { }
public sealed class ServiceB { }
public sealed class ServiceC { }

public abstract class AbstractTarget
{
    [Inject] protected ServiceA A { get; } = default!;
}

public class Target : AbstractTarget
{
    [Inject] private readonly ServiceB _b = default!;

    private ServiceC _c = default!;

    public ServiceB B => _b;
    public ServiceC C => _c;

    [Inject]
    public void Construct(ServiceC c) => _c = c;

    public (ServiceA, ServiceB, ServiceC) GetServices() => (A, _b, _c);
}
