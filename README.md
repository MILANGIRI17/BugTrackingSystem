# Bug Tracker
This is a bug tracker system, where user can create a bug, attach screenshots and asign it to developers as well, and developer can search unassigned open bugs and work on it.
### Tools
**ORM :** Entity Framework Core. 

**Database:** SQL Server

## Configuration
Change the **"username" and "password"** with you database credentials in the  **"connection string"** use **dotnet ef database update or update-database** in the **package manager console** to migrate.

Right Click on solution and click **configure startup projects**
and click **multiple startup projects** then set both **BugTracker.API** and **BugTracker.Web** projects as startup.

**Registration:**

You can register as user, by clicking the **register** and fill the required fields and uncheck "As Developer" checkbox.

You can register as Developer, by clicking the **register** and fill the required fields and check "As Developer" checkbox.
