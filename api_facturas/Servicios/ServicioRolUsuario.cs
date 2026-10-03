// ServicioRolUsuario — la capa de NEGOCIO del puente rol_usuario (v3).
// Las búsquedas sin resultados son 404 (el recurso preguntado no
// tiene asignaciones) — lo decide el negocio, no el repositorio.

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioRolUsuario : IServicioRolUsuario
{
    private readonly IRepositorioRolUsuario _repositorio;

    public ServicioRolUsuario(IRepositorioRolUsuario repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<RolUsuario>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodosAsync(limite);
    }

    public async Task<List<RolUsuario>> ObtenerPorUsuarioAsync(string fkemail)
    {
        fkemail = fkemail.Trim();
        if (fkemail == "")
        {
            throw new ArgumentException("El usuario no puede estar vacío.");
        }
        var lista = await _repositorio.ObtenerPorUsuarioAsync(fkemail);
        if (lista.Count == 0)
        {
            throw new NoEncontradoExcepcion($"No hay asignaciones en rol_usuario para usuario = {fkemail}");
        }
        return lista;
    }

    public async Task<List<RolUsuario>> ObtenerPorRolAsync(int fkidrol)
    {
        if (fkidrol <= 0)
        {
            throw new ArgumentException("El id de rol debe ser un entero mayor que cero.");
        }
        var lista = await _repositorio.ObtenerPorRolAsync(fkidrol);
        if (lista.Count == 0)
        {
            throw new NoEncontradoExcepcion($"No hay asignaciones en rol_usuario para rol = {fkidrol}");
        }
        return lista;
    }

    public async Task CrearAsync(RolUsuario asignacion)
    {
        await _repositorio.CrearAsync(asignacion);
    }

    public async Task<int> EliminarAsync(string fkemail, int fkidrol)
    {
        fkemail = fkemail.Trim();
        if (fkemail == "")
        {
            throw new ArgumentException("El usuario no puede estar vacío.");
        }
        var filas = await _repositorio.EliminarAsync(fkemail, fkidrol);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe la pareja ({fkemail}, {fkidrol}) en rol_usuario");
        }
        return filas;
    }

    public async Task<int> ReemplazarAsync(string fkemail, int fkidrol, RolUsuario nueva)
    {
        // El PUT trae la pareja ENTERA: no hay nada que conservar.
        return await MoverAsync(fkemail, fkidrol, nueva.Fkemail, nueva.Fkidrol);
    }

    public async Task<int> ActualizarAsync(string fkemail, int fkidrol,
                                           string? nuevoEmail, int? nuevoRol)
    {
        // EL PATCH DE UNA TABLA PUENTE: llega UNO de los dos lados y el otro
        // se conserva. Mover un rol de una persona a otra es cambiar el
        // email; cambiarle el rol a la misma persona es cambiar el idrol.
        if (nuevoEmail == null && nuevoRol == null)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        return await MoverAsync(fkemail, fkidrol,
                                nuevoEmail ?? fkemail, nuevoRol ?? fkidrol);
    }

    /// <summary>Lo comun al PUT y al PATCH: validar y mover.</summary>
    private async Task<int> MoverAsync(string emailViejo, int rolViejo,
                                       string emailNuevo, int rolNuevo)
    {
        emailViejo = emailViejo.Trim();
        emailNuevo = emailNuevo.Trim();
        if (emailViejo == "" || emailNuevo == "")
        {
            throw new ArgumentException("El usuario no puede estar vacío.");
        }

        var filas = await _repositorio.ReemplazarAsync(emailViejo, rolViejo,
                                                       emailNuevo, rolNuevo);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la pareja ({emailViejo}, {rolViejo}) en rol_usuario");
        }
        return filas;
    }
}
