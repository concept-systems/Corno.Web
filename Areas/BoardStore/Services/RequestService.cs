using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.BoardStore.Services;

public class RequestService : BaseService<Request>, IRequestService
{
    #region -- Constructors --
    public RequestService(IGenericRepository<Request> genericRepository) : base(genericRepository)
    {
        SetIncludes($"{nameof(Request.Stacks)}");
    }
    #endregion
}