// RolUsuario — el MODELO de una tabla PUENTE (v3): una pareja de llaves
// foráneas (usuario.email y rol.id) con PK COMPUESTA. Representa qué ROLES tiene cada USUARIO.
// No tiene más campos: la asignación existe o no existe.

namespace ApiFacturas.Modelos;

public class RolUsuario
{
    public required string Fkemail { get; set; }

    public int Fkidrol { get; set; }
}
