ARG DOTNET_VERSION=6.0
ARG BUILD_VERSION=release
ARG IMAGE_VERSION=$DOTNET_VERSION
ARG RUNTIME=linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:$IMAGE_VERSION AS base
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true

FROM mcr.microsoft.com/dotnet/sdk:$IMAGE_VERSION AS build
WORKDIR /src
COPY Core/Core.csproj Core/
COPY Application Application/
COPY Data/Data.csproj Data/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY TweenScreen.Functions/TweenScreen.Functions.csproj TweenScreen.Functions/
RUN dotnet restore ./TweenScreen.Functions/TweenScreen.Functions.csproj
COPY . .
WORKDIR /src/TweenScreen.Functions
RUN dotnet build "TweenScreen.Functions.csproj" --runtime "$RUNTIME" -c "$BUILD_VERSION" -o /app/build

FROM build AS publish
RUN dotnet publish "TweenScreen.Functions.csproj" --runtime "$RUNTIME" -c "$BUILD_VERSION" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TweenScreen.Functions.dll"]