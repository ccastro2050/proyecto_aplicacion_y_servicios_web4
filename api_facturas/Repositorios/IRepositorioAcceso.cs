namespace ApiFacturas.Repositorios;

// ============================================================
// IRepositorioAcceso — las DOS preguntas del control de acceso.
//
// Es un repositorio nuevo y no un método más en el de usuario, porque la
// pregunta «¿puede este usuario entrar aquí?» no es del CRUD de usuario: es
// del acceso. Cruza tres tablas y la responde un procedimiento.
// ============================================================
public interface IRepositorioAcceso
{
    /// <summary>¿Este correo tiene acceso a esta ruta?
    ///
    /// Lo responde `verificar_acceso_ruta`, que YA EXISTE en la base y cruza
    /// usuario → rol_usuario → rutarol. La API no arma ese JOIN: repetirlo en
    /// C# dejaría la regla en dos sitios.</summary>
    Task<bool> TieneAccesoAsync(string email, string nombreRuta);

    /// <summary>Las rutas a las que este correo SÍ puede entrar.
    ///
    /// Solo sirve para que la interfaz arme su menú. NO es el control de
    /// acceso: esconder una entrada del menú no protege nada, y quien escriba
    /// la dirección a mano entra igual si el servicio no comprueba.</summary>
    Task<List<string>> RutasPermitidasAsync(string email);
}
