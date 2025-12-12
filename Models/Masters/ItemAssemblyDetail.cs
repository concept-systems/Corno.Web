using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Masters
{
    public class ItemAssemblyDetail : BaseModel
    {
        public int? ItemId { get; set; }
        public int? AssemblySequence { get; set; }
        public double? Quantity { get; set; }

        [Required]
        public virtual Item Item { get; set; }

        #region -- Public Methods --
        public void Copy(ItemAssemblyDetail other)
        {
            AssemblySequence = other.AssemblySequence;
            Quantity = other.Quantity;
            Item = other.Item;
        }
        #endregion
    }
}