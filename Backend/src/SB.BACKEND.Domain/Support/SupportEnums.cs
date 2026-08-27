namespace SB.BACKEND.Domain.Support;

public enum TipoSolicitud
{
    Soporte = 1,
    Requerimiento = 2,
}

public enum PrioridadSolicitud
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4,
}

public enum EstadoSolicitud
{
    Registrada = 1,
    EnAnalisis = 2,
    EnProgreso = 3,
    EnEsperaSolicitante = 4,
    Resuelta = 5,
    Cerrada = 6,
}

public enum TipoNotificacion
{
    Creacion = 1,
    Asignacion = 2,
    Reasignacion = 3,
    CambioEstado = 4,
    Resolucion = 5,
    Cierre = 6,
    Reapertura = 7,
    Comentario = 8,
}
