# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

# Copy csproj and restore as distinct layers
COPY ConcernsCaseWork/ConcernsCaseWork/*.csproj .
COPY ConcernsCaseWork/. .

RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ConcernsCaseWork.dll"]