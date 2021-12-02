# Unfortunately, the default mcr.microsoft.com/mssql/server image currently
# does not support macbook M1 architecture.

# mcr.microsoft.com/azure-sql-edge is a minimalistic version of mssql/server
# image wich works on all known platforms.
FROM mcr.microsoft.com/azure-sql-edge:latest

# Copy entrypoint & setup scripts
RUN mkdir -p /app

COPY . /app
