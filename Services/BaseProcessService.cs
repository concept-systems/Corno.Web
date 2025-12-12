using Corno.Web.Models;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class BaseProcessService : MasterService<Process>, IBaseProcessService
{
    #region -- Constructors --
    public BaseProcessService(IGenericRepository<Process> repository)
        : base(repository)
    {
    }
    #endregion

    #region -- Public Methods --

    public async System.Threading.Tasks.Task<Process> GetBySymbolAsync(string symbol)
    {
        return await FirstOrDefaultAsync(p => p.ShortName == symbol, p => p).ConfigureAwait(false);
    }
    #endregion
}