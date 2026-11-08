FROM maven:3.9-eclipse-temurin-25 AS build
ARG HALCYON_SHA=e94e4d0f79f1100629935ff68400f4aa35367655
RUN git init /src && git -C /src fetch --depth 1 https://github.com/halcyon-project/Halcyon.git ${HALCYON_SHA} && git -C /src checkout FETCH_HEAD
COPY halcyon/open-mode.patch /open-mode.patch
WORKDIR /src
RUN sed -i 's/\r$//' /open-mode.patch && git apply -p2 /open-mode.patch
RUN --mount=type=cache,target=/root/.m2 for i in 1 2 3 4 5; do if mvn -q -B -ntp -Daether.connector.basic.connectTimeout=60000 -Dmaven.test.skip=true -pl Halcyon -am package; then cp Halcyon/target/Halcyon-*.jar /halcyon.jar && exit 0; fi; sleep $((i * 30)); done; exit 1

FROM eclipse-temurin:25-jre
ADD https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/download/v2.31.1/opentelemetry-javaagent.jar /opentelemetry-javaagent.jar
COPY --from=build /halcyon.jar /opt/halcyon.jar
COPY halcyon/application.yml halcyon/settings.ttl /opt/halcyon/
RUN mkdir -p /data/lws/W3Clws /opt/halcyon/logs
WORKDIR /opt/halcyon
ENTRYPOINT ["java", "-javaagent:/opentelemetry-javaagent.jar", "-jar", "/opt/halcyon.jar", "--spring.config.location=file:/opt/halcyon/application.yml"]
