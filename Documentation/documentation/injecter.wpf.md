# Injecter.WPF [![Nuget](https://img.shields.io/nuget/v/Injecter.Wpf)](https://www.nuget.org/packages/Injecter.Wpf/)

The wpf helper includes different disposing behaviors for user controls.

```csharp
public enum DisposeBehaviour
{
    /// <summary>
    /// Dispose on application shutdown when <see cref="System.Windows.Threading.Dispatcher"/>> fires the ShutdownStarted event.
    /// </summary>
    OnDispatcherShutdown,

    /// <summary>
    /// Dispose when the window containing the control is closed.
    /// </summary>
    OnWindowClose,

    /// <summary>
    /// Dispose when the control is unloaded. It is useful for ListViewItems and similar.
    /// </summary>
    OnUnloaded,

    /// <summary>
    /// Don't dispose automatically.
    /// </summary>
    Manual,
}
```
