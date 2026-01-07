using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IStiffenerLabelService : ILabelService
{
    Task<IEnumerable> GetPendingItemsAsync(Plan plan);
    Task<List<Label>> CreateLabelsAsync(StiffenerLabelCrudDto dto, List<Plan> plan);
    //Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
    }