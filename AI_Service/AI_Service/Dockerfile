FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AI_Service.csproj", "./"]
RUN dotnet restore "AI_Service.csproj"

COPY . .
RUN dotnet build "AI_Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AI_Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI_Service.dll"]