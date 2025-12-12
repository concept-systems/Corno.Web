using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Models;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Masters.Interfaces;

public interface IBaseItemService : IMasterService<Item>
{
    #region -- Methods --

    Task<Item> GetItemAsync(int itemId);
    //Task<Item> GetByCodeAsync(string code);
    //Task<Item> GetOrCreateAsync(string code, string name);
    Task<double?> GetStockQuantityAsync(int itemId);
    Task ReceiveStockQuantityAsync(int itemId, int quantity);
    Task IssueStockQuantityAsync(int itemId, double quantity);
    Task<string> GetFlavorNameAsync(int itemId);
    //Task<IEnumerable<MasterDto>> GetPacketsAsync(int itemId);

    Task<bool> IsQcApplicableAsync(int itemId);

    Task UpdateWeightAsync(Item item, double weight, double tolerance);

    #region -- Process Methods --

    Task<Process> GetNextProcessAsync(int itemId, string processCode);
    Task<Process> GetNextProcessWithNullReturnAsync(int itemId, string processCode);
    Task<string> GetNextProcessStatusAsync(int itemId, string processCode);
    Task<List<string>> GetPreviousProcessStatusAsync(int itemId, string processCode);
    #endregion

    #endregion
}