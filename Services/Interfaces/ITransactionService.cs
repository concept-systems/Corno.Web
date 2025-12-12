namespace Corno.Web.Services.Interfaces;

public interface ITransactionService<TEntity> : IBaseService<TEntity>
    where TEntity : class
{
}