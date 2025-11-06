
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in correct order (dependencies first)
COPY ["/ConcernsCaseWork.API.Contracts/ConcernsCaseWork.API.Contracts.csproj", "ConcernsCaseWork.API.Contracts/"]
COPY ["/ConcernsCaseWork.UserContext/ConcernsCaseWork.UserContext.csproj", "ConcernsCaseWork.UserContext/"]
COPY ["/ConcernsCaseWork.Utils/ConcernsCaseWork.Utils.csproj", "ConcernsCaseWork.Utils/"]
COPY ["/ConcernsCaseWork.Data/ConcernsCaseWork.Data.csproj", "ConcernsCaseWork.Data/"]
RUN dotnet restore "ConcernsCaseWork.Data/ConcernsCaseWork.Data.csproj"
COPY . .
WORKDIR "/src/ConcernsCaseWork.Data"

## Run migrations
RUN dotnet tool install --version 8.0.18 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet-ef database update --connection "$CONNECTION_STRING"