
<img src="Design/LogoRectangle.png" alt="CoProject Logo" width=450x, height=150px></img>

[![codecov](https://codecov.io/gh/DenizYil/BDSA-Big-Hero-6/branch/main/graph/badge.svg)](https://app.codecov.io/gh/DenizYil/BDSA-Big-Hero-6/) ![workflow](https://github.com/DenizYil/BDSA-Big-Hero-6/actions/workflows/build-and-test.yml/badge.svg) ![license](https://img.shields.io/github/license/DenizYil/BDSA-Big-Hero-6.svg)
<!-- Fully customizable badges -->
![C#](https://img.shields.io/badge/language-C%23-darkgreen.svg) ![.Net](https://img.shields.io/badge/framework-.NET-purple.svg)

A simple webservice that provides a platform for students and supervisors to find, join and create projects.

## Resources
### Overleaf: 
https://www.overleaf.com/project/617ba189bf8b0297698f95b4

### Gantt diagram: 
https://docs.google.com/spreadsheets/d/1-6Lcv0StDnItwxd7fRDeHYpPXvEjfBGgA-thl6jbonU/edit#gid=0

## How to run

### Requirements
#### Docker
You need to have Docker installed with a ``MSSQL Database``-image.

The system is tested with image ``mcr.microsoft.com/azure-sql-edge:latest`` and ``mcr.microsoft.com/mssql/server:2019-latest``.

#### .NET 6
You need to have ``.NET Core 6.0.0`` runtime and ``.NET 6.0.100`` SDK (or later versions) installed.

#### 

### Getting started
1. Install ``.NET Entity Framework Core tools`` using the command ```dotnet tool install --global dotnet-ef```.
2. Run your docker ``MSSQL``-image. The command is dependent on what image you use. Example command:
   
   ```cmd
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d --name "CoProjectMSSQL" mcr.microsoft.com/mssql/server:2019-latest
   ```
   
   Make sure you update the variable ``$password`` with your own password for database user ``sa``.
3. Set the connection string to your user secrets for project ``CoProject/Server``. Example command below:
   
   ```cmd
   dotnet user-secrets set "ConnectionStrings:CoProject" "Server=127.0.0.1;Database=CoProject;User Id=sa;Password=$passwordForSA" --project CoProject/Server
   ```
   
   Make sure you update the variable ``$passwordForSA`` with the set password for database user ``sa``.
4. Build and model your running ``MSSQL`` database using the command ``dotnet ef database update --project CoProject/Infrastructure`` for the project ``CoProject/Infrastructure``.
5. Start the application by running ``dotnet run --project CoProject/Server`` for the project ``CoProject/Server``.
