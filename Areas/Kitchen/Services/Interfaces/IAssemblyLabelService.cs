using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IAssemblyLabelService : ILabelService
{
    Task<List<Label>> CreateLabelsAsync(SubAssemblyCrudDto dto, Plan plan, Label label1, bool bUpdateDatabase);
    Task<BaseReport> CreateLabelReportAsync(IEnumerable<Label> labels, bool bDuplicate);
    Task UpdateDatabaseAsync(Label assemblyLabel, List<Label> labels, Plan plan);
    Task<LabelViewDto> CreateViewDtoAsync(int? id);
}