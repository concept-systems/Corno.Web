using Corno.Web.Areas.Kitchen.Dto.Kitting;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Sorting;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface ILabelService : IBaseService<Label>
{
    Task PerformKitting(KittingCrudDto dto, string userId);
    Task PerformSorting(SortingCrudDto dto, string userId);

    Task<List<SalvaginiExcelDto>> Import(Stream fileStream, string filePath,
        string newStatus, IBaseProgressService progressService, string userId);

    Task<DataSourceResult> GetIndexDataSource(DataSourceRequest request);
}