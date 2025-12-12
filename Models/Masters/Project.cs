using System;
using System.ComponentModel;

namespace Corno.Web.Models.Masters;

public class Project : MasterModel
{
    public string MenuXml { get; set; }
    [DisplayName("Start Date")]
    public DateTime? StartDate { get; set; }
    [DisplayName("End Date")]
    public DateTime? EndDate { get; set; }
}