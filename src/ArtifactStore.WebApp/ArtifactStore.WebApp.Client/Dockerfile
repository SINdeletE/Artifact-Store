FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ArtifactStore.WebApp/ArtifactStore.WebApp.Client/ArtifactStore.WebApp.Client.csproj", "src/ArtifactStore.WebApp/ArtifactStore.WebApp.Client/"]
RUN dotnet restore "src/ArtifactStore.WebApp/ArtifactStore.WebApp.Client/ArtifactStore.WebApp.Client.csproj"
COPY . .
WORKDIR "/src/src/ArtifactStore.WebApp/ArtifactStore.WebApp.Client"
RUN dotnet build "./ArtifactStore.WebApp.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactStore.WebApp.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactStore.WebApp.Client.dll"]
