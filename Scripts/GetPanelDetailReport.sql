-- Stored Procedure: GetPanelDetailReport
-- Purpose: Optimize PanelDetailRpt report performance by moving data processing to database
-- Parameters: 
--   @LotNos: Comma-separated list of Lot Numbers
--   @Families: Comma-separated list of Family names
-- Returns: Report data matching PanelDetailReportDto structure
-- 
-- Note: This procedure uses STRING_SPLIT which is available in SQL Server 2016+
-- For SQL Server 2014 or earlier, replace STRING_SPLIT with a custom split function

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPanelDetailReport]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[GetPanelDetailReport]
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPanelDetailReport]
    @LotNos NVARCHAR(MAX),
    @Families NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH LotNos AS (
        -- XML-based splitter for compatibility <= 120
        SELECT LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) AS LotNo
        FROM (SELECT CAST('<x>' + REPLACE(ISNULL(@LotNos, ''), ',', '</x><x>') + '</x>' AS XML) AS x) t
        CROSS APPLY t.x.nodes('/x') AS m(n)
        WHERE LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) <> ''
    ),
    Families AS (
        SELECT LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) AS Family
        FROM (SELECT CAST('<x>' + REPLACE(ISNULL(@Families, ''), ',', '</x><x>') + '</x>' AS XML) AS x) t
        CROSS APPLY t.x.nodes('/x') AS m(n)
        WHERE LTRIM(RTRIM(m.n.value('.','nvarchar(255)'))) <> ''
    ),
    LabelStatusFlags AS (
        -- For each label, identify whether it has any Bent/Sorted/Packed detail rows and capture the latest scan dates
        SELECT
            l.WarehouseOrderNo,
            l.Position,
            l.Quantity,
            l.Id AS LabelId,
            MAX(CASE WHEN ld.Status = 'Bent'   THEN 1 ELSE 0 END)    AS HasBent,
            MAX(CASE WHEN ld.Status = 'Bent'   THEN ld.ScanDate END) AS BentDate,
            MAX(CASE WHEN ld.Status = 'Sorted' THEN 1 ELSE 0 END)    AS HasSorted,
            MAX(CASE WHEN ld.Status = 'Sorted' THEN ld.ScanDate END) AS SortedDate,
            MAX(CASE WHEN ld.Status = 'Packed' THEN 1 ELSE 0 END)    AS HasPacked,
            MAX(CASE WHEN ld.Status = 'Packed' THEN ld.ScanDate END) AS PackedDate
        FROM dbo.Label AS l
        LEFT JOIN dbo.LabelDetail AS ld
            ON l.Id = ld.LabelId AND ld.Status IN ('Bent','Sorted','Packed')
        GROUP BY l.WarehouseOrderNo, l.Position, l.Quantity, l.Id
    ),
    AggregatedLabels AS (
        -- Aggregate label quantities by WarehouseOrderNo + Position
        SELECT
            WarehouseOrderNo,
            Position,
            SUM(CASE WHEN HasBent = 1   THEN Quantity ELSE 0 END) AS KitQuantity,
            MAX(BentDate)    AS KitDate,
            SUM(CASE WHEN HasSorted = 1 THEN Quantity ELSE 0 END) AS SortQuantity,
            MAX(SortedDate)  AS SortDate,
            SUM(CASE WHEN HasPacked = 1 THEN Quantity ELSE 0 END) AS PackQuantity,
            MAX(PackedDate)  AS PackDate
        FROM LabelStatusFlags
        GROUP BY WarehouseOrderNo, Position
    )
    SELECT
        p.[System]            AS OneLineItemCode,
        p.SoNo,
        p.WarehouseOrderNo,
        pid.[Group]           AS Family,
        pid.Position,
        pid.DrawingNo         AS ItemCode,
        pid.Description       AS ItemName,
        pid.OrderQuantity,
        pid.Reserved1         AS Color,
        ISNULL(SUM(al.KitQuantity), 0)  AS KitQuantity,
        MAX(al.KitDate)                 AS KitDate,
        ISNULL(SUM(al.SortQuantity), 0) AS SortQuantity,
        MAX(al.SortDate)                AS SortDate,
        ISNULL(SUM(al.PackQuantity), 0) AS PackQuantity,
        MAX(al.PackDate)                AS PackDate
    FROM dbo.[Plan] AS p
    INNER JOIN dbo.PlanItemDetail AS pid
        ON p.Id = pid.PlanId
    INNER JOIN LotNos AS ln
        ON p.LotNo = ln.LotNo
    INNER JOIN Families AS f
        ON pid.[Group] = f.Family
    LEFT JOIN AggregatedLabels AS al
        ON p.WarehouseOrderNo = al.WarehouseOrderNo
        AND pid.Position = al.Position
    GROUP BY
        p.[System], p.SoNo, p.WarehouseOrderNo,
        pid.[Group], pid.Position, pid.DrawingNo, pid.Description, pid.OrderQuantity, pid.Reserved1
    ORDER BY
        p.[System], p.SoNo, p.WarehouseOrderNo, pid.[Group], pid.Position;
END;
GO

