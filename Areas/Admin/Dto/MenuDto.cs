using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Dto;

public class MenuDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Menu Name is required")]
    [MaxLength(200)]
    public string MenuName { get; set; }
    
    [Required(ErrorMessage = "Display Name is required")]
    [MaxLength(200)]
    public string DisplayName { get; set; }
    
    [MaxLength(500)]
    public string MenuPath { get; set; }
    
    public int? ParentMenuId { get; set; }
    public string ParentMenuName { get; set; }
    
    [MaxLength(100)]
    public string ControllerName { get; set; }
    
    [MaxLength(100)]
    public string ActionName { get; set; }
    
    [MaxLength(100)]
    public string Area { get; set; }
    
    [MaxLength(100)]
    public string IconClass { get; set; }
    
    public string RouteValues { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool IsVisible { get; set; }
    
    public bool IsActive { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    
    // For tree structure
    public List<MenuDto> ChildMenus { get; set; }
    public bool HasChildren { get; set; }
    public int Level { get; set; }
}

public class MenuIndexDto
{
    public int Id { get; set; }
    public string MenuName { get; set; }
    public string DisplayName { get; set; }
    public string MenuPath { get; set; }
    public string ParentMenuName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string Area { get; set; }
    public string IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; }
    public bool IsActive { get; set; }
}

