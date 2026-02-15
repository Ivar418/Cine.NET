ARG DOTNET_SDK_VERSION=10.0

# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /app/out

# ===== Runtime stage =====
FROM nginx:alpine

# SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy static files
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
