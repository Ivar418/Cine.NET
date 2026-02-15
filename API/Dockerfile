ARG DOTNET_SDK_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR /src

# Copy csproj for better layer caching
ARG PROJECT_NAME=API
COPY ${PROJECT_NAME}.csproj .
RUN dotnet restore ${PROJECT_NAME}.csproj

# Copy everything else and build
COPY . .
RUN dotnet test ${PROJECT_NAME}.csproj --no-restore
RUN dotnet publish ${PROJECT_NAME}.csproj -c Release -o /app/out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_SDK_VERSION} AS runtime
WORKDIR /app


RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*

# Bind to 8080 inside container
ENV ASPNETCORE_URLS=http://+:8080 \
DOTNET_RUNNING_IN_CONTAINER=true \
	DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_EnableDiagnostics=0
EXPOSE 8080

COPY --from=build /app/out ./

# Run as non-root user for better security
RUN useradd -m appuser \
    && chown -R appuser:appuser /app
USER appuser

# Optional: Set up health check endpoint
HEALTHCHECK --interval=10s --timeout=3s --retries=10 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "API.dll"]