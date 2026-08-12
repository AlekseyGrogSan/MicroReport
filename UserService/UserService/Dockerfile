# ---------------------------------------------------
# 1. Этап сборки (SDK .NET 10)
# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["UserService.csproj", "./"]
RUN dotnet restore "UserService.csproj"

# Копируем исходники и собираем релизную версию
COPY . .
RUN dotnet publish "UserService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---------------------------------------------------
# 2. Этап запуска (Runtime .NET 10)
# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]