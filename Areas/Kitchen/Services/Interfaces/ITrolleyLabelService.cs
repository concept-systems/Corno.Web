using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Areas.Kitchen.Dto.TrolleyLabel;
using Corno.Web.Reports;
using Corno.Web.Areas.Kitchen.Dto.Label;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ITrolleyLabelService : ILabelService
{
    Task<List<Label>> CreateLabelsAsync(TrolleyLabelCrudDto dto, Plan plan);
    Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
    Task<LabelViewDto> CreateViewDtoAsync(int? id);
}
