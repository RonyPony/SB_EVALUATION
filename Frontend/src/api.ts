export type ProblemDetails = {
  title?: string;
  detail?: string;
  status?: number;
};
export type PagedResult<T> = {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
};
export type User = {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  roles: string[];
};
export type Role = {
  id: string;
  name: string;
  description?: string;
  permissions: string[];
};
export type Permission = { id: string; name: string; description: string };
export type Area = {
  id: string;
  nombre: string;
  descripcion?: string;
  activo: boolean;
  rowVersion: string;
};
export type GovernmentEntity = {
  id: string;
  nombre: string;
  categoria: string;
  poderDelEstado: string;
  sector: string;
  activo: boolean;
  isDeleted: boolean;
  createdAt: string;
  updatedAt?: string;
  deletedAt?: string;
  rowVersion: string;
};
export type RequestItem = {
  id: string;
  codigo: string;
  titulo: string;
  descripcion: string;
  tipo: number;
  prioridad: number;
  estado: number;
  areaId: string;
  area: string;
  solicitanteId: string;
  responsableId?: string;
  responsable?: string;
  referenciaEvidencia?: string;
  fechaCompromiso?: string;
  createdAt: string;
  updatedAt?: string;
  rowVersion: string;
};
export type HistoryItem = {
  id: string;
  estadoAnterior: number;
  estadoNuevo: number;
  comentario: string;
  usuarioId: string;
  createdAt: string;
};
export type CommentItem = {
  id: string;
  usuarioId: string;
  contenido: string;
  esInterno: boolean;
  createdAt: string;
  updatedAt?: string;
  rowVersion: string;
};
export type NotificationItem = {
  id: string;
  solicitudId: string;
  tipo: number;
  titulo: string;
  mensaje: string;
  leida: boolean;
  fechaLectura?: string;
  createdAt: string;
};
export type Dashboard = {
  abiertas: number;
  cerradas: number;
  vencidas: number;
  porEstado: Record<string, number>;
  porPrioridad: Record<string, number>;
  ultimas: Array<{
    id: string;
    codigo: string;
    titulo: string;
    estado: number;
    responsable?: string;
    createdAt: string;
    fechaCompromiso?: string;
  }>;
};
export type LoginResponse = {
  accessToken: string;
  tokenType: string;
  expiresAt: string;
};

const apiUrl = import.meta.env.VITE_API_URL?.replace(/\/$/, "");
if (!apiUrl)
  throw new Error(
    "VITE_API_URL es obligatoria. Configúrala antes de iniciar el frontend.",
  );

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}
let tokenProvider: () => string | null = () => null;
let unauthorizedHandler: () => void = () => undefined;
export function configureApi(
  getToken: () => string | null,
  onUnauthorized: () => void,
) {
  tokenProvider = getToken;
  unauthorizedHandler = onUnauthorized;
}

function query(params: Record<string, string | number | boolean | undefined>) {
  const value = new URLSearchParams();
  Object.entries(params).forEach(([key, item]) => {
    if (item !== undefined && item !== "") value.set(key, String(item));
  });
  const result = value.toString();
  return result ? `?${result}` : "";
}

export async function request<T>(
  path: string,
  init: RequestInit = {},
): Promise<T> {
  const headers = new Headers(init.headers);
  if (init.body) headers.set("Content-Type", "application/json");
  const token = tokenProvider();
  if (token) headers.set("Authorization", `Bearer ${token}`);
  let response: Response;
  try {
    response = await fetch(`${apiUrl}${path}`, { ...init, headers });
  } catch {
    throw new ApiError(0, "No fue posible conectar con el servidor.");
  }
  if (response.status === 401) unauthorizedHandler();
  if (!response.ok) {
    let problem: ProblemDetails = {};
    try {
      problem = (await response.json()) as ProblemDetails;
    } catch {
      /* response without JSON */
    }
    throw new ApiError(
      response.status,
      problem.detail ||
        problem.title ||
        `La solicitud falló (${response.status}).`,
    );
  }
  return response.status === 204
    ? (undefined as T)
    : (response.json() as Promise<T>);
}

export const authApi = {
  login: (body: { username: string; password: string }) =>
    request<LoginResponse>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(body),
    }),
  me: () => request<User>("/api/auth/me"),
};
export const dashboardApi = { get: () => request<Dashboard>("/api/dashboard") };
export const areasApi = {
  list: (activeOnly = false) =>
    request<Area[]>(`/api/areas${query({ activeOnly })}`),
  get: (id: string) => request<Area>(`/api/areas/${id}`),
  create: (body: { nombre: string; descripcion?: string }) =>
    request<Area>("/api/areas", { method: "POST", body: JSON.stringify(body) }),
  update: (
    id: string,
    body: { nombre: string; descripcion?: string; rowVersion: string },
  ) =>
    request<Area>(`/api/areas/${id}`, {
      method: "PUT",
      body: JSON.stringify(body),
    }),
  status: (id: string, activo: boolean, rowVersion: string) =>
    request<void>(`/api/areas/${id}/estado`, {
      method: "PATCH",
      body: JSON.stringify({ activo, rowVersion }),
    }),
  remove: (id: string, rowVersion: string) =>
    request<void>(`/api/areas/${id}${query({ rowVersion })}`, {
      method: "DELETE",
    }),
};
export const entitiesApi = {
  list: (params: Record<string, string | number | boolean | undefined> = {}) =>
    request<PagedResult<GovernmentEntity>>(
      `/api/entidades-gubernamentales${query(params)}`,
    ),
  deleted: (
    params: Record<string, string | number | boolean | undefined> = {},
  ) =>
    request<PagedResult<GovernmentEntity>>(
      `/api/entidades-gubernamentales/eliminadas${query(params)}`,
    ),
  get: (id: string) =>
    request<GovernmentEntity>(`/api/entidades-gubernamentales/${id}`),
  create: (
    body: Omit<
      GovernmentEntity,
      "id" | "activo" | "isDeleted" | "createdAt" | "rowVersion"
    >,
  ) =>
    request<GovernmentEntity>("/api/entidades-gubernamentales", {
      method: "POST",
      body: JSON.stringify(body),
    }),
  update: (
    id: string,
    body: {
      nombre: string;
      categoria: string;
      poderDelEstado: string;
      sector: string;
      rowVersion: string;
    },
  ) =>
    request<GovernmentEntity>(`/api/entidades-gubernamentales/${id}`, {
      method: "PUT",
      body: JSON.stringify(body),
    }),
  status: (id: string, activo: boolean, rowVersion: string) =>
    request<void>(`/api/entidades-gubernamentales/${id}/estado`, {
      method: "PATCH",
      body: JSON.stringify({ activo, rowVersion }),
    }),
  remove: (id: string, rowVersion: string) =>
    request<void>(
      `/api/entidades-gubernamentales/${id}${query({ rowVersion })}`,
      { method: "DELETE" },
    ),
  restore: (id: string, rowVersion: string) =>
    request<void>(
      `/api/entidades-gubernamentales/${id}/restaurar${query({ rowVersion })}`,
      { method: "PATCH" },
    ),
};
export const usersApi = {
  list: () => request<User[]>("/api/users"),
  get: (id: string) => request<User>(`/api/users/${id}`),
  analysts: (search = "", pageNumber = 1, pageSize = 100) =>
    request<PagedResult<User>>(
      `/api/users/analysts${query({ search, pageNumber, pageSize })}`,
    ),
  assignRole: (userId: string, roleId: string) =>
    request<void>(`/api/users/${userId}/roles/${roleId}`, { method: "PUT" }),
  removeRole: (userId: string, roleId: string) =>
    request<void>(`/api/users/${userId}/roles/${roleId}`, { method: "DELETE" }),
};
export const rolesApi = {
  list: () => request<Role[]>("/api/roles"),
  create: (body: { name: string; description?: string }) =>
    request<Role>("/api/roles", { method: "POST", body: JSON.stringify(body) }),
  update: (id: string, body: { name: string; description?: string }) =>
    request<Role>(`/api/roles/${id}`, {
      method: "PUT",
      body: JSON.stringify(body),
    }),
  remove: (id: string) =>
    request<void>(`/api/roles/${id}`, { method: "DELETE" }),
  assignPermission: (roleId: string, permissionId: string) =>
    request<void>(`/api/roles/${roleId}/permissions/${permissionId}`, {
      method: "PUT",
    }),
  removePermission: (roleId: string, permissionId: string) =>
    request<void>(`/api/roles/${roleId}/permissions/${permissionId}`, {
      method: "DELETE",
    }),
};
export const permissionsApi = {
  list: () => request<Permission[]>("/api/permissions"),
};
export const requestsApi = {
  list: (params: Record<string, string | number | boolean | undefined> = {}) =>
    request<PagedResult<RequestItem>>(`/api/solicitudes${query(params)}`),
  get: (id: string) => request<RequestItem>(`/api/solicitudes/${id}`),
  create: (body: {
    titulo: string;
    descripcion: string;
    tipo: number;
    prioridad: number;
    areaId: string;
    referenciaEvidencia?: string;
    fechaCompromiso?: string;
  }) =>
    request<RequestItem>("/api/solicitudes", {
      method: "POST",
      body: JSON.stringify(body),
    }),
  update: (
    id: string,
    body: {
      titulo: string;
      descripcion: string;
      tipo: number;
      areaId: string;
      referenciaEvidencia?: string;
      fechaCompromiso?: string;
      rowVersion: string;
    },
  ) =>
    request<RequestItem>(`/api/solicitudes/${id}`, {
      method: "PUT",
      body: JSON.stringify(body),
    }),
  patch: (
    id: string,
    body: {
      titulo?: string;
      descripcion?: string;
      tipo?: number;
      areaId?: string;
      referenciaEvidencia?: string;
      rowVersion: string;
    },
  ) =>
    request<RequestItem>(`/api/solicitudes/${id}`, {
      method: "PATCH",
      body: JSON.stringify(body),
    }),
  priority: (id: string, prioridad: number, rowVersion: string) =>
    request<void>(`/api/solicitudes/${id}/prioridad`, {
      method: "PATCH",
      body: JSON.stringify({ prioridad, rowVersion }),
    }),
  assign: (id: string, responsableId: string, rowVersion: string) =>
    request<void>(`/api/solicitudes/${id}/asignacion`, {
      method: "PATCH",
      body: JSON.stringify({ responsableId, rowVersion }),
    }),
  status: (
    id: string,
    estado: number,
    comentario: string,
    rowVersion: string,
    comentarioResolucion?: string,
  ) =>
    request<void>(`/api/solicitudes/${id}/estado`, {
      method: "PATCH",
      body: JSON.stringify({
        estado,
        comentario,
        comentarioResolucion,
        rowVersion,
      }),
    }),
  reopen: (id: string, comentario: string, rowVersion: string) =>
    request<void>(`/api/solicitudes/${id}/reabrir`, {
      method: "PATCH",
      body: JSON.stringify({ comentario, rowVersion }),
    }),
  remove: (id: string, rowVersion: string) =>
    request<void>(`/api/solicitudes/${id}${query({ rowVersion })}`, {
      method: "DELETE",
    }),
  history: (id: string) =>
    request<HistoryItem[]>(`/api/solicitudes/${id}/historial`),
  comments: (id: string) =>
    request<CommentItem[]>(`/api/solicitudes/${id}/comentarios`),
  addComment: (id: string, contenido: string, esInterno: boolean) =>
    request<CommentItem>(`/api/solicitudes/${id}/comentarios`, {
      method: "POST",
      body: JSON.stringify({ contenido, esInterno }),
    }),
  updateComment: (
    id: string,
    commentId: string,
    contenido: string,
    rowVersion: string,
  ) =>
    request<CommentItem>(`/api/solicitudes/${id}/comentarios/${commentId}`, {
      method: "PATCH",
      body: JSON.stringify({ contenido, rowVersion }),
    }),
  removeComment: (id: string, commentId: string, rowVersion: string) =>
    request<void>(
      `/api/solicitudes/${id}/comentarios/${commentId}${query({ rowVersion })}`,
      { method: "DELETE" },
    ),
};
export const notificationsApi = {
  list: () => request<NotificationItem[]>("/api/notificaciones"),
  count: () =>
    request<{ count: number }>("/api/notificaciones/no-leidas/count"),
  read: (id: string) =>
    request<void>(`/api/notificaciones/${id}/leida`, { method: "PATCH" }),
  readAll: () =>
    request<void>("/api/notificaciones/leer-todas", { method: "PATCH" }),
};
