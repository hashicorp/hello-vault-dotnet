#!/bin/sh

for i in {1..120};
do
    /opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U sa -P "${SA_PASSWORD}" -d master -i /home/database/populate.sql
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
