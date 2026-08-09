# MicroReport
# MicroReport — Event-Driven Microservices Platform for AI Document Generation

**MicroReport** — это событийно-ориентированная (Event-Driven) микросервисная платформа для автоматической генерации отчетов, резюме и документов с использованием ИИ, семантического поиска по файлам (RAG) и аналитики в реальном времени.

  

## 🚀 Ключевые возможности

- **ИИ-Генерация документов:** Создание отчетов, резюме и выписок на основе промптов с экспортом в PDF/DOCX.
    
      
    
- **RAG & Semantic Search:** Поиск по смыслу среди ранее созданных документов пользователя с использованием векторных эмбеддингов.
    
      
    
- **Real-time Уведомления:** Отслеживание статуса генерации тяжелых документов в реальном времени через WebSockets.
    
      
    
- **Учет ресурсов и Аналитика:** Мониторинг расхода токенов LLM, гибкая система лимитов и аналитический разбор активности.
    
      
    
- **Изолированное хранение:** Структурированная файловая система пользователей с безопасным хранением бинарников в S3.
    
      
    

## 🛠 Технологический стек

|**Категория**|**Технологии**|
|---|---|
|**Backend & Core**|.NET 8+, ASP.NET Core Web API, Minimal APIs, C#|
|**Межсервисная связь**|gRPC (синхронная), Apache Kafka (событийная асинхронная)|
|**Базы данных & S3**|PostgreSQL, Pgvector / Qdrant (Векторная БД), Redis, MinIO (S3)|
|**ORM & Data Access**|Entity Framework Core (Write-модель), Dapper (Read-модель)|
|**AI & ML Integration**|Semantic Kernel / LangChain.NET, Ollama / OpenAI API|
|**API Gateway & Real-time**|YARP (Yet Another Reverse Proxy), SignalR|
|**Infrastructure & DevOps**|Docker, Docker Compose, Serilog (JSON), GitHub Actions (CI/CD)|

## 🏛 Архитектура сервисов

Система спроектирована по принципу **Database per Service** с четким разделением ответственности:

  

- **User Service:** Аутентификация/авторизация (Custom JWT + Refresh Tokens), управление профилями и правами доступа.
    
      
    
- **Document Service:** Бизнес-логика создания, управления документами, оркестрация генерации и интеграция с S3-хранилищем.
    
      
    
- **AI-Agent:** Изолированный сервис для работы с LLM, составления промптов и расчета эмбеддингов.
    
      
    
- **RAG & Search Service:** Векторизация загруженных файлов, индексация чанков и выдача контекста для ИИ по gRPC.
    
      
    
- **Notification Service:** Вычитывание событий из Kafka и Push-оповещение пользователей через SignalR Hub.
    
      
    
- **Analytics Service:** Сбор метрик потраченных токенов, агрегация статистики и ИИ-анализ трендов использования.
    
      
    

## 💡 Примененные паттерны и подходы

- **CQRS (Command Query Responsibility Segregation):** Разделение операций чтения и записи с помощью `MediatR`.
    
      
    
- **Transactional Outbox Pattern:** Гарантированная доставка событий в Kafka даже при сбоях брокера или БД.
    
      
    
- **Result Pattern (`ErrorOr`):** Явная обработка бизнес-ошибок без выброса тяжелых `Exceptions`.
    
      
    
- **Options Pattern:** Типизированная конфигурация сервисов через `IOptions<T>` с поддержкой горячей перезагрузки.
    
      
    
- **Global Exception Handling:** Централизованный перехват системных сбоев через `IExceptionHandler` в формате `ProblemDetails` (RFC 7807).
    
      
    

## 🚦 Быстрый запуск

1. Склонируйте репозиторий:
    
      
    
    Bash
    
    ```
    git clone https://github.com/your-username/MicroReport.git
    cd MicroReport
    ```
    
2. Запустите инфраструктурные контейнеры (PostgreSQL, Kafka, Redis, MinIO):
    
      
    
    Bash
    
    ```
    docker compose up -d
    ```
    
3. Примените миграции и запустите API Gateway:
    
      
    
    Bash
    
    ```
    dotnet run --project src/ApiGateway
    ```
    
