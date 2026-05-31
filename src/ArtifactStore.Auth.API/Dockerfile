FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ArtifactStore.Auth.API/ArtifactStore.Auth.API.csproj", "src/ArtifactStore.Auth.API/"]
COPY ["src/ArtifactStore.Auth.Domain/ArtifactStore.Auth.Domain.csproj", "src/ArtifactStore.Auth.Domain/"]
COPY ["src/ArtifactStore.Auth.Application/ArtifactStore.Auth.Application.csproj", "src/ArtifactStore.Auth.Application/"]
COPY ["src/ArtifactStore.Auth.Infrastructure/ArtifactStore.Auth.Infrastructure.csproj", "src/ArtifactStore.Auth.Infrastructure/"]
RUN dotnet restore "src/ArtifactStore.Auth.API/ArtifactStore.Auth.API.csproj"
COPY ["src/ArtifactStore.Auth.API/", "src/ArtifactStore.Auth.API/"]
COPY ["src/ArtifactStore.Auth.Domain/", "src/ArtifactStore.Auth.Domain/"]
COPY ["src/ArtifactStore.Auth.Application/", "src/ArtifactStore.Auth.Application/"]
COPY ["src/ArtifactStore.Auth.Infrastructure/", "src/ArtifactStore.Auth.Infrastructure/"]
WORKDIR "/src/src/ArtifactStore.Auth.API"
RUN dotnet build "./ArtifactStore.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactStore.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactStore.Auth.API.dll"]
