
<img src="Design/LogoRectangle.png" alt="CoProject Logo" width=450x, height=150px></img>

[![codecov](https://codecov.io/gh/DenizYil/BDSA-Big-Hero-6/branch/main/graph/badge.svg)](https://app.codecov.io/gh/DenizYil/BDSA-Big-Hero-6/) ![workflow](https://github.com/DenizYil/BDSA-Big-Hero-6/actions/workflows/build-and-test.yml/badge.svg) ![license](https://img.shields.io/github/license/DenizYil/BDSA-Big-Hero-6.svg)
<!-- Fully customizable badges -->
![C#](https://img.shields.io/badge/language-C%23-darkgreen.svg) ![.Net](https://img.shields.io/badge/framework-.NET-purple.svg)

A simple webservice that provides a platform for students and supervisors to find, join and create projects.

# How to run

## Windows

Run the set up Powershell script by doing
```powershell
.\Setup.ps1
```

## Other platforms
Please continue reading for a guide to set it up by yourself. 

# Requirements
## Docker
You need to have Docker installed with a ``MSSQL Database``-image.

The system is tested with image ``mcr.microsoft.com/azure-sql-edge:latest`` and ``mcr.microsoft.com/mssql/server:2019-latest``.

## .NET 6
You need to have ``.NET Core 6.0.0`` runtime, ``.NET 6.0.100`` SDK and ``ASP Net Core 6.0.0`` (or later versions) installed.

# Getting started
1. Install `.NET Entity Framework Core tools`: 
   ```powershell
   dotnet tool install --global dotnet-ef
   ```
2. Set your variables that you'll need:
   ```powershell
   $password = New-Guid
   $database = "CoProject"
   $connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password;Trusted_Connection=False;Encrypt=False"
   ```
3. Run your docker ``MSSQL``-image. The command is dependent on what image you use:
   ```powershell
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 --name "CoProject-Group4" -d mcr.microsoft.com/mssql/server:2019-latest
   ```
4. Set your connection string secret:
   ```powershell
   dotnet user-secrets set $connectionString --project CoProject/Server
   dotnet user-secrets set $connectionString --project CoProject/Infrastructure
   ```
5. Build and model your running ``MSSQL`` database for the project ``CoProject/Infrastructure``:
   ```powershell
   dotnet ef database update --project CoProject/Infrastructure
   ```
6. Start the application:
   ```powershell
   dotnet run --project CoProject/Server
   ```
