using Injecter.UwpExample.Model;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Injecter.UwpExample.Services
{
    public sealed class ItemService : IItemService
    {
        public Task<IImmutableSet<Item>> GetAllItemsAsync() => Task.FromResult((IImmutableSet<Item>)ImmutableHashSet.Create(
            new Item { Name = "Item 1" },
            new Item { Name = "Item 2" },
            new Item { Name = "Item 3" },
            new Item { Name = "Item 4" },
            new Item { Name = "Item 5" }));
    }
}
