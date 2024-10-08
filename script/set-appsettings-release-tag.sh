#!/bin/bash

# exit on failures
set -e
set -o pipefail

apt-get update && apt-get install jq -y

RELEASE_TAG="$1"

APP_SETTINGS_FILES=(
  appsettings.json
  appsettings.Development.json
  appsettings.Staging.json
  appsettings.Production.json
)

for app_settings_file in "${APP_SETTINGS_FILES[@]}"
do
  echo "$(cat "$app_settings_file" | jq --arg releasetag "$RELEASE_TAG" '.ConcernsCasework.ReleaseTag=$releasetag')" > "$app_settings_file"
done
