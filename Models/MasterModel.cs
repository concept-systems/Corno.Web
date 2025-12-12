using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Models;

public class MasterModel : BaseModel
{
    [Required]
    [Column("Name")]
    public string Name { get; set; }
    [Column("Description")]
    public string Description { get; set; }
}