-- Stored Procedure: GetScanningUserWiseReport
-- Purpose: Optimize ScanningUserWiseRpt report performance by moving data processing to database
-- Parameters: 
--   @FromDate: Start date for filtering cartons
--   @ToDate: End date for filtering cartons
--   @UserId: User ID to filter by
--   @UserName: User name for display
-- Returns: Report data showing scanning details by user

CREATE OR ALTER PROCEDURE [dbo].[GetScanningUserWiseReport]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @UserId NVARCHAR(255),
    @UserName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        @UserName AS UserName,
        c.PackingDate,
        pid.[Group] AS Family,
        c.WarehouseOrderNo,
        cd.WarehousePosition,
        pid.ItemCode,
        pid.Description AS ItemName,
        CAST(c.CartonNo AS NVARCHAR(50)) AS CartonNo
    FROM dbo.Carton AS c
    INNER JOIN dbo.CartonDetail AS cd
        ON c.Id = cd.CartonId
    INNER JOIN dbo.[Plan] AS p
        ON c.WarehouseOrderNo = p.WarehouseOrderNo
    INNER JOIN dbo.PlanItemDetail AS pid
        ON p.Id = pid.PlanId
        AND cd.Position = pid.Position
    WHERE CAST(c.PackingDate AS DATE) >= CAST(@FromDate AS DATE)
        AND CAST(c.PackingDate AS DATE) <= CAST(@ToDate AS DATE)
        AND c.ModifiedBy = @UserId
    ORDER BY c.PackingDate, c.WarehouseOrderNo, cd.WarehousePosition;
END;
GO

