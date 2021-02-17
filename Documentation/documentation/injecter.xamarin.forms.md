# Injecter.Xamarin.Forms [![Nuget](https://img.shields.io/nuget/v/Injecter.Xamarin.Forms)](https://www.nuget.org/packages/Injecter.Xamarin.Forms/)

The Xamarin.Forms version of the library has helper classes that make it integrate into Xamarin.Forms seamlessly, offering multiple ways to consume the underlying library.

## Getting Started

In order to start using Injecter, you will need to set up you own DI container. You can use any container that is compatible with the Microsoft.Extensions.DependencyInjection.Abstractions package. In our example we will use the Microsoft.Extensions.DependencyInjection package.

1. Create a new Xamarin.Forms application

2. Add the NuGet package Microsoft.Extensions.DependencyInjection

3. Create some services or view models you want to inject

4. Modify your App.xaml.cs to create the DI container at startup

```csharp
public partial class App : Application
{
    public App()
    {
        CompositionRoot.ServiceProvider = new ServiceCollection()
            .AddSharedLogic()
            .BuildServiceProvider();

        InitializeComponent();

        MainPage = new MainPage();
    }

    // ...
}
```

5. Create the View you for the Counter

6. Inject via Attached Property

## Inject via Attached Property

You can inject into any framework element you create using the a set of provided Attached Properties. In XAML apply the XamlInjecter.Inject attached property to initialize the injection (The value of the property (True or False) does not matter):

```xml
<ContentPage
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:d="http://xamarin.com/schemas/2014/forms/design"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:sampleLogic="clr-namespace:SampleLogic;assembly=SampleLogic"
    xmlns:forms="clr-namespace:Injecter.Xamarin.Forms;assembly=Injecter.Xamarin.Forms"
    mc:Ignorable="d"
    x:Class="XamarinSample.MainPage"
    x:DataType="sampleLogic:ICounter"

    forms:XamlInjecter.Inject="True">
```

You can also create a new IServiceScope when injecting into the Control. When doing this you must define a DisposeBehavior to use:

```xml
<ContentPage
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:d="http://xamarin.com/schemas/2014/forms/design"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:sampleLogic="clr-namespace:SampleLogic;assembly=SampleLogic"
    xmlns:forms="clr-namespace:Injecter.Xamarin.Forms;assembly=Injecter.Xamarin.Forms"
    mc:Ignorable="d"
    x:Class="XamarinSample.MainPage"
    x:DataType="sampleLogic:ICounter"
    
    forms:XamlInjecter.InjectScoped="OnDisappearing">
```

The OnDisappearing behavior is only supported for Page, Cell and BaseShellItem types.

After this any marked injection target in the class will be injected

```csharp
public partial class MainPage
{
    [Inject] public ICounter ViewModel { get; } = default!;

    public MainPage()
    {
        InitializeComponent();

        BindingContext = ViewModel;
    }

    // ...
}
```

If you want to get the IServiceScope associated with the control, you can get it via the IScopeStore interface

```csharp
public partial class MainPage
{
    [Inject] private readonly IScopeStore _store = default!;

    public MainPage()
    {
        InitializeComponent();

        IServiceScope scope = _store.GetScope(this);
    }
}
```

If you want to do manual injection you can do that by using the IInjecter interface directly:
```csharp
public partial class MainPage
{
    [Inject] private ICounter ViewModel { get; } = default!;

    public MainPage()
    {
        CompositionRoot.ServiceProvider
            .GetRequiredService<IInjecter>()
            .InjectIntoType(this, createScope: false);

        DataContext = ViewModer;

        InitializeComponent();
    }
}
```
