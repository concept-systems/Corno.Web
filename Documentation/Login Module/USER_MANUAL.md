# Login Module - User Manual

## Table of Contents
1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Dashboard](#dashboard)
4. [User Management](#user-management)
5. [Role Management](#role-management)
6. [Menu Management](#menu-management)
7. [Access Rules Management](#access-rules-management)
8. [Audit Log](#audit-log)
9. [My Profile](#my-profile)
10. [Password Reset](#password-reset)
11. [Troubleshooting](#troubleshooting)

---

## Introduction

The Login Module provides a comprehensive user authentication and authorization system with the following features:

- **User Management**: Create, edit, and manage users
- **Role Management**: Create and assign roles to users
- **Menu Management**: Create and organize application menus
- **Access Control**: Fine-grained permission management (Menu, Page, and Control levels)
- **Audit Logging**: Track all user activities
- **Session Management**: Monitor and manage user sessions
- **Password Reset**: Secure password recovery

---

## Getting Started

### First Login

1. Navigate to the login page
2. Enter your **Username** and **Password**
3. Optionally check **Remember Me** to stay logged in
4. Click **Sign In**

### Navigation

After login, you'll see:
- **Top Navigation Bar**: Contains user menu and logout
- **Sidebar Menu**: Application menus based on your permissions
- **Main Content Area**: Current page content

---

## Dashboard

The Dashboard provides an overview of system statistics and recent activity.

### Features

- **Statistics Cards**: 
  - Total Users
  - Total Roles
  - Active Users

- **Recent Activity**: Last 10 actions performed in the system

- **Quick Actions**: 
  - Create User
  - Create Role
  - Manage Menus
  - Access Rules

### Accessing Dashboard

Click **Dashboard** in the sidebar menu or navigate to `/Admin/Dashboard`.

---

## User Management

### Viewing Users

1. Navigate to **Admin** → **Users**
2. View the list of all users
3. Use search and filters to find specific users

### Creating a User

1. Click **+ Create User** button
2. Fill in the form:
   - **Username***: Unique username
   - **First Name***: User's first name
   - **Last Name***: User's last name
   - **Email***: Valid email address
   - **Phone**: Phone number (optional)
   - **Password***: Initial password
   - **Confirm Password***: Re-enter password
   - **Roles**: Select roles to assign
   - **Account Status**: Active or Locked
3. Click **Save**

### Editing a User

1. Find the user in the list
2. Click **Edit** button
3. Modify the information
4. Click **Save**

### Deleting a User

1. Find the user in the list
2. Click **Delete** button
3. Confirm deletion

**Note**: Deleting a user is a permanent action. Consider deactivating instead.

### Changing User Password

1. Find the user in the list
2. Click **Change Password**
3. Enter new password
4. Confirm password
5. Click **Save**

### Assigning Roles to User

1. Edit the user
2. In the **Roles** section, check/uncheck roles
3. Click **Save**

---

## Role Management

### Viewing Roles

1. Navigate to **Admin** → **Roles**
2. View the list of all roles

### Creating a Role

1. Click **+ Create Role** button
2. Fill in:
   - **Role Name***: Unique role name
   - **Description**: Role description
3. Click **Save**

### Editing a Role

1. Find the role in the list
2. Click **Edit**
3. Modify information
4. Click **Save**

### Deleting a Role

1. Find the role in the list
2. Click **Delete**
3. Confirm deletion

**Note**: Ensure no users are assigned to the role before deleting.

### Managing Role Permissions

1. Find the role in the list
2. Click **Access** button
3. This opens the Access Rules page (see Access Rules Management section)

---

## Menu Management

### Viewing Menus

1. Navigate to **Admin** → **Menus**
2. View the hierarchical menu tree

### Creating a Menu

1. Click **+ Create Menu** button
2. Fill in the form:
   - **Parent Menu**: Select parent (optional for root menu)
   - **Menu Name***: Unique identifier
   - **Display Name***: Name shown in UI
   - **Description**: Menu description
   - **Menu Type**: 
     - **Container**: Menu group (no page)
     - **Page**: Links to a page
     - **External Link**: External URL
   - **Controller**: MVC controller name (if Page type)
   - **Action**: MVC action name (if Page type)
   - **Area**: MVC area name (if applicable)
   - **Icon Class**: CSS class for icon (e.g., "fa fa-user")
   - **Display Order**: Order in menu (0 = first)
   - **Visible**: Show in menu
   - **Active**: Enable menu
3. Click **Save**

### Editing a Menu

1. Find the menu in the tree
2. Click **Edit**
3. Modify information
4. Click **Save**

### Deleting a Menu

1. Find the menu in the tree
2. Click **Delete**
3. Confirm deletion

**Warning**: Deleting a menu will also delete all child menus.

### Reordering Menus

1. Use **Move Up** / **Move Down** buttons
2. Or drag and drop in the menu tree (if supported)

---

## Access Rules Management

Access Rules allow you to control what users and roles can access at three levels:
1. **Menu Level**: Which menus are visible
2. **Page Level**: Which pages can be accessed
3. **Control Level**: Which buttons/controls are available

### Accessing Access Rules

1. Navigate to **Admin** → **Access Rules**
2. Or click **Access** button from Role/User management

### Setting Access Level

Choose one:
- **User Level**: Assign permissions to specific user
- **Role Level**: Assign permissions to all users in a role

### Selecting Target

- **For User Level**: Select user from dropdown
- **For Role Level**: Select role from dropdown

### Menu-Level Permissions

1. In the **Menu Tree** panel, check/uncheck menus
2. ✓ = Allowed, ☐ = Denied
3. Changes apply to selected user/role

### Page-Level Permissions

1. Select a menu item that has a page (Controller/Action)
2. In the **Page Access** panel, set permissions for each action:
   - **Index**: List page
   - **Create**: Create page
   - **Edit**: Edit page
   - **View**: View page
   - **Delete**: Delete action
3. Check **Override** to set user-specific permission (if at role level)

### Control-Level Permissions

1. In the **Control Access** panel, manage button/control permissions
2. Click **Add Control** to add a new control
3. Enter:
   - **Control ID**: Unique identifier (e.g., "btnCreate")
   - **Control Name**: Display name
   - **Access**: Allow/Deny
4. Click **Save**

### Saving Permissions

1. After making changes, click **Save All**
2. Changes are applied immediately
3. Users will see updated permissions on next page load

### Copying Permissions

1. Click **Copy from Role** dropdown
2. Select a role to copy permissions from
3. Permissions will be copied to current user/role

---

## Audit Log

The Audit Log tracks all activities in the system for security and compliance.

### Viewing Audit Logs

1. Navigate to **Admin** → **Audit Log**
2. View the list of all logged activities

### Filtering Logs

Use filters to find specific logs:
- **Date Range**: From and To dates
- **User**: Filter by specific user
- **Action**: Filter by action type (Login, Create, Edit, Delete, etc.)
- **Entity Type**: Filter by entity (User, Role, Menu, etc.)

### Log Information

Each log entry shows:
- **Timestamp**: When the action occurred
- **User**: Who performed the action
- **Action**: What action was performed
- **Entity**: What was affected
- **Details**: Additional information
- **IP Address**: User's IP address

### Exporting Logs

1. Apply filters (optional)
2. Click **Export to Excel** or **Export to PDF**
3. File will be downloaded

---

## My Profile

### Viewing Profile

1. Click your username in the top-right corner
2. Select **My Profile**
3. View your profile information

### Editing Profile

1. Click **Edit** button
2. Modify:
   - First Name
   - Last Name
   - Email
   - Phone
   - Profile Picture (upload)
3. Click **Save**

### Changing Password

1. In profile page, scroll to **Change Password** section
2. Enter:
   - **Current Password***
   - **New Password***
   - **Confirm Password***
3. Click **Change Password**

**Password Requirements**:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

### Viewing Active Sessions

1. In profile page, click **View Sessions**
2. See all active sessions:
   - Device/Browser
   - IP Address
   - Login Time
   - Last Activity
3. Click **End Session** to logout from a specific device

---

## Password Reset

### Forgot Password

If you forget your password:

1. On login page, click **Forgot Password?**
2. Enter your **Email Address**
3. Click **Send Reset Link**
4. Check your email for reset link
5. Click the link in the email

### Resetting Password

1. After clicking email link, you'll see Reset Password page
2. Enter:
   - **New Password***
   - **Confirm Password***
3. Click **Reset Password**
4. You'll be redirected to login page
5. Login with your new password

**Note**: Reset links expire after 24 hours for security.

---

## Troubleshooting

### Cannot Login

**Problem**: Username/password not working

**Solutions**:
1. Check if Caps Lock is on
2. Verify username is correct
3. Try password reset
4. Contact administrator if account is locked

### Menu Not Showing

**Problem**: Expected menu item is not visible

**Solutions**:
1. Check if you have permission for that menu
2. Contact administrator to assign permissions
3. Verify menu is active and visible

### Cannot Access Page

**Problem**: Getting "Access Denied" error

**Solutions**:
1. You don't have permission for that page
2. Contact administrator to grant access
3. Check if your role has the required permissions

### Button Not Showing

**Problem**: Expected button is not visible

**Solutions**:
1. You don't have control-level permission
2. Contact administrator to grant control access
3. Button may be hidden based on your role

### Session Expired

**Problem**: Getting logged out unexpectedly

**Solutions**:
1. Session may have expired due to inactivity
2. Another session may have logged you out
3. Simply login again

### Password Reset Not Working

**Problem**: Not receiving reset email

**Solutions**:
1. Check spam/junk folder
2. Verify email address is correct
3. Wait a few minutes and try again
4. Contact administrator if issue persists

---

## Best Practices

### For Administrators

1. **Regular Audits**: Review audit logs regularly
2. **Permission Management**: Follow principle of least privilege
3. **User Management**: Deactivate instead of deleting users
4. **Password Policy**: Enforce strong passwords
5. **Session Management**: Monitor active sessions

### For Users

1. **Password Security**: Use strong, unique passwords
2. **Session Management**: Logout when done
3. **Profile Updates**: Keep profile information current
4. **Report Issues**: Contact administrator for access issues

---

## Support

For additional help:
1. Check this user manual
2. Contact your system administrator
3. Review audit logs for troubleshooting

---

**Last Updated**: 2024-01-15  
**Version**: 1.0

