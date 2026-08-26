import { useEffect, useMemo, useState } from "react";
import logo from "./assets/SUPERINTENDENCIA_DE_BANCOS.svg";
import homeIcon from "./assets/home.svg";
import {
  ApiError,
  areasApi,
  dashboardApi,
  entitiesApi,
  notificationsApi,
  permissionsApi,
  requestsApi,
  rolesApi,
  usersApi,
  type Area,
  type Dashboard,
  type GovernmentEntity,
  type NotificationItem,
  type Permission,
  type RequestItem,
  type Role,
  type User,
} from "./api";
import { useAuth } from "./auth";
import "./App.css";

type NavItem = { id: string; label: string; roles: string[]; icon?: string };
const allRoles = ["Admin", "Administrador", "Analista", "Solicitante", "User"];
const adminRoles = ["Admin", "Administrador"];
const navigation: NavItem[] = [
  { id: "inicio", label: "Inicio", roles: allRoles, icon: homeIcon },
  { id: "solicitudes", label: "Solicitudes", roles: allRoles },
  {
    id: "crear",
    label: "Crear solicitud",
    roles: ["Admin", "Administrador", "Solicitante", "User"],
  },
  { id: "areas", label: "Áreas", roles: allRoles },
  { id: "entidades", label: "Entidades gubernamentales", roles: adminRoles },
  { id: "usuarios", label: "Usuarios", roles: adminRoles },
  { id: "roles", label: "Roles", roles: adminRoles },
  { id: "notificaciones", label: "Notificaciones", roles: allRoles },
];
const titles: Record<string, string> = {
  ...Object.fromEntries(navigation.map(({ id, label }) => [id, label])),
  detalle: "Detalle de solicitud",
  denegado: "Acceso denegado",
};
const statusNames = [
  "",
  "Registrada",
  "En análisis",
  "En progreso",
  "En espera del solicitante",
  "Resuelta",
  "Cerrada",
];
const priorityNames = ["", "Baja", "Media", "Alta", "Crítica"];
const errorText = (error: unknown) =>
  error instanceof ApiError ? error.message : "Ocurrió un error inesperado.";

function Login() {
  const { login, user } = useAuth();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  useEffect(() => {
    if (user) location.hash = "inicio";
  }, [user]);
  const submit = async (event: React.FormEvent) => {
    event.preventDefault();
    setBusy(true);
    setError("");
    try {
      await login(username.trim(), password);
    } catch (reason) {
      const apiError = reason as ApiError;
      setError(
        apiError.status === 401
          ? "El usuario o la contraseña no son válidos."
          : errorText(reason),
      );
      setBusy(false);
    }
  };
  return (
    <main className="login-page">
      <div className="login-wrap">
        <img
          src={logo}
          alt="Superintendencia de Bancos de la República Dominicana"
        />
        <form className="login-card" onSubmit={submit}>
          <p className="eyebrow">Plataforma de soporte</p>
          <h1>Iniciar sesión</h1>
          <p>Ingresa tus credenciales institucionales.</p>
          <label>
            <span>Usuario</span>
            <input
              autoComplete="username"
              required
              value={username}
              onChange={(event) => setUsername(event.target.value)}
            />
          </label>
          <label>
            <span>Contraseña</span>
            <input
              autoComplete="current-password"
              type="password"
              required
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </label>
          {error && (
            <p className="form-message" role="alert">
              {error}
            </p>
          )}
          <button className="button primary" disabled={busy}>
            {busy ? "Iniciando sesión…" : "Iniciar sesión"}
          </button>
        </form>
      </div>
    </main>
  );
}

function LoadState({ loading, error }: { loading: boolean; error: string }) {
  return loading ? (
    <p className="empty-block" role="status">
      Cargando información…
    </p>
  ) : error ? (
    <p className="form-message" role="alert">
      {error}
    </p>
  ) : null;
}

function DashboardPage() {
  const [data, setData] = useState<Dashboard>();
  const [error, setError] = useState("");
  useEffect(() => {
    dashboardApi
      .get()
      .then(setData)
      .catch((e) => setError(errorText(e)));
  }, []);
  if (!data) return <LoadState loading={!error} error={error} />;
  return (
    <>
      <div className="welcome">
        <div>
          <p className="eyebrow">Panel de seguimiento</p>
          <h2>Resumen de solicitudes</h2>
        </div>
        <span className="status-pill">Datos del servidor</span>
      </div>
      <div className="metrics">
        {[
          ["Solicitudes abiertas", data.abiertas],
          ["Solicitudes cerradas", data.cerradas],
          ["Solicitudes vencidas", data.vencidas],
        ].map(([label, value]) => (
          <article className="metric" key={label}>
            <span>{label}</span>
            <strong>{value}</strong>
          </article>
        ))}
      </div>
      <div className="dashboard-grid">
        <section className="subpanel">
          <h3>Solicitudes por estado</h3>
          {Object.entries(data.porEstado).map(([key, value]) => (
            <p className="summary-row" key={key}>
              <span>{statusNames[Number(key)] || key}</span>
              <strong>{value}</strong>
            </p>
          ))}
        </section>
        <section className="subpanel">
          <h3>Solicitudes por prioridad</h3>
          {Object.entries(data.porPrioridad).map(([key, value]) => (
            <p className="summary-row" key={key}>
              <span>{priorityNames[Number(key)] || key}</span>
              <strong>{value}</strong>
            </p>
          ))}
        </section>
      </div>
      <section className="subpanel latest">
        <h3>Últimas solicitudes</h3>
        {data.ultimas.length ? (
          data.ultimas.map((item) => (
            <button
              className="list-row"
              key={item.id}
              onClick={() => (location.hash = `detalle/${item.id}`)}>
              <span>
                <strong>{item.codigo}</strong> {item.titulo}
              </span>
              <span>{statusNames[item.estado]}</span>
            </button>
          ))
        ) : (
          <p className="empty-block">No hay solicitudes recientes.</p>
        )}
      </section>
    </>
  );
}

function AreaPage() {
  const { hasRole } = useAuth();
  const admin = hasRole(...adminRoles);
  const [items, setItems] = useState<Area[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState<Area>();
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const load = () => {
    setLoading(true);
    areasApi
      .list(false)
      .then(setItems)
      .catch((e) => setError(errorText(e)))
      .finally(() => setLoading(false));
  };
  useEffect(load, []);
  const edit = (item?: Area) => {
    setEditing(item);
    setName(item?.nombre ?? "");
    setDescription(item?.descripcion ?? "");
  };
  const save = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    try {
      editing
        ? await areasApi.update(editing.id, {
            nombre: name,
            descripcion: description,
            rowVersion: editing.rowVersion,
          })
        : await areasApi.create({ nombre: name, descripcion: description });
      edit();
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  const toggle = async (item: Area) => {
    try {
      await areasApi.status(item.id, !item.activo, item.rowVersion);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  const remove = async (item: Area) => {
    if (!confirm(`¿Eliminar el área “${item.nombre}”?`)) return;
    try {
      await areasApi.remove(item.id, item.rowVersion);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">Catálogo</p>
          <h2>Áreas</h2>
        </div>
        {admin && (
          <button className="button primary" onClick={() => edit()}>
            Nueva área
          </button>
        )}
      </div>
      <LoadState loading={loading} error={error} />
      {admin && (editing || name !== "") && (
        <form className="inline-form" onSubmit={save}>
          <label>
            <span>Nombre *</span>
            <input
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </label>
          <label>
            <span>Descripción</span>
            <input
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </label>
          <button className="button primary">Guardar</button>
          <button
            type="button"
            className="button secondary"
            onClick={() => edit()}>
            Cancelar
          </button>
        </form>
      )}
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id}>
                <td>{item.nombre}</td>
                <td>{item.descripcion || "—"}</td>
                <td>{item.activo ? "Activa" : "Inactiva"}</td>
                <td className="actions">
                  <button onClick={() => edit(item)}>Editar</button>
                  <button onClick={() => toggle(item)}>
                    {item.activo ? "Desactivar" : "Activar"}
                  </button>
                  <button onClick={() => remove(item)}>Eliminar</button>
                </td>
              </tr>
            ))}
            {!loading && !items.length && (
              <tr>
                <td className="empty" colSpan={4}>
                  No hay áreas registradas.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function EntityPage() {
  const [items, setItems] = useState<GovernmentEntity[]>([]);
  const [deleted, setDeleted] = useState(false);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");
  const [editing, setEditing] = useState<Partial<GovernmentEntity>>();
  const load = () =>
    (deleted
      ? entitiesApi.deleted({ search, pageNumber: 1, pageSize: 100 })
      : entitiesApi.list({ search, pageNumber: 1, pageSize: 100 })
    )
      .then((r) => setItems(r.items))
      .catch((e) => setError(errorText(e)));
  useEffect(() => {
    load();
  }, [deleted]);
  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editing) return;
    const body = {
      nombre: editing.nombre!,
      categoria: editing.categoria!,
      poderDelEstado: editing.poderDelEstado!,
      sector: editing.sector!,
    };
    try {
      editing.id
        ? await entitiesApi.update(editing.id, {
            ...body,
            rowVersion: editing.rowVersion!,
          })
        : await entitiesApi.create(body);
      setEditing(undefined);
      load();
    } catch (reason) {
      setError(errorText(reason));
    }
  };
  const act = async (
    item: GovernmentEntity,
    action: "status" | "remove" | "restore",
  ) => {
    try {
      if (action === "status")
        await entitiesApi.status(item.id, !item.activo, item.rowVersion);
      if (action === "remove")
        await entitiesApi.remove(item.id, item.rowVersion);
      if (action === "restore")
        await entitiesApi.restore(item.id, item.rowVersion);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">Mantenimiento</p>
          <h2>Entidades gubernamentales</h2>
        </div>
        <div className="actions">
          <button
            className="button secondary"
            onClick={() => setDeleted(!deleted)}>
            {deleted ? "Ver activas" : "Ver eliminadas"}
          </button>
          <button className="button primary" onClick={() => setEditing({})}>
            Nueva entidad
          </button>
        </div>
      </div>
      <div className="filter-row">
        <input
          aria-label="Buscar entidades"
          placeholder="Buscar"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <button className="button secondary" onClick={load}>
          Buscar
        </button>
      </div>
      {error && <p className="form-message">{error}</p>}
      {editing && (
        <form className="entity-form" onSubmit={save}>
          {(["nombre", "categoria", "poderDelEstado", "sector"] as const).map(
            (field) => (
              <label key={field}>
                <span>
                  {field === "poderDelEstado"
                    ? "Poder del Estado"
                    : field[0].toUpperCase() + field.slice(1)}{" "}
                  *
                </span>
                <input
                  required
                  value={editing[field] ?? ""}
                  onChange={(e) =>
                    setEditing({ ...editing, [field]: e.target.value })
                  }
                />
              </label>
            ),
          )}
          <div className="form-actions">
            <button
              type="button"
              className="button secondary"
              onClick={() => setEditing(undefined)}>
              Cancelar
            </button>
            <button className="button primary">Guardar</button>
          </div>
        </form>
      )}
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Categoría</th>
              <th>Sector</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id}>
                <td>{item.nombre}</td>
                <td>{item.categoria}</td>
                <td>{item.sector}</td>
                <td>
                  {item.activo
                    ? "Activa"
                    : item.isDeleted
                      ? "Eliminada"
                      : "Inactiva"}
                </td>
                <td className="actions">
                  {deleted ? (
                    <button onClick={() => act(item, "restore")}>
                      Restaurar
                    </button>
                  ) : (
                    <>
                      <button
                        onClick={async () =>
                          setEditing(await entitiesApi.get(item.id))
                        }>
                        Editar
                      </button>
                      <button onClick={() => act(item, "status")}>
                        {item.activo ? "Desactivar" : "Activar"}
                      </button>
                      <button onClick={() => act(item, "remove")}>
                        Eliminar
                      </button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [error, setError] = useState("");
  const load = () =>
    Promise.all([usersApi.list(), rolesApi.list()])
      .then(([u, r]) => {
        setUsers(u);
        setRoles(r);
      })
      .catch((e) => setError(errorText(e)));
  useEffect(() => {
    load();
  }, []);
  const toggle = async (user: User, role: Role) => {
    try {
      user.roles.includes(role.name)
        ? await usersApi.removeRole(user.id, role.id)
        : await usersApi.assignRole(user.id, role.id);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  return (
    <section>
      <p className="eyebrow">Seguridad</p>
      <h2>Usuarios</h2>
      {error && <p className="form-message">{error}</p>}
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Usuario</th>
              <th>Correo</th>
              <th>Estado</th>
              <th>Roles</th>
            </tr>
          </thead>
          <tbody>
            {users.map((user) => (
              <tr key={user.id}>
                <td>
                  <button
                    className="link-button"
                    onClick={() => usersApi.get(user.id).then(() => undefined)}>
                    {user.username}
                  </button>
                </td>
                <td>{user.email}</td>
                <td>{user.isActive ? "Activo" : "Inactivo"}</td>
                <td className="role-list">
                  {roles.map((role) => (
                    <label key={role.id}>
                      <input
                        type="checkbox"
                        checked={user.roles.includes(role.name)}
                        onChange={() => toggle(user, role)}
                      />{" "}
                      {role.name}
                    </label>
                  ))}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [error, setError] = useState("");
  const [editing, setEditing] = useState<Partial<Role>>();
  const load = () =>
    Promise.all([rolesApi.list(), permissionsApi.list()])
      .then(([r, p]) => {
        setRoles(r);
        setPermissions(p);
      })
      .catch((e) => setError(errorText(e)));
  useEffect(() => {
    load();
  }, []);
  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editing?.name) return;
    try {
      editing.id
        ? await rolesApi.update(editing.id, {
            name: editing.name,
            description: editing.description,
          })
        : await rolesApi.create({
            name: editing.name,
            description: editing.description,
          });
      setEditing(undefined);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  const permission = async (role: Role, p: Permission) => {
    try {
      role.permissions.includes(p.name)
        ? await rolesApi.removePermission(role.id, p.id)
        : await rolesApi.assignPermission(role.id, p.id);
      load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">Seguridad</p>
          <h2>Roles</h2>
        </div>
        <button className="button primary" onClick={() => setEditing({})}>
          Nuevo rol
        </button>
      </div>
      {error && <p className="form-message">{error}</p>}
      {editing && (
        <form className="inline-form" onSubmit={save}>
          <label>
            <span>Nombre *</span>
            <input
              required
              value={editing.name ?? ""}
              onChange={(e) => setEditing({ ...editing, name: e.target.value })}
            />
          </label>
          <label>
            <span>Descripción</span>
            <input
              value={editing.description ?? ""}
              onChange={(e) =>
                setEditing({ ...editing, description: e.target.value })
              }
            />
          </label>
          <button className="button primary">Guardar</button>
          <button
            type="button"
            className="button secondary"
            onClick={() => setEditing(undefined)}>
            Cancelar
          </button>
        </form>
      )}
      <div className="cards-list">
        {roles.map((role) => (
          <article className="subpanel" key={role.id}>
            <div className="card-heading">
              <div>
                <h3>{role.name}</h3>
                <p>{role.description || "Sin descripción"}</p>
              </div>
              <div className="actions">
                <button onClick={() => setEditing(role)}>Editar</button>
                <button
                  onClick={async () => {
                    await rolesApi.remove(role.id);
                    load();
                  }}>
                  Eliminar
                </button>
              </div>
            </div>
            <details>
              <summary>Permisos ({role.permissions.length})</summary>
              <div className="permission-grid">
                {permissions.map((p) => (
                  <label key={p.id}>
                    <input
                      type="checkbox"
                      checked={role.permissions.includes(p.name)}
                      onChange={() => permission(role, p)}
                    />
                    {p.name}
                  </label>
                ))}
              </div>
            </details>
          </article>
        ))}
      </div>
    </section>
  );
}

function RequestList() {
  const { user, hasRole } = useAuth();
  const [items, setItems] = useState<RequestItem[]>([]);
  const [search, setSearch] = useState("");
  const [estado, setEstado] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const load = () => {
    setLoading(true);
    const params: Record<string, string | number | undefined> = {
      search,
      estado: estado ? Number(estado) : undefined,
      pageNumber: 1,
      pageSize: 100,
    };
    if (hasRole("Solicitante", "User") && !hasRole(...adminRoles))
      params.solicitanteId = user?.id;
    if (hasRole("Analista")) params.responsableId = user?.id;
    requestsApi
      .list(params)
      .then((r) => setItems(r.items))
      .catch((e) => setError(errorText(e)))
      .finally(() => setLoading(false));
  };
  useEffect(load, []);
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">Seguimiento</p>
          <h2>Solicitudes</h2>
        </div>
      </div>
      <div className="filter-row">
        <input
          aria-label="Buscar solicitudes"
          placeholder="Código o título"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select
          aria-label="Filtrar por estado"
          value={estado}
          onChange={(e) => setEstado(e.target.value)}>
          <option value="">Todos los estados</option>
          {statusNames.slice(1).map((name, index) => (
            <option value={index + 1} key={name}>
              {name}
            </option>
          ))}
        </select>
        <button className="button secondary" onClick={load}>
          Filtrar
        </button>
      </div>
      <LoadState loading={loading} error={error} />
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Código</th>
              <th>Título</th>
              <th>Área</th>
              <th>Prioridad</th>
              <th>Estado</th>
              <th>Responsable</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id}>
                <td>
                  <button
                    className="link-button"
                    onClick={() => (location.hash = `detalle/${item.id}`)}>
                    {item.codigo}
                  </button>
                </td>
                <td>{item.titulo}</td>
                <td>{item.area}</td>
                <td>{priorityNames[item.prioridad]}</td>
                <td>{statusNames[item.estado]}</td>
                <td>{item.responsable || "Sin asignar"}</td>
              </tr>
            ))}
            {!loading && !items.length && (
              <tr>
                <td className="empty" colSpan={6}>
                  No se encontraron solicitudes.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function RequestForm() {
  const [areas, setAreas] = useState<Area[]>([]);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  useEffect(() => {
    areasApi
      .list(true)
      .then(setAreas)
      .catch((e) => setError(errorText(e)));
  }, []);
  const submit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setBusy(true);
    setError("");
    const form = new FormData(e.currentTarget);
    try {
      const created = await requestsApi.create({
        titulo: String(form.get("titulo")),
        descripcion: String(form.get("descripcion")),
        tipo: Number(form.get("tipo")),
        prioridad: Number(form.get("prioridad")),
        areaId: String(form.get("areaId")),
        referenciaEvidencia:
          String(form.get("referenciaEvidencia")) || undefined,
        fechaCompromiso: String(form.get("fechaCompromiso")) || undefined,
      });
      location.hash = `detalle/${created.id}`;
    } catch (reason) {
      setError(errorText(reason));
      setBusy(false);
    }
  };
  return (
    <form className="request-form" onSubmit={submit}>
      <div className="form-intro">
        <p className="eyebrow">Nueva solicitud</p>
        <h2>Información de la solicitud</h2>
        <p>Completa los campos obligatorios para registrar el requerimiento.</p>
      </div>
      {error && <p className="form-message">{error}</p>}
      <div className="form-grid">
        <label>
          <span>Título *</span>
          <input required maxLength={200} name="titulo" />
        </label>
        <label>
          <span>Área *</span>
          <select required name="areaId" defaultValue="">
            <option value="" disabled>
              Selecciona un área
            </option>
            {areas.map((area) => (
              <option value={area.id} key={area.id}>
                {area.nombre}
              </option>
            ))}
          </select>
        </label>
        <label>
          <span>Tipo *</span>
          <select name="tipo" defaultValue="1">
            <option value="1">Soporte</option>
            <option value="2">Requerimiento</option>
          </select>
        </label>
        <label>
          <span>Prioridad *</span>
          <select name="prioridad" defaultValue="2">
            {priorityNames.slice(1).map((name, index) => (
              <option value={index + 1} key={name}>
                {name}
              </option>
            ))}
          </select>
        </label>
        <label>
          <span>Fecha compromiso</span>
          <input type="datetime-local" name="fechaCompromiso" />
        </label>
        <label>
          <span>Evidencia textual o URL</span>
          <input maxLength={1000} name="referenciaEvidencia" />
        </label>
        <label className="full">
          <span>Descripción *</span>
          <textarea required maxLength={4000} rows={6} name="descripcion" />
        </label>
      </div>
      <div className="form-actions">
        <button
          type="button"
          className="button secondary"
          onClick={() => (location.hash = "solicitudes")}>
          Cancelar
        </button>
        <button className="button primary" disabled={busy}>
          {busy ? "Guardando…" : "Guardar solicitud"}
        </button>
      </div>
    </form>
  );
}

function RequestDetail({ id }: { id: string }) {
  const { hasRole } = useAuth();
  const staff = hasRole("Admin", "Administrador", "Analista");
  const admin = hasRole(...adminRoles);
  const [item, setItem] = useState<RequestItem>();
  const [comments, setComments] = useState<
    Awaited<ReturnType<typeof requestsApi.comments>>
  >([]);
  const [history, setHistory] = useState<
    Awaited<ReturnType<typeof requestsApi.history>>
  >([]);
  const [analysts, setAnalysts] = useState<User[]>([]);
  const [error, setError] = useState("");
  const load = () =>
    Promise.all([
      requestsApi.get(id),
      requestsApi.comments(id),
      requestsApi.history(id),
      staff ? usersApi.analysts().then((r) => r.items) : Promise.resolve([]),
    ])
      .then(([r, c, h, a]) => {
        setItem(r);
        setComments(c);
        setHistory(h);
        setAnalysts(a);
      })
      .catch((e) => setError(errorText(e)));
  useEffect(() => {
    load();
  }, [id]);
  if (!item) return <LoadState loading={!error} error={error} />;
  const refresh = async (action: Promise<unknown>) => {
    try {
      await action;
      await load();
    } catch (e) {
      setError(errorText(e));
    }
  };
  const edit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const f = new FormData(e.currentTarget);
    await refresh(
      requestsApi.patch(id, {
        titulo: String(f.get("titulo")),
        descripcion: String(f.get("descripcion")),
        rowVersion: item.rowVersion,
      }),
    );
  };
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">{item.codigo}</p>
          <h2>{item.titulo}</h2>
        </div>
        <span className="status-pill">{statusNames[item.estado]}</span>
      </div>
      {error && <p className="form-message">{error}</p>}
      <div className="detail-grid">
        <section className="subpanel">
          <h3>Información</h3>
          <form onSubmit={edit} className="stack">
            <label>
              <span>Título</span>
              <input name="titulo" defaultValue={item.titulo} />
            </label>
            <label>
              <span>Descripción</span>
              <textarea name="descripcion" defaultValue={item.descripcion} />
            </label>
            <button className="button secondary">Guardar cambios</button>
          </form>
          <dl>
            <dt>Área</dt>
            <dd>{item.area}</dd>
            <dt>Prioridad</dt>
            <dd>{priorityNames[item.prioridad]}</dd>
            <dt>Responsable</dt>
            <dd>{item.responsable || "Sin asignar"}</dd>
            <dt>Fecha compromiso</dt>
            <dd>
              {item.fechaCompromiso
                ? new Date(item.fechaCompromiso).toLocaleString()
                : "Sin definir"}
            </dd>
            <dt>Evidencia</dt>
            <dd>{item.referenciaEvidencia || "Sin evidencia"}</dd>
          </dl>
          {admin && (
            <button
              className="button secondary danger"
              onClick={() => refresh(requestsApi.remove(id, item.rowVersion))}>
              Eliminar solicitud
            </button>
          )}
        </section>
        {staff && (
          <section className="subpanel">
            <h3>Gestión</h3>
            <label>
              <span>Responsable</span>
              <select
                defaultValue={item.responsableId || ""}
                onChange={(e) =>
                  refresh(
                    requestsApi.assign(id, e.target.value, item.rowVersion),
                  )
                }>
                <option value="" disabled>
                  Seleccionar analista
                </option>
                {analysts.map((a) => (
                  <option value={a.id} key={a.id}>
                    {a.username}
                  </option>
                ))}
              </select>
            </label>
            <label>
              <span>Prioridad</span>
              <select
                value={item.prioridad}
                onChange={(e) =>
                  refresh(
                    requestsApi.priority(
                      id,
                      Number(e.target.value),
                      item.rowVersion,
                    ),
                  )
                }>
                {priorityNames.slice(1).map((n, i) => (
                  <option value={i + 1} key={n}>
                    {n}
                  </option>
                ))}
              </select>
            </label>
            <form
              className="stack"
              onSubmit={(e) => {
                e.preventDefault();
                const f = new FormData(e.currentTarget);
                refresh(
                  requestsApi.status(
                    id,
                    Number(f.get("estado")),
                    String(f.get("comentario")),
                    item.rowVersion,
                    String(f.get("resolucion")) || undefined,
                  ),
                );
              }}>
              <label>
                <span>Nuevo estado</span>
                <select name="estado">
                  {statusNames.slice(1).map((n, i) => (
                    <option value={i + 1} key={n}>
                      {n}
                    </option>
                  ))}
                </select>
              </label>
              <label>
                <span>Comentario</span>
                <textarea required name="comentario" />
              </label>
              <label>
                <span>Resolución (requerida al resolver)</span>
                <textarea name="resolucion" />
              </label>
              <button className="button primary">Cambiar estado</button>
            </form>
            {item.estado === 6 && (
              <button
                className="button secondary"
                onClick={() => {
                  const comment = prompt("Motivo de reapertura");
                  if (comment)
                    refresh(requestsApi.reopen(id, comment, item.rowVersion));
                }}>
                Reabrir
              </button>
            )}
          </section>
        )}
      </div>
      <div className="detail-grid">
        <section className="subpanel">
          <h3>Comentarios</h3>
          <form
            className="stack"
            onSubmit={(e) => {
              e.preventDefault();
              const f = new FormData(e.currentTarget);
              refresh(
                requestsApi.addComment(
                  id,
                  String(f.get("contenido")),
                  Boolean(f.get("interno")),
                ),
              );
              e.currentTarget.reset();
            }}>
            <textarea
              required
              name="contenido"
              placeholder="Agregar comentario"
            />
            {staff && (
              <label className="check">
                <input type="checkbox" name="interno" /> Comentario interno
              </label>
            )}
            <button className="button primary">Publicar comentario</button>
          </form>
          {comments.map((c) => (
            <article className="comment" key={c.id}>
              <p>{c.contenido}</p>
              <small>
                {c.esInterno ? "Interno · " : ""}
                {new Date(c.createdAt).toLocaleString()}
              </small>
              <div className="actions">
                <button
                  onClick={() => {
                    const value = prompt("Editar comentario", c.contenido);
                    if (value)
                      refresh(
                        requestsApi.updateComment(
                          id,
                          c.id,
                          value,
                          c.rowVersion,
                        ),
                      );
                  }}>
                  Editar
                </button>
                <button
                  onClick={() =>
                    refresh(requestsApi.removeComment(id, c.id, c.rowVersion))
                  }>
                  Eliminar
                </button>
              </div>
            </article>
          ))}
        </section>
        <section className="subpanel">
          <h3>Historial</h3>
          {history.map((h) => (
            <article className="comment" key={h.id}>
              <strong>
                {statusNames[h.estadoAnterior]} → {statusNames[h.estadoNuevo]}
              </strong>
              <p>{h.comentario}</p>
              <small>{new Date(h.createdAt).toLocaleString()}</small>
            </article>
          ))}
        </section>
      </div>
    </section>
  );
}

function NotificationsPage() {
  const [items, setItems] = useState<NotificationItem[]>([]);
  const [count, setCount] = useState(0);
  const [error, setError] = useState("");
  const load = () =>
    Promise.all([notificationsApi.list(), notificationsApi.count()])
      .then(([n, c]) => {
        setItems(n);
        setCount(c.count);
      })
      .catch((e) => setError(errorText(e)));
  useEffect(() => {
    load();
  }, []);
  return (
    <section>
      <div className="toolbar">
        <div>
          <p className="eyebrow">Centro de avisos</p>
          <h2>Notificaciones ({count} sin leer)</h2>
        </div>
        <button
          className="button secondary"
          onClick={async () => {
            await notificationsApi.readAll();
            load();
          }}>
          Marcar todas como leídas
        </button>
      </div>
      {error && <p className="form-message">{error}</p>}
      <div className="cards-list">
        {items.map((n) => (
          <article
            className={`notification ${n.leida ? "read" : ""}`}
            key={n.id}>
            <div>
              <h3>{n.titulo}</h3>
              <p>{n.mensaje}</p>
              <small>{new Date(n.createdAt).toLocaleString()}</small>
            </div>
            {!n.leida && (
              <button
                className="button secondary"
                onClick={async () => {
                  await notificationsApi.read(n.id);
                  load();
                }}>
                Marcar como leída
              </button>
            )}
          </article>
        ))}
        {!items.length && (
          <p className="empty-block">No tienes notificaciones.</p>
        )}
      </div>
    </section>
  );
}

function Page({ route }: { route: string }) {
  if (route === "inicio") return <DashboardPage />;
  if (route === "solicitudes") return <RequestList />;
  if (route === "crear") return <RequestForm />;
  if (route === "areas") return <AreaPage />;
  if (route === "entidades") return <EntityPage />;
  if (route === "usuarios") return <UsersPage />;
  if (route === "roles") return <RolesPage />;
  if (route === "notificaciones") return <NotificationsPage />;
  if (route.startsWith("detalle/"))
    return <RequestDetail id={route.split("/")[1]} />;
  return (
    <section className="empty-state">
      <h2>Acceso denegado</h2>
      <p>No tienes autorización para abrir esta página.</p>
    </section>
  );
}

function App() {
  const { user, loading, logout } = useAuth();
  const [route, setRoute] = useState(() => location.hash.slice(1) || "inicio");
  const [menuOpen, setMenuOpen] = useState(false);
  useEffect(() => {
    const fn = () => setRoute(location.hash.slice(1) || "inicio");
    addEventListener("hashchange", fn);
    return () => removeEventListener("hashchange", fn);
  }, []);
  const items = useMemo(
    () =>
      navigation.filter((item) =>
        item.roles.some((role) => user?.roles.includes(role)),
      ),
    [user],
  );
  if (loading) return <div className="boot">Cargando sesión…</div>;
  if (!user) return <Login />;
  if (route === "login") {
    location.hash = "inicio";
    return null;
  }
  const base = route.split("/")[0];
  const authorized =
    base === "detalle" || items.some((item) => item.id === base);
  return (
    <div className="app-shell">
      <button
        className="menu-toggle"
        aria-label={menuOpen ? "Cerrar menú" : "Abrir menú"}
        onClick={() => setMenuOpen(!menuOpen)}>
        <span />
        <span />
        <span />
      </button>
      {menuOpen && (
        <button
          className="backdrop"
          aria-label="Cerrar menú"
          onClick={() => setMenuOpen(false)}
        />
      )}
      <aside className={`sidebar ${menuOpen ? "open" : ""}`}>
        <div className="brand">
          <img
            src={logo}
            alt="Superintendencia de Bancos de la República Dominicana"
          />
        </div>
        <nav aria-label="Navegación principal">
          {items.map((item) => (
            <a
              key={item.id}
              href={`#${item.id}`}
              className={base === item.id ? "active" : ""}
              aria-current={base === item.id ? "page" : undefined}
              onClick={() => setMenuOpen(false)}>
              {item.icon ? (
                <img src={item.icon} alt="" />
              ) : (
                <span className="nav-marker" />
              )}
              <span>{item.label}</span>
            </a>
          ))}
        </nav>
        <div className="profile">
          <strong>{user.username}</strong>
          <small>{user.roles.join(", ")}</small>
          <button onClick={logout}>Cerrar sesión</button>
        </div>
      </aside>
      <div className="main-column">
        <header className="topbar">
          <h1>
            {authorized
              ? titles[base] || "Detalle de solicitud"
              : "Acceso denegado"}
          </h1>
          <div className="user-chip">
            <span>SB</span>
            <div>
              <strong>{user.username}</strong>
              <small>{user.roles.join(", ")}</small>
            </div>
          </div>
        </header>
        <main>
          <div className="content-panel">
            {authorized ? <Page route={route} /> : <Page route="denegado" />}
          </div>
        </main>
      </div>
    </div>
  );
}
export default App;
