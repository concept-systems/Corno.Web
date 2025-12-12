using Corno.Web.Dtos;

namespace Corno.Web.Areas.Admin.Dto.SqlQuery;

public class SqlQuoryIndexDto : MasterDto
{
    public string CurrentStock { get; set; }
    public double? Cost { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
}