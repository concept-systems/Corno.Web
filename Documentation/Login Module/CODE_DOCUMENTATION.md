# Login Module - Code Documentation

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Database Schema](#database-schema)
3. [Code Flow](#code-flow)
4. [Key Components](#key-components)
5. [Permission System](#permission-system)
6. [Session Management](#session-management)
7. [Audit Logging](#audit-logging)

---

## Architecture Overview

The Login Module follows a **3-tier architecture**:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (Controllers, Views, Helpers)          │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          Business Logic Layer           │
│  (Services, Interfaces)                 │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          Data Access Layer              │
│  (Repositories, DbContext, Models)      │
└─────────────────────────────────────────┘
```

### Technology Stack
- **Framework**: ASP.NET MVC 5
- **ORM**: Entity Framework 6
- **Database**: SQL Server
- **Authentication**: ASP.NET Identity
- **Dependency Injection**: Autofac
- **UI Framework**: Kendo UI, Bootstrap

---

## Database Schema

### Core Tables

#### 1. AspNetLoginHistories (Enhanced)
**Purpose**: Tracks login history and active sessions

**Key Fields**:
- `SessionId`: ASP.NET Session ID
- `LastActivityTime`: Last user activity
- `UserAgent`: Browser/device info
- `DeviceInfo`: Device type, OS
- `IsActive`: Active session flag

**Flow**:
```
Login → Create Record (IsActive=true, LogoutTime=null)
Activity → Update LastActivityTime
Logout → Update LogoutTime, Set IsActive=false
```

#### 2. Menus
**Purpose**: Stores menu structure

**Hierarchy**:
```
Root Menu (ParentMenuId = null)
  └─ Child Menu 1 (ParentMenuId = Root.Id)
      └─ Grandchild Menu (ParentMenuId = Child1.Id)
```

**Key Fields**:
- `MenuName`: Unique identifier
- `MenuPath`: Full path from root
- `ParentMenuId`: For hierarchy
- `ControllerName`, `ActionName`, `Area`: For navigation

#### 3. PermissionTypes
**Purpose**: Defines permission levels

**Types**:
1. **Menu**: Menu visibility
2. **Page**: Page/action access
3. **Control**: Button/control access

#### 4. AccessControls
**Purpose**: Stores permissions

**Structure**:
- `PermissionTypeId`: Menu/Page/Control
- `MenuId`: Reference to menu (for Menu/Page)
- `ControllerName`, `ActionName`: For Page/Control
- `ControlId`: For Control level
- `UserId` OR `RoleId`: Assignment (mutually exclusive)
- `IsAllowed`: Allow/Deny flag

#### 5. AuditLogs
**Purpose**: Audit trail of all actions

**Key Fields**:
- `Action`: Login, Create, Edit, Delete, etc.
- `EntityType`: User, Role, Menu, etc.
- `EntityId`: ID of affected entity
- `Details`: Additional information (JSON)

#### 6. PasswordResetTokens
**Purpose**: Password reset functionality

**Flow**:
```
ForgotPassword → Generate Token → Store in DB → Send Email
ResetPassword → Validate Token → Update Password → Mark Token as Used
```

---

## Code Flow

### 1. Login Flow

```
User enters credentials
    ↓
AccountController.Login (POST)
    ↓
Validate credentials (UserManager.FindAsync)
    ↓
Check active sessions (UserService.HasActiveSessionAsync)
    ↓
Invalidate previous sessions (if any)
    ↓
Update login history (UserService.UpdateLoginHistoryAsync)
    - Create AspNetLoginHistory record
    - Set SessionId, UserAgent, DeviceInfo
    - Set IsActive = true
    ↓
Sign in user (AuthenticationManager.SignIn)
    ↓
Redirect to Home
```

### 2. Menu Rendering Flow

```
User requests page
    ↓
HomeController.Index
    ↓
MenuService.GetUserMenusAsync(userId)
    ↓
PermissionService.HasMenuAccessAsync (for each menu)
    - Check user-specific permissions
    - Check role-based permissions
    - Return true/false
    ↓
Filter menus based on permissions
    ↓
Render menu tree (MenuHelper.RenderMenuTree)
    ↓
Display to user
```

### 3. Permission Check Flow

```
User accesses page/control
    ↓
PageAuthorizeAttribute.AuthorizeCore (for pages)
OR
ControlPermissionHelper.HasControlAccess (for controls)
    ↓
PermissionService.HasPageAccessAsync / HasControlAccessAsync
    ↓
Check hierarchy:
    1. Menu-level permission (if applicable)
    2. Page-level permission
    3. Control-level permission (if applicable)
    ↓
Priority: User-level > Role-level > Default (Deny)
    ↓
Return Allow/Deny
```

### 4. Access Control Assignment Flow

```
Admin opens Access Rules page
    ↓
AccessControlController.Index
    ↓
Load menu tree (MenuService.GetMenuTreeAsync)
    ↓
Load existing permissions (PermissionService.GetMenuPermissionsAsync)
    ↓
Display in UI (Menu Tree + Permission Grid)
    ↓
Admin assigns permissions
    ↓
Save permissions (PermissionService.SaveMenuPermissionsAsync)
    ↓
Create/Update AccessControl records
    ↓
Audit log entry (AuditLogService.LogAsync)
```

---

## Key Components

### Controllers

#### 1. MenuController
**Location**: `Areas/Admin/Controllers/MenuController.cs`

**Actions**:
- `Index`: Display menu tree
- `Create`: Create new menu
- `Edit`: Edit existing menu
- `Delete`: Delete menu
- `GetMenuTree`: Get menu tree (AJAX)
- `MoveMenu`: Reorder menu

**Flow**:
```
Create/Edit Menu
    ↓
Validate MenuDto
    ↓
MenuService.CreateAsync / UpdateAsync
    ↓
Save to database
    ↓
Audit log entry
    ↓
Redirect to Index
```

#### 2. AccessControlController
**Location**: `Areas/Admin/Controllers/AccessControlController.cs`

**Actions**:
- `Index`: Display access rules UI
- `GetMenuTree`: Get menu tree for permissions
- `GetMenuPermissions`: Get menu permissions
- `SaveMenuPermissions`: Save menu permissions
- `GetPagePermissions`: Get page permissions
- `SavePagePermissions`: Save page permissions
- `GetControlPermissions`: Get control permissions
- `SaveControlPermissions`: Save control permissions

#### 3. AuditLogController
**Location**: `Areas/Admin/Controllers/AuditLogController.cs`

**Actions**:
- `Index`: Display audit log
- `GetLogs`: Get filtered audit logs (AJAX)
- `ExportToExcel`: Export logs to Excel

#### 4. DashboardController
**Location**: `Areas/Admin/Controllers/DashboardController.cs`

**Actions**:
- `Index`: Display dashboard
- `GetDashboardData`: Get dashboard statistics (AJAX)

#### 5. ProfileController
**Location**: `Areas/Admin/Controllers/ProfileController.cs`

**Actions**:
- `Index`: Display user profile
- `UpdateProfile`: Update profile information
- `ChangePassword`: Change password
- `GetActiveSessions`: Get active sessions
- `InvalidateSession`: End a session

### Services

#### 1. MenuService
**Location**: `Areas/Admin/Services/MenuService.cs`

**Key Methods**:
- `GetMenuTreeAsync`: Get hierarchical menu tree
- `GetUserMenusAsync`: Get menus filtered by user permissions
- `CreateAsync`: Create menu
- `UpdateAsync`: Update menu
- `DeleteAsync`: Delete menu
- `MoveMenuAsync`: Reorder menu

#### 2. PermissionService
**Location**: `Areas/Admin/Services/PermissionService.cs`

**Key Methods**:
- `HasMenuAccessAsync`: Check menu access
- `HasPageAccessAsync`: Check page access
- `HasControlAccessAsync`: Check control access
- `GetMenuPermissionsAsync`: Get menu permissions
- `SaveMenuPermissionsAsync`: Save menu permissions
- `GetAllUserPermissionsAsync`: Get all user permissions (for caching)

**Permission Priority**:
1. User-specific permission (highest priority)
2. Role-based permission
3. Default: Deny

#### 3. AuditLogService
**Location**: `Areas/Admin/Services/AuditLogService.cs`

**Key Methods**:
- `LogAsync`: Log an action
- `GetLogsAsync`: Get filtered audit logs

**Usage**:
```csharp
await _auditLogService.LogAsync(
    userId: "user123",
    action: "Create",
    entityType: "User",
    entityId: "newuser456",
    entityName: "John Doe",
    details: "Created new user",
    ipAddress: "192.168.1.10",
    userAgent: "Mozilla/5.0..."
);
```

#### 4. UserService (Enhanced)
**Location**: `Areas/Admin/Services/UserService.cs`

**New Methods**:
- `UpdateLastActivityAsync`: Update last activity time
- `GetActiveSessionsAsync`: Get active sessions
- `HasActiveSessionAsync`: Check if user has active session
- `InvalidateSessionAsync`: End a specific session
- `InvalidateAllSessionsAsync`: End all user sessions

### Helpers

#### 1. MenuHelper
**Location**: `Helpers/MenuHelper.cs`

**Methods**:
- `HasMenuAccess`: Check menu access
- `GetUserMenus`: Get user menus
- `RenderMenuTree`: Render menu tree HTML

**Usage in Views**:
```razor
@if (Html.HasMenuAccess(menuId))
{
    <a href="@Url.Action(...)">Menu Item</a>
}

@Html.RenderMenuTree(Model.Menus)
```

#### 2. ControlPermissionHelper
**Location**: `Helpers/ControlPermissionHelper.cs`

**Methods**:
- `HasControlAccess`: Check control access
- `RenderButtonIfAllowed`: Render button if allowed
- `RenderLinkIfAllowed`: Render link if allowed
- `HasPageAccess`: Check page access

**Usage in Views**:
```razor
@Html.RenderButtonIfAllowed("btnCreate", "Create", "Create", "User")

@if (Html.HasControlAccess("btnDelete", "User", "Delete"))
{
    <button id="btnDelete">Delete</button>
}
```

### Attributes

#### 1. PageAuthorizeAttribute
**Location**: `Attributes/PageAuthorizeAttribute.cs`

**Usage**:
```csharp
[PageAuthorize]
public ActionResult Create()
{
    // Only users with page permission can access
}
```

#### 2. ControlAuthorizeAttribute
**Location**: `Attributes/ControlAuthorizeAttribute.cs`

**Usage**:
```csharp
[ControlAuthorize(ControlId = "btnDelete")]
public ActionResult Delete(int id)
{
    // Only users with control permission can access
}
```

---

## Permission System

### Three-Level Permission Structure

```
Level 1: Menu
    ↓ (if allowed)
Level 2: Page
    ↓ (if allowed)
Level 3: Control
```

### Permission Assignment

**User-Level**:
- Specific user gets specific permission
- Overrides role-based permissions
- Stored in `AccessControls` with `UserId` set

**Role-Level**:
- All users in role get permission
- Stored in `AccessControls` with `RoleId` set

### Permission Check Algorithm

```csharp
1. Check User-Level Permission
   - If found: Return IsAllowed value
   
2. Check Role-Level Permissions
   - Get all user roles
   - Check permissions for each role
   - If any role has explicit Deny: Return false
   - If any role has Allow: Return true
   
3. Default: Return false (Deny)
```

### Example Permission Hierarchy

```
Menu: "User Management" (Allowed)
    ↓
Page: "User/Create" (Allowed)
    ↓
Control: "btnDelete" (Denied)
```

Result: User can see menu, access Create page, but Delete button is hidden.

---

## Session Management

### Session Lifecycle

```
1. Login
   - Create AspNetLoginHistory record
   - Set IsActive = true
   - Set SessionId = HttpContext.Session.SessionID
   - Set LoginTime = DateTime.Now
   - Set LastActivityTime = DateTime.Now

2. Activity
   - Update LastActivityTime on each request
   - (Via SessionValidationAttribute)

3. Logout
   - Set LogoutTime = DateTime.Now
   - Set IsActive = false
   - Update LastActivityTime
```

### Concurrent Session Handling

**Option 1: Single Session (Current Implementation)**
- On login, invalidate all previous sessions
- User can only have one active session

**Option 2: Multiple Sessions**
- Allow multiple sessions per user
- Track all active sessions
- Provide UI to manage sessions

### Session Validation

**SessionValidationAttribute**:
- Runs on each request
- Checks if session is still active
- Updates LastActivityTime
- Redirects to login if session invalidated

---

## Audit Logging

### Automatic Logging

**Logged Actions**:
- Login/Logout
- User CRUD operations
- Role CRUD operations
- Menu CRUD operations
- Permission changes
- Password changes

### Log Structure

```csharp
AuditLog {
    UserId: "user123",
    UserName: "john.doe",
    Action: "Create",
    EntityType: "User",
    EntityId: "newuser456",
    EntityName: "Jane Smith",
    Details: "Created new user with email jane@example.com",
    IpAddress: "192.168.1.10",
    UserAgent: "Mozilla/5.0...",
    Timestamp: 2024-01-15 10:30:00
}
```

### Usage in Controllers

```csharp
await _auditLogService.LogAsync(
    userId: User.Identity.GetUserId(),
    action: "Create",
    entityType: "Menu",
    entityId: menu.Id.ToString(),
    entityName: menu.DisplayName,
    details: $"Created menu: {menu.DisplayName}",
    ipAddress: Request.UserHostAddress,
    userAgent: Request.UserAgent
);
```

---

## Best Practices

### 1. Permission Checks
- Always check permissions at controller level
- Use attributes for page-level authorization
- Use helpers for control-level checks in views

### 2. Audit Logging
- Log all sensitive operations
- Include relevant details
- Don't log passwords or sensitive data

### 3. Session Management
- Update LastActivityTime on each request
- Invalidate sessions on logout
- Handle concurrent sessions appropriately

### 4. Error Handling
- Use try-catch in controllers
- Log errors appropriately
- Return user-friendly error messages

### 5. Performance
- Cache user permissions
- Use indexes on database tables
- Minimize permission checks in loops

---

## Troubleshooting

### Menu Not Showing
1. Check menu `IsVisible` and `IsActive` flags
2. Check menu permissions in `AccessControls` table
3. Verify user has correct roles
4. Check `MenuHelper.HasMenuAccess` return value

### Permission Not Working
1. Check `AccessControls` table for permission record
2. Verify `IsAllowed` value
3. Check if User-level permission overrides Role-level
4. Verify permission type (Menu/Page/Control)

### Session Issues
1. Check `AspNetLoginHistories` table
2. Verify `IsActive` and `LogoutTime` values
3. Check `SessionId` matches current session
4. Verify `LastActivityTime` is updating

---

**Last Updated**: 2024-01-15  
**Version**: 1.0

