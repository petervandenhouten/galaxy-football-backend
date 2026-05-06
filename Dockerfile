FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY galaxy-football-backend.sln ./
COPY src/galaxy-football-server/galaxy-football-server.csproj src/galaxy-football-server/
COPY src/infrastructure/Cloudflare.Library/Cloudflare.Library.csproj src/infrastructure/Cloudflare.Library/
COPY src/domain/domain.csproj src/domain/
COPY src/infrastructure/Database.Layer/Database.Layer.csproj src/infrastructure/Database.Layer/
COPY src/infrastructure/Git.Library/Git.Library.csproj src/infrastructure/Git.Library/
COPY src/application/application.csproj src/application/
COPY test/unit-tests/Application.Tests/Application.Tests.csproj test/unit-tests/Application.Tests/


# Restore dependencies as distinct layers
RUN dotnet restore galaxy-football-backend.sln

# Copy the rest of the source code
COPY src/ src/

WORKDIR /app/src/galaxy-football-server
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "galaxy-football-server.dll"]