# 🚀 MicroShop — Microservices Assignment

A full microservices architecture built with **.NET 8**, **Vue 3**, **Docker**, **Redis**, and **RabbitMQ**.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                   Vue 3 Frontend (Port 3000)             │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP
┌──────────────────────────▼──────────────────────────────┐
│              API Gateway / Ocelot (Port 5000)            │
└──┬──────────┬────────────┬──────────────┬───────────────┘
   │          │            │              │
   ▼          ▼            ▼              ▼
UserSvc  UrlShortener  GoodsSvc   MessageBroker
(5001)    (5002)        (5003)       (5004)
   │          │                        │
   │      Redis Cache              RabbitMQ
   │      (6379)                   (5672)
   │
 SQLite (each service has its own DB)
```

## 📦 Services

| Service | Port | Description |
|---------|------|-------------|
| **API Gateway** | 5000 | Ocelot reverse proxy, routes all requests |
| **UserService** | 5001 | Register / Login / Logout (JWT) |
| **UrlShortenerService** | 5002 | URL shortening + Redis cache |
| **GoodsService** | 5003 | Products, Discounts, Orders/Payment |
| **MessageBrokerService** | 5004 | Contact form via RabbitMQ |
| **Redis** | 6379 | Cache layer for URL resolution |
| **RabbitMQ** | 5672 / 15672 | Message broker for contact messages |
| **Frontend (Vue 3)** | 3000 | Full UI (Shop, URL Shortener, Contact) |

---

## 🚀 Quick Start (Docker)

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run everything with one command:
```bash
docker-compose up --build
```

Then open: **http://localhost:3000**

- RabbitMQ Management UI: http://localhost:15672 (guest / guest)
- Swagger UIs:
  - User Service: http://localhost:5001/swagger
  - URL Service: http://localhost:5002/swagger
  - Goods Service: http://localhost:5003/swagger
  - Message Service: http://localhost:5004/swagger

---

## 💻 Running Locally (Visual Studio / VS Code)

### Backend (.NET 8 — Visual Studio purple)

1. Open `MicroservicesSolution.sln` in **Visual Studio 2022**
2. Right-click solution → **Set Startup Projects** → Multiple startup projects
3. Set all 4 services + ApiGateway to **Start**
4. Make sure Redis is running locally (or via Docker):
   ```bash
   docker run -d -p 6379:6379 redis:7-alpine
   docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management-alpine
   ```
5. Press **F5** — all services start on their ports

### Frontend (Vue 3 — VS Code blue)

```bash
cd frontend
npm install
npm run dev
```

Open **http://localhost:3000**

> ⚠️ The frontend dev server proxies `/api` to `http://localhost:5000` (API Gateway).
> Make sure the gateway is running before you open the frontend.

---

## 🔑 API Endpoints

### UserService (`/api/auth`)
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login, returns JWT |
| POST | `/api/auth/logout` | Logout (client drops token) |
| GET | `/api/auth/profile` | Get current user profile |
| PUT | `/api/auth/profile` | Update profile |
| GET | `/api/auth/validate?token=` | Validate JWT token |

### UrlShortenerService (`/api/url`)
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/url/shorten` | Create short URL |
| GET | `/r/{code}` | Redirect to original URL |
| GET | `/api/url/my-urls` | Get my URLs (auth required) |
| DELETE | `/api/url/{code}` | Delete URL (auth required) |

### GoodsService
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get product |
| POST | `/api/products` | Create product (auth) |
| POST | `/api/discounts` | Create discount code (auth) |
| POST | `/api/discounts/validate` | Validate discount code |
| GET | `/api/discounts/customer/{id}` | Get customer's discounts |
| POST | `/api/orders` | Create order + payment |
| GET | `/api/orders/customer/{id}` | Get customer's orders |
| GET | `/api/orders/{id}` | Get order details |

### MessageBrokerService (`/api/contact`)
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/contact/send` | Send contact message → RabbitMQ |
| GET | `/api/contact` | Get all messages |
| GET | `/api/contact/customer/{id}` | Get customer's messages |
| GET | `/api/contact/status` | Check RabbitMQ connection |

---

## 🧪 Test Credentials

After running, register an account at **http://localhost:3000/register**

To create a discount code via API (Swagger):
```json
POST /api/discounts
{
  "code": "SAVE10",
  "percentage": 10,
  "customerId": 1,
  "expiresInDays": 30
}
```

---

## 🎯 Assignment Features Coverage

| Feature | Implementation |
|---------|---------------|
| ✅ Microservices architecture | 4 independent .NET 8 services |
| ✅ API Gateway | Ocelot with route configuration |
| ✅ Service: User (Login/Register/Logout) | UserService + JWT auth |
| ✅ Service: Goods/Discount/Payment | GoodsService (shared CustomerId) |
| ✅ Service: URL Shortener | UrlShortenerService |
| ✅ Cache Service | Redis (StackExchange.Redis) |
| ✅ Message Broker | RabbitMQ (customer contact) |
| ✅ Docker | Docker + Docker Compose |
| ✅ Frontend | Vue 3 (Login, Shop, URL, Contact) |
| ✅ Inter-service HTTP | HttpClient / Ocelot routing |
| ✅ Database per service | SQLite (EF Core) per service |

---

## 📁 Project Structure

```
MicroservicesSolution/
├── MicroservicesSolution.sln       ← Open this in Visual Studio
├── docker-compose.yml              ← Run everything with Docker
├── UserService/
│   ├── Controllers/AuthController.cs
│   ├── Services/AuthService.cs     ← JWT + BCrypt
│   ├── Models/, DTOs/, Data/
│   └── Dockerfile
├── UrlShortenerService/
│   ├── Controllers/UrlController.cs
│   ├── Services/UrlShortenerSvc.cs ← Redis caching
│   └── Dockerfile
├── GoodsService/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   ├── DiscountsController.cs
│   │   └── OrdersController.cs     ← Payment logic
│   └── Dockerfile
├── MessageBrokerService/
│   ├── Controllers/ContactController.cs
│   ├── Services/RabbitMqService.cs
│   ├── Services/ContactMessageWorker.cs ← Background consumer
│   └── Dockerfile
├── ApiGateway/
│   ├── ocelot.json                 ← Route config
│   └── Dockerfile
└── frontend/                       ← Open folder in VS Code
    ├── src/
    │   ├── views/                  ← All pages
    │   ├── stores/                 ← Pinia (auth, cart)
    │   └── router/                 ← Vue Router
    ├── package.json
    └── Dockerfile
```
