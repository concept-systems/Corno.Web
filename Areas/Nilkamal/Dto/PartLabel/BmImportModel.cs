using System;
using Ganss.Excel;

namespace Corno.Web.Areas.Nilkamal.Dto.PartLabel;

public class BmImportDto
{
    [Column("CompanyCode")]
    [FormulaResult]
    public string CompanyCode { get; set; }
    [Column("SalesOrder")]
    [FormulaResult]
    public string SoNo { get; set; }
    [Column("Pos")]
    [FormulaResult]
    public string SoPosition { get; set; }
    [Column("ProductionOrder")]
    [FormulaResult]
    public string ProductionOrderNo { get; set; }
    [Column("RPLOrder")]
    [FormulaResult]
    public string WarehouseOrderNo { get; set; }
    [Column("RPLPos")]
    [FormulaResult]
    public string WarehousePosition { get; set; }

    [Column("DueDate")]
    [FormulaResult]
    public DateTime? DueDate { get; set; }

    [Column("ParentCode")]
    [FormulaResult]
    public string ParentItemCode { get; set; }
    [Column("ParentDesc")]
    [FormulaResult]
    public string ParentItemName { get; set; }
    [Column("CarcassQty")]
    [FormulaResult]
    public string CarcassCode { get; set; }

    [Column("AssemblyCode")]
    [FormulaResult]
    public string SubAssemblyCode { get; set; }
    [Column("AssemblyDesc")]
    [FormulaResult]
    public string SubAssemblyItemName { get; set; }
    [Column("AssemblyQty")]
    [FormulaResult]
    public int? SubAssemblyQuantity { get; set; }

    [Column("ChildCode")]
    [FormulaResult]
    public string ItemCode { get; set; }
    [Column("ChildDesc")]
    [FormulaResult]
    public string ItemName { get; set; }
    [Column("ChildPos")]
    [FormulaResult]
    public string Position { get; set; }
    [Column("ChildQty")]
    [FormulaResult]
    public int? ChildQuantity { get; set; }
    [Column("Manufacturing/ BO")]
    [FormulaResult]
    public string ItemType { get; set; }
    [Column("ChildDrawing")]
    [FormulaResult]
    public string DrawingNo { get; set; }

    [Column("FinishedGoodProductClass")]
    [FormulaResult]
    public string FamilyCode { get; set; }

    [Column("FinishedGoodItem")]
    [FormulaResult]
    public string FinishedGoodItem { get; set; }

    [Column("SalesOrderBookingWarehouse")]
    [FormulaResult]
    public string WarehouseCode { get; set; }
    [Column("Branch")]
    [FormulaResult]
    public string WarehouseName { get; set; }
    [Column("OldItemCode")]
    [FormulaResult]
    public string BaanItemCode { get; set; }
    [Column("OldItemDesc")]
    [FormulaResult]
    public string BaanItemName { get; set; }
    [Column("OneLineItemCode")]
    [FormulaResult]
    public string OneLineItemCode { get; set; }
    [Column("Color")]
    public string Color { get; set; }

    public string Remark { get; set; }
    public string Status { get; set; }
}