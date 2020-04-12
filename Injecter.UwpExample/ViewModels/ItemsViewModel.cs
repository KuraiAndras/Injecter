using Injecter.UwpExample.Model;
using Injecter.UwpExample.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Injecter.UwpExample.ViewModels
{
    public sealed class ItemsViewModel : IItemsViewModel
    {
        private readonly IItemService _itemService;
        private readonly ObservableCollection<Item> _items;

        public ItemsViewModel(IItemService itemService)
        {
            _itemService = itemService;

            _items = new ObservableCollection<Item>();

            Items = new ReadOnlyObservableCollection<Item>(_items);
        }

        public ReadOnlyObservableCollection<Item> Items { get; }

        public async Task InitializeAsync()
        {
            var items = await _itemService.GetAllItemsAsync();

            foreach (var item in items)
            {
                _items.Add(item);
            }
        }
    }
}
