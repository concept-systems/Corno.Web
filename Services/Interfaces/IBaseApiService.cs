using Corno.Services.Base.Interfaces;

namespace Corno.Concept.Portal.Services.Interfaces;

public interface IBaseApiService : IService
{
    object Get(string action, object value, ApiName api);
    object Post(string action, object value, ApiName api);
}