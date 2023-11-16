﻿# Stage 1
ARG ASPNET_IMAGE_TAG=8.0.0-bookworm-slim
ARG NODEJS_IMAGE_TAG=16-bullseye
ARG COMMIT_SHA=not-set

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

ARG COMMIT_SHA

WORKDIR /build

ENV DEBIAN_FRONTEND=noninteractive

COPY ConcernsCaseWork/. .

RUN dotnet restore ConcernsCaseWork
RUN dotnet build ConcernsCaseWork "/p:customBuildMessage=Manifest commit SHA... ${COMMIT_SHA};" -c Release

RUN dotnet new tool-manifest
RUN dotnet tool install dotnet-ef

RUN mkdir -p /app/SQL
RUN dotnet ef migrations script --output /app/SQL/DbMigrationScript.sql --idempotent -p /build/ConcernsCaseWork.Data
RUN touch /app/SQL/DbMigrationScript.sql

RUN dotnet publish ConcernsCaseWork -c Release -o /app --no-build

COPY ./script/web-docker-entrypoint.sh /app/docker-entrypoint.sh
COPY ./script/set-appsettings-release-tag.sh /app/set-appsettings-release-tag.sh

# Stage 2 - Build assets
FROM node:${NODEJS_IMAGE_TAG} as build
COPY --from=publish /app /app
WORKDIR /app/wwwroot
RUN npm install
RUN npm run build

# Stage 3 - Final
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final

ARG COMMIT_SHA

RUN apt-get update
RUN apt-get install unixodbc curl gnupg jq -y
RUN curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
RUN curl https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc
RUN curl https://packages.microsoft.com/config/debian/12/prod.list | tee /etc/apt/sources.list.d/mssql-release.list

RUN apt-get update
RUN ACCEPT_EULA=Y apt-get install -y msodbcsql18
RUN ACCEPT_EULA=Y apt-get install -y mssql-tools18

COPY --from=build /app /app
WORKDIR /app
RUN chmod +x ./docker-entrypoint.sh
RUN chmod +x ./set-appsettings-release-tag.sh
RUN echo "Setting appsettings releasetag=${COMMIT_SHA}"
RUN ./set-appsettings-release-tag.sh "$COMMIT_SHA"

EXPOSE 80/tcp
