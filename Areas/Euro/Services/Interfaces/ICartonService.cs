using Corno.Web.Areas.Euro.Dto.Carton;
using Corno.Web.Services.Packing.Interfaces;
using Kendo.Mvc.UI;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Euro.Services.Interfaces;

public interface ICartonService : IBaseCartonService
{
    Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
    Task<CartonViewDto> ViewAsync(int? id);
}

