# Название приложения
Сервер событий



## Запуск
 Для запуска приложения: 
 1. Скачать на рабочую станцию репозиторий
 2. Перейти в папку yandex_project_work\Microservices\EventServer

## Работа с базой данных

 1. Установите базу данных PostgreSQL V16+
 2. Перейти в папку yandex_project_work\Microservices\EventServer
 3. Откройте файл appsettings.json
 4. в свойстве ConnectionStrings:DefaultConnection требуется указать вашу строку подключения

схема и таблицы создаются миграцией при запуске, через Migration

> Запуск с консоли
  ```bash
  dotnet build EventServer.csproj
```
 ```bash
dotnet run EventServer.csproj
 ```

> Запуск в IDE среде Visual Studio
  - Запустить файл yandex_project_work\Microservices\Microservices.slnx
  - Нажать F5

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
 ```bash
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: bookstore_test
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: secret123
    ports:
      - "5432:5432"
 ```

 запустить команду 
 ```bash
docker compose up -d 
```
Если контейнер PostgreSQL запустился — Docker работает корректно. Остановите его 
```bash
docker compose down 
```

## Описание
 Реализуемые эндпоинты REST API 
- events
  -	GET /events?title=value1&from=value2&to=value3&page=value4&pageSize=value5 — получить список всех событий с пагинацией (Ответ 200) - Формат передачи даты для парметров from, to = 2026-05-11T11:41:33.182Z
  -	GET /events/{id} — получить событие по id
  -	POST /events — создать событие
  -	PUT /events/{id} — обновить событие целиком
  -	DELETE /events/{id} — удалить событие
  -	POST /events/{id}/book - добавления планирования события
 
- bookings
  -	GET /bookings/{Id} — Получение информации бронирования по идентификатору - возвращает  Booking
 
## Events свойства
 - Id: Guid - Идентифкатор соыбтия
 - Title: string - Титл
 - Description: string - Описание
 - TotalSeats: int - Общее количество мест
 - AvailableSeats: int - Текущее количество свободных мест
 - StartAt: DateTime - Начало
 - EndAt: DateTime - Окончание 


## Booking свойства 
 - Id: Guid - уникальный идентификатор брони
 - EventId: Guid - идентификатор события, к которому относится бронь
 - Status: BookingStatus - текущий статус брони (описание ниже)
 - CreatedAt: DateTime - дата и время создания брони
 - ProcessedAt:  DateTime? - дата и время обработки брони


## EventRequest запрос добавления и обновления события
 - *Title: string - Титл
 - Description: string - Описание
 - *TotalSeats: int - Общее количество мест
 - *StartAt: DateTime - Начало
 - *EndAt: DateTime - Окончание 

## BookingStatus статусы
 - Pending = 1 - бронь создана, ожидает обработки
 - Confirmed = 2 - бронь подтверждена
 - Rejected = 3 - бронь отклонена

## Сценарий добавления брони
  1. Создать событие через POST /events. Объектная модель EventRequest
  2. В ответе добавления скопировать Id (идентификатор события)
  3. создать бронь через POST /evets/{id}/book - в поле Id указать скопированное значение
  4. после получения 202 вы получите модель с данными и Status:1, если ззапрос прошел успешно. В противном случае 409 Conflict  (Нет свободных мест)
  5. скопируйте Id свойство и встравьте в  GET /bookings/{Id}
  6. через пять секунд после добавления статус по текущему идентификатору поменятся на 2

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




