# Injecter.Hosting.Wpf [![Nuget](https://img.shields.io/nuget/v/Injecter.Hosting.Wpf)](https://www.nuget.org/packages/Injecter.Hosting.Wpf/)

IHostLifeTime implementation for WPF. Usage:

```csharp
IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(/* */)
    .UseWpfLifetime()
    .Build();
```
