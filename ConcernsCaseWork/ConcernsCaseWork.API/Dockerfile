# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0@sha256:ad76b13c57f17dc931267a0f8361386b40c6ad142770646e3b6f87be9f74f165 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 3001
EXPOSE 3000


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0@sha256:aa5e8bb6f35d537e82416c8d134481cf91f5da6023c662b6299931adc7cd20a3 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConcernsCaseWork.API/ConcernsCaseWork.API.csproj", "ConcernsCaseWork.API/"]
COPY ["ConcernsCaseWork.API.Contracts/ConcernsCaseWork.API.Contracts.csproj", "ConcernsCaseWork.API.Contracts/"]
COPY ["ConcernsCaseWork.Data/ConcernsCaseWork.Data.csproj", "ConcernsCaseWork.Data/"]
COPY ["ConcernsCaseWork.UserContext/ConcernsCaseWork.UserContext.csproj", "ConcernsCaseWork.UserContext/"]
COPY ["ConcernsCaseWork.Utils/ConcernsCaseWork.Utils.csproj", "ConcernsCaseWork.Utils/"]
COPY ["ConcernsCaseWork.Logging/ConcernsCaseWork.Logging.csproj", "ConcernsCaseWork.Logging/"]
RUN dotnet restore "./ConcernsCaseWork.API/ConcernsCaseWork.API.csproj"
COPY ./ConcernsCaseWork/ .
WORKDIR "/src/ConcernsCaseWork.API"
RUN dotnet build "./ConcernsCaseWork.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConcernsCaseWork.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConcernsCaseWork.API.dll"]