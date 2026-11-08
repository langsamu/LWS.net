FROM rust:1.97-slim-bookworm AS build
RUN apt-get update && apt-get install -y --no-install-recommends git ca-certificates pkg-config cmake && rm -rf /var/lib/apt/lists/*
RUN git clone --depth 1 https://github.com/sparq-org/sparq.git /src
WORKDIR /src
RUN cargo build --locked -p sparq-lws-core

FROM debian:bookworm-slim
COPY --from=build /src/target/debug/sparq-lws-core /usr/local/bin/sparq-lws-core
ENV SOLID_SERVER_PROTOCOL=lws
ENV SOLID_SERVER_OPEN_MODE=1
ENV SOLID_SERVER_BIND=0.0.0.0:3000
ENTRYPOINT ["sparq-lws-core"]
