# Galaxy Football Backend

Galaxy Football Backend is a .NET 8 backend for the Galaxy Football game simulation. The repository contains the HTTP API, application logic for league and match processing, domain models, database access, infrastructure integrations, and unit tests.

## Project Structure

- `src/galaxy-football-server` - ASP.NET Core API host, controllers, background jobs, configuration, and startup.
- `src/application` - game rules, factories, match engine, league calculations, and scripts.
- `src/domain` - domain entities such as clubs, teams, players, leagues, matches, and users.
- `src/infrastructure/Database.Layer` - Entity Framework Core database layer and migrations.
- `src/infrastructure/Cloudflare.Library` - Cloudflare R2 / S3-compatible storage helpers used for log and file handling.
- `test/unit-tests/Application.Tests` - xUnit tests for the application layer.

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core with PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- Serilog for structured logging
- Cloudflare R2 via the AWS S3 SDK
- xUnit for unit tests

## Prerequisites

Install the following before building locally:

- .NET SDK 8.0
- PostgreSQL database reachable from the machine or container running the API
- Docker Desktop or Docker Engine if you want to build and deploy the container image

## Required Configuration

The server is configured from environment variables. For nested .NET configuration keys, use double underscores in environment variables.

### Database

These values are required at startup:

- `GALAXY_FOOTBALL_DB_NAME`
- `GALAXY_FOOTBALL_DB_USER`
- `GALAXY_FOOTBALL_DB_PASSWORD`
- `GALAXY_FOOTBALL_DB_URL`
- `GALAXY_FOOTBALL_DB_PORT` - optional, defaults to `5432`

### Authentication and Internal Jobs

- `GALAXY_FOOTBALL_JWT_KEY` - required for JWT token generation and validation
- `GALAXY_FOOTBALL_API_KEY` - required for the internal daily job endpoint (`X-API-KEY` header)

### Cloudflare R2 / Log Uploading

The application registers `LogUploaderService` on startup, so these values are also required unless the service is changed in code:

- `CLOUDFLARE__ACCESS_KEY`
- `CLOUDFLARE__SECRET_KEY`
- `CLOUDFLARE__S3_URL`
- `CLOUDFLARE__BUCKET_NAME`

Notes:

- Setting `CLOUDFLARE__BUCKET_NAME=DISABLED` skips the actual upload step, but the access key, secret key, and S3 URL are still required during service construction.
- Production logging writes rolling files under `%TMPDIR%/galaxyfootball/logs/`. Set `TMPDIR` to a writable directory in your deployment environment.

## Local Development

### Restore and build

From the repository root:

```powershell
dotnet restore galaxy-football-backend.sln
dotnet build galaxy-football-backend.sln
```

### Run the API

Set the required environment variables, then run:

```powershell
dotnet run --project src/galaxy-football-server/galaxy-football-server.csproj
```

Useful defaults from the codebase:

- In development, launch settings use `http://localhost:5097`
- In container or server environments, the app listens on `http://0.0.0.0:${PORT}`
- If `PORT` is not set, the app uses `8080`

### Database behavior on startup

At startup the API:

1. Builds the PostgreSQL connection string from environment variables.
2. Runs Entity Framework Core migrations automatically.
3. Seeds the `Game` table with an initial row if no game exists.
4. Updates the stored database version when the game metadata version changes.

This means the target database user needs permission to apply migrations.

### Run tests

```powershell
dotnet test galaxy-football-backend.sln
```

If you only want the application tests:

```powershell
dotnet test test/unit-tests/Application.Tests/Application.Tests.csproj
```

## Building a Release Artifact

To create a publish output without Docker:

```powershell
dotnet publish src/galaxy-football-server/galaxy-football-server.csproj -c Release -o .\publish
```

Run the published app:

```powershell
dotnet .\publish\galaxy-football-server.dll
```

## Docker Build and Run

The repository includes a multi-stage Dockerfile that builds the solution with the .NET 8 SDK image and runs the API on the .NET 8 ASP.NET runtime image.

### Build the image

```powershell
docker build -t galaxy-football-backend:latest .
```

### Run the container

```powershell
docker run --rm -p 8080:8080 \
  -e PORT=8080 \
  -e GALAXY_FOOTBALL_DB_NAME=galaxyfootball \
  -e GALAXY_FOOTBALL_DB_USER=postgres \
  -e GALAXY_FOOTBALL_DB_PASSWORD=change-me \
  -e GALAXY_FOOTBALL_DB_URL=your-postgres-host \
  -e GALAXY_FOOTBALL_DB_PORT=5432 \
  -e GALAXY_FOOTBALL_JWT_KEY=replace-with-a-long-random-secret \
  -e GALAXY_FOOTBALL_API_KEY=replace-with-an-internal-job-key \
  -e CLOUDFLARE__ACCESS_KEY=your-access-key \
  -e CLOUDFLARE__SECRET_KEY=your-secret-key \
  -e CLOUDFLARE__S3_URL=https://your-account-id.r2.cloudflarestorage.com \
  -e CLOUDFLARE__BUCKET_NAME=your-bucket \
  -e TMPDIR=/tmp \
  galaxy-football-backend:latest
```

## Deployment Notes

### Direct deployment

For a VM or bare host deployment:

1. Install the .NET 8 runtime.
2. Provision a PostgreSQL database.
3. Export the required environment variables.
4. Run `dotnet publish`.
5. Start `galaxy-football-server.dll` behind a process manager such as `systemd`, NSSM, or a platform-specific service manager.
6. Put the service behind a reverse proxy or ingress that terminates TLS.

### Container deployment

For a container platform such as Azure Container Apps, AWS ECS, Fly.io, Railway, or Kubernetes:

1. Build and push the image from the included Dockerfile.
2. Inject the required environment variables as secrets or configuration.
3. Expose container port `8080`.
4. Ensure the container can reach PostgreSQL.
5. Mount or provide a writable temp directory and set `TMPDIR`.
6. Allow the application to run database migrations on startup.

## Operational Notes

- The API enables CORS with `AllowAnyOrigin`, `AllowAnyHeader`, and `AllowAnyMethod`. Tighten this for production.
- `UseHttpsRedirection()` is enabled, so deployments should be aware of proxy and forwarded-header behavior.
- The current login endpoint contains placeholder credentials in code. Review and replace that flow before exposing the service publicly.
- Log uploads target Cloudflare R2 using an S3-compatible client.

## Common Commands

```powershell
dotnet restore galaxy-football-backend.sln
dotnet build galaxy-football-backend.sln
dotnet test galaxy-football-backend.sln
dotnet run --project src/galaxy-football-server/galaxy-football-server.csproj
docker build -t galaxy-football-backend:latest .
```