using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Masters
{
    public class ProductAssemblyDetail : BaseModel
    {
        public int? ProductId { get; set; }
        public int? PackingTypeId { get; set; }
        public int? ItemId { get; set; }
        public int? SubAssemblySequence { get; set; }
        public double? Quantity { get; set; }

        [Required]
        public virtual Product Product { get; set; }

        #region -- Public Methods --
        public void Copy(ProductAssemblyDetail other)
        {
           PackingTypeId = other.PackingTypeId;
           Quantity = other.Quantity;
           ProductId = other.ProductId;
           SubAssemblySequence = other.SubAssemblySequence;
           Product = other.Product;
        }
        #endregion
    }
}