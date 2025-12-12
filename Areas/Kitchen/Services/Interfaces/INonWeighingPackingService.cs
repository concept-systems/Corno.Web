using Corno.Web.Areas.Kitchen.Dto.Carton;
using System.Threading.Tasks;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface INonWeighingPackingService : ICartonService
{
    Task ValidateBarcodeAsync(CartonCrudDto dto);
    Task<ReportBook> Preview(CartonCrudDto dto);
    Task<ReportBook> Print(CartonCrudDto dto, string userId);
    Task<CartonViewDto> View(int? id);
}