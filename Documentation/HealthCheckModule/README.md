# Health Check Module Documentation

## Overview

The Health Check Module provides comprehensive system monitoring and data integrity validation for the Corno application. It consists of automated background checks, manual check capabilities, and auto-fix functionality for common data issues.

## Documentation Structure

### 1. [Code Implementation Documentation](CODE_IMPLEMENTATION.md)
Complete technical documentation covering:
- Architecture and component design
- Database schema
- Service layer implementation
- Controller and view structure
- Scheduled jobs configuration
- Performance optimizations
- Data integrity check logic

### 2. [User Manual](USER_MANUAL.md)
End-user guide covering:
- Accessing the Health Check Module
- Using the dashboard
- Viewing and filtering reports
- Running manual checks
- Auto-fixing issues
- Understanding status indicators
- Troubleshooting

### 3. [Wireframe Designs](WIREFRAMES.md)
Visual design documentation including:
- Dashboard layout
- Data integrity issues page
- Component descriptions
- Color scheme and typography
- Responsive design considerations

### 4. [Deployment Guide](DEPLOYMENT_GUIDE.md)
Step-by-step deployment instructions:
- Prerequisites
- Database migration steps
- Application deployment
- Configuration
- Verification procedures
- Rollback plan

## Quick Start

### For Developers

1. Review [Code Implementation Documentation](CODE_IMPLEMENTATION.md)
2. Run database migration script: `Database/Migration_HealthCheckModule.sql`
3. Build and deploy application
4. Verify scheduler starts in `Global.asax.cs`

### For Administrators

1. Review [Deployment Guide](DEPLOYMENT_GUIDE.md)
2. Backup database
3. Run migration script
4. Deploy application files
5. Verify functionality

### For End Users

1. Review [User Manual](USER_MANUAL.md)
2. Navigate to Admin → Health Check
3. Monitor dashboard regularly
4. Run manual checks as needed
5. Auto-fix issues when available

## Key Features

### System Health Checks
- Database connectivity monitoring
- Connection pool monitoring
- Disk space monitoring
- Memory usage tracking
- Error rate analysis
- Application performance testing

### Data Integrity Checks
- Plan quantity validation
- PackQuantity validation from Cartons
- Carton barcode consistency
- Label status sequence validation

### Auto-Fix Capabilities
- Automatic quantity recalculation
- PackQuantity updates from Cartons
- Status updates for completed plans

### Performance Optimizations
- Date-based filtering (90 days)
- Status-based skipping (Packed plans)
- Batch processing (50 plans per batch)
- Database indexes for performance

## Scheduled Jobs

- **Health Check Job**: Runs every 1 hour
- **Data Integrity Check Job**: Runs every 6 hours

Both jobs can also be triggered manually from the UI.

## Database Tables

- `HealthCheckReports`: Individual health check results
- `HealthCheckSummary`: Aggregated health check summaries
- `DataIntegrityReports`: Data integrity check results

## Related Files

### Database
- `Database/Migration_HealthCheckModule.sql`: Database migration script

### Models
- `Models/HealthCheck/HealthCheckReport.cs`
- `Models/HealthCheck/HealthCheckSummary.cs`
- `Models/HealthCheck/DataIntegrityReport.cs`
- `Models/Plan/Plan.cs` (updated with Status property)

### Services
- `Services/HealthCheck/Interfaces/IHealthCheckService.cs`
- `Services/HealthCheck/HealthCheckService.cs`
- `Services/HealthCheck/HealthCheckJob.cs`
- `Services/HealthCheck/DataIntegrity/IDataIntegrityHealthCheckService.cs`
- `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckService.cs`
- `Services/HealthCheck/DataIntegrity/DataIntegrityHealthCheckJob.cs`
- `Areas/Admin/Services/Interfaces/IHealthCheckUIService.cs`
- `Areas/Admin/Services/HealthCheckUIService.cs`

### Controllers
- `Areas/Admin/Controllers/HealthCheckController.cs`

### Views
- `Areas/Admin/Views/HealthCheck/Index.cshtml`
- `Areas/Admin/Views/HealthCheck/DataIntegrity.cshtml`

### Configuration
- `App_Start/HealthCheckConfig.cs`
- `Global.asax.cs` (updated)

## Support

For issues or questions:
1. Check application logs
2. Review relevant documentation
3. Contact development team

## Version History

- **v1.0** (2024-12-19): Initial release
  - System health checks
  - Data integrity checks
  - Auto-fix functionality
  - Dashboard and reporting

