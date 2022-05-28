FROM mcr.microsoft.com/dotnet/runtime:5.0.3-alpine3.13-amd64

# COPY bin/Release/net5.0/publish/ App/
COPY Omega/bin/Debug/net5.0/publish/ App/

WORKDIR /App

ENTRYPOINT ["dotnet", "Omega.dll"]
