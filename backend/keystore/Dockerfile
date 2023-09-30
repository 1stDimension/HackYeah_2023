#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HackYeah.Backend.Keystore.csproj", "."]
RUN dotnet restore "./HackYeah.Backend.Keystore.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "HackYeah.Backend.Keystore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HackYeah.Backend.Keystore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV HACKYEAH__DATABASEFILE="/data/keystore.db"
VOLUME ["/data"]
ENTRYPOINT ["dotnet", "HackYeah.Backend.Keystore.dll"]