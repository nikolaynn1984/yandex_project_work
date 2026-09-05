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
COPY ["Servers/Bookings.Server/Bookings.Server.csproj", "Servers/Bookings.Server/"]
COPY ["Core/Bookings/Bookings.Application/Bookings.Application.csproj", "Core/Bookings/Bookings.Application/"]
COPY ["Core/Bookings/Bookings.Domain/Bookings.Domain.csproj", "Core/Bookings/Bookings.Domain/"]
COPY ["Core/Bookings/Bookings.Infrastructure/Bookings.Infrastructure.csproj", "Core/Bookings/Bookings.Infrastructure/"]
COPY ["Core/Communication.API/Communication.API.csproj", "Core/Communication.API/"]
COPY ["Core/Exceptions.Handling/Exceptions.Handling.csproj", "Core/Exceptions.Handling/"]
RUN dotnet restore "./Servers/Bookings.Server/Bookings.Server.csproj"
COPY . .
WORKDIR "/src/Servers/Bookings.Server"
RUN dotnet build "./Bookings.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bookings.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookings.Server.dll"]