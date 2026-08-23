# Названия приложений
[Сервер событий - Account.Server](#auth-server)
[Сервер бронирования - Bookings.Server](#bookings-server)
[Сервер авторизации - Events.Server](#events-server)



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

## Работа с базой данных

 При запуске docker-compose создаются и запускаются три базы данных
 - users-db
 - events-db
 - bookings-db

 схема и таблицы создаются миграцией при запуске, через Migration

 Строки подключения в окружающей среде приложений 
 - account-server
 - events-server
 - bookings-server


## Работа с брокером сообщений
 При запуске docker-compose скачивается и запускаются kafka, zookeeper и kafka-ui
 Настройка Kafka__BootstrapServers находятся в окружающей среде приложенийъ
 - events-server
 - bookings-server

>

## Сервер авториззации
<a id="auth-server"></a>


## Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Account.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

## Роли
  - User - пол умолчанию при регистрации
  - Admin

## Описание
 Реализуемые эндпоинты REST API 
- auth
  - POST / register - регистрация пользователя
  - POST / login - авторизация пользователя

>

## Сервер бронирования
<a id="bookings-server"></a>

## Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Bookings.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

## Описание
 Реализуемые эндпоинты REST API 
- bookings
  - GET /bookings/{Id} — Получение информации бронирования по идентификатору - возвращает  Booking
  - POST /bookings/{Id} — Бронирование места по идентифкатору события
  - DELETE /bookings/{id} — Отменить бронирование (Для роли User доступно только свое бронирование/Для роли Admin доступно отмена любой брони)


## Booking свойства 
 - Id: Guid - уникальный идентификатор брони
 - EventId: Guid - идентификатор события, к которому относится бронь
 - Status: BookingStatus - текущий статус брони (описание ниже)
 - CreatedAt: DateTime - дата и время создания брони
 - ProcessedAt:  DateTime? - дата и время обработки брони




## BookingStatus статусы
 - Pending = 1 - бронь создана, ожидает обработки
 - Confirmed = 2 - бронь подтверждена
 - Rejected = 3 - бронь отклонена
 - Cancelled = 4 - бронь отменена

 >

## Сервер событие
<a id="events-server"></a>

## Аутентификация/Авторизация пользователей
 1. Перейти в папку yandex_project_work\Microservices\Servers\Events.Server
 2. Откройте файл appsettings.json
 3. Настроить блок Jwt

    Jwt:Key = не может быть меньше 32 символов

## Описание
 Реализуемые эндпоинты REST API 
- events
  - GET /events?title=value1&from=value2&to=value3&page=value4&pageSize=value5 — получить список всех событий с пагинацией (Ответ 200) - Формат передачи даты для парметров from, to = 2026-05-11T11:41:33.182Z
  - GET /events/{id} — получить событие по id
  - POST /events — создать событие (Доступно только для роли Admin)
  - PUT /events/{id} — обновить событие целиком (Доступно только для роли Admin)
  - DELETE /events/{id} — удалить событие (Доступно только для роли Admin)

## Events свойства
 - Id: Guid - Идентифкатор соыбтия
 - Title: string - Титл
 - Description: string - Описание
 - TotalSeats: int - Общее количество мест
 - AvailableSeats: int - Текущее количество свободных мест
 - StartAt: DateTime - Начало
 - EndAt: DateTime - Окончание 

## EventRequest запрос добавления и обновления события
 - *Title: string - Титл
 - Description: string - Описание
 - *TotalSeats: int - Общее количество мест
 - *StartAt: DateTime - Начало
 - *EndAt: DateTime - Окончание 



## Тестирование
 > По умолчанию в конфигурации applicationUrl = http://localhost:5185
 * для запуска SwaggerUI требуется открыть браузер и указать адресс http://localhost:5185/swagger/index.html

 Для запуска модульных тестов
  * перейти в папку yandex_project_work\Microservices\EventServiceTests

  Тестирование реализовано с EF Core через InMemory-провайдер
   
  > Запустить в консоли
 ```bash
dotnet test EventServiceTests.csproj
 ```
 Для запуска итеграционных тесттов 
  * потребуется установленный и запущен Docker Desktop (Windows/macOS) или Docker Engine (Linux) на вашей машине
    Для того чтоб убедиться что docker работает требуется запустить docker-compose.yml

 запустить команду 
```bash
docker compose up -d 
```
Если контейнер PostgreSQL запустился — Docker работает корректно. Остановите его 
```bash
docker compose down 
```

## Использование
 Первые шаги
 - Пройти регистрацию пользователя POST auth/register
 - Авторизоваться в POST auth/login
 - Скопировать полученный Токен
 - В правой стороне окна Swagger найти замок на любом из метов
 - Вставить скопированный токен
 - Нажать Login
 - Нажать крестик


 





## Сценарий добавления брони
  1. Создать событие через POST /events. Объектная модель EventRequest
  2. В ответе добавления скопировать Id (идентификатор события)
  3. создать бронь через POST /evets/{id}/book - в поле Id указать скопированное значение
  4. после получения 202 вы получите модель с данными и Status:1, если ззапрос прошел успешно. В противном случае 409 Conflict  (Нет свободных мест)
  5. скопируйте Id свойство и встравьте в  GET /bookings/{Id}
  6. через пять секунд после добавления статус по текущему идентификатору поменятся на 2



## Формат ошибки авторизации
  - 401 ошибка аутентификации
  - 403 не достаточно прав

## Формат ошибки 400, 404, 409, 500
   Problem Details (RFC 7807)
- title — краткое человеческое описание типа проблемы;
- status — HTTP-код состояния;
- detail — подробное описание для человека;
- instance — URI конкретного экземпляра ошибки 

## Технологии (Стек)
- [C#]
- [NET.Core 10]
- [PostgreSQL 16+]
