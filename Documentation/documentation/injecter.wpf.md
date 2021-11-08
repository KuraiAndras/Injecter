# Injecter.WPF [![Nuget](https://img.shields.io/nuget/v/Injecter.Wpf)](https://www.nuget.org/packages/Injecter.Wpf/)

The WPF version of the library has helper classes that make it integrate into WPF seamlessly, offering multiple ways to consume the underlying library.

## Getting Started

In order to start using Injecter, you will need to set up you own DI container. You can use any container that is compatible with the Microsoft.Extensions.DependencyInjection.Abstractions package. In our example we will use the Microsoft.Extensions.DependencyInjection package.

1. Create a new WPF application (.NET Framework, .NET Core and .NET 5+ are all supported)

2. Add the NuGet package Microsoft.Extensions.DependencyInjection

3. Create some services or view models you want to inject

4. Modify your App.xaml.cs to create the DI container at startup

```csharp
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        // Create the DI container
        _serviceProvider = new ServiceCollection()
            .AddInjecter() // Register Injecter
            .AddSingleton<ICounter, Counter>() // Add your own services
            .BuildServiceProvider();

        // Set the ServiceProvider on the CompositionRoot
        CompositionRoot.ServiceProvider = _serviceProvider;

        // Dispose container when the application exits
        Exit += OnExit;
    }

    private void OnExit(object o, ExitEventArgs args) => _serviceProvider.Dispose();
}
```

5. Create the View you for the Counter

6. Inject via Attached Property

You can inject into any framework element you create using the a set of provided Attached Properties. In XAML apply the XamlInjecter.Inject attached property to initialize the injection (The value of the property (True or False) does not matter):

```xml
<UserControl
    x:Class="WpfSample.HelloControl"
    ...
    xmlns:wpf="clr-namespace:Injecter.Wpf;assembly=Injecter.Wpf"
    ...
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    d:DataContext="{d:DesignInstance sampleLogic:ICounter}"
    ...
    wpf:XamlInjecter.Inject="True">
```

You can also create a new IServiceScope when injecting into the Framework Element. When doing this you must define a DisposeBehavior to use:

```xml
<UserControl
    x:Class="WpfSample.HelloControl"
    ...
    xmlns:wpf="clr-namespace:Injecter.Wpf;assembly=Injecter.Wpf"
    ...
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    d:DataContext="{d:DesignInstance sampleLogic:ICounter}"
    ...
    wpf:XamlInjecter.InjectScoped="OnWindowClose">

```

After this any marked injection target in the class will be injected

```csharp
public partial class HelloControl
{
    [Inject] private ICounter ViewModel { get; } = default!;

    public HelloControl() => InitializeComponent();

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        DataContext = ViewModel;
    }

    // ...
}
```

If you want to get the IServiceScope associated with the control, you can get it via the IScopeStore interface

```csharp
public partial class HelloControl
{
    [Inject] private readonly IScopeStore _store = default!;

    public HelloControl()
    {
        InitializeComponent();
        IServiceScope scope = _store.GetScope(this);
    }
}
```

If you want to do manual injection you can do that by using the IInjecter interface directly:
```csharp
public partial class HelloControl
{
    [Inject] private ICounter ViewModel { get; } = default!;

    public HelloControl()
    {
        CompositionRoot.ServiceProvider
            .GetRequiredService<IInjecter>()
            .InjectIntoType(this, createScope: false);

        DataContext = ViewModer;

        InitializeComponent();
    }
}
```
