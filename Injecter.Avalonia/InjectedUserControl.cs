using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter.Avalonia
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl() => Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

        protected IServiceScope Scope { get; }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            Scope?.Dispose();
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl() => ViewModel = Scope.ServiceProvider.GetRequiredService<TViewModel>();

        protected TViewModel ViewModel { get; }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);

            DataContext = ViewModel;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
