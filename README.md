# Injecter

[![Nuget](https://img.shields.io/nuget/v/Injecter)](https://www.nuget.org/packages/Injecter/) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KuraiAndras_Injecter&metric=alert_status)](https://sonarcloud.io/dashboard?id=KuraiAndras_Injecter) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KuraiAndras_Injecter&metric=coverage)](https://sonarcloud.io/dashboard?id=KuraiAndras_Injecter)

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
| Unity | InjectedMonoBehavior, SceneInjector, InjectStarter | [![openupm](https://img.shields.io/npm/v/com.injecter.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity/) | [link](https://github.com/KuraiAndras/Injecter) |
| UWP | InjectedPage, InjectedUserControl |  [![Nuget](https://img.shields.io/nuget/v/Injecter.Uwp)](https://www.nuget.org/packages/Injecter.Uwp/) | [link](https://github.com/KuraiAndras/Injecter) |
| WPF | InjectedUserControl, InjectedWindow | [![Nuget](https://img.shields.io/nuget/v/Injecter.Wpf)](https://www.nuget.org/packages/Injecter.Wpf/) | [link](https://github.com/KuraiAndras/Injecter) |
| Xamarin.Forms | InjectedPage| [![Nuget](https://img.shields.io/nuget/v/Injecter.Xamarin.Forms)](https://www.nuget.org/packages/Injecter.Xamarin.Forms/) | [link](https://github.com/KuraiAndras/Injecter) |

Other contributions are welcome, if you want this project to extend to some other libraries, feel free to raise an issue, or submit a pull request!

# Injecter.Unity

Since version 3.0.1 you need to provide the following dlls yourself:
- Injecter
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.DependencyInjection.Abstractions

## Initialize
Create a class that inherits from InjectStarter:
```c#
// Customize script execution order, so Awake is called first in you scene
// Usually -999 works nicely
[DefaultExecutionOrder(-999)]
public sealed class ExampleInjector : InjectStarter
{
    // Override CreateServiceProvider to add service registrations
    protected override IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        // Mandatory to call AddSceneInjector, optionally configure options
        services.AddSceneInjector(
            injecterOptions => injecterOptions.UseCaching = true,
            sceneInjectorOptions =>
            {
                sceneInjectorOptions.DontDestroyOnLoad = true;
                sceneInjectorOptions.InjectionBehavior = SceneInjectorOptions.Behavior.Factory;
            });


        // Use the usual IServiceCollection methods
        services.AddTransient<IExampleService, ExampleService>();

        // Resolve scripts already in the scene with FindObjectOfType()
        services.AddSingleton<MonoBehaviourService>(_ => GameObject.FindObjectOfType<MonoBehaviourService>());

        // Either:

        // Return a built ServiceProvider
        return services.BuildServiceProvider();
    }
}
```

Add this script to any one GameObject in your scene.

## Usage in MonoBehavior

Use the InjectAttribute to inject into a MonoBehavior:

**When using the CompositionRoot Behavior option you need to inherit from InjectedMonoBehavior**

```c#
public class ExampleScript : MonoBehaviour
{
    [Inject] private readonly IExampleService1 _exampleService1;
    [Inject] private IExampleService2 ExampleService2 { get; }

    private IExampleService3 _exampleService3;

    [Inject]
    private void Construct(IExampleService3 exampleService3)
    {
        _exampleService3 = exampleService3;
    }
}
```

Supported injection methods for InjectAttribute: Field, Property, Method. Injection happens in this order. **Constructor injection does not work.**

## Usage in Prefabs when using the Factory Behavior option

Injecting into prefabs:

```c#
// Get a prefab that contains a script which needs injection.
GameObject prefab = ;
// IGameObjectInjector and ISceneInjector are services added by default to Services
IGameObjectFactory gameObjectFactory = ;
ISceneInjector sceneInjector = ;

// Either:

// Instantiate the usual way
var instance = GameObject.Instantiate(prefab);
// Inject into freshly created GameObject
sceneInjector.InjectIntoGameObject(instance);

// Or:

// Use IGameObjectFactory which wraps GameObject.Instantiate(...) methods
var instance = gameObjectFactory.Instantiate(prefab); // Prefab is created and injected
```
You don't have to call InjectIntoGameObject on prefab children. When InjectIntoGameObject is called all the scripts on the game object and it's children which have the InjectAttribute gets injected.

## Scopes, Disposables

 - An IServiceScope is created for every script found in a GameObject.
 - Thus each MonoBehavior injected has it's own scope (Scoped lifetime services start from here).
 - A DestroyDetector script is added to every GameObject that receives injection. When the game object is destroyed, the DestroyDetector disposes of all the scopes that got created for that specific game object.
 - Thus if you create a prefab, destroy one of it's children then only the scopes associated with that child are disposed.
 - DestroyDetector is internal, and is hidden in the Inspector.
 - Destroying the game object holding the SceneInjector disposes of the IServiceProvider if it is disposable
 - When using the CompositionRoot behavior option the InjectedMonoBehavior handles disposing of the scope when destroyed and no DestroyDetector is created.

## Options

You can customize some behavior of the SceneInjector by providing an action to configure the options when calling AddSceneInjector

Current options:

| Name | Description | Default value|
|---|---|---|
| Behavior | CompositionRoot: Use the static service provider with inherited MonoBehaviors. Factory: use the SceneInjector and IGameObjectFactory | Factory |
| DontDestroyOnLoad | Calls GameObject.DontDestroyOnLoad(SceneInjector) during initialization. This prevents the game object from being destroyed | True |

## Notes
  - To see sample usage check out tests and test scenes

# Injecter.Unity.Hosting [![openupm](https://img.shields.io/npm/v/com.injecter.unity.hosting?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity.hosting/)

IHostLifeTime implementation for unity. Usage:

```csharp
IHost host = new HostBuilder()
    .ConfigureServices(/* */)
    .UseUnityLifetime()
    .Build();
```

# Injecter.FastMediatR [![Nuget](https://img.shields.io/nuget/v/Injecter.FastMediatR)](https://www.nuget.org/packages/Injecter.FastMediatR/)

An IMediatR proxy which helps you use MediatR in performance critical scenarios.

Using the IFastMediator interface results in:

- No pipeline behaviors are called.
- The call is not asynchronous.
- When repeating a request with an IFastMediator instance the request handlers get cached.

## Usage
```c#
// Create a regular request
public sealed class Add : IRequest<int>
{
    public int A { get; set; }

    public int B { get; set; }
}

// Implement ISyncHandler
public sealed class AddHandler : ISyncHandler<Add, int>
{
    public Task<int> Handle(Add request, CancellationToken cancellationToken) => Task.FromResult(HandleSync(request));

    public int HandleSync(Add request) => request.A + request.B;
}

// Add MediatR and FastMediatR
IServiceProvider serviceProvider = new ServiceCollection()
    .AddFastMediatR()
    .AddMediatR() //Add assemblies
    .BuildServiceProvider()

// Use it
IFastMediator fastMediator = serviceProvider.GetRequiredService<IFastMediator>();
Add request = new Add { A = 1, B = 1 };

int result = fastMediator.SendSync(request);

Assert.Equal(2, result);

fastMediator.Dispose();
```

When sending your requests through IMediator everything works the usual way, because ISyncHandler inherits from IRequestHandler.