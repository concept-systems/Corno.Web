using Corno.Web.Areas.Euro.Dto.Label;
using System.Collections.Generic;
using System.Collections;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Euro.Services.Interfaces;

public interface IPartLabelService : ILabelService
{
    Task<IEnumerable> GetPendingItemsAsync(Plan plan);
    Task<List<Label>> CreateLabelsAsync(PartLabelCrudDto dto, Plan plan);
}

