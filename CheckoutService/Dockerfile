FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MicroServicesSolution.sln ./
COPY CheckoutService/CheckoutService.csproj ./CheckoutService/

WORKDIR /src/CheckoutService
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/CheckoutService
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "CheckoutService.dll"]