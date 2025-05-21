# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for caching
COPY MicroServicesSolution.sln ./
COPY AdapterService/AdapterService.csproj ./AdapterService/

# Restore only AdapterService project
WORKDIR /src/AdapterService
RUN dotnet restore

# Copy full source
WORKDIR /src
COPY . .

# Publish AdapterService
WORKDIR /src/AdapterService
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "AdapterService.dll"]
