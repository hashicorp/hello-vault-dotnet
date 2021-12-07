#!/bin/sh

for i in {1..60};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -d master /home/db-data/populate.sql
    if [ $? -eq 0 ]
    then
        echo "database is populated & ready!"
        exit 0
    else
        echo "waiting for database to be ready..."
        sleep 1
    fi
done

echo "error: could not populate the database after 60 seconds"
exit 1
