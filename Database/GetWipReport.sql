-- Stored Procedure: GetWipReport
-- Purpose: Optimize WipRpt report performance by moving data processing to database
-- Parameters: 
--   @FromDate: Start date for filtering plans
--   @ToDate: End date for filtering plans
-- Returns: Report data matching WipReportDto structure

CREATE OR ALTER PROCEDURE [dbo].[GetWipReport]
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH CartonRackingAggregated AS (
        -- Aggregate CartonRackingDetails by status and WarehouseOrderNo
        SELECT 
            c.WarehouseOrderNo,
            c.Id AS CartonId,
            c.PackingDate,
            MAX(CASE WHEN crd.Status = 'RackIn' THEN crd.ScanDate END) AS RackInDate,
            MAX(CASE WHEN crd.Status = 'RackOut' THEN crd.ScanDate END) AS RackOutDate,
            MAX(CASE WHEN crd.Status = 'Dispatch' THEN crd.ScanDate END) AS DispatchDate
        FROM dbo.Carton AS c
        LEFT JOIN dbo.CartonRackingDetail AS crd
            ON c.Id = crd.CartonId
            AND crd.Status IN ('RackIn', 'RackOut', 'Dispatch')
        GROUP BY c.WarehouseOrderNo, c.Id, c.PackingDate
    ),
    PlanCartonSummary AS (
        -- Summarize carton data per plan
        SELECT 
            p.WarehouseOrderNo,
            COUNT(DISTINCT c.CartonId) AS CartonCount,
            MAX(c.RackInDate) AS RackInDate,
            MAX(c.RackOutDate) AS RackOutDate,
            MAX(c.DispatchDate) AS DispatchDate,
            MAX(c.PackingDate) AS LastDate
        FROM dbo.[Plan] AS p
        LEFT JOIN CartonRackingAggregated AS c
            ON p.WarehouseOrderNo = c.WarehouseOrderNo
        WHERE CAST(p.DueDate AS DATE) >= CAST(@FromDate AS DATE)
            AND CAST(p.DueDate AS DATE) <= CAST(@ToDate AS DATE)
        GROUP BY p.WarehouseOrderNo
    )
    SELECT 
        p.DueDate,
        p.[System] AS OneLineItemCode,
        p.WarehouseOrderNo,
        m.Name AS BranchName,
        ISNULL(pcs.CartonCount, 0) AS CartonCount,
        pcs.RackInDate,
        pcs.RackOutDate,
        pcs.DispatchDate,
        '' AS LoadNo,
        '' AS PrintStatus,
        '' AS RackInStatus,
        CASE WHEN pcs.LastDate IS NOT NULL 
            THEN CONVERT(VARCHAR(10), pcs.LastDate, 103) 
            ELSE '' END AS LastDate,
        (SELECT TOP 1 pid.[Group] 
         FROM dbo.PlanItemDetail AS pid 
         WHERE pid.PlanId = p.Id 
         ORDER BY pid.[Group]) AS Families
    FROM dbo.[Plan] AS p
    INNER JOIN PlanCartonSummary AS pcs
        ON p.WarehouseOrderNo = pcs.WarehouseOrderNo
    LEFT JOIN dbo.MiscMaster AS m
        ON p.WarehouseId = m.Id
        AND m.MiscType = 'Warehouse'
    WHERE CAST(p.DueDate AS DATE) >= CAST(@FromDate AS DATE)
        AND CAST(p.DueDate AS DATE) <= CAST(@ToDate AS DATE)
    ORDER BY p.DueDate, p.WarehouseOrderNo;
END;
GO

