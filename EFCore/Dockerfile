# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG BUILD_CONFIGURATION=Release

COPY Directory.Build.props ./
COPY src/EfCorePractice.Domain/EfCorePractice.Domain.csproj src/EfCorePractice.Domain/
COPY src/EfCorePractice.Application/EfCorePractice.Application.csproj src/EfCorePractice.Application/
COPY src/EfCorePractice.Infrastructure/EfCorePractice.Infrastructure.csproj src/EfCorePractice.Infrastructure/
COPY src/EfCorePractice.Api/EfCorePractice.Api.csproj src/EfCorePractice.Api/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/EfCorePractice.Api/EfCorePractice.Api.csproj

COPY src/ src/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/EfCorePractice.Api/EfCorePractice.Api.csproj \
    -c ${BUILD_CONFIGURATION} \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8185 \
    ASPNETCORE_ENVIRONMENT=Production \
    TZ=Asia/Shanghai

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd --gid 10001 appgroup \
    && useradd --uid 10001 --gid appgroup --create-home appuser

COPY --from=build --chown=appuser:appgroup /app/publish ./

USER appuser

EXPOSE 8185

HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl -f http://127.0.0.1:8185/health || exit 1

ENTRYPOINT ["dotnet", "EfCorePractice.Api.dll"]
