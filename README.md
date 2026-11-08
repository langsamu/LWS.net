# Run suite console against ebremer/lws-server
```bash
docker compose -f Suite/Docker/ebremer-suite.yaml up
```

# Run suite console against linkedwebstorage/lwsd
```bash
docker compose -f Suite/Docker/lwsd-suite.yaml up
```

Traces and logs from the suite and the server are in the Aspire dashboard at http://localhost:18888.

# Run suite console against sparq-org/sparq
```bash
docker compose -f Suite/Docker/sparq-suite.yaml up
```
Traces and logs from the suite and the server are in the Aspire dashboard at http://localhost:18888. Server traces come from an OpenTelemetry eBPF Instrumentation sidecar.

# Run suite console against jeswr/CommunitySolidServer (feat/lws)
```bash
docker compose -f Suite/Docker/css-suite.yaml up
```
Runs the pinned feat/lws commit with an allow-all config and storage at the server root. Traces and logs are in the Aspire dashboard at http://localhost:18888.

# Run suite console against halcyon-project/Halcyon
```bash
docker compose -f Suite/Docker/halcyon-suite.yaml up
```
Traces and logs from the suite and the server are in the Aspire dashboard at http://localhost:18888. Server traces come from the OpenTelemetry Java agent. Halcyon runs in dev-only open mode (`:LWSOpenMode true`) with a source patch (`Suite/Docker/halcyon/open-mode.patch`) against Halcyon pinned by commit SHA. Its storage root is `/W3Clws/`.

# Run suite console
```pwsh
dotnet run --project Suite\Console --output Detailed --Suite:BaseUri=http://localhost:8080/
```

# Run suite tests
```pwsh
$env:Suite__BaseUri = 'http://localhost:8080/'
dotnet run --project Suite\Test --output Detailed
```
