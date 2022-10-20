#!/bin/bash

# exit on failures
set -e
set -o pipefail

MSSQL_INITIAL_DATABASE="${MSSQL_INITIAL_DATABASE:?}"

echo "CREATE DATABASE $MSSQL_INITIAL_DATABASE;" > ./setup.sql
echo "GO" >> ./setup.sql

echo "Creating initial database ..."
until /opt/mssql-tools/bin/sqlcmd -S db -U sa -P "$MSSQL_SA_PASSWORD" -d master -i ./setup.sql
do
  echo "not ready yet..."
  sleep 1
done

rm ./setup.sql

echo "Created database $MSSQL_INITIAL_DATABASE ..."
