-- Stored Procedure: GetPanelLabelReport
-- Purpose: Optimize PanelLabelRpt report performance by moving data processing to database
-- Parameters: 
--   @LotNo: Lot Number to filter by
-- Returns: Report data for panel labels

CREATE OR ALTER PROCEDURE [dbo].[GetPanelLabelReport]
    @LotNo NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.[System] AS OneLineItemCode,
        l.SoNo,
        l.WarehouseOrderNo,
        l.Position,
        l.SerialNo,
        pid.DrawingNo AS ItemCode,
        i.Name AS ItemName,
        pid.Reserved1 AS Color,
        l.Quantity,
        CASE WHEN l.Status = 'Packed' THEN l.Quantity ELSE 0 END AS ScanQuantity
    FROM dbo.Label AS l
    INNER JOIN dbo.[Plan] AS p
        ON l.WarehouseOrderNo = p.WarehouseOrderNo
    INNER JOIN dbo.PlanItemDetail AS pid
        ON p.Id = pid.PlanId
        AND l.Position = pid.Position
    INNER JOIN dbo.Item AS i
        ON l.ItemId = i.Id
    WHERE p.LotNo = @LotNo
    ORDER BY l.WarehouseOrderNo, l.Position, l.SerialNo;
END;
GO

