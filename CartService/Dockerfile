FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MicroServicesSolution.sln ./
COPY CartService/CartService.csproj ./CartService/

WORKDIR /src/CartService
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/CartService
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "CartService.dll"]