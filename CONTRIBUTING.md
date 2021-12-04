# Contributing Guide

If you're interested in contributing to this project, here is a guide on how to set up your project.

## Windows

1. Clone the project
1. Navigate to `./BackEnd/CoProject.Server`
1. Run the following PWSH commands:

```powershell
dotnet tool install --global dotnet-ef

$password = New-Guid
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d --name "CoProjectMSSQL" mcr.microsoft.com/mssql/server:2019-latest

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

## M1 (Apple Silicon)

```powershell
dotnet tool install --global dotnet-ef (Remember to add to PATH for future use)

docker run -d --name "CoProjectDB" -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=urpassword' -p 1433:1433 mcr.microsoft.com/azure-sql-edge

dotnet user-secrets set "ConnectionStrings:CoProject" "Server=127.0.0.1;Database=CoProject;User Id=sa;Password=passwordforsa" --project CoProject/Server

dotnet ef database update --project CoProject/Infrastructure
```
