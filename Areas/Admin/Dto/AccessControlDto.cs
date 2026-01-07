using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Dto;

public class AccessControlDto
{
    public string Id { get; set; }
    
    [Required]
    public int PermissionTypeId { get; set; }
    public string PermissionTypeName { get; set; }
    
    public int? MenuId { get; set; }
    public string MenuName { get; set; }
    
    [MaxLength(100)]
    public string ControllerName { get; set; }
    
    [MaxLength(100)]
    public string ActionName { get; set; }
    
    [MaxLength(100)]
    public string Area { get; set; }
    
    [MaxLength(200)]
    public string ControlId { get; set; }
    
    [MaxLength(200)]
    public string ControlName { get; set; }
    
    public string UserId { get; set; }
    public string UserName { get; set; }
    
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    
    public bool IsAllowed { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class AccessControlIndexDto
{
    public string Id { get; set; }
    public string PermissionTypeName { get; set; }
    public string MenuName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string ControlId { get; set; }
    public string UserName { get; set; }
    public string RoleName { get; set; }
    public bool IsAllowed { get; set; }
}

public class PermissionAssignmentDto
{
    public int MenuId { get; set; }
    public string MenuName { get; set; }
    public string MenuPath { get; set; }
    public bool HasMenuAccess { get; set; }
    
    public Dictionary<string, bool> PagePermissions { get; set; } // Action -> IsAllowed
    public Dictionary<string, bool> ControlPermissions { get; set; } // ControlId -> IsAllowed
}

