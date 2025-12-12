using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing
{
    [Serializable]
    public class SubAssembly : BaseModel
    {
        #region -- Constructors --
        public SubAssembly()
        {
            SubAssemblyDetails = new List<SubAssemblyDetail>();
        }
        #endregion

        #region -- Properties --
        public DateTime? AssemblyDate { get; set; }
        public int? ProductId { get; set; }
        public int? PackingTypeId { get; set; }
        public int? LineId { get; set; }
        public string PalletNo { get; set; }
        public string LoadNo { get; set; }
        public string PoNo { get; set; }
        public string SoNo { get; set; }
        public string BatchNo { get; set; }
        public string ProductionOrderNo { get; set; }

        public List<SubAssemblyDetail> SubAssemblyDetails { get; set; }
        #endregion
    }
}