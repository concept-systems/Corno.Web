using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Dto;

public class AuditLogDto
{
    public long Id { get; set; }
    
    public string UserId { get; set; }
    public string UserName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Action { get; set; }
    
    [MaxLength(100)]
    public string EntityType { get; set; }
    
    [MaxLength(128)]
    public string EntityId { get; set; }
    
    [MaxLength(200)]
    public string EntityName { get; set; }
    
    public string Details { get; set; }
    
    [MaxLength(50)]
    public string IpAddress { get; set; }
    
    [MaxLength(500)]
    public string UserAgent { get; set; }
    
    public DateTime Timestamp { get; set; }
}

public class AuditLogIndexDto
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public string EntityName { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AuditLogFilterDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

