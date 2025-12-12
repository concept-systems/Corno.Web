using System.Threading.Tasks;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IWebSocketService : IService
{
    Task<bool> LedOperation(int ledNo, int value);
}