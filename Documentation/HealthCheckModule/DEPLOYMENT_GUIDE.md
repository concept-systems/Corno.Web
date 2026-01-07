# Health Check Module - Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Database Migration](#database-migration)
3. [Application Deployment](#application-deployment)
4. [Configuration](#configuration)
5. [Verification](#verification)
6. [Rollback Plan](#rollback-plan)

## Prerequisites

### Software Requirements
- SQL Server 2012 or higher
- .NET Framework 4.8
- IIS 7.5 or higher
- Visual Studio 2019 or higher (for development)

### Database Requirements
- Database backup before migration
- Sufficient disk space for new tables and indexes
- Database user with CREATE TABLE, ALTER TABLE, CREATE INDEX permissions

### Application Requirements
- Quartz.NET package installed
- Entity Framework 6.5.1 or higher
- Autofac for dependency injection

## Database Migration

### Step 1: Backup Database

```sql
BACKUP DATABASE [YourDatabaseName] 
TO DISK = 'C:\Backups\YourDatabaseName_PreHealthCheck.bak'
WITH FORMAT, COMPRESSION;
```

### Step 2: Review Migration Script

1. Open `Database/Migration_HealthCheckModule.sql`
2. Replace `[YourDatabaseName]` with your actual database name (line 19)
3. Review all changes
4. Verify indexes to be created

### Step 3: Run Migration Script

**Option A: SQL Server Management Studio**
1. Open SQL Server Management Studio
2. Connect to your database server
3. Open `Database/Migration_HealthCheckModule.sql`
4. Execute the script (F5)
5. Review output for any errors

**Option B: Command Line**
```bash
sqlcmd -S YourServer -d YourDatabase -i Database/Migration_HealthCheckModule.sql
```

### Step 4: Verify Migration

```sql
-- Check tables exist
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('HealthCheckReports', 'HealthCheckSummary', 'DataIntegrityReports');

-- Check indexes exist
SELECT * FROM sys.indexes 
WHERE object_id IN (
    OBJECT_ID('HealthCheckReports'),
    OBJECT_ID('HealthCheckSummary'),
    OBJECT_ID('DataIntegrityReports'),
    OBJECT_ID('Plan'),
    OBJECT_ID('Label'),
    OBJECT_ID('Carton'),
    OBJECT_ID('LabelDetail')
);

-- Check Plan.Status column exists
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Plan' AND COLUMN_NAME = 'Status';
```

### Step 5: Update Existing Plans

The migration script automatically updates existing Plans with Status:
- Plans with all items packed → Status = 'Packed'
- Other plans → Status = 'InProgress'

Verify the update:
```sql
SELECT Status, COUNT(*) as Count
FROM Plan
GROUP BY Status;
```

## Application Deployment

### Step 1: Build Application

1. Open solution in Visual Studio
2. Set configuration to **Release**
3. Build solution (Ctrl+Shift+B)
4. Verify no build errors

### Step 2: Copy Files

Copy the following new files to your deployment:

**Models:**
- `Models/HealthCheck/HealthCheckReport.cs`
- `Models/HealthCheck/HealthCheckSummary.cs`
- `Models/HealthCheck/DataIntegrityReport.cs`

**Services:**
- `Services/HealthCheck/Interfaces/IHealthCheckService.cs`
- `Services/HealthCheck/HealthCheckService.cs`
- `Services/HealthCheck/HealthCheckJob.cs`
- `Services/HealthCheck/DataIntegrity/IDataIntegrityHealthCheckService.cs`
- `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckService.cs`
- `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckJob.cs`

**Controllers:**
- `Areas/Admin/Controllers/HealthCheckController.cs`

**Services (Admin):**
- `Areas/Admin/Services/Interfaces/IHealthCheckUIService.cs`
- `Areas/Admin/Services/HealthCheckUIService.cs`

**DTOs:**
- `Areas/Admin/Dto/HealthCheck/HealthCheckReportDto.cs`
- `Areas/Admin/Dto/HealthCheck/HealthCheckSummaryDto.cs`
- `Areas/Admin/Dto/HealthCheck/DataIntegrityIssueDto.cs`
- `Areas/Admin/Dto/HealthCheck/HealthCheckDashboardDto.cs`

**Views:**
- `Areas/Admin/Views/HealthCheck/Index.cshtml`
- `Areas/Admin/Views/HealthCheck/DataIntegrity.cshtml`

**Configuration:**
- `App_Start/HealthCheckConfig.cs`

**Updated Files:**
- `Global.asax.cs` (added HealthCheckConfig.Start() and Stop())
- `Models/Plan/Plan.cs` (added Status property)
- `Corno.Web.csproj` (added all new files)

### Step 3: Update Web.config (if needed)

Verify connection string is correct:
```xml
<connectionStrings>
  <add name="CornoContext" connectionString="..." />
</connectionStrings>
```

### Step 4: Deploy to IIS

1. Stop IIS application pool
2. Copy files to IIS directory
3. Start IIS application pool
4. Verify application starts without errors

### Step 5: Verify Dependencies

Ensure the following NuGet packages are installed:
- Quartz (3.13.1 or compatible)
- EntityFramework (6.5.1 or higher)
- Autofac (8.4.0 or compatible)
- Newtonsoft.Json (latest)

## Configuration

### Scheduler Configuration

The scheduler is automatically started in `Global.asax.cs`:

```csharp
protected void Application_Start(object sender, EventArgs e)
{
    // ... other initialization ...
    HealthCheckConfig.Start();
}
```

**Default Schedule:**
- Health Check Job: Every 1 hour
- Data Integrity Check Job: Every 6 hours

To modify the schedule, edit `App_Start/HealthCheckConfig.cs`:

```csharp
// Change health check to every 30 minutes
.WithSimpleSchedule(x => x
    .WithIntervalInMinutes(30)
    .RepeatForever())
```

### Data Integrity Configuration

Edit constants in `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckService.cs`:

```csharp
private const int DaysToCheck = 90;  // Change to check more/fewer days
private const int BatchSize = 50;    // Change batch size
```

## Verification

### Step 1: Check Application Starts

1. Check application logs for errors
2. Verify no exceptions during startup
3. Check Quartz scheduler started successfully

### Step 2: Verify Database Connection

1. Navigate to Health Check Dashboard
2. Click "Run Health Check Now"
3. Verify "Database Connectivity" check passes

### Step 3: Verify Scheduled Jobs

1. Wait for scheduled job to run (or trigger manually)
2. Check `HealthCheckReports` table for new entries
3. Check `HealthCheckSummary` table for new summary

### Step 4: Verify Data Integrity Checks

1. Navigate to Data Integrity Issues page
2. Click "Run Data Integrity Check"
3. Wait for check to complete
4. Verify issues are displayed in grid

### Step 5: Test Auto-Fix

1. Select a fixable issue
2. Click "Auto-Fix Selected Issues"
3. Verify issue status changes to "Fixed"
4. Verify data is corrected in database

## Rollback Plan

### Database Rollback

If you need to rollback the database changes:

```sql
-- Drop tables (if needed)
DROP TABLE IF EXISTS DataIntegrityReports;
DROP TABLE IF EXISTS HealthCheckSummary;
DROP TABLE IF EXISTS HealthCheckReports;

-- Drop indexes
DROP INDEX IF EXISTS IX_Plan_Status_DueDate ON Plan;
DROP INDEX IF EXISTS IX_Plan_WarehouseOrderNo_Status ON Plan;
DROP INDEX IF EXISTS IX_Label_WarehouseOrderNo_LabelDate ON Label;
DROP INDEX IF EXISTS IX_Carton_WarehouseOrderNo_PackingDate ON Carton;
DROP INDEX IF EXISTS IX_LabelDetail_LabelId_Status_ScanDate ON LabelDetail;

-- Remove Status column from Plan (optional)
ALTER TABLE Plan DROP COLUMN IF EXISTS Status;

-- Restore from backup if needed
RESTORE DATABASE [YourDatabaseName] 
FROM DISK = 'C:\Backups\YourDatabaseName_PreHealthCheck.bak'
WITH REPLACE;
```

### Application Rollback

1. Stop IIS application pool
2. Restore previous version of files
3. Remove HealthCheckConfig.Start() and Stop() from Global.asax.cs
4. Start IIS application pool

## Troubleshooting

### Issue: Scheduler Not Starting

**Symptoms**: No health check reports being created

**Solution**:
1. Check application logs for Quartz errors
2. Verify Quartz.NET package is installed
3. Check Global.asax.cs has HealthCheckConfig.Start()
4. Verify application has necessary permissions

### Issue: Database Migration Fails

**Symptoms**: Migration script errors

**Solution**:
1. Check database user has necessary permissions
2. Verify database name is correct
3. Check for existing tables/indexes
4. Review error messages in SQL output

### Issue: Health Checks Not Running

**Symptoms**: No reports in database

**Solution**:
1. Check scheduler is started
2. Verify job is scheduled correctly
3. Check application logs for errors
4. Manually trigger health check to test

### Issue: Performance Issues

**Symptoms**: Checks taking too long

**Solution**:
1. Reduce DaysToCheck constant
2. Increase BatchSize constant
3. Verify indexes are created
4. Check database performance

## Post-Deployment Checklist

- [ ] Database migration completed successfully
- [ ] All tables created
- [ ] All indexes created
- [ ] Plan.Status column added
- [ ] Application builds without errors
- [ ] Application starts without errors
- [ ] Scheduler starts successfully
- [ ] Health check runs successfully
- [ ] Data integrity check runs successfully
- [ ] Dashboard displays correctly
- [ ] Reports grid displays data
- [ ] Auto-fix functionality works
- [ ] Application logs show no errors

## Support

For issues or questions:
1. Review application logs
2. Check Code Implementation Documentation
3. Review User Manual
4. Contact development team

