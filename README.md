# PS_ReservAr
Aplicación Web centrada en la administración y compra de eventos recreativos y de entretenimiento.

## Guía de Inicio Rápido

Sigue estos pasos para configurar el entorno de desarrollo y ejecutar el proyecto localmente.

### 1. Requisitos Previos
Antes de comenzar, asegúrate de tener instalado:
- [**.NET SDK**](https://dotnet.microsoft.com/download) (Versión 8.0 o superior).
- [**Git**](https://git-scm.com/).

### 2. Descargar el Repositorio
Clona el proyecto en tu máquina local utilizando el siguiente comando en tu terminal:

```bash
git clone https://github.com/tu-usuario/PS_ReservAr.git
cd ReservAr
```

### 3. Instalar Dependencias
El proyecto utiliza varios paquetes NuGet (como Entity Framework Core y JWT). Para instalarlos todos automáticamente, ejecuta:

```bash
dotnet restore
```

### 4. Configuración y Ejecución
1. **Base de Datos**: 
   - Asegúrate de que el servicio de PostgreSQL esté corriendo.
   - Crea una base de datos o verifica que las credenciales en `appsettings.json` coincidan con tu instancia local (Usuario, Contraseña, Puerto).
   - Por defecto, el proyecto busca el usuario `UARAT10` y la base `RA_Staging`.
   - Ejecuta los siguientes comandos en tu terminal de PostgreSQL (`psql`) o herramienta de administración (como pgAdmin) para configurar el acceso:

   ```sql
   -- 1. Crear el usuario con la contraseña configurada
   CREATE USER "UARAT10" WITH PASSWORD '6g1Se2"75mK8';

   -- 2. Crear la base de datos asignando al usuario como dueño
   CREATE DATABASE "RA_Staging" OWNER "UARAT10";

   -- 3. Otorgar permisos completos para CRUD y gestión de tablas
   GRANT ALL PRIVILEGES ON DATABASE "RA_Staging" TO "UARAT10";
   -- Conectado a la base RA_Staging, otorga permisos sobre el esquema public:
   \c "RA_Staging"
   GRANT ALL ON SCHEMA public TO "UARAT10";
   ```


### 4. Configuración y Ejecución
1. Asegúrate de configurar la cadena de conexión a la base de datos y la clave JWT en el archivo `appsettings.json`.
2. Aplica las migraciones de la base de datos (si utilizas EF Core):
   ```bash
   dotnet ef database update
   ```
3. Inicia la aplicación:
   ```bash
   dotnet run
   ```
