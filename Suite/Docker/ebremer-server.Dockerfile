FROM maven:3-eclipse-temurin-25 AS build
RUN git clone --depth 1 https://github.com/ebremer/lws-server.git /src
WORKDIR /src
RUN mvn -B -ntp package -DskipTests

FROM eclipse-temurin:25-jre
ADD https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/download/v2.31.1/opentelemetry-javaagent.jar /opentelemetry-javaagent.jar
COPY --from=build /src/target/lws-server.jar /lws-server.jar
ENTRYPOINT ["java", "-javaagent:/opentelemetry-javaagent.jar", "-jar", "/lws-server.jar"]
