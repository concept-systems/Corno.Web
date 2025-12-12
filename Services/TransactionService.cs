using Corno.Web.Models.Base;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class TransactionService<TEntity> : BaseService<TEntity>, ITransactionService<TEntity>
    where TEntity : TransactionModel, new()
{
    #region -- Constructors --
    public TransactionService(IGenericRepository<TEntity> genericRepository) : base(genericRepository)
    {
    }
    #endregion
}