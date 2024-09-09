﻿# Stage 1
ARG ASPNET_SDK_TAG=8.0
ARG ASPNET_IMAGE_TAG=8.0-bookworm-slim
ARG NODEJS_IMAGE_TAG=20.15-bullseye
ARG COMMIT_SHA=not-set

# ==============================================
# Base SDK
# ==============================================
FROM "mcr.microsoft.com/dotnet/sdk:${ASPNET_SDK_TAG}" AS builder
ARG COMMIT_SHA
WORKDIR /build
COPY ConcernsCaseWork/. .
RUN dotnet restore ConcernsCaseWork
RUN dotnet build ConcernsCaseWork "/p:customBuildMessage=Manifest commit SHA... ${COMMIT_SHA};" -c Release
RUN dotnet publish ConcernsCaseWork -c Release -o /app --no-build

# ==============================================
# Entity Framework: Migration Builder
# ==============================================
FROM builder AS efbuilder
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef
RUN mkdir /sql
WORKDIR /app
RUN dotnet ef migrations bundle -r linux-x64 --configuration Release -p /build/ConcernsCaseWork.Data --no-build -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
ENTRYPOINT [ "./migratedb" ]

# ==============================================
# Front End Builder
# ==============================================
FROM node:${NODEJS_IMAGE_TAG} AS frontend
COPY --from=builder /app/wwwroot /app/wwwroot
WORKDIR /app/wwwroot
RUN npm install
RUN npm run build

# ==============================================
# Application
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
LABEL org.opencontainers.image.source=https://github.com/DFE-Digital/record-concerns-support-trusts
ARG COMMIT_SHA
COPY --from=builder /app /app
COPY --from=frontend /app/wwwroot /app/wwwroot
COPY ./script/set-appsettings-release-tag.sh /app/set-appsettings-release-tag.sh
COPY ./script/web-docker-entrypoint.sh /app/docker-entrypoint.sh
WORKDIR /app
RUN chown -R app:app /app
RUN chmod +x ./docker-entrypoint.sh
RUN chmod +x ./set-appsettings-release-tag.sh
RUN echo "Setting appsettings releasetag=${COMMIT_SHA}"
RUN ./set-appsettings-release-tag.sh "$COMMIT_SHA"
USER app
EXPOSE 8080/tcp
