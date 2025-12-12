using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.BoardStore.Services;

public class MovementService : BaseService<Movement>, IMovementService
{
    #region -- Constructors --
    public MovementService(IGenericRepository<Movement> genericRepository) : base(genericRepository)
    {
        
    }
    #endregion
}