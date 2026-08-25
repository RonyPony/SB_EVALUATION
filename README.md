# SB.BACKEND

Web API ASP.NET Core en .NET 8 con Onion Architecture y autenticación JWT Bearer.

## Proyectos

- `Domain`: modelo de dominio, sin referencias a otros proyectos.
- `Application`: contratos de autenticación; solo referencia `Domain`.
- `Services`: JWT y usuario demostrativo; referencia `Application` y `Domain`.
- `Infrastructure`: registro para persistencia futura, todavía sin base de datos ni repositorios ficticios.
- `Api`: controladores y Composition Root.

## Restaurar y compilar

```powershell
dotnet restore .\SB.BACKEND.sln
dotnet build .\SB.BACKEND.sln --configuration Debug --no-restore
```

## Secretos de desarrollo

Development incluye valores deliberadamente inseguros para ejecutar la demostración. Para reemplazar la clave con User Secrets:

```powershell
dotnet user-secrets set "Jwt:SecretKey" "replace-with-a-random-key-of-at-least-32-characters" --project .\src\SB.BACKEND.Api
dotnet user-secrets set "DemoUser:Username" "local-user" --project .\src\SB.BACKEND.Api
dotnet user-secrets set "DemoUser:Password" "local-development-password" --project .\src\SB.BACKEND.Api
```

No almacene secretos reales en `appsettings*.json` ni en Git.

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

## Probar autenticación

```powershell
$login = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:5080/api/auth/login" `
  -ContentType "application/json" `
  -Body '{"username":"demo","password":"demo-password-not-real"}'

Invoke-RestMethod -Uri "http://localhost:5080/api/sample/protected" `
  -Headers @{ Authorization = "Bearer $($login.accessToken)" }

Invoke-RestMethod -Uri "http://localhost:5080/api/sample/admin" `
  -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```

También puede ejecutar en orden las solicitudes de `src/SB.BACKEND.Api/SB.BACKEND.Api.http` desde Visual Studio.
