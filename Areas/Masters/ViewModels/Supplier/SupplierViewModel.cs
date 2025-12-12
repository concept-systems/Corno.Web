using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.ViewModels.Supplier;

public class SupplierViewModel : MasterDto
{
    #region -- Constructors --

    public SupplierViewModel()
    {
        SupplierItemDetails = new List<SupplierItemDetailViewModel>();
    }
    #endregion

    #region -- Properties --
    public int? CityId { get; set; }
    public int? StateId { get; set; }
    public string Address { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string GSTIN { get; set; }
    public double CreditLimit { get; set; }
    public double CreditDays { get; set; }
    public ICollection<SupplierItemDetailViewModel> SupplierItemDetails { get; set; }
    #endregion

}