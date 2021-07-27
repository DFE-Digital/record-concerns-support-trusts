# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY ConcernsCaseWork/ConcernsCaseWork/*.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY ConcernsCaseWork/. ./
WORKDIR /source
RUN dotnet publish -c Release -o /app --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app ./
ENTRYPOINT ["dotnet", "ConcernsCaseWork.dll"]