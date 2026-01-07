using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IStoreShirwalLabelService : ILabelService
{
    Task<List<Label>> CreateLabelsAsync(StoreShirwalCrudDto dto, Plan plan);
    //Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate);
    Task UpdateDatabaseAsync(List<Label> labels, Plan plan);
    }