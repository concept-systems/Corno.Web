namespace Corno.Web.Areas.Admin.Dto;

public class LogDto
{
    public string Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
}