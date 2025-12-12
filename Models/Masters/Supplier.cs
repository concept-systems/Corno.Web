using System.Collections.Generic;

namespace Corno.Web.Models.Masters;

public class Supplier : PartyBase
{
    public Supplier()
    {
        SupplierItemDetails = new List<SupplierItemDetail>();
    }

    public string GSTIN { get; set; }
    public double CreditLimit { get; set; }
    public double CreditDays { get; set; }

    public ICollection<SupplierItemDetail> SupplierItemDetails { get; set; }
}