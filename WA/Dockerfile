ARG DOTNET_SDK_VERSION=10.0
ARG ENVIRONMENT

# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR .

ARG ENVIRONMENT
RUN test -n "$ENVIRONMENT" || (echo "❌ ENVIRONMENT build-arg is required, try running with --build-arg ENVIRONMENT=Development OR add it to your build args" && exit 1)
RUN echo "🔧 Building for environment: $ENVIRONMENT"

# copy csproj first (sneller cachen)
COPY WA/WA.csproj WA/
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
RUN dotnet restore WA/WA.csproj

# copy rest
COPY WA/ WA/
COPY SharedLibrary/ SharedLibrary/
RUN dotnet publish WA/WA.csproj \
    -c Release \
    -p:EnvironmentName=${ENVIRONMENT} \
    -o /app/out

RUN ls -R /app/out/wwwroot

# ===== Runtime stage =====
FROM nginx:alpine

COPY WA/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
