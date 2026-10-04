# Smart Home Sensor Management API

## Prerequisites

- Docker and Docker Compose

## Getting Started

### Option 1: Using Docker Compose (Recommended)

The easiest way to start the application is to use Docker Compose:

```bash
./init.sh
```

This script will:

1. Build and start the PostgreSQL and application containers
2. Wait for the services to be ready
3. Display information about how to access the API

Alternatively, you can run Docker Compose directly:

```bash
docker-compose up -d
```

The API will be available at http://localhost:8080

### Option 2: Manual setup

If you prefer to run the application without Docker:

1. Start the PostgreSQL database:

```bash
docker-compose up -d postgres
```

2. Build and run the application:

```bash
go build -o smarthome
./smarthome
```

## API Testing

A Postman collection is provided for testing the API. Import the `smarthome-api.postman_collection.json` file into Postman to get started.

## API Endpoints

- `GET /health` - Health check
- `GET /api/v1/sensors` - Get all sensors
- `GET /api/v1/sensors/:id` - Get a specific sensor
- `POST /api/v1/sensors` - Create a new sensor
- `PUT /api/v1/sensors/:id` - Update a sensor
- `DELETE /api/v1/sensors/:id` - Delete a sensor
- `PATCH /api/v1/sensors/:id/value` - Update a sensor's value and status

## Temperature API

Отдельный сервис `temperature-api` (C#, .NET 9) отдаёт случайное значение температуры.
Монолит `smart_home` обращается к нему по адресу из переменной `TEMPERATURE_API_URL`.

Сервис доступен на http://localhost:8081

### Эндпоинты

- `GET /health` — проверка доступности
- `GET /temperature?location=Living%20Room` — показание по названию комнаты
- `GET /temperature?sensorId=1` — показание по идентификатору датчика
- `GET /temperature/1` — показание по идентификатору датчика в пути
- `GET /swagger` — документация OpenAPI

Если не передать `location`, он определяется по `sensorId` (1 — Living Room, 2 — Bedroom,
3 — Kitchen, иначе Unknown). Если не передать `sensorId`, он определяется по `location`
(Living Room — 1, Bedroom — 2, Kitchen — 3, иначе 0).

### Формат ответа

```json
{
  "value": 21.4,
  "unit": "°C",
  "timestamp": "2026-10-04T19:30:00.0000000Z",
  "location": "Living Room",
  "status": "active",
  "sensor_id": "1",
  "sensor_type": "temperature",
  "description": "Temperature reading for Living Room"
}
```

### Проверка задания

```bash
cd apps
docker compose up --build -d

curl -X POST http://localhost:8080/api/v1/sensors \
  -H "Content-Type: application/json" \
  -d '{"name":"Living Room Temperature","type":"temperature","location":"Living Room","unit":"°C"}'

curl http://localhost:8080/api/v1/sensors
curl http://localhost:8080/api/v1/sensors
```

Поле `value` в ответе `Get All Sensors` меняется при каждом вызове, потому что монолит
берёт его из `temperature-api`, а не из базы.
