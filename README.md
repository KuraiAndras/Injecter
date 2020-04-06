# Injecter

[![Nuget](https://img.shields.io/nuget/v/Injecter)](https://www.nuget.org/packages/Injecter/)

Small library to provide an attribute named Inject, and a service which injects the field, property or method marked with the injected attribute.

The main usage target is for in frameworks which don't support dependency injection and create classes only with a parameterless constructor. Examples: WPF UserControl, Unity MonoBehavior, etc...

This project also provides a helper implementations for use in various .Net client frameworks.

## Usage

```c#
//Setup

IServiceCollection services = ;// Get your IServiceCollection

// Add injecter
services.AddInjecter(options => options.UseCaching = true)

// Usage

IInjecter injecter = ;// Get a hold of an IInjecter instance
MyUserControl contol = ;// Get some class you want to inject into

// Call injecter with one of the api methods
injecter.InjectIntoType(typeof(MyUserControl), control);
IServiceScope scope = injecter.InjectIntoType<MyUserControl>(control);
// You should dispose of the returned scope when the injection target is no longer in use.

public class MyUserControl : UserControl
{
    [Inject] private readonly ISomeService _service;

    [Inject] private ISomeOtherService OtherService { get; }

    [Inject]
    private void Construct(IAnotherService service1, IEvenAnotherService service2)
    {
        // Assign to fields.
    }
}

```

Supported injection methods for InjectAttribute: Field, Property, Method. Injection happens in this order. 

To make things easier when working with some actual framework a composition root is provided. This static class holds an instance for IServiceProvider and can be used to use this library easier.

```c#
using System;

namespace Injecter
{
    public static class CompositionRoot
    {
        public static IServiceProvider ServiceProvider { get; set; } = default;
    }
}
```

If you are using CompositionRoot it is advised to set the ServiceProvider property at the entry point of your application after service registration. One example from UWP:

```c#
public sealed partial class App : Windows.UI.Xaml.Application
{
    private readonly IHost _host;

    public App()
    {
        // Yay even compatible with Generic Host!!!
        _host = new HostBuilder()
            .ConfigureLogging(builder => builder.AdMyLogging())
            .ConfigureServices(services => services.AddMyServices())
            .Build();

        // Set composition root
        CompositionRoot.ServiceProvider = _host.Services;

        _host.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

        _host.Start();

        InitializeComponent();
        Suspending += OnSuspending;
    }
}
```

Regular console application:

```c#

public static void Main(string[] args)
{
    var serviceProvider = new ServiceCollection().AddMyServices().BuildServiceProvider();

    // Set composition root
    CompositionRoot.ServiceProvider = serviceProvider;
}

```

Preferably you want to inject into instances before you start using them. For that purpose this project provides several other libraries.

## Default services:

```c#
public static IServiceCollection AddInjecter(this IServiceCollection services, Action<InjecterOptions> optionsBuilder = null)
{
    if (services is null) throw new ArgumentNullException(nameof(services));

    var options = new InjecterOptions();

    optionsBuilder?.Invoke(options);

    services.AddSingleton(options);
    services.AddSingleton<IInjecter, Injecter>();

    return services;
}
```

Options can be changed during runtime.

## Options

| Name | Description | Default value|
|---|---|---|
| UseCaching | During injection cache the fields, properties, methods needing injection | True|

# Framework implementations

Inherit from classes provided by these frameworks. Each call CompositionRoot.ServiceProvider to retrieve an IInjecter instance in the parameterless constructor and then injects into that class.

These implementations rely on setting CompositionRoot.ServiceProvider at the startup of your application.

The generic overrides of these classes resolves the generic type parameter and sets an instance of that as the DataContext/BindingContext when applicable, and provide a protected property called ViewModel with the provided type.

| Platform | Provided classes | Package | Project site |
| --- | --- | --- | --- |
| Avalonia | InjectedUserControl, InjectedWindow | [![Nuget](https://img.shields.io/nuget/v/Injecter.Avalonia)](https://www.nuget.org/packages/Injecter.Avalonia/) | [link](https://github.com/KuraiAndras/Injecter) |
| Injecter.Unity | InjectedMonoBehavior | [![openupm](https://img.shields.io/npm/v/com.injecter.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity/) | [link](https://github.com/KuraiAndras/Injecter) |
| UnityExtensions.DependencyInjection | SceneInjector, Injector | [![openupm](https://img.shields.io/npm/v/com.unityextensions.dependencyinjection?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.unityextensions.dependencyinjection/) | [link](https://github.com/KuraiAndras/UnityExtensions.DependencyInjection) |
| UWP | InjectedPage, InjectedUserControl |  [![Nuget](https://img.shields.io/nuget/v/Injecter.Uwp)](https://www.nuget.org/packages/Injecter.Uwp/) | [link](https://github.com/KuraiAndras/Injecter) |
| WPF | InjectedUserControl, InjectedWindow | [![Nuget](https://img.shields.io/nuget/v/Injecter.Wpf)](https://www.nuget.org/packages/Injecter.Wpf/) | [link](https://github.com/KuraiAndras/Injecter) |
| Xamarin.Forms | InjectedPage| [![Nuget](https://img.shields.io/nuget/v/Injecter.Xamarin.Forms)](https://www.nuget.org/packages/Injecter.Xamarin.Forms/) | [link](https://github.com/KuraiAndras/Injecter) |

Other contributions are welcome, if you want this project to extend to some other libraries, feel free to raise an issue, or submit a pull request!