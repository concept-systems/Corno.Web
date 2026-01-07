# Login Module - Deployment Guide

## Overview

This guide provides step-by-step instructions for deploying the Login Module to your production environment.

---

## Prerequisites

- SQL Server 2012 or higher
- ASP.NET MVC 5 application
- Administrator access to database
- Backup of existing database

---

## Deployment Steps

### Step 1: Backup Database

**CRITICAL**: Always backup before making changes.

```sql
BACKUP DATABASE [YourDatabaseName] 
TO DISK = 'C:\Backups\YourDatabaseName_PreLoginModule_' + 
    CONVERT(VARCHAR, GETDATE(), 112) + '.bak'
WITH FORMAT, INIT, NAME = 'Full Backup Before Login Module';
```

### Step 2: Review Migration Script

1. Open `Database/Migration_LoginModule.sql`
2. Review all changes
3. **IMPORTANT**: Replace `[YourDatabaseName]` on line 6 with your actual database name
4. Verify connection string matches your environment

### Step 3: Run Database Migration

#### On Development/Test First

1. Connect to test database
2. Run migration script
3. Verify all tables created
4. Test application functionality
5. Fix any issues

#### On Production

1. Schedule maintenance window
2. Notify users of downtime
3. Run migration script
4. Verify success
5. Test critical functionality

### Step 4: Deploy Application Code

1. **Build Solution**
   ```bash
   msbuild Corno.Web.sln /p:Configuration=Release
   ```

2. **Copy Files to Server**
   - Copy all new files to production server
   - Ensure all DLLs are updated

3. **Update Web.config** (if needed)
   - Verify connection string
   - Check any new app settings

4. **Register Services** (if not auto-registered)
   - Update `Windsor/ServicesModule.cs`
   - Or update DI container configuration

### Step 5: Verify Deployment

#### Database Verification

```sql
-- Check tables
SELECT COUNT(*) FROM Menus;
SELECT COUNT(*) FROM PermissionTypes;
SELECT COUNT(*) FROM AccessControls;
SELECT COUNT(*) FROM AuditLogs;
SELECT COUNT(*) FROM PasswordResetTokens;

-- Check AspNetLoginHistories enhancements
SELECT TOP 1 SessionId, LastActivityTime, UserAgent, DeviceInfo, IsActive 
FROM AspNetLoginHistories;
```

#### Application Verification

1. **Login Test**
   - Login with existing user
   - Verify session is tracked
   - Check `AspNetLoginHistories` table

2. **Menu Test**
   - Navigate to Admin → Menus
   - Create a test menu
   - Verify it appears

3. **Permission Test**
   - Navigate to Admin → Access Rules
   - Assign a permission
   - Verify it works

4. **Audit Log Test**
   - Perform an action
   - Check Audit Log
   - Verify entry created

### Step 6: Create Initial Data

#### Create Default Menus

1. Login as administrator
2. Navigate to Admin → Menus
3. Create menu structure:
   ```
   Dashboard
   ├── User Management
   ├── Role Management
   ├── Menu Management
   ├── Access Rules
   ├── Audit Log
   └── My Profile
   ```

#### Create Default Roles (if needed)

1. Navigate to Admin → Roles
2. Create roles:
   - Administrator
   - Manager
   - User
   - Viewer

#### Assign Permissions

1. Navigate to Admin → Access Rules
2. Select "Administrator" role
3. Grant all permissions
4. Save

### Step 7: Post-Deployment

1. **Monitor Logs**
   - Check application logs
   - Check database logs
   - Monitor for errors

2. **Performance Check**
   - Monitor query performance
   - Check index usage
   - Optimize if needed

3. **User Training**
   - Provide user manual
   - Train administrators
   - Document any customizations

---

## Rollback Plan

If issues occur, follow these steps:

### 1. Stop Application

- Stop IIS application pool
- Or stop web server

### 2. Restore Database

```sql
RESTORE DATABASE [YourDatabaseName] 
FROM DISK = 'C:\Backups\YourDatabaseName_PreLoginModule_YYYYMMDD.bak'
WITH REPLACE;
```

### 3. Revert Code

- Restore previous application version
- Remove new files
- Restart application

---

## Configuration

### Connection String

Ensure `Web.config` has correct connection string:

```xml
<connectionStrings>
  <add name="CornoContext" 
       connectionString="Server=YourServer;Database=YourDatabase;Integrated Security=true;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Service Registration

If using Autofac, ensure services are registered in `Windsor/ServicesModule.cs`:

```csharp
builder.RegisterType<MenuService>().As<IMenuService>().InstancePerLifetimeScope();
builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
builder.RegisterType<AuditLogService>().As<IAuditLogService>().InstancePerLifetimeScope();
builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerLifetimeScope();
```

---

## Troubleshooting

### Issue: Tables Not Created

**Solution**:
1. Check SQL script executed successfully
2. Verify database name is correct
3. Check user permissions
4. Review error messages

### Issue: Application Errors After Deployment

**Solution**:
1. Check service registrations
2. Verify connection string
3. Check application logs
4. Verify all DLLs deployed

### Issue: Permissions Not Working

**Solution**:
1. Verify `AccessControls` table has data
2. Check user roles assigned
3. Verify permission service registered
4. Check application logs

### Issue: Sessions Not Tracking

**Solution**:
1. Verify `AspNetLoginHistories` table has new columns
2. Check `SessionValidationAttribute` is active
3. Verify session state enabled
4. Check application logs

---

## Maintenance

### Regular Tasks

1. **Review Audit Logs**
   - Weekly review
   - Check for suspicious activity
   - Archive old logs

2. **Cleanup Old Data**
   - Remove expired password reset tokens
   - Archive old audit logs
   - Clean inactive sessions

3. **Performance Monitoring**
   - Monitor query performance
   - Check index usage
   - Optimize as needed

### Database Maintenance Scripts

```sql
-- Clean expired password reset tokens (older than 7 days)
DELETE FROM PasswordResetTokens 
WHERE ExpiryDate < DATEADD(DAY, -7, GETDATE()) 
   OR (IsUsed = 1 AND UsedDate < DATEADD(DAY, -7, GETDATE()));

-- Archive old audit logs (older than 1 year)
-- (Create archive table first)
INSERT INTO AuditLogs_Archive
SELECT * FROM AuditLogs 
WHERE Timestamp < DATEADD(YEAR, -1, GETDATE());

DELETE FROM AuditLogs 
WHERE Timestamp < DATEADD(YEAR, -1, GETDATE());
```

---

## Support

For deployment issues:
1. Review this deployment guide
2. Check `CODE_DOCUMENTATION.md` for technical details
3. Review application logs
4. Check database error logs
5. Contact development team

---

**Last Updated**: 2024-01-15  
**Version**: 1.0

