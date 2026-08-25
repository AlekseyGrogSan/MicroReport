# 1. Staging для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080

# 2. Staging для сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файл проекта
COPY ["DocumentService.csproj", "./"]
RUN dotnet restore "DocumentService.csproj"

# Копируем исходный код и собираем
COPY . .
RUN dotnet build "DocumentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DocumentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentService.dll"]