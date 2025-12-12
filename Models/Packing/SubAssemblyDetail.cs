using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing
{
    [Serializable]
    public class SubAssemblyDetail : BaseModel
    {
        #region -- Properties --
        public int? SubAssemblyId { get; set; }
        public int? ProductId { get; set; }
        public int? ItemId { get; set; }

        public string ItemCode { get; set; }

        public int? PackingTypeId { get; set; }

        public string Barcode { get; set; }
        public double? Quantity { get; set; }

        [Required]
        public virtual SubAssembly SubAssembly { get; set; }
        #endregion
    }
}