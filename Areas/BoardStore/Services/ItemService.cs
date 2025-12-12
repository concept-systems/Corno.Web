using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.BoardStore.Services;

public class ItemService : MasterService<Item>, IItemService
{
    #region -- Constructors --
    public ItemService(IGenericRepository<Item> genericRepository) : base(genericRepository)
    {
        SetIncludes($"{nameof(Item.StackItems)}");
    }
    #endregion
}