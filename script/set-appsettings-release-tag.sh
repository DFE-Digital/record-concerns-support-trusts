#!/bin/bash

# exit on failures
set -e
set -o pipefail

RELEASE_TAG="$1"

APP_SETTINGS_FILES=(
  appsettings.json
  appsettings.Development.json
  appsettings.Staging.json
  appsettings.Production.json
)

for app_settings_file in "${APP_SETTINGS_FILES[@]}"
do
  echo "$(cat "$app_settings_file" | jq --arg releasetag "$RELEASE_TAG" '.ReleaseTag=$releasetag')" > "$app_settings_file"
done
