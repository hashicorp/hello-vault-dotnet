#!/bin/sh

# There are two docker-compose files provided:
#   - docker-compose.arm64.yaml (to run on arm64 architectures / Apple M1)
#   - docker-compose.yaml       (to run on all other architectures)
#
# The arm64 variant uses mcr.microsoft.com/azure-sql-edge and
# mcr.microsoft.com/mssql-tools images in place of the default
# mcr.microsoft.com/mssql/server image. This is to work around mssql/server's
# incompatibility with arm64 architecture
if [[ `uname -m` == 'arm64' ]]; then
    docker compose -f docker-compose.arm64.yaml down
    docker compose -f docker-compose.arm64.yaml build
    docker compose -f docker-compose.arm64.yaml up -d
else
    docker compose -f docker-compose.yaml down
    docker compose -f docker-compose.yaml build
    docker compose -f docker-compose.yaml up -d
fi
