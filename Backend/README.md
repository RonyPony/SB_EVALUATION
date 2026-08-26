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

## Entidades gubernamentales

El catálogo inicial se carga desde `src/SB.BACKEND.Infrastructure/Data/Seed/ListaEntidadesGubernamentales.xlsx`.
El inicializador valida las cuatro columnas, carga 181 entidades y omite nombres ya existentes —incluidos los eliminados— para no reactivarlos ni duplicarlos.

- `GET /api/entidades-gubernamentales`: búsqueda, filtros, orden y paginación.
- `GET /api/entidades-gubernamentales/{id}`
- `POST /api/entidades-gubernamentales`
- `PUT /api/entidades-gubernamentales/{id}`
- `PATCH /api/entidades-gubernamentales/{id}/estado`
- `DELETE /api/entidades-gubernamentales/{id}?rowVersion={base64}`
- `GET /api/entidades-gubernamentales/eliminadas`
- `PATCH /api/entidades-gubernamentales/{id}/restaurar?rowVersion={base64}`

Los endpoints usan las políticas `GOVERNMENT_ENTITY.VIEW`, `CREATE`, `UPDATE`, `DELETE` y `RESTORE`.
La eliminación es lógica y el filtro global de EF Core excluye registros eliminados. Las escrituras devuelven un `RowVersion` Base64 que debe enviarse en operaciones posteriores para detectar concurrencia optimista.

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


### Modelo y persistencia

- `Area`: catálogo auditable con nombre normalizado, estado y eliminación lógica.
- `SolicitudSoporte`: solicitud vinculada a un área, al usuario solicitante y opcionalmente a un responsable.
- `HistorialSolicitud`: registro inmutable y cronológico de transiciones y asignaciones.
- `ComentarioSolicitud`: comentario público o interno con auditoría y eliminación lógica.
- `Notificacion`: notificación persistida por usuario y solicitud.

Las relaciones usan `DeleteBehavior.Restrict`. Los filtros globales excluyen solicitudes, áreas y comentarios eliminados. Las fechas se guardan como `datetimeoffset` en UTC. Las escrituras de entidades auditables actualizan automáticamente `CreatedAt`, `UpdatedAt`, `CreatedBy` y `UpdatedBy`; la eliminación lógica establece `IsDeleted`, `DeletedAt` y `DeletedBy`. La concurrencia optimista utiliza `rowversion` enviado como Base64.

La migración `20260826040652_AddSupportPlatform` crea las tablas, claves foráneas, índices y la secuencia `SolicitudCodeSequence`. El código se genera en SQL Server con el formato `SOL-{AÑO}-{CONSECUTIVO}` y tiene índice único. La secuencia evita el patrón inseguro `Count() + 1` y garantiza valores distintos bajo concurrencia.

### Roles y autorización

El inicializador crea idempotentemente `Administrador`, `Analista` y `Solicitante`. Se conservan `Admin` y `User`; `Admin` se considera equivalente administrativo para compatibilidad. El usuario demo recibe `Administrador` sin duplicar relaciones. `SupportAdmin` admite `Admin` o `Administrador`; `SupportStaff` admite además `Analista`.

La autorización por recurso se completa en los servicios: el administrador ve todas las solicitudes; el analista ve las asignadas y las disponibles sin responsable; el solicitante solo ve las propias. Los comentarios internos se restringen al personal de soporte. Un responsable debe ser un usuario activo con rol `Analista`, `Administrador` o `Admin`.

### Flujo de estados

Las transiciones se centralizan en el dominio:

- `Registrada` → `EnAnalisis`.
- `EnAnalisis` → `EnProgreso`.
- `EnProgreso` → `EnEsperaSolicitante` o `Resuelta`.
- `EnEsperaSolicitante` → `EnProgreso`.
- `Resuelta` → `EnProgreso` o `Cerrada`.
- `Cerrada` → `EnProgreso` únicamente mediante reapertura autorizada.

Cada cambio registra usuario, fecha UTC, estado anterior, estado nuevo y comentario. Resolver exige comentario de resolución y cerrar exige una resolución registrada. Una transición inválida produce `409 Conflict`. La asignación y reasignación también conservan responsable anterior, responsable nuevo, actor y fecha en el historial.

### Notificaciones y consistencia

`INotificationService` desacopla la creación de notificaciones del caso de uso. Se generan registros por creación, asignación, reasignación, cambio de estado, resolución, cierre, reapertura y comentario público. La modificación, el historial y la notificación se confirman mediante el mismo `SecurityDbContext` y `SaveChangesAsync`, por lo que forman una única unidad transaccional.

### Endpoints

- Áreas: `GET|POST /api/areas`, `GET|PUT|DELETE /api/areas/{id}`, `PATCH /api/areas/{id}/estado`.
- Solicitudes: `GET|POST /api/solicitudes`, `GET|PUT|PATCH|DELETE /api/solicitudes/{id}`.
- Operaciones: `PATCH /api/solicitudes/{id}/prioridad`, `/asignacion`, `/estado` y `/reabrir`.
- Trazabilidad: `GET /api/solicitudes/{id}/historial`.
- Comentarios: `GET|POST /api/solicitudes/{id}/comentarios`, `PATCH|DELETE /api/solicitudes/{id}/comentarios/{comentarioId}`.
- Responsables elegibles: `GET /api/users/analysts?search=&pageNumber=1&pageSize=20`.
- Notificaciones: `GET /api/notificaciones`, `GET /api/notificaciones/no-leidas/count`, `PATCH /api/notificaciones/{id}/leida`, `PATCH /api/notificaciones/leer-todas`.
- Métricas: `GET /api/dashboard/solicitudes`.

El listado acepta búsqueda, estado, prioridad, área, solicitante, responsable, solicitudes sin responsable, tipo, rangos de creación y compromiso, vencidas, orden y paginación. Abiertas son las solicitudes no cerradas; cerradas tienen estado `Cerrada`; vencidas son las no cerradas con compromiso anterior a la hora UTC actual. Todas las métricas excluyen eliminadas y respetan el alcance del usuario.

### Ejemplo de operación

```powershell
$token = (Invoke-RestMethod -Method Post -Uri "http://localhost:5080/api/auth/login" -ContentType "application/json" -Body '{"username":"demo","password":"valor-configurado"}').accessToken
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Method Get -Uri "http://localhost:5080/api/areas" -Headers $headers
Invoke-RestMethod -Method Get -Uri "http://localhost:5080/api/solicitudes?pageNumber=1&pageSize=20" -Headers $headers
Invoke-RestMethod -Method Get -Uri "http://localhost:5080/api/dashboard/solicitudes" -Headers $headers
```


## Referencia documental

Documento de Prueba Técnica De Ronel Cruz
Versión 1.0  
Agosto de 2026  
Uso exclusivo de la Superintendencia de Bancos  
Distribución autorizada por el área responsable
