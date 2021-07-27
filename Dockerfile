# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY amsd-casework/ConcernsCaseWork/*.sln .
COPY amsd-casework/ConcernsCaseWork/ConcernsCaseWork/*.csproj ./amsd-casework/
RUN dotnet restore

# copy everything else and build app
COPY amsd-casework/. ./
WORKDIR /source/amsd-casework
RUN dotnet publish -c release -o /app --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ConcernsCaseWork.dll"]