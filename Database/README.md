# Database Migration Script

## Migration_LoginModule.sql

This script creates and enhances database tables for the Login Module.

### What This Script Does

1. **Enhances AspNetLoginHistories Table**
   - Adds `SessionId` column
   - Adds `LastActivityTime` column
   - Adds `UserAgent` column
   - Adds `DeviceInfo` column
   - Adds `IsActive` column
   - Creates indexes for performance

2. **Creates New Tables**
   - `Menus` - Menu structure
   - `PermissionTypes` - Permission type definitions
   - `AccessControls` - Access control permissions
   - `AuditLogs` - Audit trail
   - `PasswordResetTokens` - Password reset tokens

3. **Creates Indexes**
   - Performance indexes on all tables
   - Filtered indexes where applicable

### Before Running

1. **Backup Your Database**
   ```sql
   BACKUP DATABASE [YourDatabaseName] 
   TO DISK = 'C:\Backups\YourDatabaseName_BeforeLoginModule.bak'
   ```

2. **Check Database Name**
   - Open the script
   - Find line 6: `USE [YourDatabaseName]`
   - Replace `[YourDatabaseName]` with your actual database name

3. **Verify Connection**
   - Ensure you have proper permissions
   - Test connection to database

### Running the Script

#### Option 1: SQL Server Management Studio
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Open `Migration_LoginModule.sql`
4. Replace `[YourDatabaseName]` with actual database name
5. Execute the script (F5)

#### Option 2: Command Line
```bash
sqlcmd -S ServerName -d DatabaseName -i Migration_LoginModule.sql
```

#### Option 3: PowerShell
```powershell
Invoke-Sqlcmd -ServerInstance "ServerName" -Database "DatabaseName" -InputFile "Migration_LoginModule.sql"
```

### Verification

After running the script, verify all tables exist:

```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN (
    'Menus', 
    'PermissionTypes', 
    'AccessControls', 
    'AuditLogs', 
    'PasswordResetTokens'
)
ORDER BY TABLE_NAME;
```

Verify AspNetLoginHistories enhancements:

```sql
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetLoginHistories'
AND COLUMN_NAME IN ('SessionId', 'LastActivityTime', 'UserAgent', 'DeviceInfo', 'IsActive')
ORDER BY COLUMN_NAME;
```

### Rollback (If Needed)

If you need to rollback:

```sql
-- Drop new tables
DROP TABLE IF EXISTS PasswordResetTokens;
DROP TABLE IF EXISTS AuditLogs;
DROP TABLE IF EXISTS AccessControls;
DROP TABLE IF EXISTS PermissionTypes;
DROP TABLE IF EXISTS Menus;

-- Remove columns from AspNetLoginHistories (if needed)
ALTER TABLE AspNetLoginHistories
DROP COLUMN IF EXISTS IsActive;
ALTER TABLE AspNetLoginHistories
DROP COLUMN IF EXISTS DeviceInfo;
ALTER TABLE AspNetLoginHistories
DROP COLUMN IF EXISTS UserAgent;
ALTER TABLE AspNetLoginHistories
DROP COLUMN IF EXISTS LastActivityTime;
ALTER TABLE AspNetLoginHistories
DROP COLUMN IF EXISTS SessionId;
```

### Troubleshooting

**Error: Table already exists**
- Script checks for existing tables and skips creation
- This is safe - script is idempotent

**Error: Column already exists**
- Script checks for existing columns
- This is safe - script is idempotent

**Error: Permission denied**
- Ensure you have ALTER TABLE and CREATE TABLE permissions
- Run as database owner or sysadmin

**Error: Foreign key constraint**
- Ensure AspNetUsers and AspNetRoles tables exist
- These are part of ASP.NET Identity

### Production Deployment

For production deployment:

1. **Test on Development First**
   - Run script on development database
   - Test all functionality
   - Fix any issues

2. **Schedule Maintenance Window**
   - Plan for downtime if needed
   - Notify users

3. **Backup Before Migration**
   - Full database backup
   - Transaction log backup

4. **Run Script**
   - Execute during maintenance window
   - Monitor for errors

5. **Verify**
   - Run verification queries
   - Test application functionality

6. **Monitor**
   - Check application logs
   - Monitor performance

### Notes

- Script is **idempotent** - safe to run multiple times
- Script uses `IF NOT EXISTS` checks
- No data loss - only adds columns/tables
- Existing data is preserved

---

**Last Updated**: 2024-01-15  
**Version**: 1.0

