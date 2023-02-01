# Copyright (c) HashiCorp, Inc.
# SPDX-License-Identifier: MPL-2.0

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj
COPY *.csproj /app
RUN dotnet restore

# Copy all files
COPY . /app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

RUN apt-get update && \
    apt-get install -y --no-install-recommends curl

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "WebService.dll" ]

EXPOSE 80

HEALTHCHECK \
    --start-period=1s \
    --interval=1s \
    --timeout=1s \
    --retries=30 \
        CMD curl --fail -s --header "Content-Length: 0" http://localhost/Healthcheck || exit 1
