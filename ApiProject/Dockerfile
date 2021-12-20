FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj
COPY *.csproj /app
RUN dotnet restore

# Copy all files
COPY . /app
RUN dotnet publish -c Release -o out

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl

HEALTHCHECK \
    --start-period=1s \
    --interval=1s \
    --timeout=1s \
    --retries=30 \
        CMD curl --fail -s http://localhost:8080/healthcheck || exit 1

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "WebApi.dll" ]
