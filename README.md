# ReservAr

Sistema web para gestión de eventos, sectores, asientos, usuarios y reservaciones.

---

## 📋 Requisitos previos

- .NET SDK 10
- PostgreSQL
- Visual Studio Code o Visual Studio
- Entity Framework CLI

Instalar Entity Framework CLI:

bash
dotnet tool install --global dotnet-ef


Si ya está instalado:

bash
dotnet tool update --global dotnet-ef


---

## 🗄️ Configurar base de datos

Crear una base PostgreSQL:

sql
CREATE DATABASE "RA_Staging";


Configurar la cadena de conexión en appsettings.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=RA_Staging;Username=postgres;Password=TU_PASSWORD"
  }
}


---

## 📦 Restaurar dependencias

bash
dotnet restore


---

## 🛠️ Compilar el proyecto

bash
dotnet build


---

## 🧱 Ejecutar migraciones

Crear migración:

bash
dotnet ef migrations add InitialCreate


Aplicar migraciones:

bash
dotnet ef database update


---

## ▶️ Ejecutar la aplicación

bash
dotnet run


La aplicación se levanta en:


http://localhost:5183


---

## 🌐 Accesos principales

Frontend:


http://localhost:5183/index.html
http://localhost:5183/login.html
http://localhost:5183/signup.html
http://localhost:5183/reservations.html


Swagger:


http://localhost:5183/swagger/index.html


---

## 🔐 Autenticación

Obtener token:

http
POST /api/v1/auth


Body:

json
{
  "email": "usuario@email.com",
  "password": "password"
}


El token se guarda en localStorage como:


jwtToken


---

## 📡 Endpoints principales

### 👤 Users

http
POST /api/v1/users
GET /api/v1/users/by-email?email=usuario@email.com


---

### 🎟️ Events

http
GET /api/v1/events
GET /api/v1/events/{eventId}
POST /api/v1/events
PUT /api/v1/events/{eventId}


---

### 🏟️ Sectors

http
GET /api/v1/sectors?eventId=1
GET /api/v1/sectors/{sectorId}
POST /api/v1/sectors
PUT /api/v1/sectors/{sectorId}/price


---

### 💺 Seats

http
GET /api/v1/seats?sectorId=1
GET /api/v1/seats/{seatId}
POST /api/v1/seats
PATCH /api/v1/seats/{seatId}


---

### 🧾 Reservations

http
POST /api/v1/reservations
GET /api/v1/reservations
GET /api/v1/reservations/{reservationId}
PATCH /api/v1/reservations/{reservationId}
PATCH /api/v1/reservations/expire-pending


---

## 🔄 Flujo del sistema

1. Registrarse en /signup.html
2. Iniciar sesión en /login.html
3. Seleccionar evento en /index.html
4. Elegir sector y asiento
5. Reservar asiento
6. Se inicia un timer de 5 minutos
7. Si no paga → reserva expira

Liberar reservas vencidas:

http
PATCH /api/v1/reservations/expire-pending


---

## 🧠 Estados del sistema

### Evento


DISPONIBLE
SOLD-OUT
FINALIZADO


### Asiento


DISPONIBLE
RESERVADO
VENDIDO


### Reserva


PENDIENTE
PAGANDO
EXPIRADO


---

## ⚠️ Notas importantes

- Las APIs protegidas requieren JWT
- El token expira automáticamente
- Si expira → redirige a login
- El frontend usa localStorage
- El endpoint expire-pending libera asientos automáticamente

---

## 🚀 Tips de desarrollo

- Ejecutar expire-pending antes de cargar asientos
- Usar Swagger para pruebas rápidas
- Verificar estados en base de datos si algo no coincide

---

## 👨‍💻 Autor

Proyecto desarrollado como sistema de gestión de eventos y reservas tipo Ticketek.
