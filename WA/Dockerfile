FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src

ENV ASPNETCORE_URLS=http://0.0.0.0:5031
EXPOSE 5031

CMD ["dotnet", "run", "--no-launch-profile", "--urls", "http://0.0.0.0:5031"]