# Injecter.UWP [![Nuget](https://img.shields.io/nuget/v/Injecter.Uwp)](https://www.nuget.org/packages/Injecter.Uwp/)

The UWP version of the library has helper classes that make it integrate into UWP seamlessly, offering multiple ways to consume the underlying library.

## Getting Started

In order to start using Injecter, you will need to set up you own DI container. You can use any container that is compatible with the Microsoft.Extensions.DependencyInjection.Abstractions package. In our example we will use the Microsoft.Extensions.DependencyInjection package.

1. Create a new UWP application

2. Add the NuGet package Microsoft.Extensions.DependencyInjection

3. Create some services or view models you want to inject

4. Modify your App.xaml.cs to create the DI container at startup

```csharp
public sealed partial class App : Application
{
    public App()
    {
        CompositionRoot.ServiceProvider = new ServiceCollection()
            .AddSharedLogic()
            .BuildServiceProvider();

        InitializeComponent();
        // ...
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        Window.Current.Closed += (_, _) => ((IDisposable)CompositionRoot.ServiceProvider!).Dispose();
        // ..
    }

    // ...
}
```

5. Create the View you for the Counter

6. Inject via Attached Property

## Inject via Attached Property

You can inject into any framework element you create using the a set of provided Attached Properties. In XAML apply the XamlInjecter.Inject attached property to initialize the injection (The value of the property (True or False) does not matter):

```xml
<UserControl
    x:Class="UwpSample.HelloControl"
    ...
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:uwp="using:Injecter.Uwp"
    mc:Ignorable="d"
    ...
    d:DataContext="{d:DesignInstance sampleLogic:ICounter}"
    uwp:XamlInjecter.Inject="True">
```

You can also create a new IServiceScope when injecting into the Framework Element. When doing this you must define a DisposeBehavior to use:

```xml
<UserControl
    x:Class="UwpSample.HelloControl"
    ...
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:uwp="using:Injecter.Uwp"
    mc:Ignorable="d"
    ...
    d:DataContext="{d:DesignInstance sampleLogic:ICounter}"
    uwp:XamlInjecter.InjectScoped="OnUnloaded">

```

After this any marked injection target in the class will be injected

```csharp
public partial class HelloControl
{
    [Inject] public ICounter ViewModel { get; } = default!;

    public HelloControl()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
    // ..
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
