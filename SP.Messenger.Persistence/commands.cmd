dotnet ef database update 0
dotnet ef migrations remove
dotnet ef migrations add initdb
dotnet ef database update