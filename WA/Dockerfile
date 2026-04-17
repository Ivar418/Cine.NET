ARG DOTNET_SDK_VERSION=10.0

# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR .

# copy csproj first (sneller cachen)
COPY WA/WA.csproj WA/
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
RUN dotnet restore WA/WA.csproj

# copy rest
COPY WA/ WA/
COPY SharedLibrary/ SharedLibrary/
RUN dotnet publish WA/WA.csproj \
    -c Release \
    --no-restore \
    -o /app/out

# ===== Runtime stage =====
FROM nginx:alpine

# The environment is set at build time, but can be overridden at runtime via blazor-environment header
# We use a template for nginx.conf and envsubst to replace the environment name
COPY WA/nginx.conf /etc/nginx/conf.d/default.conf.template

COPY --from=build /app/out/wwwroot /usr/share/nginx/html

# Default to Production if not set
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80
CMD ["sh", "-c", "envsubst '${ASPNETCORE_ENVIRONMENT}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
