using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Microsoft.Extensions.DependencyInjection;

namespace Injecter.Avalonia
{
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl() =>
            Scope = CompositionRoot.ServiceProvider?.GetRequiredService<IInjecter>().InjectIntoType(this, false);

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
}
