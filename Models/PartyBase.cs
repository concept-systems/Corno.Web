using Ganss.Excel;

namespace Corno.Web.Models;

public class PartyBase : MasterModel
{
    [Ignore]
    public byte[] Photo { get; set; }
    public string Address { get; set; }
    public int? CityId { get; set; }
    public int? StateId { get; set; }
    public int? CountryId { get; set; }
    public string Pin { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }

    public string ContactPerson { get; set; }
}