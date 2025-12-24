using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.DemoProject.Services.Interfaces;

public interface IProductLabelService : IBaseService<Label>
{
    Task<List<Label>> CreateLabelsAsync(LabelCrudDto dto, string userId);
    Task<BaseReport> CreateLabelReportAsync(List<Label> labels, Product product, int? labelFormatId, bool bDuplicate);
    Task UpdateDatabaseAsync(List<Label> labels);
}