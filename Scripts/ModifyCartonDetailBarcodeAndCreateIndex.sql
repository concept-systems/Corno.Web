-- Modify CartonDetail.Barcode Column and Create Index
-- Purpose: Change Barcode column from NVARCHAR(MAX) to NVARCHAR(200) and create index
-- This will improve query performance for Barcode lookups

USE [Godrej.New.Kitchen];
GO

-- Set required options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'Starting CartonDetail.Barcode modification...';
PRINT '========================================';
GO

-- Step 1: Check current column type
DECLARE @CurrentType NVARCHAR(128);
SELECT @CurrentType = TYPE_NAME(system_type_id) + 
    CASE 
        WHEN max_length = -1 THEN '(MAX)'
        WHEN TYPE_NAME(system_type_id) IN ('nvarchar', 'nchar') THEN '(' + CAST(max_length/2 AS VARCHAR) + ')'
        WHEN TYPE_NAME(system_type_id) IN ('varchar', 'char') THEN '(' + CAST(max_length AS VARCHAR) + ')'
        ELSE ''
    END
FROM sys.columns 
WHERE object_id = OBJECT_ID('CartonDetail') AND name = 'Barcode';

PRINT 'Current Barcode column type: ' + ISNULL(@CurrentType, 'NOT FOUND');
GO

-- Step 2: Check if there are any values longer than 200 characters
DECLARE @MaxLength INT;
DECLARE @LongBarcodeCount INT;

SELECT @MaxLength = MAX(LEN(Barcode))
FROM CartonDetail
WHERE Barcode IS NOT NULL;

SELECT @LongBarcodeCount = COUNT(*)
FROM CartonDetail
WHERE Barcode IS NOT NULL AND LEN(Barcode) > 200;

IF @MaxLength > 200
BEGIN
    PRINT 'WARNING: Found ' + CAST(@LongBarcodeCount AS VARCHAR) + ' rows with Barcode longer than 200 characters';
    PRINT 'These will be truncated. Please review before proceeding.';
    
    -- Show sample of long barcodes
    SELECT TOP 10 
        Id, 
        Barcode, 
        LEN(Barcode) AS BarcodeLength
    FROM CartonDetail
    WHERE Barcode IS NOT NULL AND LEN(Barcode) > 200
    ORDER BY LEN(Barcode) DESC;
END
ELSE
BEGIN
    PRINT 'All Barcode values are within 200 characters. Safe to proceed.';
END
GO

-- Step 3: Drop the index if it exists (in case it was created as a computed column or other type)
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_Barcode' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    DROP INDEX IX_CartonDetail_Barcode ON CartonDetail;
    PRINT 'Dropped existing IX_CartonDetail_Barcode index';
END
GO

-- Step 4: Alter the column to NVARCHAR(200)
BEGIN TRY
    ALTER TABLE CartonDetail
    ALTER COLUMN Barcode NVARCHAR(200) NULL;
    PRINT 'Successfully altered CartonDetail.Barcode to NVARCHAR(200)';
END TRY
BEGIN CATCH
    PRINT 'Error altering column: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- Step 5: Create the index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_Barcode' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    BEGIN TRY
        CREATE NONCLUSTERED INDEX IX_CartonDetail_Barcode 
        ON CartonDetail(Barcode)
        WHERE Barcode IS NOT NULL;
        PRINT 'Successfully created index: IX_CartonDetail_Barcode';
    END TRY
    BEGIN CATCH
        PRINT 'Error creating index: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
ELSE
BEGIN
    PRINT 'Index IX_CartonDetail_Barcode already exists';
END
GO

-- Step 6: Update statistics
UPDATE STATISTICS CartonDetail;
PRINT 'Updated statistics for CartonDetail table';
GO

PRINT '========================================';
PRINT 'CartonDetail.Barcode modification completed!';
PRINT '========================================';
GO

