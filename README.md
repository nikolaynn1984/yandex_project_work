# Название приложения
Сервер событий

## Запуск
 Для запуска приложения: 
 1. Скачать на рабочую станцию репозиторий
 2. Перейти в парку yandex_project_work/Microservices

- Запуск с консоли
  dotnet build EventServer.csproj
  dotnet run EventServer.csproj

- Запуск в IDE среде Visual Studio
  Запустить файл Event.Server.sln
  Нажать F5

## Тестирование
 По умолчанию в конфигурации applicationUrl = http://localhost:5185
 для запуска SwaggerUI требуется открыть браузер и указать адресс http://localhost:5185/swagger/index.html

## Описание
 	Реализуемые эндпоинты REST API
  o	GET /events — получить список всех событий (Ответ 200)
  o	GET /events/{id} — получить событие по id;
  o	POST /events — создать событие
  o	PUT /events/{id} — обновить событие целиком
  o	DELETE /events/{id} — удалить событие

## Технологии (Стек)
- [C#]
- [NET.Core 10]
