using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace Injecter.Xamarin.Forms
{
    public abstract class InjectedContentPage : ContentPage
    {
        protected InjectedContentPage(DisposeBehaviour behaviour = DisposeBehaviour.OnNoParent)
        {
            Behaviour = behaviour;
            Scope = CompositionRoot.ServiceProvider is not null
                ? CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this)
                : null;
        }

        public DisposeBehaviour Behaviour { get; }
        protected IServiceScope? Scope { get; }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null && Behaviour == DisposeBehaviour.OnNoParent)
            {
                Scope?.Dispose();
            }
        }
    }

    public abstract class InjectedContentPage<TViewModel> : InjectedContentPage
    {
        protected InjectedContentPage(DisposeBehaviour behaviour = DisposeBehaviour.OnNoParent)
            : base(behaviour)
            => BindingContext = ViewModel;

        [Inject] protected TViewModel ViewModel { get; } = default!;
    }
}
