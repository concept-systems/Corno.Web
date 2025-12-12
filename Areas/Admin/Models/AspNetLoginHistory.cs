using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Models;

public class AspNetLoginHistory
{
    [Key]
    public string Id { get; set; }
    public string AspNetUserId { get; set; }
    public string UserName { get; set; }
    public DateTime? LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public string IpAddress { get; set; }
    public string MachineName { get; set; }
    public string HostName { get; set; }
    [Required]
    public LoginResult LoginResult { get; set; }

    public virtual AspNetUser AspNetUser { get; set; }
}