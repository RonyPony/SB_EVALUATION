import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  areasApi,
  authApi,
  configureApi,
  dashboardApi,
  entitiesApi,
  notificationsApi,
  permissionsApi,
  requestsApi,
  rolesApi,
  usersApi,
} from "./api";

const calls: Array<{ url: string; init: RequestInit }> = [];
beforeEach(() => {
  calls.length = 0;
  configureApi(
    () => "token-real",
    () => undefined,
  );
  vi.stubGlobal(
    "fetch",
    vi.fn(async (url: string, init: RequestInit = {}) => {
      calls.push({ url, init });
      const body = url.includes("/login")
        ? {
            accessToken: "x",
            tokenType: "Bearer",
            expiresAt: new Date(Date.now() + 60000).toISOString(),
          }
        : url.includes("/count")
          ? { count: 0 }
          : url.includes("/dashboard")
            ? {
                abiertas: 0,
                cerradas: 0,
                vencidas: 0,
                porEstado: {},
                porPrioridad: {},
                ultimas: [],
              }
            : url.includes("pageNumber") ||
                url.includes("entidades-gubernamentales")
              ? {
                  items: [],
                  pageNumber: 1,
                  pageSize: 20,
                  totalRecords: 0,
                  totalPages: 0,
                }
              : [];
      return new Response(JSON.stringify(body), {
        status: 200,
        headers: { "Content-Type": "application/json" },
      });
    }),
  );
});
afterEach(() => vi.unstubAllGlobals());

describe("contratos HTTP", () => {
  it("envía login exacto sin Bearer y consulta perfil con Bearer", async () => {
    await authApi.login({ username: "usuario", password: "clave" });
    await authApi.me();
    expect(calls[0].url).toBe("http://api.test/api/auth/login");
    expect(calls[0].init.method).toBe("POST");
    expect(calls[0].init.body).toBe(
      '{"username":"usuario","password":"clave"}',
    );
    expect(new Headers(calls[1].init.headers).get("Authorization")).toBe(
      "Bearer token-real",
    );
  });
  it("cubre dashboard, áreas, entidades, seguridad, solicitudes y notificaciones", async () => {
    await dashboardApi.get();
    await areasApi.list(false);
    await areasApi.get("a");
    await areasApi.create({ nombre: "A" });
    await areasApi.update("a", { nombre: "A", rowVersion: "v" });
    await areasApi.status("a", false, "v");
    await areasApi.remove("a", "v");
    await entitiesApi.list({ pageNumber: 1 });
    await entitiesApi.deleted();
    await entitiesApi.get("e");
    await entitiesApi.create({
      nombre: "N",
      categoria: "C",
      poderDelEstado: "P",
      sector: "S",
    });
    await entitiesApi.update("e", {
      nombre: "N",
      categoria: "C",
      poderDelEstado: "P",
      sector: "S",
      rowVersion: "v",
    });
    await entitiesApi.status("e", false, "v");
    await entitiesApi.remove("e", "v");
    await entitiesApi.restore("e", "v");
    await usersApi.list();
    await usersApi.get("u");
    await usersApi.analysts();
    await usersApi.assignRole("u", "r");
    await usersApi.removeRole("u", "r");
    await rolesApi.list();
    await rolesApi.create({ name: "R" });
    await rolesApi.update("r", { name: "R" });
    await rolesApi.remove("r");
    await rolesApi.assignPermission("r", "p");
    await rolesApi.removePermission("r", "p");
    await permissionsApi.list();
    await requestsApi.list();
    await requestsApi.get("s");
    await requestsApi.create({
      titulo: "T",
      descripcion: "D",
      tipo: 1,
      prioridad: 2,
      areaId: "a",
    });
    await requestsApi.update("s", {
      titulo: "T",
      descripcion: "D",
      tipo: 1,
      areaId: "a",
      rowVersion: "v",
    });
    await requestsApi.patch("s", { titulo: "T", rowVersion: "v" });
    await requestsApi.priority("s", 3, "v");
    await requestsApi.assign("s", "u", "v");
    await requestsApi.status("s", 2, "C", "v");
    await requestsApi.reopen("s", "C", "v");
    await requestsApi.remove("s", "v");
    await requestsApi.history("s");
    await requestsApi.comments("s");
    await requestsApi.addComment("s", "C", false);
    await requestsApi.updateComment("s", "c", "C", "v");
    await requestsApi.removeComment("s", "c", "v");
    await notificationsApi.list();
    await notificationsApi.count();
    await notificationsApi.read("n");
    await notificationsApi.readAll();
    expect(calls).toHaveLength(46);
    expect(
      calls.every((call) => call.url.startsWith("http://api.test/api/")),
    ).toBe(true);
    expect(
      calls
        .slice(1)
        .every(
          (call) =>
            new Headers(call.init.headers).get("Authorization") ===
            "Bearer token-real",
        ),
    ).toBe(true);
  });
});
