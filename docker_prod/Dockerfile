FROM mcr.microsoft.com/dotnet/aspnet:5.0

# RUN ["apt-get", "update"]
# RUN ["apt-get", "install", "-y", "vim"]

RUN echo "alias ll='ls -al --color'" >> ~/.bashrc

COPY ./app/ app/

WORKDIR /app

ENTRYPOINT ["dotnet", "Omega.dll"]
