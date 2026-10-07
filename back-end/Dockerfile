# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore first (cached as long as the project files don't change)
COPY MangaWhisper.Api/MangaWhisper.Api.csproj MangaWhisper.Api/
COPY MangaWhisper.Application/MangaWhisper.Application.csproj MangaWhisper.Application/
COPY MangaWhisper.Common/MangaWhisper.Common.csproj MangaWhisper.Common/
COPY MangaWhisper.Domain/MangaWhisper.Domain.csproj MangaWhisper.Domain/
COPY MangaWhisper.Infrastructure/MangaWhisper.Infrastructure.csproj MangaWhisper.Infrastructure/
RUN dotnet restore MangaWhisper.Api/MangaWhisper.Api.csproj

COPY . .
RUN dotnet publish MangaWhisper.Api/MangaWhisper.Api.csproj -c Release -o /app/publish --no-restore

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "MangaWhisper.Api.dll"]
