# Set the major version of dotnet
ARG DOTNET_VERSION=8.0
# Set the major version of nodejs
ARG NODEJS_VERSION_MAJOR=22

ARG COMMIT_SHA=not-set

# ==============================================
# Base SDK
# ==============================================
FROM "mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-azurelinux3.0" AS builder
ARG COMMIT_SHA
WORKDIR /build
RUN ["tdnf", "update", "--security", "-y"]
RUN ["tdnf", "install", "-y", "jq"]
RUN ["tdnf", "clean", "all"]
COPY ConcernsCaseWork/. .
RUN dotnet restore ConcernsCaseWork
RUN dotnet build ConcernsCaseWork "/p:customBuildMessage=Manifest commit SHA... ${COMMIT_SHA};" -c Release
RUN dotnet publish ConcernsCaseWork -c Release -o /app --no-build
WORKDIR /app
COPY ./script/set-appsettings-release-tag.sh set-appsettings-release-tag.sh
RUN chmod +x ./set-appsettings-release-tag.sh
RUN echo "Setting appsettings releasetag=${COMMIT_SHA}"
RUN ./set-appsettings-release-tag.sh "$COMMIT_SHA"
RUN rm ./set-appsettings-release-tag.sh

# ==============================================
# Entity Framework: Migration Builder
# ==============================================
FROM builder AS efbuilder
WORKDIR /build
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef
RUN mkdir /sql
RUN dotnet ef migrations bundle -r linux-x64 --configuration Release -p ConcernsCaseWork.Data --no-build -o /sql/migratedb

# ==============================================
# Entity Framework: Migration Runner
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0" AS initcontainer
WORKDIR /sql
COPY --from=efbuilder /sql /sql
COPY --from=builder /app/appsettings* /ConcernsCaseWork/
RUN chown "$APP_UID" "/sql" -R
RUN chown "$APP_UID" "/ConcernsCaseWork" -R
USER $APP_UID

# ==============================================
# Front End Builder
# ==============================================
FROM "node:${NODEJS_VERSION_MAJOR}-bullseye-slim" AS frontend
COPY ConcernsCaseWork/ConcernsCaseWork/wwwroot /app/wwwroot
WORKDIR /app/wwwroot
RUN npm install
RUN npm run build

# ==============================================
# Application
# ==============================================
FROM "mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0" AS final
LABEL org.opencontainers.image.source="https://github.com/DFE-Digital/record-concerns-support-trusts"
COPY --from=builder /app /app
COPY --from=frontend /app/wwwroot /app/wwwroot
COPY ./script/web-docker-entrypoint.sh /app/docker-entrypoint.sh
WORKDIR /app
RUN chmod +x ./docker-entrypoint.sh
USER $APP_UID
