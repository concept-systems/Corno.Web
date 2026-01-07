using System;

namespace Corno.Web.Areas.Nilkamal.Dto.PacketLabel;

public class PacketViewDetailDto 
{
    #region -- Properties --
    public int Id { get; set; }
    public DateTime? ScanDate { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public string UserName { get; set; }

    #endregion
}