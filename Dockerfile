# Stage 1
ARG ASPNET_IMAGE_TAG=8.0-bookworm-slim
ARG NODEJS_IMAGE_TAG=20.15-bullseye
ARG COMMIT_SHA=not-set

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

ARG COMMIT_SHA

WORKDIR /build

ENV DEBIAN_FRONTEND=noninteractive

COPY ConcernsCaseWork/. .

RUN dotnet restore ConcernsCaseWork
RUN dotnet build ConcernsCaseWork "/p:customBuildMessage=Manifest commit SHA... ${COMMIT_SHA};" -c Release
RUN dotnet publish ConcernsCaseWork -c Release -o /app --no-build

COPY ./script/web-docker-entrypoint.sh /app/docker-entrypoint.sh
COPY ./script/init-docker-entrypoint.sh /app/init.sh
COPY ./script/set-appsettings-release-tag.sh /app/set-appsettings-release-tag.sh

# Stage 2 - Build assets
FROM node:${NODEJS_IMAGE_TAG} AS build
COPY --from=publish /app/wwwroot /app/wwwroot
WORKDIR /app/wwwroot
RUN npm install
RUN npm run build

# Stage 3 - Final
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
LABEL org.opencontainers.image.source=https://github.com/DFE-Digital/record-concerns-support-trusts

ARG COMMIT_SHA

COPY --from=publish /app /app
COPY --from=build /app/wwwroot /wwwroot
WORKDIR /app
RUN chmod +x ./docker-entrypoint.sh
RUN chmod +x ./init.sh
RUN chmod +x ./set-appsettings-release-tag.sh
RUN echo "Setting appsettings releasetag=${COMMIT_SHA}"
RUN ./set-appsettings-release-tag.sh "$COMMIT_SHA"

USER app
EXPOSE 8080/tcp
