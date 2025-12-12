using Corno.Web.Services.Packing.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ICartonService : IBaseCartonService
{
    DataSourceResult GetIndexDataSource(DataSourceRequest request);
    System.Threading.Tasks.Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
}