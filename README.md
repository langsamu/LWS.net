# Run suite console against ebremer/lws-server
```bash
docker compose -f Suite/Docker/ebremer-suite.yaml up
```
Traces and logs from the suite and the server are in the Aspire dashboard at http://localhost:18888.

# Run suite console
```pwsh
dotnet run --project Suite\Console --output Detailed --Suite:BaseUri=http://localhost:8080/
```

# Run suite tests
```pwsh
$env:Suite__BaseUri = 'http://localhost:8080/'
dotnet run --project Suite\Test --output Detailed
```
