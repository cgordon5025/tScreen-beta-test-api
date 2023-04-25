ARG DOTNET_VERSION=6.0
ARG BUILD_VERSION=release
ARG IMAGE_VERSION=$DOTNET_VERSION
ARG RUNTIME=linux-x64

# Primary application layer
FROM mcr.microsoft.com/dotnet/aspnet:$IMAGE_VERSION AS base
WORKDIR /app

# Dependencies for IronPDF
RUN apt update \
    && apt install -y libgdiplus libxkbcommon-x11-0 libc6 libc6-dev libgtk2.0-0 \
    libnss3 libatk-bridge2.0-0 libx11-xcb1 libxcb-dri3-0 libdrm-common libgbm1 \
    libasound2 libxrender1 libfontconfig1 libxshmfence1

# Layer for building .NET application
# Note: we don't need any of this after the application is build thus
# it's seperated into it's own layer
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS build
WORKDIR /src
COPY Core/Core.csproj Core/
COPY Application Application/
COPY Data/Data.csproj Data/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY TweenScreen.Api/TweenScreen.Api.csproj TweenScreen.Api/
RUN dotnet restore ./TweenScreen.Api/TweenScreen.Api.csproj
COPY . .
WORKDIR /src/TweenScreen.Api
RUN dotnet build "TweenScreen.Api.csproj" --runtime "$RUNTIME" -c "$BUILD_VERSION" -o /app/build

# Publish layer
FROM build AS publish
RUN dotnet publish "TweenScreen.Api.csproj" --runtime "$RUNTIME" -c "$BUILD_VERSION" -o /app/publish

# Final application image which is used to run the API
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 7300/tcp
EXPOSE 5300/tcp
ENTRYPOINT ["dotnet", "TweenScreen.Api.dll"]
