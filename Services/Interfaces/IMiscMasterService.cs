using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Dtos;
using Corno.Web.Models.Masters;

namespace Corno.Web.Services.Interfaces;

public interface IMiscMasterService : IMasterService<MiscMaster>
{
    Task<MiscMaster> GetOrCreateAsync(string code, string name, string miscMasterType, bool bSave = true);
    Task<IEnumerable<MasterDto>> GetViewModelListAsync(string miscType);
    Task<MiscMaster> GetListAsync(string code, string miscType);
    Task<MiscMaster> GetByCodeAsync(string code, string miscType);
    Task<MiscMaster> GetByNameAsync(string name, string miscType);
    Task<bool> ExistsAsync(string code, string miscType);
}