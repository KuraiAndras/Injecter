using Injecter.Uwp;
using Injecter.UwpExample.Services;
using Injecter.UwpExample.ViewModels;
using Windows.UI.Popups;

namespace Injecter.UwpExample
{
    public sealed partial class MainPage : MainPageBase
    {
        [Inject] private readonly IItemService _itemService = default;

        public MainPage()
        {
            InitializeComponent();

            ViewModel.InitializeAsync();
            var items = _itemService.GetAllItemsAsync().GetAwaiter().GetResult();

            _ = new MessageDialog(items.Count.ToString()).ShowAsync();
        }
    }

    public abstract class MainPageBase : InjectedPage<IItemsViewModel>
    {
    }
}
