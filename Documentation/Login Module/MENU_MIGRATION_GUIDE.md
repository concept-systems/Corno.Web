# Menu Migration Guide

## Overview

This guide explains how to migrate your existing XML sitemap menus to the database-driven menu system.

## Migration Process

### Step 1: Run the Migration

1. **Navigate to Menu Management**
   - Go to: `/Admin/Menu/Index`
   - You must be logged in as an Administrator

2. **Trigger Migration**
   - Click the "Migrate from XML" button (or call the API endpoint)
   - The migration will:
     - Read the XML sitemap from `Project.MenuXml` (for project "Active")
     - Parse all menu nodes recursively
     - Create/update menu records in the `Menus` table
     - Preserve all attributes (controller, action, area, icon, roles, etc.)

3. **API Endpoint** (if calling directly):
   ```
   POST /Admin/Menu/MigrateFromXml
   Parameters:
   - projectName (optional, default: "Active")
   ```

### Step 2: Verify Migration

1. **Check Menu Table**
   - Go to: `/Admin/Menu/Index`
   - You should see all your menus in a tree structure
   - Verify that all menu items from XML are present

2. **Check Menu Structure**
   - Verify parent-child relationships are correct
   - Check that icons, controllers, actions, and areas are preserved

### Step 3: Test Menu Display

1. **Login and Navigate**
   - Login to the application
   - Go to the home page
   - The menu should now load from the database instead of XML

2. **Verify Permissions**
   - Menus should be filtered based on user permissions
   - Only menus you have access to should be visible

## Migration Details

### What Gets Migrated

- **Menu Name**: From `name` or `title` attribute
- **Display Name**: From `title` attribute
- **Controller**: From `controller` attribute
- **Action**: From `action` attribute (defaults to "Index")
- **Area**: From `area` attribute
- **Icon**: From `icon` attribute
- **Route Values**: Stored as JSON (misctype, reportName, formname, web, windows)
- **Parent-Child Relationships**: Preserved from XML hierarchy
- **Display Order**: Auto-generated based on XML order

### What Doesn't Get Migrated

- **Roles**: Roles are stored in the Description field for reference, but actual permissions need to be set up separately in Access Control
- **Legacy Role-Based Filtering**: The old XML role-based filtering is replaced with the new permission system

## Post-Migration Steps

### 1. Set Up Permissions

After migration, you need to configure permissions:

1. **Go to Access Control**: `/Admin/AccessControl/Index`
2. **Assign Menu Permissions**:
   - Select a role or user
   - Check/uncheck menu access
   - Set page-level permissions (Index, Create, Edit, etc.)
   - Set control-level permissions (buttons, etc.)

### 2. Clean Up (Optional)

Once you've verified everything works:

1. **Backup the XML**: Keep a backup of `Project.MenuXml` for reference
2. **Remove XML Loading**: The `HomeController` now uses database menus automatically
3. **Update Documentation**: Update any documentation that references XML sitemap

## Troubleshooting

### No Menus Appear After Migration

1. **Check Menu Table**: Verify menus were created in the database
2. **Check Permissions**: Ensure you have menu access permissions set
3. **Check IsActive/IsVisible**: Verify menus are marked as active and visible

### Menus Appear But Not Filtered

1. **Set Up Permissions**: Go to Access Control and assign menu permissions
2. **Check Permission Service**: Verify `IPermissionService` is working correctly

### Migration Fails

1. **Check XML Format**: Ensure `Project.MenuXml` is valid XML
2. **Check Logs**: Review error messages in the migration result
3. **Check Database**: Ensure Menu table exists and is accessible

## Rollback

If you need to rollback:

1. **Restore XML**: The original XML is still in `Project.MenuXml`
2. **Revert HomeController**: Change `HomeController.Index()` back to XML loading
3. **Clear Menu Table**: Optionally clear migrated menus from database

## Notes

- The migration is **idempotent**: Running it multiple times will update existing menus
- Menus are matched by `MenuName` and `MenuPath`
- If a menu already exists, it will be updated instead of created
- The migration preserves the hierarchical structure from XML

