using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Services.Packing.Interfaces;
using Kendo.Mvc.UI;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ICartonService : IBaseCartonService
{
    //DataSourceResult GetIndexDataSource(DataSourceRequest request);
    Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
    Task<CartonViewDto> ViewAsync(int? id);
}