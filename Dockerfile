FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
EXPOSE 80

WORKDIR /app
COPY . .

ENTRYPOINT ["dotnet", "SP.Messenger.Hub.Service.dll"]