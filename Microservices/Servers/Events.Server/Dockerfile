# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Servers/Events.Server/Events.Server.csproj", "Servers/Events.Server/"]
COPY ["Core/Events/Events.Application/Events.Application.csproj", "Core/Events/Events.Application/"]
COPY ["Core/Events/Events.Domain/Events.Domain.csproj", "Core/Events/Events.Domain/"]
COPY ["Core/Events/Events.Infrastructure/Events.Infrastructure.csproj", "Core/Events/Events.Infrastructure/"]
COPY ["Core/Communication.API/Communication.API.csproj", "Core/Communication.API/"]
COPY ["Core/Exceptions.Handling/Exceptions.Handling.csproj", "Core/Exceptions.Handling/"]
RUN dotnet restore "./Servers/Events.Server/Events.Server.csproj"
COPY . .
WORKDIR "/src/Servers/Events.Server"
RUN dotnet build "./Events.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Events.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Events.Server.dll"]