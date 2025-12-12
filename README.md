# Corno.Web

ASP.NET MVC Web Application for Corno ERP System

## Overview

This is the main web application for the Corno ERP system, built using ASP.NET MVC framework.

## Features

- Kitchen Management
- Admin Panel
- Master Data Management
- Reporting System
- Label Management
- Carton Management
- Plan Management

## Technology Stack

- ASP.NET MVC
- Entity Framework
- Kendo UI
- Bootstrap
- Telerik Reporting

## Project Structure

- `Areas/` - MVC Areas for different modules (Kitchen, Admin, Masters, etc.)
- `Controllers/` - Application controllers
- `Models/` - Data models
- `Services/` - Business logic services
- `Repository/` - Data access layer
- `Views/` - Razor views
- `Content/` - Static content (CSS, images)
- `Scripts/` - JavaScript files

## Getting Started

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8
- SQL Server
- IIS (for deployment)

### Installation

1. Clone the repository
2. Restore NuGet packages
3. Configure connection string in `Web.config`
4. Build the solution
5. Run the application

## Configuration

Update the connection string in `Web.config`:

```xml
<connectionStrings>
  <add name="CornoContext" connectionString="..." />
</connectionStrings>
```

## License

[Add your license information here]

