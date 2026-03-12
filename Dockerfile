ARG DOTNET_SDK_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR /src

# Copy csproj files (keep structure for correct ProjectReference paths)
COPY API/API.csproj API/
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY NuGet.Container.config ./NuGet.Config


RUN dotnet restore API/API.csproj --configfile ./NuGet.Config

# Copy everything else
COPY API/ API/
COPY SharedLibrary/ SharedLibrary/

RUN dotnet test API/API.csproj --no-restore
RUN dotnet publish API/API.csproj -c Release -o /app/out /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_SDK_VERSION} AS runtime
WORKDIR /app

RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

COPY --from=build /app/out ./

RUN useradd -m appuser \
 && chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=10s --timeout=3s --retries=10 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "API.dll"]