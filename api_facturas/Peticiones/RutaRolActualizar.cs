// ============================================================
// RutaRolActualizar — la PETICIÓN del PATCH del puente rutarol.
//
// Ningún campo es obligatorio: PATCH manda SOLO el lado que se
// mueve. `{"fkidrol": 3}` le pasa esa ruta a otro rol;
// `{"fkidruta": 7}` le cambia la ruta al mismo rol.
//
// El body vacío `{}` pasa la validación de forma y lo rechaza el
// servicio con un 400: «no mandó nada que cambiar» no es un error
// de FORMA.
// ============================================================

namespace ApiFacturas.Peticiones;

public class RutaRolActualizar
{
    public int? Fkidruta { get; set; }

    public int? Fkidrol { get; set; }
}
