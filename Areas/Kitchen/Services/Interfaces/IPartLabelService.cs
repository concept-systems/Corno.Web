using Corno.Web.Areas.Kitchen.Dto.Label;
using System.Collections.Generic;
using System.Collections;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IPartLabelService : ILabelService
{
    Task<IEnumerable> GetPendingItemsAsync(Plan plan);
    Task<List<Label>> CreateLabelsAsync(PartLabelCrudDto dto, Plan plan);
}