# Usage

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
    [Inject] private readonly ISomeService _service = default!;

    [Inject] private ISomeOtherService OtherService { get; } = default!

    [Inject]
    private void Construct(IAnotherService service1, IEvenAnotherService service2)
    {
        // Assign to fields.
    }
}

```

Since version 4.0.0 the IInjecter interface can be run without creating a new service scope
```c#
IServiceScope? scope = injecter.InjectIntoType<MyUserControl>(control, false);
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

## Default services

| Service Type    | Implementation Type | Lifetime  |
|-----------------|---------------------|-----------|
| InjecterOptions | InjecterOptions     | Singleton |
| IInjecter       | Injecter            | Singleton |
| IScopeStore     | ScopeStore          | Singleton |

## Options

Options can be changed during runtime through .

| Name       | Description                                                              | Default value |
|------------|--------------------------------------------------------------------------|---------------|
| UseCaching | During injection cache the fields, properties, methods needing injection | True          |
