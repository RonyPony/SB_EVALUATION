# Frontend de la plataforma de soporte SB

Cliente web desarrollado con React 19, TypeScript y Vite. Consume el API mediante JWT Bearer y guarda la sesión únicamente en `sessionStorage`.

## Requisitos

- Node.js 20 o posterior.
- npm 10 o posterior.
- Backend disponible en `http://localhost:5000`.

## Configuración

 `.env.example` como `.env.local` si necesita modificar el origen del API:

```powershell
Copy-Item .env.example .env.local
```

La única variable pública requerida es:

```dotenv
VITE_API_URL=http://localhost:5000
```

No coloque claves JWT, contraseñas ni secretos en variables `VITE_*`: Vite las incorpora al código entregado al navegador.

## Ejecutar

```powershell
npm install
npm run dev
```

Abra `http://localhost:5173`. Para ingresar localmente use `demo` / `demo` después de iniciar el backend en ambiente Development.

## Flujo principal

1. Iniciar sesión.
2. Consultar el panel y las solicitudes visibles para el rol actual.
3. Crear una solicitud seleccionando área, tipo y prioridad.
4. Como analista o administrador, asignar responsable y cambiar el estado.
5. Consultar comentarios, historial y notificaciones.

## Calidad y pruebas

```powershell
npm run lint
npm run build
npm test
```

`src/api.test.ts` comprueba la construcción de solicitudes HTTP, encabezados JWT, respuestas y errores del API.

## Decisiones

- React organiza la interfaz en componentes funcionales.
- TypeScript valida contratos y estados durante compilación.
- La URL del backend se inyecta por ambiente.
- No existen refresh tokens ni logout remoto; la sesión desaparece al expirar, cerrar sesión o recibir HTTP 401.
