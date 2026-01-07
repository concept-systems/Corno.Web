-- =============================================
-- Login Module Database Migration Script
-- =============================================
-- Description: Creates tables and enhances existing tables for Login Module
-- Version: 1.0
-- Date: 2024-01-15
-- 
-- IMPORTANT: 
-- 1. Replace [YourDatabaseName] with your actual database name (line 6)
-- 2. Backup your database before running this script
-- 3. Run this script on a test environment first
-- 4. Review all changes before applying to production
-- =============================================

-- =============================================
-- IMPORTANT: Replace [YourDatabaseName] below with your actual database name
-- Current database from Web.config: Godrej.New.Kitchen
-- =============================================
USE [Godrej.New.Kitchen]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Note: GO statements create separate batches, so each batch commits automatically
-- This is safer for migrations as each step commits independently

PRINT 'Starting Login Module Migration...';
PRINT '=====================================';

-- =============================================
-- STEP 1: Enhance AspNetLoginHistories Table
-- =============================================
PRINT '';
PRINT 'Step 1: Enhancing AspNetLoginHistories table...';

-- Check if columns already exist before adding
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'SessionId')
BEGIN
    ALTER TABLE AspNetLoginHistories
    ADD SessionId NVARCHAR(200) NULL;
    PRINT '  ✓ Added SessionId column';
END
ELSE
    PRINT '  ⚠ SessionId column already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'LastActivityTime')
BEGIN
    ALTER TABLE AspNetLoginHistories
    ADD LastActivityTime DATETIME NULL;
    PRINT '  ✓ Added LastActivityTime column';
END
ELSE
    PRINT '  ⚠ LastActivityTime column already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'UserAgent')
BEGIN
    ALTER TABLE AspNetLoginHistories
    ADD UserAgent NVARCHAR(500) NULL;
    PRINT '  ✓ Added UserAgent column';
END
ELSE
    PRINT '  ⚠ UserAgent column already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'DeviceInfo')
BEGIN
    ALTER TABLE AspNetLoginHistories
    ADD DeviceInfo NVARCHAR(200) NULL;
    PRINT '  ✓ Added DeviceInfo column';
END
ELSE
    PRINT '  ⚠ DeviceInfo column already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'IsActive')
BEGIN
    ALTER TABLE AspNetLoginHistories
    ADD IsActive BIT DEFAULT 1;
    PRINT '  ✓ Added IsActive column';
END
ELSE
    PRINT '  ⚠ IsActive column already exists';

GO

-- Update existing records using dynamic SQL (separate batch after columns are added)
DECLARE @updateSql NVARCHAR(MAX);

-- Check if IsActive column exists before updating
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'IsActive')
BEGIN
    -- Update existing records to have IsActive = 1 where LogoutTime is NULL
    SET @updateSql = N'UPDATE AspNetLoginHistories SET IsActive = 1 WHERE LogoutTime IS NULL;';
    EXEC sp_executesql @updateSql;
    
    -- Update existing records to have IsActive = 0 where LogoutTime is NOT NULL
    SET @updateSql = N'UPDATE AspNetLoginHistories SET IsActive = 0 WHERE LogoutTime IS NOT NULL;';
    EXEC sp_executesql @updateSql;
    
    -- Update any NULL values
    SET @updateSql = N'UPDATE AspNetLoginHistories SET IsActive = CASE WHEN LogoutTime IS NULL THEN 1 ELSE 0 END WHERE IsActive IS NULL;';
    EXEC sp_executesql @updateSql;
    
    PRINT '  ✓ Updated existing records with IsActive values';
END

GO

-- Create indexes (only if columns exist) - Using dynamic SQL to avoid parse-time validation
DECLARE @sql NVARCHAR(MAX);

-- Index on SessionId
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'SessionId')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetLoginHistories_SessionId' AND object_id = OBJECT_ID('AspNetLoginHistories'))
    BEGIN
        SET @sql = N'CREATE INDEX IX_AspNetLoginHistories_SessionId ON AspNetLoginHistories(SessionId) WHERE SessionId IS NOT NULL;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Created index IX_AspNetLoginHistories_SessionId';
    END
END

-- Index on IsActive
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'IsActive')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetLoginHistories_Active' AND object_id = OBJECT_ID('AspNetLoginHistories'))
    BEGIN
        SET @sql = N'CREATE INDEX IX_AspNetLoginHistories_Active ON AspNetLoginHistories(AspNetUserId, IsActive) WHERE IsActive = 1 AND LogoutTime IS NULL;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Created index IX_AspNetLoginHistories_Active';
    END
END

-- Index on LastActivityTime
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'LastActivityTime')
    AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'IsActive')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetLoginHistories_LastActivity' AND object_id = OBJECT_ID('AspNetLoginHistories'))
    BEGIN
        SET @sql = N'CREATE INDEX IX_AspNetLoginHistories_LastActivity ON AspNetLoginHistories(LastActivityTime) WHERE IsActive = 1;';
        EXEC sp_executesql @sql;
        PRINT '  ✓ Created index IX_AspNetLoginHistories_LastActivity';
    END
END

-- =============================================
-- STEP 2: Create Menus Table
-- =============================================
PRINT '';
PRINT 'Step 2: Creating Menus table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Menus')
BEGIN
    CREATE TABLE Menus (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CompanyId INT NULL,
        SerialNo INT NULL DEFAULT 0,
        Code NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        MenuName NVARCHAR(200) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        MenuPath NVARCHAR(500) NULL,
        ParentMenuId INT NULL,
        ControllerName NVARCHAR(100) NULL,
        ActionName NVARCHAR(100) NULL,
        Area NVARCHAR(100) NULL,
        IconClass NVARCHAR(100) NULL,
        RouteValues NVARCHAR(MAX) NULL,
        DisplayOrder INT DEFAULT 0,
        IsVisible BIT DEFAULT 1,
        IsActive BIT DEFAULT 1,
        Description NVARCHAR(500) NULL,
        Source NVARCHAR(50) DEFAULT 'Manual',
        CreatedBy NVARCHAR(128) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        ExtraProperties NVARCHAR(MAX) NULL,
        
        FOREIGN KEY (ParentMenuId) REFERENCES Menus(Id),
        FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id)
    );
    
    CREATE INDEX IX_Menus_ParentMenuId ON Menus(ParentMenuId);
    CREATE INDEX IX_Menus_ControllerAction ON Menus(ControllerName, ActionName, Area) 
        WHERE ControllerName IS NOT NULL;
    
    PRINT '  ✓ Created Menus table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ Menus table already exists';
    
    -- Add standard columns if they don't exist
    DECLARE @addColumnSql NVARCHAR(MAX);
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'CompanyId')
    BEGIN
        ALTER TABLE Menus ADD CompanyId INT NULL;
        PRINT '  ✓ Added CompanyId column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'SerialNo')
    BEGIN
        ALTER TABLE Menus ADD SerialNo INT NULL DEFAULT 0;
        PRINT '  ✓ Added SerialNo column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'Code')
    BEGIN
        ALTER TABLE Menus ADD Code NVARCHAR(100) NULL;
        PRINT '  ✓ Added Code column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'Status')
    BEGIN
        ALTER TABLE Menus ADD Status NVARCHAR(50) NULL;
        PRINT '  ✓ Added Status column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'Source')
    BEGIN
        ALTER TABLE Menus ADD Source NVARCHAR(50) DEFAULT 'Manual';
        PRINT '  ✓ Added Source column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE Menus ADD DeletedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Menus_DeletedBy' AND parent_object_id = OBJECT_ID('Menus'))
        BEGIN
            ALTER TABLE Menus ADD CONSTRAINT FK_Menus_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added DeletedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'DeletedDate')
    BEGIN
        ALTER TABLE Menus ADD DeletedDate DATETIME NULL;
        PRINT '  ✓ Added DeletedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'ExtraProperties')
    BEGIN
        ALTER TABLE Menus ADD ExtraProperties NVARCHAR(MAX) NULL;
        PRINT '  ✓ Added ExtraProperties column';
    END
    
    -- Update existing records
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'Source')
    BEGIN
        SET @addColumnSql = N'UPDATE Menus SET Source = ''Manual'' WHERE Source IS NULL;';
        EXEC sp_executesql @addColumnSql;
    END
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Menus') AND name = 'SerialNo')
    BEGIN
        SET @addColumnSql = N'UPDATE Menus SET SerialNo = 0 WHERE SerialNo IS NULL;';
        EXEC sp_executesql @addColumnSql;
    END
END

-- =============================================
-- STEP 3: Create PermissionTypes Table
-- =============================================
PRINT '';
PRINT 'Step 3: Creating PermissionTypes table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PermissionTypes')
BEGIN
    CREATE TABLE PermissionTypes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CompanyId INT NULL,
        SerialNo INT NULL DEFAULT 0,
        Code NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        Name NVARCHAR(50) NOT NULL UNIQUE,
        Description NVARCHAR(200) NULL,
        DisplayOrder INT DEFAULT 0,
        CreatedBy NVARCHAR(128) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        ExtraProperties NVARCHAR(MAX) NULL,
        
        FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id)
    );
    
    -- Insert default permission types
    INSERT INTO PermissionTypes (Name, Description, DisplayOrder) VALUES
    ('Menu', 'Menu item visibility and access', 1),
    ('Page', 'Page/Action access (Index, Create, Edit, View, Delete)', 2),
    ('Control', 'Control access (Buttons, Links, etc.)', 3);
    
    PRINT '  ✓ Created PermissionTypes table';
    PRINT '  ✓ Inserted default permission types';
END
ELSE
BEGIN
    PRINT '  ⚠ PermissionTypes table already exists';
    
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'CompanyId')
    BEGIN
        ALTER TABLE PermissionTypes ADD CompanyId INT NULL;
        PRINT '  ✓ Added CompanyId column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'SerialNo')
    BEGIN
        ALTER TABLE PermissionTypes ADD SerialNo INT NULL DEFAULT 0;
        PRINT '  ✓ Added SerialNo column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'Code')
    BEGIN
        ALTER TABLE PermissionTypes ADD Code NVARCHAR(100) NULL;
        PRINT '  ✓ Added Code column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'Status')
    BEGIN
        ALTER TABLE PermissionTypes ADD Status NVARCHAR(50) NULL;
        PRINT '  ✓ Added Status column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'CreatedBy')
    BEGIN
        ALTER TABLE PermissionTypes ADD CreatedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PermissionTypes_CreatedBy' AND parent_object_id = OBJECT_ID('PermissionTypes'))
        BEGIN
            ALTER TABLE PermissionTypes ADD CONSTRAINT FK_PermissionTypes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added CreatedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'CreatedDate')
    BEGIN
        ALTER TABLE PermissionTypes ADD CreatedDate DATETIME DEFAULT GETDATE();
        PRINT '  ✓ Added CreatedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'ModifiedBy')
    BEGIN
        ALTER TABLE PermissionTypes ADD ModifiedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PermissionTypes_ModifiedBy' AND parent_object_id = OBJECT_ID('PermissionTypes'))
        BEGIN
            ALTER TABLE PermissionTypes ADD CONSTRAINT FK_PermissionTypes_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added ModifiedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'ModifiedDate')
    BEGIN
        ALTER TABLE PermissionTypes ADD ModifiedDate DATETIME NULL;
        PRINT '  ✓ Added ModifiedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE PermissionTypes ADD DeletedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PermissionTypes_DeletedBy' AND parent_object_id = OBJECT_ID('PermissionTypes'))
        BEGIN
            ALTER TABLE PermissionTypes ADD CONSTRAINT FK_PermissionTypes_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added DeletedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'DeletedDate')
    BEGIN
        ALTER TABLE PermissionTypes ADD DeletedDate DATETIME NULL;
        PRINT '  ✓ Added DeletedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PermissionTypes') AND name = 'ExtraProperties')
    BEGIN
        ALTER TABLE PermissionTypes ADD ExtraProperties NVARCHAR(MAX) NULL;
        PRINT '  ✓ Added ExtraProperties column';
    END
    
    -- Ensure default types exist
    IF NOT EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Menu')
    BEGIN
        INSERT INTO PermissionTypes (Name, Description, DisplayOrder) VALUES ('Menu', 'Menu item visibility and access', 1);
        PRINT '  ✓ Inserted Menu permission type';
    END
    IF NOT EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Page')
    BEGIN
        INSERT INTO PermissionTypes (Name, Description, DisplayOrder) VALUES ('Page', 'Page/Action access (Index, Create, Edit, View, Delete)', 2);
        PRINT '  ✓ Inserted Page permission type';
    END
    IF NOT EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Control')
    BEGIN
        INSERT INTO PermissionTypes (Name, Description, DisplayOrder) VALUES ('Control', 'Control access (Buttons, Links, etc.)', 3);
        PRINT '  ✓ Inserted Control permission type';
    END
END

-- =============================================
-- STEP 4: Create AccessControls Table
-- =============================================
PRINT '';
PRINT 'Step 4: Creating AccessControls table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AccessControls')
BEGIN
    CREATE TABLE AccessControls (
        Id NVARCHAR(128) PRIMARY KEY DEFAULT NEWID(),
        CompanyId INT NULL,
        SerialNo INT NULL DEFAULT 0,
        Code NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        PermissionTypeId INT NOT NULL,
        MenuId INT NULL,
        ControllerName NVARCHAR(100) NULL,
        ActionName NVARCHAR(100) NULL,
        Area NVARCHAR(100) NULL,
        ControlId NVARCHAR(200) NULL,
        ControlName NVARCHAR(200) NULL,
        UserId NVARCHAR(128) NULL,
        RoleId NVARCHAR(128) NULL,
        IsAllowed BIT DEFAULT 1,
        CreatedBy NVARCHAR(128) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        ExtraProperties NVARCHAR(MAX) NULL,
        
        FOREIGN KEY (PermissionTypeId) REFERENCES PermissionTypes(Id),
        FOREIGN KEY (MenuId) REFERENCES Menus(Id),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id),
        FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id),
        
        CHECK ((UserId IS NULL AND RoleId IS NOT NULL) OR (UserId IS NOT NULL AND RoleId IS NULL))
    );
    
    CREATE INDEX IX_AccessControls_User ON AccessControls(UserId) WHERE UserId IS NOT NULL;
    CREATE INDEX IX_AccessControls_Role ON AccessControls(RoleId) WHERE RoleId IS NOT NULL;
    CREATE INDEX IX_AccessControls_Menu ON AccessControls(MenuId) WHERE MenuId IS NOT NULL;
    CREATE INDEX IX_AccessControls_ControllerAction ON AccessControls(ControllerName, ActionName, Area) 
        WHERE ControllerName IS NOT NULL;
    CREATE INDEX IX_AccessControls_ControlId ON AccessControls(ControlId) WHERE ControlId IS NOT NULL;
    
    PRINT '  ✓ Created AccessControls table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ AccessControls table already exists';
    
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'CompanyId')
    BEGIN
        ALTER TABLE AccessControls ADD CompanyId INT NULL;
        PRINT '  ✓ Added CompanyId column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'SerialNo')
    BEGIN
        ALTER TABLE AccessControls ADD SerialNo INT NULL DEFAULT 0;
        PRINT '  ✓ Added SerialNo column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'Code')
    BEGIN
        ALTER TABLE AccessControls ADD Code NVARCHAR(100) NULL;
        PRINT '  ✓ Added Code column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'Status')
    BEGIN
        ALTER TABLE AccessControls ADD Status NVARCHAR(50) NULL;
        PRINT '  ✓ Added Status column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'ModifiedBy')
    BEGIN
        ALTER TABLE AccessControls ADD ModifiedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AccessControls_ModifiedBy' AND parent_object_id = OBJECT_ID('AccessControls'))
        BEGIN
            ALTER TABLE AccessControls ADD CONSTRAINT FK_AccessControls_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added ModifiedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'ModifiedDate')
    BEGIN
        ALTER TABLE AccessControls ADD ModifiedDate DATETIME NULL;
        PRINT '  ✓ Added ModifiedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE AccessControls ADD DeletedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AccessControls_DeletedBy' AND parent_object_id = OBJECT_ID('AccessControls'))
        BEGIN
            ALTER TABLE AccessControls ADD CONSTRAINT FK_AccessControls_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added DeletedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'DeletedDate')
    BEGIN
        ALTER TABLE AccessControls ADD DeletedDate DATETIME NULL;
        PRINT '  ✓ Added DeletedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AccessControls') AND name = 'ExtraProperties')
    BEGIN
        ALTER TABLE AccessControls ADD ExtraProperties NVARCHAR(MAX) NULL;
        PRINT '  ✓ Added ExtraProperties column';
    END
END

-- =============================================
-- STEP 5: Create AuditLogs Table
-- =============================================
PRINT '';
PRINT 'Step 5: Creating AuditLogs table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AuditLogs')
BEGIN
    CREATE TABLE AuditLogs (
        Id BIGINT PRIMARY KEY IDENTITY(1,1),
        CompanyId INT NULL,
        SerialNo INT NULL DEFAULT 0,
        Code NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        UserId NVARCHAR(128) NULL,
        UserName NVARCHAR(256) NULL,
        Action NVARCHAR(100) NOT NULL,
        EntityType NVARCHAR(100) NULL,
        EntityId NVARCHAR(128) NULL,
        EntityName NVARCHAR(200) NULL,
        Details NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        Timestamp DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(128) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        ExtraProperties NVARCHAR(MAX) NULL,
        
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id)
    );
    
    CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
    CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp);
    CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
    CREATE INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType);
    
    PRINT '  ✓ Created AuditLogs table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ AuditLogs table already exists';
    
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'CompanyId')
    BEGIN
        ALTER TABLE AuditLogs ADD CompanyId INT NULL;
        PRINT '  ✓ Added CompanyId column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'SerialNo')
    BEGIN
        ALTER TABLE AuditLogs ADD SerialNo INT NULL DEFAULT 0;
        PRINT '  ✓ Added SerialNo column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'Code')
    BEGIN
        ALTER TABLE AuditLogs ADD Code NVARCHAR(100) NULL;
        PRINT '  ✓ Added Code column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'Status')
    BEGIN
        ALTER TABLE AuditLogs ADD Status NVARCHAR(50) NULL;
        PRINT '  ✓ Added Status column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'CreatedBy')
    BEGIN
        ALTER TABLE AuditLogs ADD CreatedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AuditLogs_CreatedBy' AND parent_object_id = OBJECT_ID('AuditLogs'))
        BEGIN
            ALTER TABLE AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added CreatedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'CreatedDate')
    BEGIN
        ALTER TABLE AuditLogs ADD CreatedDate DATETIME DEFAULT GETDATE();
        PRINT '  ✓ Added CreatedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'ModifiedBy')
    BEGIN
        ALTER TABLE AuditLogs ADD ModifiedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AuditLogs_ModifiedBy' AND parent_object_id = OBJECT_ID('AuditLogs'))
        BEGIN
            ALTER TABLE AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added ModifiedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'ModifiedDate')
    BEGIN
        ALTER TABLE AuditLogs ADD ModifiedDate DATETIME NULL;
        PRINT '  ✓ Added ModifiedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE AuditLogs ADD DeletedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AuditLogs_DeletedBy' AND parent_object_id = OBJECT_ID('AuditLogs'))
        BEGIN
            ALTER TABLE AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added DeletedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'DeletedDate')
    BEGIN
        ALTER TABLE AuditLogs ADD DeletedDate DATETIME NULL;
        PRINT '  ✓ Added DeletedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AuditLogs') AND name = 'ExtraProperties')
    BEGIN
        ALTER TABLE AuditLogs ADD ExtraProperties NVARCHAR(MAX) NULL;
        PRINT '  ✓ Added ExtraProperties column';
    END
END

-- =============================================
-- STEP 6: Create PasswordResetTokens Table
-- =============================================
PRINT '';
PRINT 'Step 6: Creating PasswordResetTokens table...';

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PasswordResetTokens')
BEGIN
    CREATE TABLE PasswordResetTokens (
        Id NVARCHAR(128) PRIMARY KEY DEFAULT NEWID(),
        CompanyId INT NULL,
        SerialNo INT NULL DEFAULT 0,
        Code NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        UserId NVARCHAR(128) NOT NULL,
        Token NVARCHAR(500) NOT NULL,
        ExpiryDate DATETIME NOT NULL,
        IsUsed BIT DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UsedDate DATETIME NULL,
        IpAddress NVARCHAR(50) NULL,
        CreatedBy NVARCHAR(128) NULL,
        ModifiedBy NVARCHAR(128) NULL,
        ModifiedDate DATETIME NULL,
        DeletedBy NVARCHAR(128) NULL,
        DeletedDate DATETIME NULL,
        ExtraProperties NVARCHAR(MAX) NULL,
        
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id),
        FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id)
    );
    
    CREATE INDEX IX_PasswordResetTokens_UserId ON PasswordResetTokens(UserId);
    CREATE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token) WHERE IsUsed = 0;
    
    PRINT '  ✓ Created PasswordResetTokens table with indexes';
END
ELSE
BEGIN
    PRINT '  ⚠ PasswordResetTokens table already exists';
    
    -- Add standard columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'CompanyId')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD CompanyId INT NULL;
        PRINT '  ✓ Added CompanyId column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'SerialNo')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD SerialNo INT NULL DEFAULT 0;
        PRINT '  ✓ Added SerialNo column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'Code')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD Code NVARCHAR(100) NULL;
        PRINT '  ✓ Added Code column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'Status')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD Status NVARCHAR(50) NULL;
        PRINT '  ✓ Added Status column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'CreatedBy')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD CreatedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PasswordResetTokens_CreatedBy' AND parent_object_id = OBJECT_ID('PasswordResetTokens'))
        BEGIN
            ALTER TABLE PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added CreatedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'ModifiedBy')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD ModifiedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PasswordResetTokens_ModifiedBy' AND parent_object_id = OBJECT_ID('PasswordResetTokens'))
        BEGIN
            ALTER TABLE PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added ModifiedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'ModifiedDate')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD ModifiedDate DATETIME NULL;
        PRINT '  ✓ Added ModifiedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD DeletedBy NVARCHAR(128) NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PasswordResetTokens_DeletedBy' AND parent_object_id = OBJECT_ID('PasswordResetTokens'))
        BEGIN
            ALTER TABLE PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES AspNetUsers(Id);
        END
        PRINT '  ✓ Added DeletedBy column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'DeletedDate')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD DeletedDate DATETIME NULL;
        PRINT '  ✓ Added DeletedDate column';
    END
    
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetTokens') AND name = 'ExtraProperties')
    BEGIN
        ALTER TABLE PasswordResetTokens ADD ExtraProperties NVARCHAR(MAX) NULL;
        PRINT '  ✓ Added ExtraProperties column';
    END
END

-- =============================================
-- VERIFICATION
-- =============================================
PRINT '';
PRINT '=====================================';
PRINT 'Verification:';
PRINT '=====================================';

-- Check AspNetLoginHistories enhancements
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'SessionId')
    AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'LastActivityTime')
    AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'UserAgent')
    AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'DeviceInfo')
    AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetLoginHistories') AND name = 'IsActive')
    PRINT '✓ AspNetLoginHistories table enhanced successfully'
ELSE
    PRINT '✗ AspNetLoginHistories table enhancement failed';

-- Check all tables
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Menus')
    PRINT '✓ Menus table exists'
ELSE
    PRINT '✗ Menus table missing';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PermissionTypes')
    PRINT '✓ PermissionTypes table exists'
ELSE
    PRINT '✗ PermissionTypes table missing';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AccessControls')
    PRINT '✓ AccessControls table exists'
ELSE
    PRINT '✗ AccessControls table missing';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AuditLogs')
    PRINT '✓ AuditLogs table exists'
ELSE
    PRINT '✗ AuditLogs table missing';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PasswordResetTokens')
    PRINT '✓ PasswordResetTokens table exists'
ELSE
    PRINT '✗ PasswordResetTokens table missing';

-- Check PermissionTypes data
IF EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Menu')
    AND EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Page')
    AND EXISTS (SELECT 1 FROM PermissionTypes WHERE Name = 'Control')
    PRINT '✓ PermissionTypes data verified'
ELSE
    PRINT '✗ PermissionTypes data missing';

PRINT '';
PRINT '=====================================';
PRINT 'Migration completed!';
PRINT '=====================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Verify all tables and indexes were created';
PRINT '2. Test the application';
PRINT '3. Create initial menu structure';
PRINT '4. Assign permissions to roles/users';
PRINT '';
PRINT 'Note: Each batch committed automatically (GO statements).';
PRINT 'If any errors occurred, check the output above.';
GO
