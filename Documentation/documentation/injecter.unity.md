# Injecter.Unity [![openupm](https://img.shields.io/npm/v/com.injecter.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity/)

Since version 3.0.1 you need to provide the following dlls yourself:
- Injecter
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.DependencyInjection.Abstractions

> [!NOTE]
> The recommended way of installing NuGet packages is through the [UnityNuget](https://github.com/xoofx/UnityNuGet) project

## Fundamentals

The `Injecter.Unity` lets you set up the following flow:

- A "composition root" is initialized part of the entry point of the application
- Create a script which needs to be injected
- Add `MonoInjector` to the `GameObject` hosting the script
- `MonoInjector` runs at `Awake`, and it's execution order (`int.MinValue` - first) is run before your own component's `Awake`. Every injected script will have it's own `IServiceScope` derived from the root scope. This scope can be retrieved through the `IScopeStore`, and the owner of the scope is the script being injected
- When the `GameObject` is destroyed, `MonoDisposer` will run during the `OnDestroy` method, with an execution order of `int.MaxValue` - last

## Getting started

### Install dependencies

1. Install `Injecter` and `Microsoft.Extensions.DependencyInjection` through [UnityNuget](https://github.com/xoofx/UnityNuGet#unitynuget-).
2. Install `Injecter.Unity` through [openupm](https://openupm.com/packages/com.injecter.unity/)
```bash
openupm add com.injecter.unity
```

### Setup root

Either create manually, or through the `Assets / Injecter` editor menu, create your composition root.

```csharp
#nullable enable
using Injecter;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public static class AppInstaller
{
    /// <summary>
    /// Set this from test assembly to disable
    /// </summary>
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Install()
    {
        if (!Run) return;

        var serviceProvider = new ServiceCollection()
            .Configure()
            .BuildServiceProvider(true);

        // Injected scripts will get the root service provider from this instance
        CompositionRoot.ServiceProvider = serviceProvider;

        Application.quitting += OnQuitting;

        /// <summary>
        /// Will dispose of all services when quitting
        /// </summary>
        async void OnQuitting()
        {
            Application.quitting -= OnQuitting;

            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }
    }

    public static IServiceCollection Configure(this IServiceCollection services)
    {
        services.AddInjecter(o => o.UseCaching = true);
        // TODO: Add services

        return services;
    }
}
```

### Inject into `MonoBehaviours`

Create a script which will receive injection

```csharp
[RequireComponent(typeof(MonoInjector))]
public class MyScript : MonoBehaviour
{
    [Inject] private readonly IMyService _service = default!;
}
```

### Add `MonoInjector`

If you decorate your script with `[RequireComponent(typeof(MonoInjector))]` then, when adding the script to a `GameObject` the editor will add the `MonoInjector` and the `MonoDisposer` script to your `GameObject`. If for some reason this does not happen (for example, when changing an already living script into one needing injection), either add the `MonoInjector` component manually, or use the editor tools included to add the missing components to scenes or prefabs (will search all instances) through the editor menu `Tools / Injecter / ...`

## Manual injection

When dynamically adding an injectable script to a `GameObject` you need to handle the injection and disposal manually.

```csharp

[Inject] private readonly IScopeStore _scopeStore = default!;

var myObject = Instantiate(prefab);
var myScript = myObject.AddComponent<MyScript>();

injecter.InjectIntoType(myScript, true);
var disposer = myObject.AddComponent<MonoInjector>();
disposer.Initialize(myScript, _scopeStore);

```

> [!Warning]
> When doing this manually, the later injected script's `Awake` method might run before the injection happens

## Testing

You can load tests and prefabs with controlled dependencies when running tests inside Unity. To do this create the following class in your test assembly:

```csharp
public static class InstallerStopper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void DisableAppInstaller()
    {
        if (Environment.GetCommandLineArgs().Contains("-runTests")
            || EditorWindow.HasOpenInstances<TestRunnerWindow>())
        {
            AppInstaller.Run = false;
        }
    }
}
```

This will stop your `AppInstaller` from running when you execute tests. This will happen if either you have the test runner window open, or you are running Unity headless with the `-runTests` parameter (typically during CI).

> [!WARNING]
> If you do this, then you must close the test runner window when entering play mode, otherwise the `AppInstaller` will not run

In your tests set up the composition root manually

```csharp
[UnityTest]
public IEnumerator My_Test_Does_Stuff()
{
    CompositionRoot.ServiceProvider = new ServiceCollection()
        .AddTransient<IService, MyTestService>()
        .BuildServiceProvider();

    // Do your tests, load scenes, prefabs, asserts etc...

    CompositionRoot.ServiceProvider.Dispose();
    (CompositionRoot.ServiceProvider as IDisposable)?.Dispose();
};
```

> [!NOTE]
> You can also do the same in the test's `Setup` or `Teardown` stage

## Migrating from `8.0.1` to `9.0.0`

1. Set up a composition root as described above.
2. Remove inheriting from the old `MonoBehaviourInjected` and similar classes
3. Optional - Decorate your injected scripts with `[RequireComponent(typeof(MonoInjector))]`
4. In the editor press the `Tools / Injecter / Ensure injection scripts on everyting` button
5. If a `GameObject` is missing the `MonoInjector` or `MonoDisposer` scripts, add them
