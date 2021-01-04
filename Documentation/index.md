# Injecter [![Nuget](https://img.shields.io/nuget/v/Injecter)](https://www.nuget.org/packages/Injecter/)

Small library to provide an attribute named Inject, and a service which injects the field, property or method marked with the injected attribute.

The main usage target is for in frameworks which don't support dependency injection and create classes only with a parameterless constructor. Examples: WPF UserControl, Unity MonoBehavior, etc...

This project also provides a helper implementations for use in various .Net client frameworks.

| Platform | Provided classes | Package |
| --- | --- | --- |
| Unity | InjectedMonoBehavior, SceneInjector, InjectStarter | [![openupm](https://img.shields.io/npm/v/com.injecter.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity/) |
| WPF | InjectedUserControl, InjectedWindow | [![Nuget](https://img.shields.io/nuget/v/Injecter.Wpf)](https://www.nuget.org/packages/Injecter.Wpf/) |
| UWP | InjectedPage, InjectedUserControl |  [![Nuget](https://img.shields.io/nuget/v/Injecter.Uwp)](https://www.nuget.org/packages/Injecter.Uwp/) |
| Xamarin.Forms | InjectedPage| [![Nuget](https://img.shields.io/nuget/v/Injecter.Xamarin.Forms)](https://www.nuget.org/packages/Injecter.Xamarin.Forms/) |
| Avalonia | InjectedUserControl, InjectedWindow | [![Nuget](https://img.shields.io/nuget/v/Injecter.Avalonia)](https://www.nuget.org/packages/Injecter.Avalonia/) |

## Injecter.Hosting

GenericHost extensions for client side frameworks.

| Platform | Package |
| --- | --- |
| Unity | [![openupm](https://img.shields.io/npm/v/com.injecter.hosting.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.hosting.unity/) |
| WPF | [![Nuget](https://img.shields.io/nuget/v/Injecter.Hosting.Wpf)](https://www.nuget.org/packages/Injecter.Hosting.Wpf/) |
