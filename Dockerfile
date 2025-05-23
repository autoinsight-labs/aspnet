FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AutoInsightAPI.csproj", "."]
RUN dotnet restore "./AutoInsightAPI.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "./AutoInsightAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AutoInsightAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y \
    libaio1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "AutoInsightAPI.dll"]