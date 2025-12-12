using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Models.Base;

public interface IBaseModel : ICornoModel
{
    int? CompanyId { get; set; }
    int? SerialNo { get; set; }
    string Code { get; set; }
    [Key]
    int Id { get; set; }
    string Status { get; set; }
    string CreatedBy { get; set; }
    DateTime? CreatedDate { get; set; }
    string ModifiedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
    string DeletedBy { get; set; }
    DateTime? DeletedDate { get; set; }

    //void Reset();
}