FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
RUN apt-get update && apt-get install -y clang zlib1g-dev
WORKDIR /src
COPY ["RinhaBackendAPI2024Q1.csproj", "./"]
RUN dotnet restore "RinhaBackendAPI2024Q1.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "RinhaBackendAPI2024Q1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RinhaBackendAPI2024Q1.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./RinhaBackendAPI2024Q1"]
