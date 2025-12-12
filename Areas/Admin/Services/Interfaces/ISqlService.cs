using System.Data;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface ISqlService : IService
{
    DataTable ExecuteQuery(string sqlQuery);
}