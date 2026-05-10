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

## Описание
 Реализуемые эндпоинты REST API
  -	GET /events — получить список всех событий (Ответ 200)
  -	GET /events/{id} — получить событие по id;
  -	POST /events — создать событие
  -	PUT /events/{id} — обновить событие целиком
  -	DELETE /events/{id} — удалить событие

## Технологии (Стек)
- [C#]
- [NET.Core 10]
