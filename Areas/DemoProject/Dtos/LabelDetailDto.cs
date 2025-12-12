using System;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class LabelDetailDto
{
    #region -- Properties --
    public DateTime? ScanDate { get; set; }

    public string Status { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    #endregion
}