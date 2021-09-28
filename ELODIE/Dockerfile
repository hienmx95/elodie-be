#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry-dev.truesight.asia/truesight/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM registry-dev.truesight.asia/truesight/dotnet-sdk:3.1-buster AS build
WORKDIR /src
COPY ["ELODIE.csproj", "./"]
RUN dotnet restore "ELODIE.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ELODIE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ELODIE.csproj" -c Release -o /app/publish

FROM base AS final

EXPOSE 8080
WORKDIR /app
RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnetd telnet nano vim libc6-dev libgdiplus dnsutils

COPY --from=publish /app/publish .
RUN chmod -R g+w /app

COPY ["docker-entrypoint.sh", "."]
RUN chmod a+x docker-entrypoint.sh
CMD ["./docker-entrypoint.sh"]
