FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ArtifactStore.Migration.CLI/ArtifactStore.Migration.CLI.csproj", "src/ArtifactStore.Migration.CLI/"]
COPY ["src/ArtifactStore.Infrastructure/ArtifactStore.Infrastructure.csproj", "src/ArtifactStore.Infrastructure/"]
COPY ["src/ArtifactStore.Application/ArtifactStore.Application.csproj", "src/ArtifactStore.Application/"]
COPY ["src/ArtifactStore.Domain/ArtifactStore.Domain.csproj", "src/ArtifactStore.Domain/"]
COPY ["src/ArtifactStore.Auth.Infrastructure/ArtifactStore.Auth.Infrastructure.csproj", "src/ArtifactStore.Auth.Infrastructure/"]
COPY ["src/ArtifactStore.Auth.Application/ArtifactStore.Auth.Application.csproj", "src/ArtifactStore.Auth.Application/"]
COPY ["src/ArtifactStore.Auth.Domain/ArtifactStore.Auth.Domain.csproj", "src/ArtifactStore.Auth.Domain/"]
RUN dotnet restore "src/ArtifactStore.Migration.CLI/ArtifactStore.Migration.CLI.csproj"

COPY ["src/ArtifactStore.Migration.CLI/", "src/ArtifactStore.Migration.CLI/"]
COPY ["src/ArtifactStore.Infrastructure/", "src/ArtifactStore.Infrastructure/"]
COPY ["src/ArtifactStore.Application/", "src/ArtifactStore.Application/"]
COPY ["src/ArtifactStore.Domain/", "src/ArtifactStore.Domain/"]
COPY ["src/ArtifactStore.Auth.Infrastructure/", "src/ArtifactStore.Auth.Infrastructure/"]
COPY ["src/ArtifactStore.Auth.Application/", "src/ArtifactStore.Auth.Application/"]
COPY ["src/ArtifactStore.Auth.Domain/", "src/ArtifactStore.Auth.Domain/"]
WORKDIR "/src/src/ArtifactStore.Migration.CLI"
RUN dotnet build "./ArtifactStore.Migration.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactStore.Migration.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactStore.Migration.CLI.dll"]
