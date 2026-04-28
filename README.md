# Flight Log .NET

Flight Log is a small web application for managing aeroclub flights.

It provides:
- registering new takeoffs (towplane + optional glider)
- listing flights currently in the air
- landing active flights
- flight report overview
- CSV export of report data

## Tech Stack

- Backend: ASP.NET Core (`FlightLogNet`)
- Frontend: Aurelia + webpack (`frontend`)
- Database: local SQLite (default; auto-initialized in Development)

## Repository Structure

- `FlightLogNet` - backend API + static hosting
- `frontend` - Aurelia frontend app
- `FlightLogNet.Tests` - xUnit tests

## Prerequisites

- .NET SDK 10
- Node.js (legacy Aurelia/webpack setup; Node 18 recommended)
- npm

## Run Backend

From repository root:

```bash
cd /home/daniellopata/flight-log-dotnet
DOTNET_CLI_HOME=/tmp/dotnet_cli_home dotnet run --project FlightLogNet/FlightLogNet.csproj
```

Backend starts on the port shown in logs, for example:

- `http://localhost:44313`

Use that URL as API base URL for frontend.

## Run Frontend (Development)

From `frontend` directory:

```bash
cd /home/daniellopata/flight-log-dotnet/frontend
npm install --legacy-peer-deps
NODE_OPTIONS=--openssl-legacy-provider npx au run
```

Frontend dev server runs at:

- `http://localhost:8080`

## Frontend Compatibility Notes

This template uses older webpack tooling, so newer Node versions can fail with OpenSSL errors.

If you hit `ERR_OSSL_EVP_UNSUPPORTED`, use:

```bash
NODE_OPTIONS=--openssl-legacy-provider npx au run
```

If npm fails with peer dependency resolution (`ERESOLVE`), use:

```bash
npm install --legacy-peer-deps
```

Optional project-local npm config:

```bash
echo "legacy-peer-deps=true" > .npmrc
echo "node-options=--openssl-legacy-provider" >> .npmrc
```

## Run Tests

From repository root:

```bash
cd /home/daniellopata/flight-log-dotnet
DOTNET_CLI_HOME=/tmp/dotnet_cli_home dotnet test FlightLogNet.Tests/FlightLogNet.Tests.csproj
```

## API Endpoints (Main)

- `GET /flight/inair` - list active flights
- `POST /flight/takeoff` - create takeoff
- `POST /flight/land` - land a flight
- `GET /flight/report` - report data
- `GET /flight/export` - CSV export
- `GET /airplane` - list club airplanes
- `GET /user` - list club users

## Typical Local Workflow

1. Start backend.
2. Start frontend.
3. Open `http://localhost:8080`.
4. Create takeoffs, monitor active flights, land flights, and export CSV.
