# Bug Tracker

This is a bug tracker system where users can create bugs, attach screenshots, and assign them to developers. 

Developers can search for unassigned open bugs and work on them.

### Tools

- **ORM:** Entity Framework Core  
- **Database:** SQL Server
- **Frameworks:** .NET 8 Web API, .NET 8 MVC

## Configuration

Update the **"username"** and **"password"** in the **connection string** with your database credentials.  
Use the following command in the **Package Manager Console** to apply migrations:

```dotnet ef database update``` or ```update-database``` 

> **Note:** If **multiple startup projects** were previously set before migration, please disable them and set **BugTracker.API** as startup project..

**To configure the startup projects:**

- Right-click on the solution.
- Click **Configure Startup Projects**.
- Select **Multiple startup projects**.
- Set both **BugTracker.API** and **BugTracker.Web** as startup projects.

## Registration

- To register as a **user**, click **Register**, fill in the required fields, and **uncheck** the "As Developer" checkbox.
- To register as a **developer**, click **Register**, fill in the required fields, and **check** the "As Developer" checkbox.