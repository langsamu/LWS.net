Each stack below also starts an Aspire dashboard. Traces and logs from the suite and the server are in the dashboard at http://localhost:18888.

# Run suite console against ebremer/lws-server
```bash
docker compose -f Suite/Docker/ebremer-suite.yaml up
```
Runs the latest default branch, not pinned. Open mode is the Java system property `lws.dev.open=true`. The storage root is `/`. Server traces come from the OpenTelemetry Java agent.

# Run suite console against linkedwebstorage/lwsd
```bash
docker compose -f Suite/Docker/lwsd-suite.yaml up
```
Runs the latest default branch, not pinned, with authentication disabled (`--no-auth`). The storage is mounted at `/lws/`, so the base URI and the storage root are `http://lwsd:3126/lws/`. Server traces come from the Node.js OpenTelemetry auto-instrumentation.

# Run suite console against sparq-org/sparq
```bash
docker compose -f Suite/Docker/sparq-suite.yaml up
```
Runs the latest default branch, not pinned, as a debug build of `sparq-lws-core`. Open mode is `SOLID_SERVER_OPEN_MODE=1`. The storage root is `/`. sparq has no OpenTelemetry exporter. An OpenTelemetry eBPF Instrumentation (OBI) sidecar creates one server span for each HTTP request. The sidecar runs privileged and shares the sparq process namespace, so it needs a Linux kernel with eBPF.

# Run suite console against jeswr/CommunitySolidServer (feat/lws)
```bash
docker compose -f Suite/Docker/css-suite.yaml up
```
Runs the pinned commit `768de49` of the `feat/lws` branch. The config `Suite/Docker/css-lws-open.json` sets allow-all authorization and root storage, so the storage root is `/`. Server traces come from the Node.js OpenTelemetry auto-instrumentation.

# Run suite console against halcyon-project/Halcyon
```bash
docker compose -f Suite/Docker/halcyon-suite.yaml up
```
Runs the pinned commit `e94e4d0`. Halcyon has no open mode, so `Suite/Docker/halcyon/open-mode.patch` adds the setting `:LWSOpenMode`. This setting gives the public agent full access. It is for development only and is enabled in `Suite/Docker/halcyon/settings.ttl`. The storage root is `/W3Clws/`, so the base URI is `http://halcyon:8888/W3Clws/`. The build caches and retries Maven downloads, because the Stony Brook Nexus repository can return 403. Server traces come from the OpenTelemetry Java agent.

# Run suite console against all servers in parallel
```bash
docker compose -f Suite/Docker/compose.all.yaml up --build
```
This command starts the five servers, one suite container for each server, and one shared Aspire dashboard at http://localhost:18888. Each resource has its own name and the attribute `lws.target`:

| Server | Server resource | Suite resource | `lws.target` |
| --- | --- | --- | --- |
| CommunitySolidServer | `server-css` | `test-css` | `css` |
| ebremer/lws-server | `server-ebremer` | `test-ebremer` | `ebremer` |
| Halcyon | `server-halcyon` | `test-halcyon` | `halcyon` |
| lwsd | `server-lwsd` | `test-lwsd` | `lwsd` |
| sparq | `server-sparq` (from OBI) | `test-sparq` | `sparq` |

The `include` element needs Docker Compose 2.20 or later. The joint stack and a single-server stack use the same dashboard ports, so they cannot run at the same time. Stop one stack before you start the other. The sparq OBI sidecar is privileged and needs a Linux kernel with eBPF. On Docker Desktop it can fail to attach, and the sparq server then has no server spans.

# Run suite console
```pwsh
dotnet run --project Suite\Console --output Detailed --Suite:BaseUri=http://localhost:8080/
```

# Run suite tests
```pwsh
$env:Suite__BaseUri = 'http://localhost:8080/'
dotnet run --project Suite\Test --output Detailed
```
