# Название приложения
Сервер событий

## Запуск
 Для запуска приложения: 
 1. Скачать на рабочую станцию репозиторий
 2. Перейти в папку yandex_project_work\Microservices\EventServer

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

  > Запустить в консоли
    ```bash
dotnet test EventServiceTests.csproj
```


## Описание
 Реализуемые эндпоинты REST API
-	GET /events?title=value1&from=value2&to=value3&page=value4&pageSize=value5 — получить список всех событий с пагинацией (Ответ 200) - Формат передачи даты для парметров from, to = 2026-05-11T11:41:33.182Z
-	GET /events/{id} — получить событие по id;
-	POST /events — создать событие
-	PUT /events/{id} — обновить событие целиком
-	DELETE /events/{id} — удалить событие


## Формат ошибки 400, 404, 500
   Problem Details (RFC 7807)

- title — краткое человеческое описание типа проблемы;
- status — HTTP-код состояния;
- detail — подробное описание для человека;
- instance — URI конкретного экземпляра ошибки 

## Технологии (Стек)
- [C#]
- [NET.Core 10]
