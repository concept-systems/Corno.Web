using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Plan
{
    public class PlanAssemblyDetail : BaseModel
    {
        public int? PlanId { get; set; }
        public int? PackingTypeId { get; set; }
        public int? ItemId { get; set; }
        public int? SubAssemblySequence { get; set; }
        public double? OrderQuantity { get; set; }
        public double? SubAssembledQuantity { get; set; }

        [Required]
        public virtual Plan Plan { get; set; }
    }
}