using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Areas.Euro.Dto.Label;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Euro.Services.Interfaces;

public interface IEuroLabelService : IBaseService<Label>, IFileImportService<LabelImportDto>
{
    DataSourceResult GetIndexDataSource(DataSourceRequest request);
    Task<List<object>> ImportAsync(Stream fileStream, string fileName, IWebProgressService progressService,
        string userId, string sessionId, ImportSessionService sessionService);
}
