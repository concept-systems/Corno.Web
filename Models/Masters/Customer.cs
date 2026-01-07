using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Masters;

public class Customer : PartyBase
{
    #region -- Constructors --
    public Customer()
    {
        CustomerUserDetails = new List<CustomerUserDetail>();
        CustomerProductDetails = new List<CustomerProductDetail>();
    }
    #endregion

    #region -- Properties --
    public int? CustomerTypeId { get; set; }
    public int? CustomerCategoryId { get; set; }
    public string SupplierCode { get; set; }

    public string GSTIN { get; set; }
    public double? CreditLimit { get; set; } = default;
    public double? CreditDays { get; set; } = default;

    public List<CustomerUserDetail> CustomerUserDetails { get; set; }
    public List<CustomerProductDetail> CustomerProductDetails { get; set; }
    #endregion

    //#region -- Methods --
    //public override bool UpdateDetails(CornoModel cornoModel)
    //{
    //    if (cornoModel is not Customer customer) return false;

    //    foreach (var productDetail in customer.CustomerProductDetails)
    //    {
    //        var existingDetail = CustomerProductDetails.FirstOrDefault(d =>
    //            d.Id == productDetail.Id);
    //        if (null == existingDetail)
    //        {
    //            CustomerProductDetails.Add(productDetail);
    //            continue;
    //        }

    //        existingDetail.Copy(productDetail);
    //    }

    //    // Remove items from list1 that are not in list2
    //    CustomerProductDetails.RemoveAll(x => customer.CustomerProductDetails.All(y => y.Id != x.Id));

    //    foreach (var userDetail in customer.CustomerUserDetails)
    //    {
    //        var existingDetail = CustomerUserDetails.FirstOrDefault(d =>
    //            d.Id == userDetail.Id);
    //        if (null == existingDetail)
    //        {
    //            CustomerUserDetails.Add(userDetail);
    //            continue;
    //        }

    //        existingDetail.Copy(userDetail);
    //    }
    //    // Remove items from list1 that are not in list2
    //    CustomerUserDetails.RemoveAll(x => customer.CustomerUserDetails.All(y => y.Id != x.Id));
        
    //    return true;
    //}
    //#endregion
}