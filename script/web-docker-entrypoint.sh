#!/bin/bash

# exit on failures
set -e
set -o pipefail

ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection:?}

declare -A mysqlconn

for keyvaluepair in $(echo "$ConnectionStrings__DefaultConnection" | sed "s/ //g; s/;/ /g")
do
  IFS=" " read -r -a ARR <<< "${keyvaluepair//=/ }"
  mysqlconn[${ARR[0]}]=${ARR[1]}
done

echo "Running database migrations ..."
until /opt/mssql-tools18/bin/sqlcmd -S "${mysqlconn[Server]}" -U "${mysqlconn[UId]}" -P "${mysqlconn[Password]}" -d "${mysqlconn[Database]}" -C -i /app/SQL/DbMigrationScript.sql -o /app/SQL/DbMigrationScriptOutput.txt
do
  cat /app/SQL/DbMigrationScriptOutput.txt
  echo "Retrying database migrations ..."
  sleep 5
done

exec "$@"
