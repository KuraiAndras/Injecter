# Injecter.Hosting.Unity [![openupm](https://img.shields.io/npm/v/com.injecter.hosting.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.hosting.unity/)

IHostLifeTime implementation for unity. Usage:

```csharp
IHost host = new HostBuilder()
    .ConfigureServices(/* */)
    .UseUnityLifetime()
    .Build();
```
