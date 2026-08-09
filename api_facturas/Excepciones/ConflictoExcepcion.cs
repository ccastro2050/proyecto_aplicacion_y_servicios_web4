// ============================================================
// ConflictoExcepcion — la excepción de negocio "conflicto de estado".
//
// NUEVA en la v2. La lanza el repositorio de factura cuando el SP
// reporta que la factura YA ESTÁ ANULADA, y el controlador la
// traduce a 409 Conflict.
//
// ¿Por qué 409 y no 400 ni 404? La petición está bien formada y la
// factura existe — el problema es un CONFLICTO con el estado actual
// del recurso (no se puede anular dos veces). Esa es exactamente la
// semántica de 409. Ver la decisión D6 en 4_research.md de la v2.
// ============================================================

namespace ApiFacturas.Excepciones;

public class ConflictoExcepcion : Exception
{
    public ConflictoExcepcion(string mensaje) : base(mensaje)
    {
    }
}
