FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/galaxy-football-server/galaxy-football-server.csproj ./galaxy-football-server/
RUN dotnet restore ./galaxy-football-server/galaxy-football-server.csproj

# Copy everything else and build
COPY src/galaxy-football-server/. ./galaxy-football-server/
WORKDIR /app/galaxy-football-server
RUN dotnet publish -c Release -o /out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose port (change if your app uses a different port)
EXPOSE 8080

# Set environment variables (optional)
# ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "galaxy-football-server.dll"]