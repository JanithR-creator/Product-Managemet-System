# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj separately for caching and restore
COPY MicroServicesSolution.sln ./
COPY ApiGateway/ApiGateway.csproj ./ApiGateway/

# Restore only the ApiGateway project
WORKDIR /src/ApiGateway
RUN dotnet restore

# Copy the full source
WORKDIR /src
COPY . .

# Now publish
WORKDIR /src/ApiGateway
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
