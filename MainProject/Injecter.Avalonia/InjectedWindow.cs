using Avalonia.Controls;
using Avalonia.LogicalTree;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter.Avalonia
{
    public abstract class InjectedWindow : Window
    {
        protected InjectedWindow() => Scope = CompositionRoot.ServiceProvider?.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

        protected IServiceScope? Scope { get; }

        protected override void OnClosed(EventArgs e)
        {
            Scope?.Dispose();

            base.OnClosed(e);
        }
    }

    public abstract class InjectedWindow<TViewModel> : InjectedWindow
    {
        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            DataContext = ViewModel;

            base.OnAttachedToLogicalTree(e);
        }
    }
}
