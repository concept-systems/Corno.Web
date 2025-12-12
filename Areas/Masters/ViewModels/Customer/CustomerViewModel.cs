using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.ViewModels.Customer;

public class CustomerViewModel : MasterDto
{

    #region -- Constructors --

    public CustomerViewModel()
    {
        CustomerUserDetails = new List<CustomerUserDetailViewModel>();
        CustomerProductDetails = new List<CustomerProductDetailViewModel>();
    }
    #endregion

    #region -- Properties --
    public int? CustomerTypeId { get; set; }
    public int? CustomerCategoryId { get; set; }
    public int? CityId { get; set; }
    public int? StateId { get; set; }
    public string SupplierCode { get; set; }
    public string ContactPerson { get; set; }
    public string Mobile { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Address { get; set; }
    public string GSTIN { get; set; }
    public double CreditLimit { get; set; }
    public double CreditDays { get; set; }
    public List<CustomerUserDetailViewModel> CustomerUserDetails { get; set; }
    public List<CustomerProductDetailViewModel> CustomerProductDetails { get; set; }
    //public override bool UpdateDetails(CornoModel cornoModel);

    #endregion
}