FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY src/galaxy-football-server/galaxy-football-server.csproj src/galaxy-football-server/
COPY src/infrastructure/Cloudflare.Library/Cloudflare.Library.csproj src/infrastructure/Cloudflare.Library/

# Restore dependencies
RUN dotnet restore src/galaxy-football-server/galaxy-football-server.csproj

# Copy the rest of the source code
COPY src/ src/

WORKDIR /app/src/galaxy-football-server
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "galaxy-football-server.dll"]