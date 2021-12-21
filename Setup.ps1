Write-Host "Ensuring tools is installed"
dotnet tool install --global dotnet-ef

$password = New-Guid

Write-Host "Starting SQL Server"
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 --name "CoProject-Group4" -d mcr.microsoft.com/mssql/server:2019-latest

$database = "CoProject"
$connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password;Trusted_Connection=False;Encrypt=False"

Write-Host "Configuring Connection String"
dotnet user-secrets init --project "CoProject/Server" 
dotnet user-secrets set "ConnectionStrings:CoProject" "$connectionString" --project "CoProject/Server"
dotnet user-secrets set "ConnectionStrings:CoProject" "$connectionString" --project "CoProject/Infrastructure"

Write-Host "Starting program... Enjoy!"
Start-Sleep 2
dotnet run --project "CoProject/Server"