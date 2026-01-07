using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Globals.Enums;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Euro.Services.Interfaces;

public interface ILabelService : IBaseService<Label>
{
    Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
    Task<LabelViewDto> CreateViewDtoAsync(int? id, LabelType? labelType = null);
    Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate, LabelType? labelType = null);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
}
