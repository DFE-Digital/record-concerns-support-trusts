#!/bin/bash

# exit on failures
set -e
set -o pipefail

MSSQL_INITIAL_DATABASE="${MSSQL_INITIAL_DATABASE:?}"

echo "Ensuring initial database exists ..."

until /opt/mssql-tools/bin/sqlcmd \
  -S db \
  -U sa \
  -P "$MSSQL_SA_PASSWORD" \
  -d master \
  -Q "IF DB_ID('$MSSQL_INITIAL_DATABASE') IS NULL CREATE DATABASE [$MSSQL_INITIAL_DATABASE]"
do
  echo "not ready yet..."
  sleep 1
done

echo "Database $MSSQL_INITIAL_DATABASE is ready ..."
