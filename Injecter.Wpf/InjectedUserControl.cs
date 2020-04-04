﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Injecter.Wpf
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Unloaded += UnloadHandler;
        }

        protected IServiceScope Scope { get; }

        protected virtual void UnloadHandler(object o, RoutedEventArgs rea)
        {
            Scope?.Dispose();

            Unloaded -= UnloadHandler;
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl() => ViewModel = Scope.ServiceProvider.GetRequiredService<TViewModel>();

        protected TViewModel ViewModel { get; }

        protected override void UnloadHandler(object o, RoutedEventArgs rea)
        {
            base.UnloadHandler(o, rea);

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}