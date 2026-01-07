using Corno.Web.Areas.Kitchen.Dto.Carcass;
using System.Threading.Tasks;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ICarcassPackingService : ICartonService
{
    Task ValidateBarcodeAsync(CarcassCrudDto dto);
    Task<ReportBook> Preview(CarcassCrudDto dto);
    Task<ReportBook> Print(CarcassCrudDto dto , string userId);
}