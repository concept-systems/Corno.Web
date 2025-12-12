namespace Corno.Web.Models.Packing;

public class NotMapped
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public byte[] ItemPhoto { get; set; }
    public string DrawingNo { get; set; }

    public string AglNo { get; set; }

    public int? PackingTypeCount { get; set; }
}