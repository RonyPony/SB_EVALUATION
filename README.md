# SB.BACKEND

Web API ASP.NET Core en .NET 8 con Onion Architecture, EF Core/SQL Server y autenticación JWT Bearer.

## Proyectos

- `Domain`: modelo de dominio, sin referencias a otros proyectos.
- `Application`: DTO, contratos de seguridad, repositorios y excepciones de aplicación.
- `Services`: casos de uso, JWT y hashing PBKDF2-SHA256.
- `Infrastructure`: EF Core, SQL Server, repositorios, migraciones y seed.
- `Api`: controladores y Composition Root.

## Restaurar y compilar

```powershell
dotnet restore .\SB.BACKEND.sln
dotnet build .\SB.BACKEND.sln --configuration Debug --no-restore
```

## Configuración local

La conexión a `LEGEND\\SQLEXPRESS`, la base `SB_EVALUATION_RONEL` y las credenciales solicitadas están configuradas directamente en `appsettings.json`. El nombre y correo del usuario `demo` permanecen en `appsettings.Development.json`; el seed guarda únicamente el hash de su contraseña en SQL Server.

Al iniciar, la aplicación aplica migraciones, crea los roles `Admin` y `User`, asigna todos los permisos a `Admin` y crea el usuario demo de forma idempotente.

Para producción se recomienda sobrescribir `ConnectionStrings__DefaultConnection`, `DemoUser__Password` y `Jwt__SecretKey` mediante variables de entorno y no versionar credenciales reales.

## Ejecutar

```powershell
dotnet run --project .\src\SB.BACKEND.Api --launch-profile http
```

API: `http://localhost:5080`; Swagger: `http://localhost:5080/swagger`; Health Check: `http://localhost:5080/health`.

## Producción

La clave de Production está vacía y la aplicación no inicia sin una clave de al menos 32 caracteres. Suminístrela desde un almacén seguro mediante `Jwt__SecretKey`:

```powershell
$env:Jwt__SecretKey = "a-secret-provided-by-the-production-secret-store"
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --project .\src\SB.BACKEND.Api
```

## Migraciones

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update --project .\src\SB.BACKEND.Infrastructure --startup-project .\src\SB.BACKEND.Api
```

## API de seguridad

- `POST /api/auth/register`, `POST /api/auth/login`
- `GET /api/users`, `GET /api/users/{id}`
- `PUT|DELETE /api/users/{userId}/roles/{roleId}`
- `GET|POST /api/roles`, `PUT|DELETE /api/roles/{id}`
- `PUT|DELETE /api/roles/{roleId}/permissions/{permissionId}`
- `GET /api/permissions` (solo lectura)

Salvo registro y login, los endpoints exigen JWT y la política `SECURITY.*` correspondiente.

## Probar autenticación

```powershell
$login = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:5080/api/auth/login" `
  -ContentType "application/json" `
  -Body '{"username":"demo","password":"the-value-stored-in-user-secrets"}'

Invoke-RestMethod -Uri "http://localhost:5080/api/sample/protected" `
  -Headers @{ Authorization = "Bearer $($login.accessToken)" }

Invoke-RestMethod -Uri "http://localhost:5080/api/sample/admin" `
  -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```

También puede ejecutar en orden las solicitudes de `src/SB.BACKEND.Api/SB.BACKEND.Api.http` desde Visual Studio.
