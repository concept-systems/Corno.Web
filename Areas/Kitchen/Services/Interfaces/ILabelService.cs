using Corno.Web.Areas.Kitchen.Dto.Kitting;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Sorting;
using Corno.Web.Globals.Enums;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ILabelService : IBaseService<Label>
{
    Task PerformKittingAsync(KittingCrudDto dto, string userId);
    Task PerformSortingAsync(SortingCrudDto dto, string userId);

    // OLD METHOD REMOVED: ImportAsync with progress service
    // This functionality is now in LabelBusinessImportService

    Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request);
    Task<LabelViewDto> CreateViewDtoAsync(int? id, LabelType? labelType = null);
    Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate, LabelType? labelType = null);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
}
