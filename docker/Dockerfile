# FROM mcr.microsoft.com/dotnet/core/aspnet:5.0.3-alpine3.13-amd64
FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY ./deploy/* app/

WORKDIR /app

ENTRYPOINT ["dotnet", "Omega.dll"]
