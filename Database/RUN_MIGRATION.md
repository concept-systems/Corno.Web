# How to Run Database Migration

## Quick Start

### Step 1: Identify Your Database Name

From your `Web.config`, your current database is:
**Database Name**: `Godrej.New.Kitchen`

### Step 2: Update Migration Script

1. Open `Database/Migration_LoginModule.sql`
2. Find line 6: `USE [YourDatabaseName]`
3. Replace `[YourDatabaseName]` with: `Godrej.New.Kitchen`

### Step 3: Run the Script

#### Option A: SQL Server Management Studio (Recommended)
1. Open SQL Server Management Studio
2. Connect to: `CONCEPT-LPT007` (from your connection string)
3. Open `Database/Migration_LoginModule.sql`
4. Verify line 6 shows: `USE [Godrej.New.Kitchen]`
5. Execute the script (F5 or Execute button)
6. Check output for success messages

#### Option B: Command Line
```bash
sqlcmd -S CONCEPT-LPT007 -d "Godrej.New.Kitchen" -U admin -P "universal1!" -i "Database\Migration_LoginModule.sql"
```

### Step 4: Verify

Run this query to verify tables were created:

```sql
USE [Godrej.New.Kitchen]
GO

-- Check new tables
SELECT 'Menus' AS TableName, COUNT(*) AS RecordCount FROM Menus
UNION ALL
SELECT 'PermissionTypes', COUNT(*) FROM PermissionTypes
UNION ALL
SELECT 'AccessControls', COUNT(*) FROM AccessControls
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs
UNION ALL
SELECT 'PasswordResetTokens', COUNT(*) FROM PasswordResetTokens;

-- Check AspNetLoginHistories enhancements
SELECT TOP 1 
    SessionId, 
    LastActivityTime, 
    UserAgent, 
    DeviceInfo, 
    IsActive 
FROM AspNetLoginHistories;
```

### Expected Results

- **PermissionTypes**: Should have 3 records (Menu, Page, Control)
- **Other tables**: May be empty initially (that's OK)
- **AspNetLoginHistories**: Should have new columns

---

## Important Notes

1. **Backup First**: Always backup before running migration
   ```sql
   BACKUP DATABASE [Godrej.New.Kitchen] 
   TO DISK = 'C:\Backups\Godrej.New.Kitchen_PreLoginModule.bak'
   ```

2. **Script is Safe**: The script checks for existing tables/columns and won't duplicate them

3. **No Data Loss**: The script only adds new tables and columns - no data is deleted

---

## Troubleshooting

**Error: Database does not exist**
- Verify database name in connection string
- Check you're connected to correct server

**Error: Permission denied**
- Ensure you're logged in as user with ALTER TABLE and CREATE TABLE permissions
- User `admin` should have these permissions

**Error: Table already exists**
- This is OK - script checks and skips existing tables
- Check if tables were created in a previous run

---

**Last Updated**: 2024-01-15

