# Login Module - Wireframe Diagrams

## Overview
This document contains detailed wireframe specifications for all screens in the Login Module. These wireframes were finalized during the implementation phase and serve as the design reference for the application.

---

## 1. Dashboard

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Dashboard                      │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules        │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ │
│  - Audit Log           │  │Users │ │Roles │ │Active│ │Sess. │ │
│  - My Profile          │  │  25  │ │  8   │ │  20  │ │  15  │ │
│                        │  └──────┘ └──────┘ └──────┘ └──────┘ │
│                        │                                        │
│                        │  ┌─────────────────────────────────┐  │
│                        │  │  Recent Activity                │  │
│                        │  ├─────────────────────────────────┤  │
│                        │  │  User | Action | Entity | Time   │  │
│                        │  │  ...  |  ...   |  ...   |  ...   │  │
│                        │  └─────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Statistics Cards (4)**: 
  - Total Users (with user icon)
  - Total Roles (with group icon)
  - Active Users (with check icon)
  - Active Sessions (with menu icon)
- **Recent Activity Grid**: 
  - Columns: Action, Entity Type, Entity/Details, User, Time, IP Address
  - Pagination: 10 items per page
  - Auto-refresh: Every 30 seconds

### Design Specifications
- **Layout**: Full-width responsive grid
- **Card Style**: Kendo UI cards with icons
- **Colors**: Primary (Users), Success (Roles), Info (Active Users), Warning (Sessions)
- **Grid**: Kendo Grid with sorting and filtering

---

## 2. User Management

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management ✓   │  │  User Management                │  │
│  - Role Management     │  │  [+ Add New]                     │  │
│  - Menu Management     │  └─────────────────────────────────┘  │
│  - Access Rules        │                                        │
│  - Audit Log           │  ┌─────────────────────────────────┐  │
│  - My Profile          │  │  Grid: Users                     │  │
│                        │  ├──────┬──────┬──────┬───────────┤  │
│                        │  │User  │Email │Last  │  Actions  │  │
│                        │  │Name  │      │Name  │           │  │
│                        │  ├──────┼──────┼──────┼───────────┤  │
│                        │  │admin │...   │...   │ [✏] [🔒] [🗑]│ │
│                        │  │user1 │...   │...   │ [✏] [🔒] [🗑]│ │
│                        │  └──────┴──────┴──────┴───────────┘  │
│                        │  [Pagination]                         │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Toolbar**: 
  - "+ Add New" button (top right)
  - Export buttons (Excel, PDF)
  - Search box
- **Grid Columns**:
  - User Name
  - Email
  - Last Name
  - Type
  - Actions (Edit, Change Password, Delete)
- **Actions**:
  - Edit: Opens Edit form
  - Change Password: Opens Change Password form
  - Delete: Confirms and deletes user

### Design Specifications
- **Grid**: Kendo Grid with `ApplyIndexSettings`
- **Responsive**: Full height, auto-fit columns
- **Actions**: Icon buttons (pencil, lock, delete)

---

## 3. Role Management

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Role Management                 │  │
│  - Role Management ✓   │  │  [+ Add New]                     │  │
│  - Menu Management     │  └─────────────────────────────────┘  │
│  - Access Rules        │                                        │
│  - Audit Log           │  ┌─────────────────────────────────┐  │
│  - My Profile          │  │  Grid: Roles                     │  │
│                        │  ├──────┬──────────────────────────┤  │
│                        │  │Role  │  Actions                  │  │
│                        │  │Name  │                          │  │
│                        │  ├──────┼──────────────────────────┤  │
│                        │  │Admin │ [✏] [🗑] [🔗]              │  │
│                        │  │User  │ [✏] [🗑] [🔗]              │  │
│                        │  └──────┴──────────────────────────┘  │
│                        │  [Pagination]                         │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Toolbar**: 
  - "+ Add New" button
  - Export buttons
  - Search box
- **Grid Columns**:
  - Role Name
  - Actions (Edit, Delete, Set Access)
- **Actions**:
  - Edit: Opens Edit form
  - Delete: Confirms and deletes role
  - Set Access: Opens Access Rules page for the role

### Design Specifications
- **Grid**: Kendo Grid with `ApplyIndexSettings`
- **Actions**: Icon buttons (pencil, delete, hyperlink for access)

---

## 4. Menu Management

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Menu Management                 │  │
│  - Role Management     │  │  [+ Add New Menu] [Refresh]      │  │
│  - Menu Management ✓   │  └─────────────────────────────────┘  │
│  - Access Rules        │                                        │
│  - Audit Log           │  ┌─────────────────────────────────┐  │
│  - My Profile          │  │  Menu Tree                       │  │
│                        │  ├─────────────────────────────────┤  │
│                        │  │  📁 Administration              │  │
│                        │  │    ├─ 📄 Dashboard             │  │
│                        │  │    ├─ 📄 User Management        │  │
│                        │  │    ├─ 📄 Role Management        │  │
│                        │  │    └─ 📄 Menu Management       │  │
│                        │  │  📁 Masters                     │  │
│                        │  │    ├─ 📄 Customer               │  │
│                        │  │    └─ 📄 Product                │  │
│                        │  └─────────────────────────────────┘  │
│                        │  (Click menu to edit)                  │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Toolbar**: 
  - "+ Add New Menu" button
  - "Refresh" button
- **Menu Tree**: 
  - Hierarchical tree view
  - Click to edit
  - Drag and drop to reorder (optional)
- **Actions**:
  - Click menu item: Opens Edit form
  - Add New: Opens Create form

### Design Specifications
- **Tree View**: Kendo TreeView
- **Interaction**: Click to edit, drag to reorder
- **Icons**: Folder (📁) for parents, Document (📄) for pages

---

## 5. Access Rules Management

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Access Rules Management        │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules ✓      │  ┌──────────┬──────────────────────┐  │
│  - Audit Log           │  │ Assign   │ [Role ▼] [User ▼]     │  │
│  - My Profile          │  │ To:      │ [Load Permissions]    │  │
│                        │  └──────────┴──────────────────────┘  │
│                        │                                        │
│                        │  ┌──────────┬──────────────────────┐  │
│                        │  │ Menu     │  Permissions          │  │
│                        │  │ Tree     │  ┌──────────────────┐ │  │
│                        │  │          │  │ [Tabs]            │ │  │
│                        │  │ 📁 Admin │  │ Menu | Page | Ctrl│ │  │
│                        │  │  ├─ User │  ├──────────────────┤ │  │
│                        │  │  └─ Role │  │ ☑ Menu 1          │ │  │
│                        │  │          │  │ ☐ Menu 2          │ │  │
│                        │  │          │  │ ☑ Menu 3          │ │  │
│                        │  └──────────┴──────────────────────┘  │
│                        │  [Save Permissions] [Reset]            │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Selector Section**:
  - "Assign To" dropdown: Role or User
  - Role/User dropdown: Select specific role or user
  - "Load Permissions" button
- **Left Panel**: Menu Tree
  - Hierarchical menu structure
  - Click to select menu
- **Right Panel**: Permissions Tabs
  - **Menu Permissions Tab**: 
    - Checkboxes for each menu
    - ☑ = Allowed, ☐ = Denied
  - **Page Permissions Tab**:
    - Grid with Controller/Action combinations
    - Checkboxes for each page action
  - **Control Permissions Tab**:
    - Grid with control IDs (btnCreate, btnDelete, etc.)
    - Checkboxes for each control
- **Action Buttons**:
  - "Save Permissions" button
  - "Reset" button

### Design Specifications
- **Layout**: Split view (30% menu tree, 70% permissions)
- **Tabs**: Kendo TabStrip with 3 tabs
- **Grids**: Kendo Grids for Page and Control permissions
- **Tree**: Kendo TreeView for menu selection

---

## 6. Audit Log

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Audit Log                        │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules        │  ┌─────────────────────────────────┐  │
│  - Audit Log ✓         │  │  Filters                         │  │
│  - My Profile          │  │  [From Date] [To Date] [User]    │  │
│                        │  │  [Action] [Entity Type]           │  │
│                        │  │  [Filter] [Export]                │  │
│                        │  └─────────────────────────────────┘  │
│                        │                                        │
│                        │  ┌─────────────────────────────────┐  │
│                        │  │  Grid: Audit Logs                │  │
│                        │  ├──────┬──────┬──────┬───────────┤  │
│                        │  │Time  │User  │Action│ Entity    │  │
│                        │  ├──────┼──────┼──────┼───────────┤  │
│                        │  │10:30 │admin │Create│ User      │  │
│                        │  │10:25 │user1 │Edit  │ Role      │  │
│                        │  └──────┴──────┴──────┴───────────┘  │
│                        │  [Pagination]                         │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Filter Section**:
  - From Date: DatePicker
  - To Date: DatePicker
  - User: Text input or Dropdown
  - Action: Text input
  - Entity Type: Text input
  - Filter button
  - Export button
- **Grid Columns**:
  - Timestamp (formatted: MM/dd/yyyy HH:mm:ss)
  - User Name
  - Action
  - Entity Type
  - Entity Name
  - Details
  - IP Address
- **Features**:
  - Pagination: 20 items per page
  - Sorting: All columns sortable
  - Filtering: Grid-level filtering
  - Export: Excel export

### Design Specifications
- **Grid**: Kendo Grid with `ApplyIndexSettings`
- **Filters**: Kendo DatePickers and TextBoxes
- **Export**: Excel export functionality

---

## 7. My Profile

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  My Profile                       │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules        │  ┌──────────────┬──────────────────┐  │
│  - Audit Log           │  │ Profile Info │ Active Sessions   │  │
│  - My Profile ✓        │  ├──────────────┼──────────────────┤  │
│                        │  │ Username:    │ ┌──────────────┐ │  │
│                        │  │ [readonly]   │ │ Session ID   │ │  │
│                        │  │              │ │ Login Time   │ │  │
│                        │  │ Email:       │ │ Last Activity │ │  │
│                        │  │ [input]      │ │ IP Address   │ │  │
│                        │  │              │ │ [End Session]│ │  │
│                        │  │ First Name:  │ └──────────────┘ │  │
│                        │  │ [input]      │                   │  │
│                        │  │              │ ┌──────────────┐ │  │
│                        │  │ Last Name:   │ │ Session ID   │ │  │
│                        │  │ [input]      │ │ ...          │ │  │
│                        │  │              │ │ [End Session]│ │  │
│                        │  │ Phone:       │ └──────────────┘ │  │
│                        │  │ [input]      │                   │  │
│                        │  │              │                   │  │
│                        │  │ [Update]    │                   │  │
│                        │  │ [Change Pwd]│                   │  │
│                        │  └──────────────┴──────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Left Panel - Profile Information**:
  - Username: Read-only text field
  - Email: Editable text field
  - First Name: Editable text field
  - Last Name: Editable text field
  - Phone Number: Editable text field
  - "Update Profile" button
  - "Change Password" button (links to Change Password page)
- **Right Panel - Active Sessions**:
  - Grid showing active sessions
  - Columns: Session ID, Login Time, Last Activity, IP Address
  - Action: "End Session" button for each session
  - Pagination: 5 items per page

### Design Specifications
- **Layout**: Split view (50% profile, 50% sessions)
- **Form**: Kendo form with validation
- **Grid**: Kendo Grid for sessions
- **Cards**: Kendo cards for each section

---

## 8. Change Password

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Change Password                  │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules        │  ┌─────────────────────────────────┐  │
│  - Audit Log           │  │  Current Password:               │  │
│  - My Profile          │  │  [password input]                 │  │
│                        │  │                                  │  │
│                        │  │  New Password:                   │  │
│                        │  │  [password input]                 │  │
│                        │  │  (min 6 characters)              │  │
│                        │  │                                  │  │
│                        │  │  Confirm Password:               │  │
│                        │  │  [password input]                 │  │
│                        │  │                                  │  │
│                        │  │  [Change Password] [Cancel]      │  │
│                        │  └─────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Form Fields**:
  - Current Password: Password input (required)
  - New Password: Password input (required, min 6 characters)
  - Confirm Password: Password input (required, must match)
- **Action Buttons**:
  - "Change Password" button (primary)
  - "Cancel" button (links back to Profile)
- **Validation**:
  - Client-side: Password match validation
  - Server-side: Old password verification
  - Password strength: Minimum 6 characters

### Design Specifications
- **Layout**: Centered card (max-width: 500px)
- **Form**: Kendo form with validation
- **Inputs**: Password type inputs
- **Validation**: Real-time validation feedback

---

## 9. Menu Create/Edit Form

### Layout Structure
```
┌─────────────────────────────────────────────────────────────────┐
│  Header: Logo | User Menu | Logout                              │
├─────────────────────────────────────────────────────────────────┤
│  Sidebar Menu          │  Main Content Area                      │
│  - Dashboard           │  ┌─────────────────────────────────┐  │
│  - User Management     │  │  Create/Edit Menu                 │  │
│  - Role Management     │  └─────────────────────────────────┘  │
│  - Menu Management     │                                        │
│  - Access Rules        │  ┌─────────────────────────────────┐  │
│  - Audit Log           │  │  Menu Name*:                     │  │
│  - My Profile          │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Display Name*:                  │  │
│                        │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Parent Menu:                     │  │
│                        │  │  [dropdown - optional]           │  │
│                        │  │                                  │  │
│                        │  │  Controller:                      │  │
│                        │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Action:                          │  │
│                        │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Area:                            │  │
│                        │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Icon Class:                     │  │
│                        │  │  [input]                         │  │
│                        │  │                                  │  │
│                        │  │  Display Order:                  │  │
│                        │  │  [numeric input]                 │  │
│                        │  │                                  │  │
│                        │  │  ☑ Visible in Menu               │  │
│                        │  │  ☑ Active                        │  │
│                        │  │                                  │  │
│                        │  │  Description:                     │  │
│                        │  │  [textarea]                      │  │
│                        │  │                                  │  │
│                        │  │  [Save] [Cancel] [Delete]        │  │
│                        │  └─────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Components
- **Form Fields**:
  - Menu Name*: Text input (required, unique)
  - Display Name*: Text input (required)
  - Parent Menu: Dropdown (optional, excludes current menu in edit)
  - Controller: Text input (optional)
  - Action: Text input (optional)
  - Area: Text input (optional)
  - Icon Class: Text input (optional, e.g., "k-i-home")
  - Display Order: Numeric input (default: 0)
  - Visible in Menu: Checkbox (default: checked)
  - Active: Checkbox (default: checked)
  - Description: Textarea (optional)
- **Action Buttons**:
  - "Save" / "Update" button (primary)
  - "Cancel" button
  - "Delete" button (only in Edit mode)

### Design Specifications
- **Layout**: Centered card (max-width: 800px)
- **Form**: Kendo form with validation
- **Dropdown**: Kendo DropDownList with hierarchical menu list
- **Validation**: Required field validation, duplicate name check

---

## Design System Specifications

### Color Scheme
- **Primary**: Blue (#0078d4) - Main actions, links
- **Success**: Green (#107c10) - Success messages, positive stats
- **Info**: Cyan (#00bcf2) - Information, active users
- **Warning**: Orange (#ff8c00) - Warnings, sessions
- **Error**: Red (#d13438) - Errors, delete actions

### Typography
- **Font Family**: System default (Segoe UI on Windows)
- **Headings**: Bold, 18-24px
- **Body Text**: Regular, 14px
- **Labels**: Regular, 12px
- **Hints**: Regular, 11px, muted color

### Spacing
- **Card Padding**: 20px
- **Form Field Spacing**: 16px vertical
- **Button Spacing**: 8px horizontal
- **Section Spacing**: 24px vertical

### Components Library
- **Cards**: Kendo UI Card component
- **Grids**: Kendo UI Grid with `ApplyIndexSettings`
- **Forms**: Kendo UI Form components
- **Buttons**: Kendo UI Button (solid, flat variants)
- **Inputs**: Kendo UI TextBox, NumericTextBox, DatePicker
- **Dropdowns**: Kendo UI DropDownList
- **Tree**: Kendo UI TreeView
- **Tabs**: Kendo UI TabStrip

### Responsive Breakpoints
- **Desktop**: > 1024px (full layout)
- **Tablet**: 768px - 1024px (stacked layout)
- **Mobile**: < 768px (single column)

---

## Implementation Notes

1. **All screens use**: `_AppLayout.cshtml` as the master layout
2. **Grid views use**: `ApplyIndexSettings()` helper for consistent styling
3. **Forms use**: Kendo UI form components with validation
4. **Navigation**: Sidebar menu filtered by user permissions
5. **Icons**: Kendo UI icons (k-i-* classes)
6. **Consistency**: All screens follow the same design patterns

---

## Future Enhancements

1. **Dark Mode**: Add dark theme support
2. **Accessibility**: ARIA labels and keyboard navigation
3. **Mobile App**: Responsive design for mobile devices
4. **Customization**: User-configurable dashboard widgets
5. **Charts**: Add charts/graphs to dashboard

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Maintained By**: Development Team

