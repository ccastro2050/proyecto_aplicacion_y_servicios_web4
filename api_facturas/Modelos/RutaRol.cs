// RutaRol — el MODELO de una tabla PUENTE (v3): una pareja de llaves
// foráneas (ruta.id y rol.id) con PK COMPUESTA. Representa qué ROLES tienen acceso a cada RUTA.
// No tiene más campos: la asignación existe o no existe.

namespace ApiFacturas.Modelos;

public class RutaRol
{
    public int Fkidruta { get; set; }

    public int Fkidrol { get; set; }
}
