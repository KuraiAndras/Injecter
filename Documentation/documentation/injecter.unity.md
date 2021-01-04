# Injecter.Unity [![openupm](https://img.shields.io/npm/v/com.injecter.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.injecter.unity/)

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
