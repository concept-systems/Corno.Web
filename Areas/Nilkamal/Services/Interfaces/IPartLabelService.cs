using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Areas.Nilkamal.Dto.PartLabel;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Nilkamal.Services.Interfaces;

public interface IPartLabelService : ILabelService
{
    Task<BaseReport> GenerateBomLabels(int? productId, int packingTypeId, int itemId, int quantity, bool bSave = true);
    Task<IEnumerable> GetPendingItemsAsync(Plan plan);
    Task<List<Label>> CreateLabelsAsync(PartLabelCrudDto dto, Plan plan);
    Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
    Task<LabelViewDto> CreateViewDtoAsync(int? id);
    Task<LabelViewDto> GetLabelViewDto(Label label);
}