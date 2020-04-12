using Injecter.UwpExample.Model;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Injecter.UwpExample.Services
{
    public interface IItemService
    {
        Task<IImmutableSet<Item>> GetAllItemsAsync();
    }
}
