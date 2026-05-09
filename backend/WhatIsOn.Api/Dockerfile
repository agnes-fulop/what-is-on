# syntax=docker/dockerfile:1.7

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore in its own layer for caching
COPY WhatIsOn.Api.csproj ./
RUN dotnet restore

# Build + publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Persistent SQLite location — mounted as a named volume in compose
RUN mkdir -p /app/data
ENV ConnectionStrings__Default="Data Source=/app/data/whatison.db"
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "WhatIsOn.Api.dll"]
