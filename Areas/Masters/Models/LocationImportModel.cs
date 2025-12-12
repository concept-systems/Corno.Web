using Ganss.Excel;

namespace Corno.Web.Areas.Masters.Models;

public class LocationImportModel
{
    [Column("Code")]
    public string Code { get; set; }
    [Column("Name")]
    public string Name { get; set; }

    public string Status { get; set; }
    public string Remark { get; set; }
    
}