using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Microsoft.Extensions.DependencyInjection;

namespace Injecter.Avalonia
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl() => Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

        protected IServiceScope? Scope { get; }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            Scope?.Dispose();
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);

            DataContext = ViewModel;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
