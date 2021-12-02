# Unfortunately, the default mcr.microsoft.com/mssql/server image currently
# does not support macbook M1 architecture.

# mcr.microsoft.com/azure-sql-edge is a minimalistic version of mssql/server
# image wich works on all known platforms.
FROM mcr.microsoft.com/azure-sql-edge:latest

# Copy sqlcmd and the required from mssql-tools image
USER root

RUN mkdir -p /opt/mssql-tools/bin && \
    mkdir -p /lib64 \
    mkdir -p /lib/x86_64-linux-gnu && \
    mkdir -p /usr/lib/x86_64-linux-gnu \
    mkdir -p /usr/lib/x86_64-linux-gnu/sasl2

COPY --from=mcr.microsoft.com/mssql-tools \
        /opt/mssql-tools/bin/sqlcmd \
        /opt/mssql-tools/bin/
COPY --from=mcr.microsoft.com/mssql-tools \
        /lib64/* \
        /lib64/
COPY --from=mcr.microsoft.com/mssql-tools \
        /lib/x86_64-linux-gnu/* \
        /lib/x86_64-linux-gnu/
COPY --from=mcr.microsoft.com/mssql-tools \
        /usr/lib/x86_64-linux-gnu/* \
        /usr/lib/x86_64-linux-gnu/
COPY --from=mcr.microsoft.com/mssql-tools \
        /usr/lib/x86_64-linux-gnu/sasl2/* \
        /usr/lib/x86_64-linux-gnu/sasl2/

# Copy entrypoint & setup scripts
RUN mkdir -p /app

COPY . /app
