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

# Run suite console
```pwsh
dotnet run --project Suite\Console --output Detailed --Suite:BaseUri=http://localhost:8080/
```

# Run suite tests
```pwsh
$env:Suite__BaseUri = 'http://localhost:8080/'
dotnet run --project Suite\Test --output Detailed
```
