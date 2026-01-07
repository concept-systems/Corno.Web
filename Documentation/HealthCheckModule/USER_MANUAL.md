# Health Check Module - User Manual

## Table of Contents
1. [Introduction](#introduction)
2. [Accessing the Health Check Module](#accessing-the-health-check-module)
3. [System Health Check Dashboard](#system-health-check-dashboard)
4. [Data Integrity Issues](#data-integrity-issues)
5. [Running Manual Checks](#running-manual-checks)
6. [Auto-Fixing Issues](#auto-fixing-issues)
7. [Understanding Status Indicators](#understanding-status-indicators)
8. [Troubleshooting](#troubleshooting)

## Introduction

The Health Check Module provides comprehensive monitoring and data integrity validation for your application. It helps you:

- Monitor system health (database, memory, disk space, performance)
- Identify data integrity issues
- Automatically fix certain types of issues
- Track health trends over time

## Accessing the Health Check Module

1. Log in to the application
2. Navigate to **Admin** → **Health Check** from the main menu
3. You will see the System Health Check Dashboard

## System Health Check Dashboard

### Overview Cards

The dashboard displays five key metrics:

1. **Overall Status**: Current system health (Healthy, Warning, Critical)
2. **Critical Issues**: Number of critical issues found
3. **Warnings**: Number of warning-level issues
4. **Auto-Fixed**: Number of issues automatically fixed
5. **Last Check**: Timestamp of the last health check

### Action Buttons

- **Run Health Check Now**: Manually triggers a system health check
- **View Data Integrity Issues**: Navigates to the Data Integrity page
- **Refresh**: Refreshes the dashboard data

### Health Check Reports Grid

The grid displays detailed health check reports with the following columns:

- **Check Date**: When the check was performed
- **Check Type**: Type of check (Database Connectivity, Memory Usage, etc.)
- **Status**: Health status (Healthy, Warning, Critical)
- **Message**: Description of the check result
- **Time (ms)**: Execution time in milliseconds
- **Auto-Fixed**: Whether the issue was automatically fixed

### Filtering Reports

You can filter reports by:

1. **From Date**: Start date for the report range
2. **To Date**: End date for the report range
3. **Status**: Filter by Healthy, Warning, or Critical

Click **Filter** to apply the filters.

## Data Integrity Issues

### Accessing Data Integrity Issues

1. From the Health Check Dashboard, click **View Data Integrity Issues**
2. Or navigate directly to **Admin** → **Health Check** → **Data Integrity**

### Data Integrity Issues Grid

The grid displays all data integrity issues with the following columns:

- **Check Date**: When the check was performed
- **Check Type**: Type of integrity check
- **Entity**: Type of entity (Plan, Label, Carton)
- **Identifier**: Unique identifier (WarehouseOrderNo, Barcode, etc.)
- **Issue Type**: Type of issue found
- **Description**: Detailed description of the issue
- **Expected**: Expected value
- **Actual**: Actual value found
- **Can Fix**: Whether the issue can be auto-fixed
- **Status**: Current status (Open, Fixed, Critical)
- **Fixed**: Whether the issue has been fixed

### Filtering Issues

You can filter issues by:

1. **From Date**: Start date for the issue range
2. **To Date**: End date for the issue range
3. **Check Type**: Filter by specific check type
4. **Status**: Filter by status (All, Open, Fixed, Critical)

Click **Filter** to apply the filters.

## Running Manual Checks

### Running System Health Check

1. On the Health Check Dashboard, click **Run Health Check Now**
2. The button will show "Running..." while the check is in progress
3. A message will appear when the check completes
4. The grid will automatically refresh to show the latest results

### Running Data Integrity Check

1. On the Data Integrity page, click **Run Data Integrity Check**
2. The button will show "Running..." while the check is in progress
3. A message will appear when the check completes
4. The grid will automatically refresh to show the latest issues

**Note**: Manual checks may take several minutes depending on the amount of data.

## Auto-Fixing Issues

### Selecting Issues to Fix

1. Navigate to the Data Integrity Issues page
2. Select one or more issues from the grid (checkboxes in the first column)
3. Only select issues where "Can Fix" is "Yes"

### Running Auto-Fix

1. After selecting fixable issues, click **Auto-Fix Selected Issues**
2. A confirmation dialog will appear
3. Click **OK** to proceed
4. The system will attempt to fix the selected issues
5. A message will show how many issues were fixed
6. The grid will refresh to show updated status

### What Gets Auto-Fixed

The system can automatically fix:

- **Plan Quantity Mismatches**: Recalculates quantities from LabelDetails
- **Plan PackQuantity Mismatches**: Updates from CartonDetails
- **Non-Zero Quantities Without Labels**: Resets quantities to 0

### What Cannot Be Auto-Fixed

The following issues require manual intervention:

- **Carton Barcode Inconsistencies**: Barcodes from different orders in the same carton
- **Label Sequence Violations**: Incorrect status sequence in LabelDetails

## Understanding Status Indicators

### Health Status Colors

- **Green (Healthy)**: System is operating normally
- **Yellow (Warning)**: Minor issues detected, monitoring recommended
- **Red (Critical)**: Serious issues detected, immediate attention required

### Status Badges

- **Healthy**: All checks passed
- **Warning**: Some checks have warnings
- **Critical**: One or more critical issues found

### Issue Status

- **Open**: Issue has been identified but not yet fixed
- **Fixed**: Issue has been resolved
- **CannotFix**: Issue cannot be automatically fixed

## Troubleshooting

### Health Check Not Running

**Problem**: Health checks are not running automatically.

**Solution**:
1. Verify the application is running
2. Check application logs for errors
3. Ensure Quartz.NET scheduler is started (check Global.asax.cs)

### Data Integrity Check Taking Too Long

**Problem**: Data integrity check is taking a very long time.

**Solution**:
1. This is normal for large datasets
2. The check processes plans in batches
3. Only plans from the last 90 days are checked
4. Plans marked as "Packed" are skipped

### Auto-Fix Not Working

**Problem**: Auto-fix is not fixing issues.

**Solution**:
1. Verify the issue is marked as "Can Fix: Yes"
2. Check if the issue type supports auto-fix
3. Review application logs for errors
4. Some issues require manual intervention

### Missing Data in Reports

**Problem**: Reports are not showing expected data.

**Solution**:
1. Check the date filters
2. Verify the status filter is set correctly
3. Ensure health checks have been run
4. Check if data exists in the database

### Performance Issues

**Problem**: Dashboard or grid is loading slowly.

**Solution**:
1. Reduce the date range in filters
2. Use status filters to narrow results
3. Check database indexes are created
4. Review database query performance

## Best Practices

1. **Regular Monitoring**: Check the dashboard daily
2. **Review Critical Issues**: Address critical issues immediately
3. **Monitor Trends**: Watch for increasing warning counts
4. **Auto-Fix Regularly**: Run auto-fix for fixable issues weekly
5. **Keep Data Clean**: Address data integrity issues promptly

## Scheduled Checks

The system automatically runs checks:

- **System Health Check**: Every 1 hour
- **Data Integrity Check**: Every 6 hours

You can also run checks manually at any time.

## Exporting Reports

You can export health check reports and data integrity issues:

1. Use the grid toolbar export buttons
2. Export to Excel or PDF
3. Reports include all filtered data

## Getting Help

If you encounter issues:

1. Check the application logs
2. Review the Code Implementation Documentation
3. Contact your system administrator
4. Review the Troubleshooting section above

