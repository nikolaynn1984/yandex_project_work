# Названия приложений
Сервер авторизации - [Account.Server](#authServer) <br/>
Сервер бронирований - [Bookings.Server](#bookingsServer) <br/>
Сервер событий - [Events.Server](#eventsServer) <br/>



## Запуск
> Для запуска приложения с папки: 
 1. Скачать на рабочую станцию репозиторий
 2. Перейти в папку yandex_project_work\Microservices
 3. Открыть папку в терминале 
 4. Запустить команду 
```bash
docker compose up -d 
```

> Запуск в IDE среде Visual Studio
  - Запустить файл yandex_project_work\Microservices\Microservices.slnx
  - Выберете docker-compose
  - нажмите F5

> Работа с базой данных

 При запуске docker-compose создаются и запускаются три базы данных
 - users-db
 - events-db
 - bookings-db

 схема и таблицы создаются миграцией при запуске, через Migration

 Строки подключения в окружающей среде приложений 
 - account-server
 - events-server
 - bookings-server


> Работа с брокером сообщений

 При запуске docker-compose  скачивается и запускаются kafka, zookeeper и kafka-ui
 Настройка Kafka__BootstrapServers находятся в окружающей среде приложений
 - events-server
 - bookings-server

> Работа с распределенным кэшированием
  При запуске docker-compose  скачивается и запускаются redis
 Настройка Redis__ConnectionString находятся в окружающей среде приложений
 - events-server

## Использование
> Первые шаги
 - Пройти регистрацию пользователя POST auth/register - http://localhost:5001/swagger/index.html
 - Авторизоваться в POST auth/login -http://localhost:5001/swagger/index.html
 - Скопировать полученный Токен

> Создание события
 - Откройте свагер сервера событий http://localhost:5003/swagger/index.html  
 - В правой стороне окна Swagger найти замок на любом из методов
 - Вставить скопированный токен
 - Нажать Login
 - Нажать крестик
 - Создать событие через POST /events. Объектная модель EventRequest
 - В ответе добавления скопировать Id (идентификатор события)

> Создание брони
 - Откройте свагер сервера событий http://localhost:5002/swagger/index.html  
 - В правой стороне окна Swagger найти замок на любом из методов
 - Вставить скопированный токен
 - Нажать Login
 - Нажать крестик
 - создать бронь через POST /bookings/{id} - в поле Id указать скопированное значение события
 - после получения 202 вы получите модель с данными и Status:1, если запрос прошел успешно
 - скопируйте Id свойство и встравьте в  GET /bookings/{Id}
 - через пять секунд после добавления статус по текущему идентификатору поменятся - (значения в BookingStatus) 




## Формат ошибки авторизации
  - 401 ошибка аутентификации
  - 403 не достаточно прав

## Формат ошибки 400, 404, 409, 500
   Problem Details (RFC 7807)
- title — краткое человеческое описание типа проблемы;
- status — HTTP-код состояния;
- detail — подробное описание для человека;
- instance — URI конкретного экземпляра ошибки 


## Сервер авториззации
<a id="authServer"></a>


> Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Account.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

> Роли
  - User - пол умолчанию при регистрации
  - Admin

> Описание
 Реализуемые эндпоинты REST API 
- auth
  - POST / register - регистрация пользователя
  - POST / login - авторизация пользователя


> Тестирование

 Для запуска модульных тестов
  * перейти в папку yandex_project_work\Microservices\Tests\Account.Unit.Test

  Тестирование реализовано с EF Core через InMemory-провайдер
   
  > Запустить в консоли
 ```bash
dotnet test Account.Unit.Test.csproj
 ```
 Для запуска итеграционных тесттов 
  * потребуется установленный и запущен Docker Desktop (Windows/macOS) или Docker Engine (Linux) на вашей машине
    Для того чтоб убедиться что docker работает требуется запустить docker-compose.yml

 запустить команду 
```bash
docker compose up -d 
```
далее 
```bash
dotnet test Account.Integration.Test.csproj
 ```
Если контейнер PostgreSQL запустился — Docker работает корректно. Остановите его 
```bash
docker compose down 
```

## Сервер бронирования
<a id="bookingsServer"></a>

> Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Bookings.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

> Описание
 Реализуемые эндпоинты REST API 
- bookings
  - GET /bookings/{Id} — Получение информации бронирования по идентификатору - возвращает  Booking
  - POST /bookings/{Id} — Бронирование места по идентифкатору события
  - DELETE /bookings/{id} — Отменить бронирование (Для роли User доступно только свое бронирование/Для роли Admin доступно отмена любой брони)


> Booking свойства
 - Id: Guid - уникальный идентификатор брони
 - EventId: Guid - идентификатор события, к которому относится бронь
 - Status: BookingStatus - текущий статус брони (описание ниже)
 - CreatedAt: DateTime - дата и время создания брони
 - ProcessedAt:  DateTime? - дата и время обработки брони




> BookingStatus статусы
 - Pending = 1 - бронь создана, ожидает обработки
 - Confirmed = 2 - бронь подтверждена
 - Rejected = 3 - бронь отклонена
 - Cancelled = 4 - бронь отменена

> Тестирование

 Для запуска модульных тестов
  * перейти в папку yandex_project_work\Microservices\Tests\Bookings.Unit.Test

  Тестирование реализовано с EF Core через InMemory-провайдер
   
  > Запустить в консоли
 ```bash
dotnet test Bookings.Unit.Test.csproj
 ```
 Для запуска итеграционных тесттов 
  * потребуется установленный и запущен Docker Desktop (Windows/macOS) или Docker Engine (Linux) на вашей машине
    Для того чтоб убедиться что docker работает требуется запустить docker-compose.yml

 запустить команду 
```bash
docker compose up -d 
```
далее 
```bash
dotnet test Bookings.Integration.Test.csproj
 ```
Если контейнер PostgreSQL запустился — Docker работает корректно. Остановите его 
```bash
docker compose down 
```

## Сервер событие
<a id="eventsServer"></a>

> Redis
Ключи:
 1. event:{id}  - хранит значение события в кэш 
 2. events:top10  - хранит топ 10 событий

  Для получения события по идентификатор и список топ 10 реализована стратегия cache-aside (GET /events/{id})
  1. Проверка в кэш, если успех то возвращаем
  2. Если кэш мимо, берем с БД
  3. Кладем в кэш

 Для методов POST /events, PUT /events/{id} реализована стратегия Update-on-Write с ttl  5 минут, для того чтоб данные были сразу прогреты 

 Для метода DELETE /events/{id} реализована стратегия Delete-on-Write для консистентности данных, так-же удаляется events:top10
  
  В переменное окружение вынесены параметры TTL
 - Redis__Top10TTL= 10
 - Redis__EventIdTTL= 5

> Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Events.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

> Описание
 Реализуемые эндпоинты REST API 
- events
  - GET /events?title=value1&from=value2&to=value3&page=value4&pageSize=value5 — получить список всех событий с пагинацией (Ответ 200) - Формат передачи даты для парметров from, to = 2026-05-11T11:41:33.182Z
  - GET /events/{id} — получить событие по id
  - GET /events/top - получить топ 10 событий
  - POST /events — создать событие (Доступно только для роли Admin)
  - PUT /events/{id} — обновить событие целиком (Доступно только для роли Admin)
  - DELETE /events/{id} — удалить событие (Доступно только для роли Admin)

> Events свойства
 - Id: Guid - Идентифкатор соыбтия
 - Title: string - Титл
 - Description: string - Описание
 - TotalSeats: int - Общее количество мест
 - AvailableSeats: int - Текущее количество свободных мест
 - StartAt: DateTime - Начало
 - EndAt: DateTime - Окончание 

> EventRequest запрос добавления и обновления события
 - *Title: string - Титл
 - Description: string - Описание
 - *TotalSeats: int - Общее количество мест
 - *StartAt: DateTime - Начало
 - *EndAt: DateTime - Окончание 


> Тестирование

 Для запуска модульных тестов
  * перейти в папку yandex_project_work\Microservices\Tests\Events.Unit.Test

  Тестирование реализовано с EF Core через InMemory-провайдер
   
  > Запустить в консоли
 ```bash
dotnet test Events.Unit.Test.csproj
 ```
 Для запуска итеграционных тесттов 
  * потребуется установленный и запущен Docker Desktop (Windows/macOS) или Docker Engine (Linux) на вашей машине
    Для того чтоб убедиться что docker работает требуется запустить docker-compose.yml

 запустить команду 
```bash
docker compose up -d 
```
далее 
```bash
dotnet test Events.Integration.Test.csproj
 ```
Если контейнер PostgreSQL запустился — Docker работает корректно. Остановите его 
```bash
docker compose down 
```

## Технологии (Стек)
- [C#]
- [NET.Core 10]
- [PostgreSQL 16+]
