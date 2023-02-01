#!/bin/sh
# Copyright (c) HashiCorp, Inc.
# SPDX-License-Identifier: MPL-2.0


for i in $(seq 1 30)
do
    if /opt/mssql-tools/bin/sqlcmd \
            -S "tcp:${DATABASE_HOSTNAME},${DATABASE_PORT}" \
            -U sa \
            -P "${SA_PASSWORD}" \
            -d master \
            -i /home/database/populate.sql;
    then
        echo "database is populated & ready!"
        exit 0
    else
        echo "waiting for database to be ready... $i"
        sleep 1
    fi
done

echo "error: could not populate the database after 30 iterations"
exit 1
