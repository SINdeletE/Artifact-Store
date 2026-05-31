FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ArtifactStore.API/ArtifactStore.API.csproj", "src/ArtifactStore.API/"]
COPY ["src/ArtifactStore.Application/ArtifactStore.Application.csproj", "src/ArtifactStore.Application/"]
COPY ["src/ArtifactStore.Domain/ArtifactStore.Domain.csproj", "src/ArtifactStore.Domain/"]
COPY ["src/ArtifactStore.Infrastructure/ArtifactStore.Infrastructure.csproj", "src/ArtifactStore.Infrastructure/"]
RUN dotnet restore "src/ArtifactStore.API/ArtifactStore.API.csproj"

COPY ["src/ArtifactStore.API/", "src/ArtifactStore.API/"]
COPY ["src/ArtifactStore.Application/", "src/ArtifactStore.Application/"]
COPY ["src/ArtifactStore.Domain/", "src/ArtifactStore.Domain/"]
COPY ["src/ArtifactStore.Infrastructure/", "src/ArtifactStore.Infrastructure/"]
WORKDIR "/src/src/ArtifactStore.API"
RUN dotnet build "./ArtifactStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactStore.API.dll"]
