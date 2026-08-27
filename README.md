# SB Evaluation — plataforma de soporte - RONEL CRUZ C.

Solución local compuesta por un API ASP.NET Core .NET 8 y un cliente React con TypeScript. El backend sigue Onion Architecture y separa Dominio, Aplicación, Servicios, Infraestructura y API.

## Requisitos

- .NET SDK 8.x.
- Node.js 20 o posterior y npm 10 o posterior.
- SQL Server 2019 o posterior, SQL Server Express o SQL Server LocalDB.
- PowerShell para ejecutar los ejemplos.

La configuración incluida utiliza `(localdb)\\MSSQLLocalDB`, autenticación integrada y la base `SB_EVALUATION`. Es una configuración local de ejemplo sin contraseñas reales. En Linux/macOS o con otra instancia SQL Server, sustituya `ConnectionStrings__DefaultConnection`.

## 1. Preparar y ejecutar la base de datos

El proyecto entrega tres migraciones EF Core en `Backend/src/SB.BACKEND.Infrastructure/Persistence/Migrations`. Al iniciar el API, `DatabaseInitializer` ejecuta automáticamente las migraciones pendientes y carga roles, permisos, el usuario de prueba y el catálogo gubernamental.

También puede aplicarlas manualmente:

```powershell
Set-Location Backend
dotnet tool restore
dotnet tool run dotnet-ef database update --project .\src\SB.BACKEND.Infrastructure --startup-project .\src\SB.BACKEND.Api
```

Como alternativa para la estructura inicial de seguridad se incluye `Backend/artifacts/InitialSecurity.sql`; las migraciones EF Core son la fuente principal para obtener el esquema completo.

Para usar otra instancia sin modificar archivos versionados:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=.\SQLEXPRESS;Database=SB_EVALUATION;Trusted_Connection=True;TrustServerCertificate=True"
```

## 2. Ejecutar el backend

```powershell
Set-Location Backend
dotnet restore .\SB.BACKEND.sln
dotnet build .\SB.BACKEND.sln --configuration Release
dotnet run --project .\src\SB.BACKEND.Api --launch-profile http
```

- API: `http://localhost:5080`
- Swagger: `http://localhost:5080/swagger`
- Health check: `http://localhost:5080/health`

El perfil Development contiene únicamente una clave JWT explícitamente insegura para desarrollo. Producción exige suministrar `Jwt__SecretKey` desde variables de ambiente o un almacén de secretos.

## 3. Ejecutar el frontend

En otra terminal:

```powershell
Set-Location Frontend
Copy-Item .env.example .env.local -ErrorAction SilentlyContinue
npm install
npm run dev
```

Abra `http://localhost:5173`. `.env.example` apunta al backend local en el puerto 5080 y no contiene secretos.

## Autenticación y usuario de prueba

En ambiente Development el inicializador crea idempotentemente este usuario no sensible:

- Usuario: `demo`
- Contraseña: `demo`
- Correo: `demo@local.invalid`
- Roles: `Admin`, `User` y `Administrador`

Puede obtener un token desde Swagger, el frontend o PowerShell:

```powershell
$login = Invoke-RestMethod -Method Post -Uri "http://localhost:5080/api/auth/login" -ContentType "application/json" -Body '{"username":"demo","password":"demo"}'
$headers = @{ Authorization = "Bearer $($login.accessToken)" }
Invoke-RestMethod -Uri "http://localhost:5080/api/dashboard/solicitudes" -Headers $headers
```

Estas credenciales son exclusivamente locales. Cambie `DemoUser__Password` y `Jwt__SecretKey` para cualquier ambiente compartido.

## Flujo principal

1. El usuario inicia sesión y obtiene un JWT Bearer.
2. Consulta el panel, áreas y solicitudes permitidas por sus roles.
3. Registra una solicitud con área, descripción, tipo y prioridad.
4. El personal autorizado la asigna y avanza por el flujo de estados.
5. Comentarios, transiciones y asignaciones quedan en el historial; los involucrados reciben notificaciones.
6. El administrador mantiene áreas, entidades gubernamentales, usuarios, roles y permisos.

## Arquitectura y decisiones principales

- `Domain`: entidades, enums y reglas de transición sin dependencias externas.
- `Application`: DTO, contratos, excepciones y abstracciones de persistencia.
- `Services`: casos de uso, autorización por recurso, JWT y hashing PBKDF2.
- `Infrastructure`: EF Core, SQL Server, repositorios, migraciones y datos semilla.
- `Api`: controladores, JWT Bearer, Swagger, CORS, Serilog y manejo global de excepciones.
- Frontend: React/TypeScript con Vite y cliente HTTP centralizado.
- Eliminación lógica y `rowversion` protegen integridad y concurrencia.
- Serilog escribe en consola y archivos diarios bajo `Backend/src/SB.BACKEND.Api/logs`.

## Pruebas incluidas

Backend:

```powershell
Set-Location Backend
dotnet test .\SB.BACKEND.sln --configuration Release
```

Incluye 19 pruebas de reglas de dominio para solicitudes y entidades gubernamentales.

Frontend:

```powershell
Set-Location Frontend
npm run lint
npm run build
npm test
```

Incluye pruebas del cliente HTTP y sus escenarios de autenticación/error.

## Configuración de producción

No se incluyen secretos reales. Defina al menos estas variables en el ambiente de despliegue:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__DefaultConnection = "<connection-string-from-secret-store>"
$env:Jwt__SecretKey = "<secret-with-at-least-32-characters>"
$env:DemoUser__Password = ""
```

Configure además `Cors__AllowedOrigins__0` con el origen público del frontend. Consulte los README de `Backend` y `Frontend` para detalles específicos.
