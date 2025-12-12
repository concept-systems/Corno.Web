using System;

namespace Corno.Web.Areas.DemoProject.ViewModels;

public class LabelDisplayDetailViewModel

{
    #region -- Properties --
    public DateTime? ScanDate { get; set; }

    public string Status { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    #endregion
}