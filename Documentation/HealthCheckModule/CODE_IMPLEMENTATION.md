# Health Check Module - Code Implementation Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Database Schema](#database-schema)
4. [Service Layer](#service-layer)
5. [Controller Layer](#controller-layer)
6. [Scheduled Jobs](#scheduled-jobs)
7. [Configuration](#configuration)
8. [Performance Optimizations](#performance-optimizations)

## Overview

The Health Check Module provides comprehensive monitoring and data integrity checking for the Corno application. It consists of two main components:

1. **System Health Checks**: Monitors application health (database connectivity, memory, disk space, error rates, performance)
2. **Data Integrity Checks**: Validates business logic integrity (Plan quantities, Carton barcodes, Label sequences)

## Architecture

### Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     User Interface Layer                     │
│  ┌──────────────────┐  ┌──────────────────────────────────┐ │
│  │ HealthCheck      │  │ DataIntegrity                    │ │
│  │ Controller       │  │ View                             │ │
│  └────────┬─────────┘  └──────────────────────────────────┘ │
└───────────┼──────────────────────────────────────────────────┘
            │
┌───────────┼──────────────────────────────────────────────────┐
│           │              Service Layer                         │
│  ┌────────▼─────────┐  ┌──────────────────────────────────┐  │
│  │ HealthCheck      │  │ DataIntegrityHealthCheck          │  │
│  │ UIService        │  │ Service                          │  │
│  └────────┬─────────┘  └──────────────────────────────────┘  │
│           │                                                    │
│  ┌────────▼─────────┐  ┌──────────────────────────────────┐  │
│  │ HealthCheck     │  │ DataIntegrityHealthCheck          │  │
│  │ Service         │  │ Service                          │  │
│  └────────┬─────────┘  └──────────────────────────────────┘  │
└───────────┼──────────────────────────────────────────────────┘
            │
┌───────────┼──────────────────────────────────────────────────┐
│           │              Data Access Layer                    │
│  ┌────────▼─────────┐  ┌──────────────────────────────────┐  │
│  │ GenericRepository│  │ GenericRepository                 │  │
│  │ (HealthCheck)    │  │ (DataIntegrity)                   │  │
│  └────────┬─────────┘  └──────────────────────────────────┘  │
└───────────┼──────────────────────────────────────────────────┘
            │
┌───────────┼──────────────────────────────────────────────────┐
│           │              Database                              │
│  ┌────────▼─────────┐  ┌──────────────────────────────────┐  │
│  │ HealthCheck      │  │ DataIntegrityReports              │  │
│  │ Reports          │  │                                  │  │
│  │ HealthCheck      │  │                                  │  │
│  │ Summary          │  │                                  │  │
│  └──────────────────┘  └──────────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│              Background Jobs (Quartz.NET)                    │
│  ┌──────────────────┐  ┌──────────────────────────────────┐ │
│  │ HealthCheckJob   │  │ DataIntegrityHealthCheckJob       │ │
│  │ (Every 1 hour)   │  │ (Every 6 hours)                   │ │
│  └──────────────────┘  └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Database Schema

### Tables

#### HealthCheckReports
Stores individual health check results.

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key |
| CheckDate | DATETIME | When the check was performed |
| CheckType | NVARCHAR(50) | Type of check (Database, Memory, etc.) |
| Status | NVARCHAR(20) | Healthy, Warning, Critical |
| Message | NVARCHAR(MAX) | Human-readable message |
| Details | NVARCHAR(MAX) | JSON with detailed information |
| ExecutionTimeMs | INT | How long the check took |
| AutoFixed | BIT | Whether the issue was auto-fixed |
| CreatedBy | NVARCHAR(100) | System or user who created |
| CreatedDate | DATETIME | Creation timestamp |

#### HealthCheckSummary
Stores aggregated summary of health checks.

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key |
| ReportDate | DATETIME | Date of the summary |
| OverallStatus | NVARCHAR(20) | Overall health status |
| TotalChecks | INT | Total number of checks |
| HealthyChecks | INT | Number of healthy checks |
| WarningChecks | INT | Number of warning checks |
| CriticalChecks | INT | Number of critical checks |
| SummaryDetails | NVARCHAR(MAX) | JSON with summary details |

#### DataIntegrityReports
Stores data integrity check results.

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key |
| CheckDate | DATETIME | When the check was performed |
| CheckType | NVARCHAR(50) | Type of check |
| Status | NVARCHAR(20) | Healthy, Warning, Critical |
| Message | NVARCHAR(MAX) | Human-readable message |
| Details | NVARCHAR(MAX) | JSON with issues found |
| ExecutionTimeMs | INT | How long the check took |
| RecordsChecked | INT | Number of records checked |
| IssuesFound | INT | Number of issues found |
| RecordsFixed | INT | Number of records fixed |
| AutoFixed | BIT | Whether issues were auto-fixed |

### Indexes

Performance indexes have been created on:
- `HealthCheckReports`: CheckDate, Status, CheckType
- `HealthCheckSummary`: ReportDate, OverallStatus
- `DataIntegrityReports`: CheckDate, Status, CheckType
- `Plan`: Status, DueDate (composite index)
- `Label`: WarehouseOrderNo, LabelDate (composite index)
- `Carton`: WarehouseOrderNo, PackingDate (composite index)
- `LabelDetail`: LabelId, Status, ScanDate (composite index)

## Service Layer

### IHealthCheckService

**Location**: `Services/HealthCheck/Interfaces/IHealthCheckService.cs`

**Methods**:
- `RunAllChecksAsync()`: Runs all health checks
- `CheckDatabaseConnectivityAsync()`: Checks database connection
- `CheckConnectionPoolAsync()`: Monitors connection pool usage
- `CheckDiskSpaceAsync()`: Checks available disk space
- `CheckMemoryUsageAsync()`: Monitors memory usage
- `CheckErrorRateAsync()`: Checks error rate in logs
- `CheckApplicationPerformanceAsync()`: Tests application performance
- `SaveHealthReportAsync()`: Saves check results to database
- `GetLatestSummaryAsync()`: Gets latest summary
- `GetRecentReportsAsync()`: Gets recent reports

### IDataIntegrityHealthCheckService

**Location**: `Services/HealthCheck/DataIntegrity/IDataIntegrityHealthCheckService.cs`

**Methods**:
- `RunAllDataIntegrityChecksAsync()`: Runs all data integrity checks
- `CheckPlanQuantitiesAsync()`: Validates Plan quantities against Labels
- `CheckPlanPackQuantityFromCartonsAsync()`: Validates PackQuantity from Cartons
- `CheckCartonBarcodeConsistencyAsync()`: Validates Carton barcode consistency
- `CheckLabelDetailSequenceAsync()`: Validates Label status sequence
- `AutoFixDataIntegrityIssuesAsync()`: Attempts to auto-fix issues

### IHealthCheckUIService

**Location**: `Areas/Admin/Services/Interfaces/IHealthCheckUIService.cs`

**Methods**:
- `GetDashboardDataAsync()`: Gets dashboard data
- `GetHealthCheckReportsAsync()`: Gets filtered health check reports
- `GetDataIntegrityIssuesAsync()`: Gets filtered data integrity issues
- `GetLatestSummaryAsync()`: Gets latest summary
- `RunManualHealthCheckAsync()`: Triggers manual health check
- `RunManualDataIntegrityCheckAsync()`: Triggers manual data integrity check
- `AutoFixDataIntegrityIssuesAsync()`: Auto-fixes selected issues

## Controller Layer

### HealthCheckController

**Location**: `Areas/Admin/Controllers/HealthCheckController.cs`

**Actions**:
- `Index()`: Main dashboard view
- `DataIntegrity()`: Data integrity issues view
- `GetHealthCheckReports()`: Returns filtered health check reports (Kendo Grid)
- `GetDataIntegrityIssues()`: Returns filtered data integrity issues (Kendo Grid)
- `RunManualHealthCheck()`: Triggers manual health check
- `RunManualDataIntegrityCheck()`: Triggers manual data integrity check
- `AutoFixIssues()`: Auto-fixes selected issues
- `GetLatestSummary()`: Returns latest summary

## Scheduled Jobs

### HealthCheckJob

**Location**: `Services/HealthCheck/HealthCheckJob.cs`

**Schedule**: Every 1 hour

**Functionality**:
- Runs all system health checks
- Saves results to database
- Creates summary report

### DataIntegrityHealthCheckJob

**Location**: `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckJob.cs`

**Schedule**: Every 6 hours

**Functionality**:
- Runs all data integrity checks
- Attempts to auto-fix issues where possible
- Saves results to database

## Configuration

### HealthCheckConfig

**Location**: `App_Start/HealthCheckConfig.cs`

**Configuration**:
- Health check job runs every 1 hour
- Data integrity check job runs every 6 hours
- Jobs are started in `Global.asax.cs` Application_Start
- Jobs are stopped in `Global.asax.cs` Application_End

### Data Integrity Configuration

**Location**: `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckService.cs`

**Constants**:
- `DaysToCheck = 90`: Only check plans from last 90 days
- `BatchSize = 50`: Process plans in batches of 50
- `PackedStatus = "Packed"`: Status for completed plans
- `InProgressStatus = "InProgress"`: Status for in-progress plans

## Performance Optimizations

### 1. Date Filtering
- Only processes plans from the last 90 days
- Reduces database load significantly

### 2. Status-Based Skipping
- Plans marked as "Packed" are skipped
- Plans are automatically marked as "Packed" when all items are fully packed
- Reduces processing time for completed plans

### 3. Batch Processing
- Plans are processed in batches of 50
- Prevents memory issues with large datasets
- Allows for better progress tracking

### 4. Database Indexes
- Composite indexes on frequently queried columns
- Improves query performance
- Reduces database load

### 5. AsNoTracking()
- Used where appropriate to reduce memory usage
- Improves query performance

### 6. Efficient LINQ Queries
- Uses `FirstOrDefaultAsync()` instead of loading full collections
- Uses `CountAsync()` for counting
- Groups data efficiently before processing

## Data Integrity Checks

### 1. Plan Quantities Check

**Purpose**: Ensures Plan's PrintQuantity, BendQuantity, and SortQuantity match LabelDetails status.

**Logic**:
1. Get all Plans from last 90 days with status "InProgress" or null
2. Skip Plans marked as "Packed"
3. For each Plan, get associated Labels
4. Calculate expected quantities from LabelDetails by status
5. Compare with PlanItemDetail quantities
6. Report mismatches

**Auto-Fix**: Recalculates quantities using PlanService.UpdateQuantitiesAsync()

### 2. Plan PackQuantity from Cartons

**Purpose**: Ensures Plan's PackQuantity matches total from Cartons.

**Logic**:
1. Get all Plans from last 90 days with status "InProgress" or null
2. Skip Plans marked as "Packed"
3. For each Plan, get associated Cartons
4. Sum quantities from CartonDetails
5. Compare with Plan.PackQuantity and PlanItemDetail.PackQuantity
6. Report mismatches

**Auto-Fix**: Updates PackQuantity from CartonDetails

### 3. Carton Barcode Consistency

**Purpose**: Ensures Cartons only contain barcodes from the same WarehouseOrderNo or ProductionOrderNo.

**Logic**:
1. Get all Cartons from last 90 days
2. For each Carton, get barcodes from CartonDetails
3. Look up Labels by barcodes
4. Verify all Labels have matching WarehouseOrderNo or ProductionOrderNo
5. Report mismatches

**Auto-Fix**: Not available (requires manual intervention)

### 4. Label Detail Sequence

**Purpose**: Ensures Labels have LabelDetails in correct sequence: Active → Printed → Bent → Sorted → Packed.

**Logic**:
1. Get all Labels from last 90 days with LabelDetails
2. Order LabelDetails by ScanDate
3. Verify status sequence is correct
4. Check for missing statuses when later statuses exist
5. Report sequence violations

**Auto-Fix**: Not available (requires manual intervention)

## Auto-Fix Mechanism

The system attempts to auto-fix issues where possible:

1. **Plan Quantity Mismatches**: Recalculates quantities from LabelDetails
2. **Plan PackQuantity Mismatches**: Updates from CartonDetails
3. **Non-Zero Quantities Without Labels**: Resets quantities to 0

Issues that cannot be auto-fixed:
- Carton barcode inconsistencies (requires data correction)
- Label sequence violations (requires manual review)

## Error Handling

- All health checks are wrapped in try-catch blocks
- Errors are logged using LogHandler.LogError()
- Failed checks return Critical status
- Scheduled jobs don't throw exceptions (logged only)

## Dependencies

- **Quartz.NET**: Job scheduling
- **Entity Framework 6**: Data access
- **Autofac**: Dependency injection
- **Kendo UI**: Grid and UI components
- **Newtonsoft.Json**: JSON serialization

## Future Enhancements

1. Email notifications for critical issues
2. Dashboard charts and graphs
3. Historical trend analysis
4. Custom check types
5. Configurable thresholds
6. Export reports to Excel/PDF
7. Real-time monitoring dashboard

