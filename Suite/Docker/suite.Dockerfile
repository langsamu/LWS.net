FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Suite/Console -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
COPY --from=build /app /app
ENTRYPOINT ["dotnet", "/app/Console.dll"]
