-- =============================================
-- Health Check and Data Integrity Module Database Migration Script
-- =============================================
-- Description: Creates tables for Health Check and Data Integrity monitoring
-- Version: 1.0
-- Date: 2024-12-19
-- 
-- IMPORTANT: 
-- 1. Replace [YourDatabaseName] with your actual database name
-- 2. Backup your database before running this script
-- 3. Run this script on a test environment first
-- 4. Review all changes before applying to production
-- =============================================

-- =============================================
-- IMPORTANT: Replace [YourDatabaseName] below with your actual database name
-- =============================================
USE [Godrej.New.Kitchen]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT 'Starting Health Check Module Migration...';
PRINT '==========================================';

-- =============================================
-- STEP 1: Create HealthCheckReports Table
-- =============================================
PRINT '';
PRINT 'Step 1: Creating HealthCheckReports table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'HealthCheckReports')
BEGIN
    CREATE TABLE HealthCheckReports (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CheckDate DATETIME NOT NULL DEFAULT GETDATE(),
        CheckType NVARCHAR(50) NOT NULL, -- Database, Memory, Disk, etc.
        Status NVARCHAR(20) NOT NULL, -- Healthy, Warning, Critical (Health Status)
        Message NVARCHAR(MAX),
        Details NVARCHAR(MAX), -- JSON format
        ExecutionTimeMs INT,
        AutoFixed BIT DEFAULT 0,
        -- Standard Columns
        CompanyId INT NULL,
        SerialNo INT NULL,
        Code NVARCHAR(50) NULL,
        -- Status column already exists above for health status
        CreatedBy NVARCHAR(128) DEFAULT 'System',
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        Ip NVARCHAR(15) NULL,
        ExtraProperties NVARCHAR(MAX) NULL
    );
    
    CREATE INDEX IX_HealthCheckReports_CheckDate ON HealthCheckReports(CheckDate);
    CREATE INDEX IX_HealthCheckReports_Status ON HealthCheckReports(Status);
    CREATE INDEX IX_HealthCheckReports_CheckType ON HealthCheckReports(CheckType);
    
    PRINT '  ✓ Created HealthCheckReports table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ HealthCheckReports table already exists';
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'CompanyId')
        ALTER TABLE HealthCheckReports ADD CompanyId INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'SerialNo')
        ALTER TABLE HealthCheckReports ADD SerialNo INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'Code')
        ALTER TABLE HealthCheckReports ADD Code NVARCHAR(50) NULL;
    -- Status column already exists for health status (Healthy/Warning/Critical)
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'DeletedBy')
        ALTER TABLE HealthCheckReports ADD DeletedBy NVARCHAR(128) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'DeletedDate')
        ALTER TABLE HealthCheckReports ADD DeletedDate DATETIME NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'Ip')
        ALTER TABLE HealthCheckReports ADD Ip NVARCHAR(15) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'ExtraProperties')
        ALTER TABLE HealthCheckReports ADD ExtraProperties NVARCHAR(MAX) NULL;
    -- Update existing column sizes to match standard
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'CreatedBy' AND max_length < 256)
        ALTER TABLE HealthCheckReports ALTER COLUMN CreatedBy NVARCHAR(128) NULL;
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckReports') AND name = 'ModifiedBy' AND max_length < 256)
        ALTER TABLE HealthCheckReports ALTER COLUMN ModifiedBy NVARCHAR(128) NULL;
    PRINT '  ✓ Added/Updated standard columns in HealthCheckReports table';
END
GO

-- =============================================
-- STEP 2: Create HealthCheckSummary Table
-- =============================================
PRINT '';
PRINT 'Step 2: Creating HealthCheckSummary table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'HealthCheckSummary')
BEGIN
    CREATE TABLE HealthCheckSummary (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ReportDate DATETIME NOT NULL DEFAULT GETDATE(),
        OverallStatus NVARCHAR(20) NOT NULL, -- Healthy, Warning, Critical
        TotalChecks INT,
        HealthyChecks INT,
        WarningChecks INT,
        CriticalChecks INT,
        SummaryDetails NVARCHAR(MAX), -- JSON
        -- Standard Columns
        CompanyId INT NULL,
        SerialNo INT NULL,
        Code NVARCHAR(50) NULL,
        Status NVARCHAR(50) NULL,
        CreatedBy NVARCHAR(128) DEFAULT 'System',
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        Ip NVARCHAR(15) NULL,
        ExtraProperties NVARCHAR(MAX) NULL
    );
    
    CREATE INDEX IX_HealthCheckSummary_ReportDate ON HealthCheckSummary(ReportDate);
    CREATE INDEX IX_HealthCheckSummary_OverallStatus ON HealthCheckSummary(OverallStatus);
    
    PRINT '  ✓ Created HealthCheckSummary table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ HealthCheckSummary table already exists';
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'CompanyId')
        ALTER TABLE HealthCheckSummary ADD CompanyId INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'SerialNo')
        ALTER TABLE HealthCheckSummary ADD SerialNo INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'Code')
        ALTER TABLE HealthCheckSummary ADD Code NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'Status')
        ALTER TABLE HealthCheckSummary ADD Status NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'DeletedBy')
        ALTER TABLE HealthCheckSummary ADD DeletedBy NVARCHAR(128) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'DeletedDate')
        ALTER TABLE HealthCheckSummary ADD DeletedDate DATETIME NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'Ip')
        ALTER TABLE HealthCheckSummary ADD Ip NVARCHAR(15) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'ExtraProperties')
        ALTER TABLE HealthCheckSummary ADD ExtraProperties NVARCHAR(MAX) NULL;
    -- Update existing column sizes to match standard
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'CreatedBy' AND max_length < 256)
        ALTER TABLE HealthCheckSummary ALTER COLUMN CreatedBy NVARCHAR(128) NULL;
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HealthCheckSummary') AND name = 'ModifiedBy' AND max_length < 256)
        ALTER TABLE HealthCheckSummary ALTER COLUMN ModifiedBy NVARCHAR(128) NULL;
    PRINT '  ✓ Added/Updated standard columns in HealthCheckSummary table';
END
GO

-- =============================================
-- STEP 3: Create DataIntegrityReports Table
-- =============================================
PRINT '';
PRINT 'Step 3: Creating DataIntegrityReports table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DataIntegrityReports')
BEGIN
    CREATE TABLE DataIntegrityReports (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CheckDate DATETIME NOT NULL DEFAULT GETDATE(),
        CheckType NVARCHAR(50) NOT NULL, -- PlanQuantities, CartonBarcodes, LabelSequence, etc.
        Status NVARCHAR(20) NOT NULL, -- Healthy, Warning, Critical (Health Status)
        Message NVARCHAR(MAX),
        Details NVARCHAR(MAX), -- JSON format with issues
        ExecutionTimeMs INT,
        RecordsChecked INT,
        IssuesFound INT,
        RecordsFixed INT,
        AutoFixed BIT DEFAULT 0,
        -- Standard Columns
        CompanyId INT NULL,
        SerialNo INT NULL,
        Code NVARCHAR(50) NULL,
        -- Status column already exists above for health status
        CreatedBy NVARCHAR(128) DEFAULT 'System',
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        Ip NVARCHAR(15) NULL,
        ExtraProperties NVARCHAR(MAX) NULL
    );
    
    CREATE INDEX IX_DataIntegrityReports_CheckDate ON DataIntegrityReports(CheckDate);
    CREATE INDEX IX_DataIntegrityReports_Status ON DataIntegrityReports(Status);
    CREATE INDEX IX_DataIntegrityReports_CheckType ON DataIntegrityReports(CheckType);
    
    PRINT '  ✓ Created DataIntegrityReports table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ DataIntegrityReports table already exists';
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'CompanyId')
        ALTER TABLE DataIntegrityReports ADD CompanyId INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'SerialNo')
        ALTER TABLE DataIntegrityReports ADD SerialNo INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'Code')
        ALTER TABLE DataIntegrityReports ADD Code NVARCHAR(50) NULL;
    -- Status column already exists for health status (Healthy/Warning/Critical)
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'DeletedBy')
        ALTER TABLE DataIntegrityReports ADD DeletedBy NVARCHAR(128) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'DeletedDate')
        ALTER TABLE DataIntegrityReports ADD DeletedDate DATETIME NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'Ip')
        ALTER TABLE DataIntegrityReports ADD Ip NVARCHAR(15) NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'ExtraProperties')
        ALTER TABLE DataIntegrityReports ADD ExtraProperties NVARCHAR(MAX) NULL;
    -- Update existing column sizes to match standard
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'CreatedBy' AND max_length < 256)
        ALTER TABLE DataIntegrityReports ALTER COLUMN CreatedBy NVARCHAR(128) NULL;
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DataIntegrityReports') AND name = 'ModifiedBy' AND max_length < 256)
        ALTER TABLE DataIntegrityReports ALTER COLUMN ModifiedBy NVARCHAR(128) NULL;
    PRINT '  ✓ Added/Updated standard columns in DataIntegrityReports table';
END
GO

-- =============================================
-- STEP 4: Add Status column to Plan table (if not exists)
-- =============================================
PRINT '';
PRINT 'Step 4: Adding Status column to Plan table...';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Plan') AND name = 'Status')
BEGIN
    ALTER TABLE [Plan] ADD Status NVARCHAR(50) NULL;
    PRINT '  ✓ Added Status column to Plan table';
END
ELSE
BEGIN
    PRINT '  ⚠ Status column already exists in Plan table';
END
GO

-- =============================================
-- STEP 5: Create Indexes on Plan table for performance
-- =============================================
PRINT '';
PRINT 'Step 5: Creating indexes on Plan table...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Plan_Status_DueDate' AND object_id = OBJECT_ID('Plan'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Plan_Status_DueDate 
    ON [Plan](Status, DueDate) 
    INCLUDE (WarehouseOrderNo, Id);
    PRINT '  ✓ Created IX_Plan_Status_DueDate index';
END
ELSE
BEGIN
    PRINT '  ⚠ IX_Plan_Status_DueDate index already exists';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Plan_WarehouseOrderNo_Status' AND object_id = OBJECT_ID('Plan'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Plan_WarehouseOrderNo_Status 
    ON [Plan](WarehouseOrderNo, Status);
    PRINT '  ✓ Created IX_Plan_WarehouseOrderNo_Status index';
END
ELSE
BEGIN
    PRINT '  ⚠ IX_Plan_WarehouseOrderNo_Status index already exists';
END
GO

-- =============================================
-- STEP 6: Create Indexes on Label table for performance
-- =============================================
PRINT '';
PRINT 'Step 6: Creating indexes on Label table...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Label_WarehouseOrderNo_LabelDate' AND object_id = OBJECT_ID('Label'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Label_WarehouseOrderNo_LabelDate 
    ON Label(WarehouseOrderNo, LabelDate) 
    INCLUDE (Barcode, Status);
    PRINT '  ✓ Created IX_Label_WarehouseOrderNo_LabelDate index';
END
ELSE
BEGIN
    PRINT '  ⚠ IX_Label_WarehouseOrderNo_LabelDate index already exists';
END
GO

-- =============================================
-- STEP 7: Create Indexes on Carton table for performance
-- =============================================
PRINT '';
PRINT 'Step 7: Creating indexes on Carton table...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Carton_WarehouseOrderNo_PackingDate' AND object_id = OBJECT_ID('Carton'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Carton_WarehouseOrderNo_PackingDate 
    ON Carton(WarehouseOrderNo, PackingDate) 
    INCLUDE (Id, CartonBarcode);
    PRINT '  ✓ Created IX_Carton_WarehouseOrderNo_PackingDate index';
END
ELSE
BEGIN
    PRINT '  ⚠ IX_Carton_WarehouseOrderNo_PackingDate index already exists';
END
GO

-- =============================================
-- STEP 8: Create Indexes on LabelDetail table for performance
-- =============================================
PRINT '';
PRINT 'Step 8: Creating indexes on LabelDetail table...';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabelDetail_LabelId_Status_ScanDate' AND object_id = OBJECT_ID('LabelDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LabelDetail_LabelId_Status_ScanDate 
    ON LabelDetail(LabelId, Status, ScanDate);
    PRINT '  ✓ Created IX_LabelDetail_LabelId_Status_ScanDate index';
END
ELSE
BEGIN
    PRINT '  ⚠ IX_LabelDetail_LabelId_Status_ScanDate index already exists';
END
GO

-- =============================================
-- STEP 9: Update existing Plans with Status
-- =============================================
PRINT '';
PRINT 'Step 9: Updating existing Plans with Status...';

-- Mark plans as Packed if all items are packed
UPDATE p
SET p.Status = 'Packed',
    p.ModifiedDate = GETDATE(),
    p.ModifiedBy = 'System'
FROM [Plan] p
WHERE p.Status IS NULL
  AND NOT EXISTS (
      SELECT 1 
      FROM PlanItemDetail pid
      WHERE pid.PlanId = p.Id
        AND (pid.PackQuantity) < (pid.OrderQuantity)
        AND (pid.OrderQuantity) > 0
  );

PRINT '  ✓ Updated ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' plans to Packed status';

-- Mark remaining plans as InProgress
UPDATE p
SET p.Status = 'InProgress',
    p.ModifiedDate = GETDATE(),
    p.ModifiedBy = 'System'
FROM [Plan] p
WHERE p.Status IS NULL;

PRINT '  ✓ Updated ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' plans to InProgress status';
GO

-- =============================================
-- STEP 10: Add Status column to LabelDetail if not exists
-- =============================================
PRINT '';
PRINT 'Step 10: Checking LabelDetail table for Status column...';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LabelDetail') AND name = 'Status')
BEGIN
    ALTER TABLE LabelDetail ADD Status NVARCHAR(50) NULL;
    PRINT '  ✓ Added Status column to LabelDetail table';
    
    -- Update existing records based on ScanDate or other logic if needed
    -- This is a placeholder - adjust based on your business logic
    PRINT '  ⚠ Please review and update Status values in LabelDetail table manually if needed';
END
ELSE
BEGIN
    PRINT '  ⚠ Status column already exists in LabelDetail table';
END
GO

PRINT '';
PRINT '==========================================';
PRINT 'Health Check Module Migration Completed!';
PRINT '==========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Review all created tables and indexes';
PRINT '2. Verify Plan Status values are correct';
PRINT '3. Test the health check functionality';
PRINT '4. Monitor the HealthCheckReports and DataIntegrityReports tables';
PRINT '';

