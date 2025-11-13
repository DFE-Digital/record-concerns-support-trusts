# Set the major version of dotnet
ARG DOTNET_VERSION=8.0
# Set the major version of nodejs
ARG NODEJS_VERSION_MAJOR=22

ARG COMMIT_SHA=not-set

# ==============================================
# Base SDK
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-azurelinux3.0 AS builder
ARG COMMIT_SHA
WORKDIR /build
RUN tdnf update --security -y && \
    tdnf install -y jq && \
    tdnf clean all

ARG PROJECT_NAME="ConcernsCaseWork"
# Copy csproj files for restore caching
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.API.Contracts/${PROJECT_NAME}.API.Contracts.csproj         ./${PROJECT_NAME}.API.Contracts/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.API/${PROJECT_NAME}.API.csproj                             ./${PROJECT_NAME}.API/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.CoreTypes/${PROJECT_NAME}.CoreTypes.csproj                 ./${PROJECT_NAME}.CoreTypes/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.Data/${PROJECT_NAME}.Data.csproj                           ./${PROJECT_NAME}.Data/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.Logging/${PROJECT_NAME}.Logging.csproj                     ./${PROJECT_NAME}.Logging/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.Redis/${PROJECT_NAME}.Redis.csproj                         ./${PROJECT_NAME}.Redis/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.Service/${PROJECT_NAME}.Service.csproj                     ./${PROJECT_NAME}.Service/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.UserContext/${PROJECT_NAME}.UserContext.csproj             ./${PROJECT_NAME}.UserContext/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.Utils/${PROJECT_NAME}.Utils.csproj                         ./${PROJECT_NAME}.Utils/
COPY ./${PROJECT_NAME}/${PROJECT_NAME}/${PROJECT_NAME}.csproj                                     ./${PROJECT_NAME}/

# Copy solution file.
COPY ./${PROJECT_NAME}/${PROJECT_NAME}.sln .

RUN dotnet restore ${PROJECT_NAME}

COPY ${PROJECT_NAME}/ /build
RUN dotnet build ${PROJECT_NAME} "/p:customBuildMessage=Manifest commit SHA... ${COMMIT_SHA};" -c Release && \
    dotnet publish ${PROJECT_NAME} -c Release -o /app --no-build

# Set release tag in appsettings
COPY ./script/set-appsettings-release-tag.sh /build/set-appsettings-release-tag.sh
RUN chmod +x /build/set-appsettings-release-tag.sh && \
    echo "Setting appsettings releasetag=${COMMIT_SHA}" && \
    /build/set-appsettings-release-tag.sh "$COMMIT_SHA"

# ==============================================
# Entity Framework: Migration Builder
# ==============================================
FROM builder AS efbuilder
WORKDIR /build/ConcernsCaseWork.Data
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef --version 8.* && \
    mkdir /sql && \
    dotnet ef migrations bundle \
        -r linux-x64 \
        --configuration Release \
        --no-build \
        -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0 AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
COPY --from=builder /app/appsettings* /ConcernsCaseWork/
RUN chown "$APP_UID" "/sql" -R && \
    chown "$APP_UID" "/ConcernsCaseWork" -R
USER $APP_UID

# ==============================================
# Front End Builder
# ==============================================
FROM node:${NODEJS_VERSION_MAJOR}-bullseye-slim AS frontend
WORKDIR /app/wwwroot
COPY ConcernsCaseWork/ConcernsCaseWork/wwwroot .
RUN npm ci --ignore-scripts && \
    npm run build

# ==============================================
# Application
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0 AS final
LABEL org.opencontainers.image.source="https://github.com/DFE-Digital/record-concerns-support-trusts"
COPY --from=builder /app /app
COPY --from=frontend /app/wwwroot /app/wwwroot

# Entrypoint script
COPY ./script/web-docker-entrypoint.sh /app/docker-entrypoint.sh
WORKDIR /app
RUN chmod +x ./docker-entrypoint.sh
USER $APP_UID
