-- Stored Procedure: GetScanningDateWiseReport
-- Purpose: Optimize ScanningDateWiseRpt report performance by moving data processing to database
-- Parameters: 
--   @WarehouseOrderNo: Warehouse Order Number to filter by
-- Returns: Report data showing scanning details by date

CREATE OR ALTER PROCEDURE [dbo].[GetScanningDateWiseReport]
    @WarehouseOrderNo NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.PackingDate,
        c.WarehouseOrderNo,
        cd.WarehousePosition,
        m.Code AS WarehouseCode,
        c.CartonNo,
        c.CartonBarcode,
        pid.ItemCode,
        pid.Description AS ItemName,
        pid.OrderQuantity,
        cd.Quantity AS ScanQuantity
    FROM dbo.Carton AS c
    INNER JOIN dbo.CartonDetail AS cd
        ON c.Id = cd.CartonId
    INNER JOIN dbo.[Plan] AS p
        ON c.WarehouseOrderNo = p.WarehouseOrderNo
    INNER JOIN dbo.PlanItemDetail AS pid
        ON p.Id = pid.PlanId
        AND cd.Position = pid.Position
    LEFT JOIN dbo.MiscMaster AS m
        ON p.WarehouseId = m.Id
        AND m.MiscType = 'Warehouse'
    WHERE c.WarehouseOrderNo = @WarehouseOrderNo
    ORDER BY c.PackingDate, c.CartonNo, cd.WarehousePosition;
END;
GO

