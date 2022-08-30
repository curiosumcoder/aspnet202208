* https://raw.githubusercontent.com/microsoft/sql-server-samples/master/samples/databases/northwind-pubs/instnwnd.sql
* dotnet tool install --global dotnet-ef
* dotnet tool update --global dotnet-ef
* dotnet add package Microsoft.EntityFrameworkCore.Design
* dotnet add package Microsoft.EntityFrameworkCore.SqlServer
* dotnet ef dbcontext scaffold -h
* dotnet ef dbcontext scaffold "Data Source=.\sqlexpress;Database=Northwind;Integrated Security=SSPI;" Microsoft.EntityFrameworkCore.SqlServer  -c NWContext
* dotnet ef dbcontext scaffold "Data Source=.\sqlexpress;Database=Northwind;Integrated Security=SSPI;" Microsoft.EntityFrameworkCore.SqlServer  -c NWContext -f -o Data
* dotnet ef dbcontext scaffold "Data Source=.\sqlexpress;Database=Northwind;Integrated Security=SSPI;" Microsoft.EntityFrameworkCore.SqlServer --schema Profile -c NWContext -f -o Models\Data
* dotnet ef dbcontext scaffold "Data Source=.\sqlexpress;Database=Northwind;Integrated Security=SSPI;" Microsoft.EntityFrameworkCore.SqlServer -c NWContext -f -o ..\Northwind.Store.Model\ --context-dir ..\Northwind.Store.Data\ -n Northwind.Store.Model --context-namespace Northwind.Store.Data


* https://docs.microsoft.com/en-us/ef/core/extensions/
* EF Core Power Tools, https://github.com/ErikEJ/EFCorePowerTools/wiki