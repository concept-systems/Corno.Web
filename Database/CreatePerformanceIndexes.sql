-- Performance Optimization Indexes
-- Purpose: Create indexes to improve query performance for Carton, Label, and Plan tables
-- Execute this script on your database to improve query performance
-- 
-- Note: Check if indexes already exist before creating to avoid errors
-- Run this script during maintenance window as index creation may lock tables

-- Set required options for filtered indexes
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE [Godrej.New.Kitchen];
GO

-- =============================================
-- Foreign Key Indexes (Critical for JOIN performance)
-- =============================================

-- CartonDetail.CartonId (Foreign Key to Carton)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_CartonId' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_CartonDetail_CartonId 
    ON CartonDetail(CartonId);
    PRINT 'Created index: IX_CartonDetail_CartonId';
END
ELSE
    PRINT 'Index already exists: IX_CartonDetail_CartonId';
GO

-- LabelDetail.LabelId (Foreign Key to Label)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LabelDetail_LabelId' AND object_id = OBJECT_ID('LabelDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LabelDetail_LabelId 
    ON LabelDetail(LabelId);
    PRINT 'Created index: IX_LabelDetail_LabelId';
END
ELSE
    PRINT 'Index already exists: IX_LabelDetail_LabelId';
GO

-- PlanItemDetail.PlanId (Foreign Key to Plan)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlanItemDetail_PlanId' AND object_id = OBJECT_ID('PlanItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PlanItemDetail_PlanId 
    ON PlanItemDetail(PlanId);
    PRINT 'Created index: IX_PlanItemDetail_PlanId';
END
ELSE
    PRINT 'Index already exists: IX_PlanItemDetail_PlanId';
GO

-- =============================================
-- Frequently Queried Columns - CartonDetail
-- =============================================

-- CartonDetail.Barcode (used in filters and joins)
-- Note: If Barcode is TEXT/NTEXT, we can't create a regular index
-- We'll try to create it, but if it fails, we'll skip it
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_Barcode' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    BEGIN TRY
        -- Check if Barcode column can be indexed (must be VARCHAR/NVARCHAR, not TEXT/NTEXT)
        DECLARE @BarcodeType NVARCHAR(128);
        SELECT @BarcodeType = TYPE_NAME(system_type_id) 
        FROM sys.columns 
        WHERE object_id = OBJECT_ID('CartonDetail') AND name = 'Barcode';
        
        IF @BarcodeType IN ('varchar', 'nvarchar', 'char', 'nchar')
        BEGIN
            CREATE NONCLUSTERED INDEX IX_CartonDetail_Barcode 
            ON CartonDetail(Barcode)
            WHERE Barcode IS NOT NULL;
            PRINT 'Created index: IX_CartonDetail_Barcode';
        END
        ELSE
        BEGIN
            PRINT 'Skipped index: IX_CartonDetail_Barcode (Barcode column type is ' + @BarcodeType + ', cannot be indexed)';
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error creating index IX_CartonDetail_Barcode: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
    PRINT 'Index already exists: IX_CartonDetail_Barcode';
GO

-- CartonDetail.Position (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_Position' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_CartonDetail_Position 
    ON CartonDetail(Position)
    WHERE Position IS NOT NULL;
    PRINT 'Created index: IX_CartonDetail_Position';
END
ELSE
    PRINT 'Index already exists: IX_CartonDetail_Position';
GO

-- CartonDetail.PackingTypeId + ItemId (composite index for common filter pattern)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartonDetail_PackingTypeId_ItemId' AND object_id = OBJECT_ID('CartonDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_CartonDetail_PackingTypeId_ItemId 
    ON CartonDetail(PackingTypeId, ItemId)
    WHERE PackingTypeId IS NOT NULL AND ItemId IS NOT NULL;
    PRINT 'Created index: IX_CartonDetail_PackingTypeId_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_CartonDetail_PackingTypeId_ItemId';
GO

-- =============================================
-- Frequently Queried Columns - Label
-- =============================================

-- Label.Barcode (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Label_Barcode' AND object_id = OBJECT_ID('Label'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Label_Barcode 
    ON Label(Barcode)
    WHERE Barcode IS NOT NULL;
    PRINT 'Created index: IX_Label_Barcode';
END
ELSE
    PRINT 'Index already exists: IX_Label_Barcode';
GO

-- Label.WarehouseOrderNo (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Label_WarehouseOrderNo' AND object_id = OBJECT_ID('Label'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Label_WarehouseOrderNo 
    ON Label(WarehouseOrderNo)
    WHERE WarehouseOrderNo IS NOT NULL;
    PRINT 'Created index: IX_Label_WarehouseOrderNo';
END
ELSE
    PRINT 'Index already exists: IX_Label_WarehouseOrderNo';
GO

-- Label.LabelDate (used in date range queries)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Label_LabelDate' AND object_id = OBJECT_ID('Label'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Label_LabelDate 
    ON Label(LabelDate)
    WHERE LabelDate IS NOT NULL;
    PRINT 'Created index: IX_Label_LabelDate';
END
ELSE
    PRINT 'Index already exists: IX_Label_LabelDate';
GO

-- =============================================
-- Frequently Queried Columns - Carton
-- =============================================

-- Carton.WarehouseOrderNo (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carton_WarehouseOrderNo' AND object_id = OBJECT_ID('Carton'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Carton_WarehouseOrderNo 
    ON Carton(WarehouseOrderNo)
    WHERE WarehouseOrderNo IS NOT NULL;
    PRINT 'Created index: IX_Carton_WarehouseOrderNo';
END
ELSE
    PRINT 'Index already exists: IX_Carton_WarehouseOrderNo';
GO

-- Carton.CartonBarcode (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carton_CartonBarcode' AND object_id = OBJECT_ID('Carton'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Carton_CartonBarcode 
    ON Carton(CartonBarcode)
    WHERE CartonBarcode IS NOT NULL;
    PRINT 'Created index: IX_Carton_CartonBarcode';
END
ELSE
    PRINT 'Index already exists: IX_Carton_CartonBarcode';
GO

-- Composite index for common filter pattern: WarehouseOrderNo + Status
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carton_WarehouseOrderNo_Status' AND object_id = OBJECT_ID('Carton'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Carton_WarehouseOrderNo_Status 
    ON Carton(WarehouseOrderNo, Status)
    WHERE WarehouseOrderNo IS NOT NULL;
    PRINT 'Created index: IX_Carton_WarehouseOrderNo_Status';
END
ELSE
    PRINT 'Index already exists: IX_Carton_WarehouseOrderNo_Status';
GO

-- Carton.PackingDate (used in date range queries)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carton_PackingDate' AND object_id = OBJECT_ID('Carton'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Carton_PackingDate 
    ON Carton(PackingDate)
    WHERE PackingDate IS NOT NULL;
    PRINT 'Created index: IX_Carton_PackingDate';
END
ELSE
    PRINT 'Index already exists: IX_Carton_PackingDate';
GO

-- =============================================
-- Frequently Queried Columns - Plan
-- =============================================

-- Plan.WarehouseOrderNo (used in filters - most critical)
-- Note: Plan is a reserved keyword, so we use [Plan]
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Plan_WarehouseOrderNo' AND object_id = OBJECT_ID('[Plan]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Plan_WarehouseOrderNo 
    ON [Plan](WarehouseOrderNo)
    WHERE WarehouseOrderNo IS NOT NULL;
    PRINT 'Created index: IX_Plan_WarehouseOrderNo';
END
ELSE
    PRINT 'Index already exists: IX_Plan_WarehouseOrderNo';
GO

-- Plan.LotNo (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Plan_LotNo' AND object_id = OBJECT_ID('[Plan]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Plan_LotNo 
    ON [Plan](LotNo)
    WHERE LotNo IS NOT NULL;
    PRINT 'Created index: IX_Plan_LotNo';
END
ELSE
    PRINT 'Index already exists: IX_Plan_LotNo';
GO

-- Plan.ProductionOrderNo (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Plan_ProductionOrderNo' AND object_id = OBJECT_ID('[Plan]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Plan_ProductionOrderNo 
    ON [Plan](ProductionOrderNo)
    WHERE ProductionOrderNo IS NOT NULL;
    PRINT 'Created index: IX_Plan_ProductionOrderNo';
END
ELSE
    PRINT 'Index already exists: IX_Plan_ProductionOrderNo';
GO

-- =============================================
-- PlanItemDetail Additional Indexes
-- =============================================

-- PlanItemDetail.Position (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlanItemDetail_Position' AND object_id = OBJECT_ID('PlanItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PlanItemDetail_Position 
    ON PlanItemDetail(Position)
    WHERE Position IS NOT NULL;
    PRINT 'Created index: IX_PlanItemDetail_Position';
END
ELSE
    PRINT 'Index already exists: IX_PlanItemDetail_Position';
GO

-- PlanItemDetail.ItemId (used in filters)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PlanItemDetail_ItemId' AND object_id = OBJECT_ID('PlanItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PlanItemDetail_ItemId 
    ON PlanItemDetail(ItemId)
    WHERE ItemId IS NOT NULL;
    PRINT 'Created index: IX_PlanItemDetail_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_PlanItemDetail_ItemId';
GO

-- =============================================
-- Additional Master/Detail Indexes (Sales, Grn, Product, Customer, Supplier, Location)
-- =============================================

------------------------------------------------------------
-- SALES: SalesOrder / SalesOrderDetail / SalesInvoice / SalesReturn
------------------------------------------------------------

-- SalesOrderDetail.SalesOrderId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesOrderDetail_SalesOrderId' AND object_id = OBJECT_ID('SalesOrderDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesOrderDetail_SalesOrderId
    ON SalesOrderDetail(SalesOrderId);
    PRINT 'Created index: IX_SalesOrderDetail_SalesOrderId';
END
ELSE
    PRINT 'Index already exists: IX_SalesOrderDetail_SalesOrderId';
GO

-- SalesOrderDetail.ItemId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesOrderDetail_ItemId' AND object_id = OBJECT_ID('SalesOrderDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesOrderDetail_ItemId
    ON SalesOrderDetail(ItemId);
    PRINT 'Created index: IX_SalesOrderDetail_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_SalesOrderDetail_ItemId';
GO

-- SalesInvoiceDetail.SalesInvoiceId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesInvoiceDetail_SalesInvoiceId' AND object_id = OBJECT_ID('SalesInvoiceDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesInvoiceDetail_SalesInvoiceId
    ON SalesInvoiceDetail(SalesInvoiceId);
    PRINT 'Created index: IX_SalesInvoiceDetail_SalesInvoiceId';
END
ELSE
    PRINT 'Index already exists: IX_SalesInvoiceDetail_SalesInvoiceId';
GO

-- SalesInvoiceDetail.ItemId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesInvoiceDetail_ItemId' AND object_id = OBJECT_ID('SalesInvoiceDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesInvoiceDetail_ItemId
    ON SalesInvoiceDetail(ItemId);
    PRINT 'Created index: IX_SalesInvoiceDetail_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_SalesInvoiceDetail_ItemId';
GO

-- SalesReturnDetail.SalesReturnId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesReturnDetail_SalesReturnId' AND object_id = OBJECT_ID('SalesReturnDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesReturnDetail_SalesReturnId
    ON SalesReturnDetail(SalesReturnId);
    PRINT 'Created index: IX_SalesReturnDetail_SalesReturnId';
END
ELSE
    PRINT 'Index already exists: IX_SalesReturnDetail_SalesReturnId';
GO

-- SalesReturnDetail.ItemId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SalesReturnDetail_ItemId' AND object_id = OBJECT_ID('SalesReturnDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SalesReturnDetail_ItemId
    ON SalesReturnDetail(ItemId);
    PRINT 'Created index: IX_SalesReturnDetail_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_SalesReturnDetail_ItemId';
GO

------------------------------------------------------------
-- GRN: Grn / GrnDetail
------------------------------------------------------------

-- GrnDetail.GrnId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GrnDetail_GrnId' AND object_id = OBJECT_ID('GrnDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GrnDetail_GrnId
    ON GrnDetail(GrnId);
    PRINT 'Created index: IX_GrnDetail_GrnId';
END
ELSE
    PRINT 'Index already exists: IX_GrnDetail_GrnId';
GO

-- GrnDetail.ItemId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GrnDetail_ItemId' AND object_id = OBJECT_ID('GrnDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GrnDetail_ItemId
    ON GrnDetail(ItemId);
    PRINT 'Created index: IX_GrnDetail_ItemId';
END
ELSE
    PRINT 'Index already exists: IX_GrnDetail_ItemId';
GO

------------------------------------------------------------
-- PRODUCT: Product / ProductItemDetail
------------------------------------------------------------

-- ProductItemDetail.ProductId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductItemDetail_ProductId' AND object_id = OBJECT_ID('ProductItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProductItemDetail_ProductId
    ON ProductItemDetail(ProductId);
    PRINT 'Created index: IX_ProductItemDetail_ProductId';
END
ELSE
    PRINT 'Index already exists: IX_ProductItemDetail_ProductId';
GO

------------------------------------------------------------
-- CUSTOMER: Customer / CustomerProductDetail
------------------------------------------------------------

-- CustomerProductDetail.CustomerId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CustomerProductDetail_CustomerId' AND object_id = OBJECT_ID('CustomerProductDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_CustomerProductDetail_CustomerId
    ON CustomerProductDetail(CustomerId);
    PRINT 'Created index: IX_CustomerProductDetail_CustomerId';
END
ELSE
    PRINT 'Index already exists: IX_CustomerProductDetail_CustomerId';
GO

------------------------------------------------------------
-- SUPPLIER: Supplier / SupplierItemDetail
------------------------------------------------------------

-- SupplierItemDetail.SupplierId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SupplierItemDetail_SupplierId' AND object_id = OBJECT_ID('SupplierItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SupplierItemDetail_SupplierId
    ON SupplierItemDetail(SupplierId);
    PRINT 'Created index: IX_SupplierItemDetail_SupplierId';
END
ELSE
    PRINT 'Index already exists: IX_SupplierItemDetail_SupplierId';
GO

------------------------------------------------------------
-- LOCATION: Location / LocationItemDetail
------------------------------------------------------------

-- LocationItemDetail.LocationId (FK)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LocationItemDetail_LocationId' AND object_id = OBJECT_ID('LocationItemDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LocationItemDetail_LocationId
    ON LocationItemDetail(LocationId);
    PRINT 'Created index: IX_LocationItemDetail_LocationId';
END
ELSE
    PRINT 'Index already exists: IX_LocationItemDetail_LocationId';
GO

PRINT '========================================';
PRINT 'Index creation script completed!';
PRINT '========================================';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Update statistics: UPDATE STATISTICS Carton; UPDATE STATISTICS CartonDetail; etc.';
PRINT '2. Monitor query performance using SQL Server Profiler or Extended Events';
PRINT '3. Review execution plans to verify index usage';
GO

