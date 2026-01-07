using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.StoreLabel;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IStoreKhalapurLabelService : ILabelService
{
    Task<List<StoreLabelCrudDetailDto>> GetPendingItemsAsync(Plan plan, string family);
    Task<List<Label>> CreateLabelsAsync(StoreLabelCrudDto dto, Plan plan);
}