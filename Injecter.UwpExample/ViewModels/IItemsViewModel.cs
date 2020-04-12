using Injecter.UwpExample.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Injecter.UwpExample.ViewModels
{
    public interface IItemsViewModel
    {
        ReadOnlyObservableCollection<Item> Items { get; }

        Task InitializeAsync();
    }
}
