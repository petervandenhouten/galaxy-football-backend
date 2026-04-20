FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY src/galaxy-football-server/galaxy-football-server.sln .
COPY src/galaxy-football-server/galaxy-football-server.csproj src/galaxy-football-server/
COPY src/infrastructure/Cloudflare.Library/Cloudflare.Library.csproj src/infrastructure/Cloudflare.Library/
COPY src/infrastructure/Git.Library/Git.Library.csproj src/infrastructure/Git.Library/

# Restore dependencies as distinct layers
RUN dotnet restore galaxy-football-server/galaxy-football-server.sln

# Copy the rest of the source code
COPY src/ src/

WORKDIR /app/src/galaxy-football-server
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "galaxy-football-server.dll"]