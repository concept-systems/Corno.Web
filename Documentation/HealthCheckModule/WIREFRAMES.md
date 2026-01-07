# Health Check Module - Wireframe Designs

## Table of Contents
1. [System Health Check Dashboard](#system-health-check-dashboard)
2. [Data Integrity Issues Page](#data-integrity-issues-page)
3. [Component Descriptions](#component-descriptions)

## System Health Check Dashboard

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  System Health Check                                                         │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Overall      │  │ Critical     │  │ Warnings     │  │ Auto-Fixed   │  │
│  │ Status       │  │ Issues       │  │              │  │              │  │
│  │              │  │              │  │              │  │              │  │
│  │ [Healthy]    │  │ [5]          │  │ [12]         │  │ [23]         │  │
│  │              │  │              │  │              │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘  │
│                                                                               │
│  ┌──────────────┐                                                           │
│  │ Last Check   │                                                           │
│  │ 12/19/2024  │                                                           │
│  │ 10:30 AM    │                                                           │
│  └──────────────┘                                                           │
│                                                                               │
│  [Run Health Check Now]  [View Data Integrity Issues]  [Refresh]           │
│                                                                               │
├─────────────────────────────────────────────────────────────────────────────┤
│  Health Check Reports                                                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  From Date: [12/12/2024]  To Date: [12/19/2024]  Status: [All ▼] [Filter] │
│                                                                               │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Check Date    │ Check Type        │ Status │ Message │ Time │ Auto │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ 12/19 10:30  │ Database          │ [✓]   │ Healthy │ 45ms │ No  │  │
│  │ 12/19 10:30  │ Connection Pool   │ [✓]   │ Healthy │ 12ms │ No  │  │
│  │ 12/19 10:30  │ Disk Space        │ [!]   │ 85% used│ 8ms  │ No  │  │
│  │ 12/19 10:30  │ Memory Usage      │ [✓]   │ Healthy │ 15ms │ No  │  │
│  │ 12/19 10:30  │ Error Rate        │ [X]   │ 15 errors│ 120ms│ Yes │  │
│  │ 12/19 10:30  │ Performance       │ [✓]   │ Healthy │ 23ms │ No  │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                               │
│  [<] 1 2 3 4 5 [>]                                                           │
│                                                                               │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Status Indicators

- `[✓]` = Healthy (Green badge)
- `[!]` = Warning (Yellow badge)
- `[X]` = Critical (Red badge)

## Data Integrity Issues Page

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  Data Integrity Issues                                                       │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  [Run Data Integrity Check]  [Auto-Fix Selected Issues]  [Refresh]          │
│                                                                               │
├─────────────────────────────────────────────────────────────────────────────┤
│  Filters                                                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  From Date: [12/12/2024]  To Date: [12/19/2024]                             │
│  Check Type: [All ▼]  Status: [Open ▼]  [Filter]                            │
│                                                                               │
├─────────────────────────────────────────────────────────────────────────────┤
│  Data Integrity Issues                                                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ ☑ │ Check Date │ Check Type      │ Entity │ Identifier │ Issue Type │  │
│  ├───┼────────────┼─────────────────┼────────┼────────────┼────────────┤  │
│  │ ☑ │ 12/19 08:00│ Plan Quantities │ Plan   │ WO-12345   │ PrintQty   │  │
│  │ ☐ │ 12/19 08:00│ Plan Quantities │ Plan   │ WO-12346   │ BendQty    │  │
│  │ ☑ │ 12/19 08:00│ PackQuantity    │ Plan   │ WO-12347   │ PackQty    │  │
│  │ ☐ │ 12/19 08:00│ Carton Barcode  │ Carton │ C-001      │ Barcode    │  │
│  │ ☐ │ 12/19 08:00│ Label Sequence  │ Label  │ L-12345    │ Sequence  │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                               │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │ Details Panel (Expanded on row click)                                  │  │
│  ├───────────────────────────────────────────────────────────────────────┤  │
│  │ Description: PlanItemDetail PrintQuantity doesn't match LabelDetails  │  │
│  │              with Printed status                                      │  │
│  │ Expected: 150                                                          │  │
│  │ Actual: 145                                                            │  │
│  │ Can Fix: Yes                                                           │  │
│  │ Status: Open                                                           │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                               │
│  [<] 1 2 3 4 5 [>]                                                           │
│                                                                               │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Component Descriptions

### Dashboard Cards

#### Overall Status Card
- **Icon**: Check mark (Green)
- **Value**: Overall status (Healthy/Warning/Critical)
- **Color**: Green/Yellow/Red based on status

#### Critical Issues Card
- **Icon**: Warning sign (Red)
- **Value**: Number of critical issues
- **Color**: Red

#### Warnings Card
- **Icon**: Information (Yellow)
- **Value**: Number of warnings
- **Color**: Yellow

#### Auto-Fixed Card
- **Icon**: Check circle (Green)
- **Value**: Number of auto-fixed issues
- **Color**: Green

#### Last Check Card
- **Icon**: Clock (Gray)
- **Value**: Last check timestamp
- **Color**: Gray

### Action Buttons

#### Run Health Check Now
- **Style**: Primary button (Blue)
- **Icon**: Refresh icon
- **Action**: Triggers manual health check
- **State**: Shows "Running..." when active

#### View Data Integrity Issues
- **Style**: Secondary button (Gray)
- **Icon**: Database icon
- **Action**: Navigates to Data Integrity page

#### Refresh
- **Style**: Secondary button (Gray)
- **Icon**: Reload icon
- **Action**: Refreshes dashboard data

### Grid Components

#### Health Check Reports Grid
- **Columns**: Check Date, Check Type, Status, Message, Time (ms), Auto-Fixed
- **Features**: 
  - Sortable columns
  - Pagination
  - Export to Excel/PDF
  - Search functionality
  - Status badges with colors

#### Data Integrity Issues Grid
- **Columns**: Checkbox, Check Date, Check Type, Entity, Identifier, Issue Type, Description, Expected, Actual, Can Fix, Status, Fixed
- **Features**:
  - Multi-select checkboxes
  - Sortable columns
  - Pagination
  - Export to Excel/PDF
  - Search functionality
  - Expandable row details
  - Status badges with colors

### Filter Components

#### Date Pickers
- **Type**: Kendo DatePicker
- **Format**: MM/dd/yyyy
- **Default**: Last 7 days

#### Dropdown Lists
- **Type**: Kendo DropDownList
- **Options**: 
  - Status: All, Healthy, Warning, Critical
  - Check Type: All, Plan Quantities, PackQuantity, Carton Barcode, Label Sequence

### Status Badges

#### Health Status Badges
- **Healthy**: Green badge with check icon
- **Warning**: Yellow badge with warning icon
- **Critical**: Red badge with X icon

#### Fix Status Badges
- **Yes**: Green badge
- **No**: Gray badge

#### Issue Status Badges
- **Open**: Yellow badge
- **Fixed**: Green badge
- **CannotFix**: Red badge

### Responsive Design

The interface is responsive and adapts to different screen sizes:

- **Desktop**: Full layout with all columns visible
- **Tablet**: Some columns may be hidden, horizontal scrolling available
- **Mobile**: Stacked layout, essential columns only

### Color Scheme

- **Primary**: Blue (#0078d4)
- **Success**: Green (#107c10)
- **Warning**: Yellow (#ffaa44)
- **Danger**: Red (#d13438)
- **Neutral**: Gray (#605e5c)

### Typography

- **Headers**: Bold, 18-24px
- **Body**: Regular, 14px
- **Labels**: Regular, 12px
- **Values**: Bold, 16-24px

### Icons

- **Check**: ✓ (Success)
- **Warning**: ! (Warning)
- **Error**: ✗ (Critical)
- **Refresh**: Circular arrow
- **Database**: Database icon
- **Filter**: Funnel icon
- **Export**: File icon

