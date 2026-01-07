-- Stored Procedure: GetShortageAllReport
-- Purpose: Optimize ShortageAllRpt report performance by moving data processing to database
-- Parameters: 
--   @FromDate: Start date for filtering plans
--   @ToDate: End date for filtering plans
--   @Families: Comma-separated list of Family names
--   @ReportType: 0=Kitting, 1=Sorting, 2=Third Label, 3=Packing
-- Returns: Report data showing shortages

CREATE OR ALTER PROCEDURE [dbo].[GetShortageAllReport]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @Families NVARCHAR(MAX),
    @ReportType INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Families AS (
        -- Split comma-separated Families
        SELECT LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) AS Family
        FROM (SELECT CAST('<x>' + REPLACE(ISNULL(@Families, ''), ',', '</x><x>') + '</x>' AS XML) AS x) t
        CROSS APPLY t.x.nodes('/x') AS m(n)
        WHERE LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) <> ''
    ),
    PlanItemQuantities AS (
        -- Get scan quantities based on report type
        SELECT 
            pid.Id AS PlanItemDetailId,
            pid.PlanId,
            pid.[Group] AS Family,
            pid.Position,
            pid.Description,
            pid.Reserved1 AS Color,
            pid.OrderQuantity,
            CASE @ReportType
                WHEN 0 THEN pid.BendQuantity
                WHEN 1 THEN pid.SortQuantity
                WHEN 2 THEN pid.SubAssemblyQuantity
                WHEN 3 THEN pid.PackQuantity
                ELSE 0
            END AS ScanQuantity
        FROM dbo.PlanItemDetail AS pid
        INNER JOIN dbo.[Plan] AS p
            ON pid.PlanId = p.Id
        WHERE CAST(p.DueDate AS DATE) >= CAST(@FromDate AS DATE)
            AND CAST(p.DueDate AS DATE) <= CAST(@ToDate AS DATE)
    )
    SELECT 
        p.DueDate,
        p.[System] AS OneLineItemCode,
        p.WarehouseOrderNo,
        piq.Family,
        piq.Position,
        piq.Description,
        piq.Color,
        piq.OrderQuantity,
        ISNULL(piq.ScanQuantity, 0) AS ScanQuantity
    FROM PlanItemQuantities AS piq
    INNER JOIN Families AS f
        ON piq.Family = f.Family
    INNER JOIN dbo.[Plan] AS p
        ON piq.PlanId = p.Id
    WHERE piq.OrderQuantity > ISNULL(piq.ScanQuantity, 0)
    ORDER BY p.DueDate, p.WarehouseOrderNo, piq.Family, piq.Position;
END;
GO

