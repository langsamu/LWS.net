# Run suite console against ebremer/lws-server
```bash
docker compose -f Suite/Docker/ebremer-suite.yaml up
```

# Run suite console
```pwsh
dotnet run --project Suite\Console --output Detailed --Suite:BaseUri=http://localhost:8080/
```

# Run suite tests
```pwsh
$env:Suite__BaseUri = 'http://localhost:8080/'
dotnet run --project Suite\Test --output Detailed
```
