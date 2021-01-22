#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /build

COPY "Mojira.sln" "Mojira.sln"
COPY "src/Common/Core/Core.csproj" "src/Common/Core/"
COPY "src/Common/EventBus/EventBus.csproj" "src/Common/EventBus/"
COPY "src/Common/ServiceDiscovery/ServiceDiscovery.csproj" "src/Common/ServiceDiscovery/"
COPY "src/Gateway/Gateway.API/Gateway.API.csproj" "src/Gateway/Gateway.API/"
COPY "src/Services/Identity/Identity.API/Identity.API.csproj" "src/Services/Identity/Identity.API/"
COPY "src/Services/Identity/Identity.Data/Identity.Data.csproj" "src/Services/Identity/Identity.Data/"
COPY "src/Services/Identity/Identity.Models/Identity.Models.csproj" "src/Services/Identity/Identity.Models/"
COPY "src/Services/Identity/Identity.Services/Identity.Services.csproj" "src/Services/Identity/Identity.Services/"
COPY "src/Services/Identity/Identity.Tests/Identity.Tests.csproj" "src/Services/Identity/Identity.Tests/"
COPY "src/Services/Notifications/Notifications.API/Notifications.API.csproj" "src/Services/Notifications/Notifications.API/"
COPY "src/Services/Notifications/Notifications.Services/Notifications.Services.csproj" "src/Services/Notifications/Notifications.Services/"
COPY "src/Services/Notifications/Notifications.Tests/Notifications.Tests.csproj" "src/Services/Notifications/Notifications.Tests/"
COPY "Local.dcproj" "Local.dcproj"

RUN dotnet restore "Mojira.sln"

COPY . .

FROM build AS publish
WORKDIR "src/Gateway/Gateway.API"
RUN dotnet publish --no-restore -c Release -o /release

FROM base AS final
WORKDIR /app
COPY --from=publish /release .
ENTRYPOINT ["dotnet", "Gateway.API.dll"]