using System.Collections;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Packing.Interfaces;
using Kendo.Mvc.UI;
using System.Threading.Tasks;
using Corno.Web.Areas.Nilkamal.Dto.PacketLabel;
using Corno.Web.Models.Packing;
using System.Collections.Generic;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Nilkamal.Services.Interfaces;

public interface ICartonService : IBaseCartonService
{
    Task<BaseReport> CreateLabelsAsync(PacketLabelCrudDto dto, Plan plan, bool bSave = false);
    Task<BaseReport> CreateLabelReportAsync(List<Carton> cartons, bool bDuplicate);
    DataSourceResult GetIndexDataSource(DataSourceRequest request);
    System.Threading.Tasks.Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
    Task<PacketViewDto> CreateViewDtoAsync(int? id);
    Task<PacketViewDto> GetLabelViewDto(Carton carton);
}