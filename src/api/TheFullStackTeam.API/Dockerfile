#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7100
EXPOSE 7101

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheFullStackTeam.API/TheFullStackTeam.API.csproj", "TheFullStackTeam.API/"]
COPY ["TheFullStackTeam.Application/TheFullStackTeam.Application.csproj", "TheFullStackTeam.Application/"]
COPY ["TheFullStackTeam.Common/TheFullStackTeam.Common.csproj", "TheFullStackTeam.Common/"]
COPY ["TheFullStackTeam.Domain/TheFullStackTeam.Domain.csproj", "TheFullStackTeam.Domain/"]
COPY ["TheFullStackTeam.Infrastructure/TheFullStackTeam.Infrastructure.csproj", "TheFullStackTeam.Infrastructure/"]
RUN dotnet restore "./TheFullStackTeam.API/TheFullStackTeam.API.csproj"
COPY . .
WORKDIR "/src/TheFullStackTeam.API"
RUN dotnet build "./TheFullStackTeam.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TheFullStackTeam.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheFullStackTeam.API.dll"]