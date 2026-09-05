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
COPY ["Servers/Account.Server/Account.Server.csproj", "Servers/Account.Server/"]
COPY ["Core/Account/Account.Application/Account.Application.csproj", "Core/Account/Account.Application/"]
COPY ["Core/Account/Account.Domain/Account.Domain.csproj", "Core/Account/Account.Domain/"]
COPY ["Core/Account/Account.Infrastructure/Account.Infrastructure.csproj", "Core/Account/Account.Infrastructure/"]
COPY ["Core/Exceptions.Handling/Exceptions.Handling.csproj", "Core/Exceptions.Handling/"]
RUN dotnet restore "./Servers/Account.Server/Account.Server.csproj"
COPY . .
WORKDIR "/src/Servers/Account.Server"
RUN dotnet build "./Account.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Account.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Account.Server.dll"]