# Contributing Guide

If you're interested in contributing to this project, here is a guide on how to set up your project.

1. Clone the project
1. Navigate to `./BackEnd/CoProject.Server`
1. Run the following PWSH commands:

```powershell
dotnet tool install --global dotnet-ef

$password = New-Guid
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest

$database = "CoProject"
$connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password"

dotnet user-secrets set "ConnectionStrings:$database" "$connectionString"
```

4. Now navigate to `../CoProject.Infrastructure`
1. Run the following command:

```powershell
dotnet ef database update
```

That should be it!