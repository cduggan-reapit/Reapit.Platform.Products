FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

ARG NUGET_ACCESS_TOKEN
ARG NUGET_USERNAME=products-ci
ARG NUGET_FEED_SOURCE=https://nuget.pkg.github.com/reapit/index.json

COPY . .

RUN dotnet nuget add source "$NUGET_FEED_SOURCE" \
        --name github \
        --store-password-in-clear-text \
        --username "$NUGET_USERNAME" \
        --password "$NUGET_ACCESS_TOKEN" \
    && dotnet restore Reapit.Platform.Products.Api/Reapit.Platform.Products.Api.csproj \
    && dotnet publish Reapit.Platform.Products.Api/Reapit.Platform.Products.Api.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

RUN apt-get update \
    && apt-get install -y curl \
    && apt-get clean

ARG ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT $ASPNETCORE_ENVIRONMENT
COPY --from=build-env /App/out .
EXPOSE 80

ENTRYPOINT ["dotnet", "Reapit.Platform.Products.Api.dll", "--urls", "http://+:80"] 