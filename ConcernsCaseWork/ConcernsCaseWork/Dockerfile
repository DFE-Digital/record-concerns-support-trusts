# THIS IS NOT THE DOCKER FILE USED IN DEPLOYMENT. THIS DOCKER FILE IS INTENDED FOR DEVELOPERS TO USE LOCALLY WHILE DEVELOPING
# IF YOU WANT TO USE THIS DOCKER FILE WHEN DEBUGGING, YOU MUST FIRST FIND OUT THE IP ADDRESS OF THE REDIS CONTAINER YOU ARE RUNNING
# AND THEN UPDATE YOUR USER-SECRETS FILE WITH THE REDIS CONTAINER IP ADDRESS INSTEAD OF THE LOCAL HOST IP ADDRESS (127.0.0.1).
#
# TO FIND OUT THE REDIS CONTAINER IP ADDRESS, START THE CONTAINER. THEN RUN
#
#             Docker inspect <container name>
#
# IN THE OUTPUT LOOK FOR: "IPAddress"
#
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ConcernsCaseWork/ConcernsCaseWork.csproj", "ConcernsCaseWork/"]
COPY ["Service.Redis/Service.Redis.csproj", "Service.Redis/"]
COPY ["Service.TRAMS/Service.TRAMS.csproj", "Service.TRAMS/"]
RUN dotnet restore "ConcernsCaseWork/ConcernsCaseWork.csproj"
COPY . .
WORKDIR "/src/ConcernsCaseWork"
RUN dotnet build "ConcernsCaseWork.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConcernsCaseWork.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConcernsCaseWork.dll"]