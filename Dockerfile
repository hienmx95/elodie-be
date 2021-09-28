#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#  FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
# WORKDIR /app

 FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ELODIE.csproj", "./"]
RUN dotnet restore "ELODIE.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ELODIE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ELODIE.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim

EXPOSE 8080
WORKDIR /app
RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnetd telnet nano vim libc6-dev libgdiplus dnsutils

COPY --from=publish /app/publish .
RUN chmod -R g+w /app

COPY ["docker-entrypoint.sh", "."]
RUN chmod a+x docker-entrypoint.sh
CMD ["./docker-entrypoint.sh"]
