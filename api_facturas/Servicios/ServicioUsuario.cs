// ============================================================
// ServicioUsuario — la capa de NEGOCIO de usuario (v3).
//
// No sabe de BCrypt (eso es del repositorio) ni de HTTP (eso es
// del controller): valida argumentos y traduce el resultado de la
// verificación al trío de negocio 200/401/404.
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioUsuario : IServicioUsuario
{
    private readonly IRepositorioUsuario _repositorio;

    public ServicioUsuario(IRepositorioUsuario repositorio)
    {
        _repositorio = repositorio;
    }

    private static string ValidarEmail(string email)
    {
        email = email.Trim();
        if (email == "")
        {
            throw new ArgumentException("El email no puede estar vacío.");
        }
        return email;
    }

    public async Task<List<Usuario>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodosAsync(limite);
    }

    public async Task<Usuario> ObtenerAsync(string email)
    {
        email = ValidarEmail(email);
        var usuario = await _repositorio.ObtenerPorEmailAsync(email);
        if (usuario == null)
        {
            throw new NoEncontradoExcepcion($"No existe un usuario con email = {email}");
        }
        return usuario;
    }

    public async Task CrearAsync(string email, string contrasena)
    {
        // La petición ya validó formas (email 1-100, contrasena 6-200);
        // el hash lo pone el repositorio. Duplicado → PK → 500.
        await _repositorio.CrearAsync(email, contrasena);
    }

    public async Task<int> ActualizarContrasenaAsync(string email, string? contrasena)
    {
        email = ValidarEmail(email);
        // El PATCH con body {} llega con contrasena null: regla de negocio.
        if (string.IsNullOrEmpty(contrasena))
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        var filas = await _repositorio.ActualizarContrasenaAsync(email, contrasena);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un usuario con email = {email}");
        }
        return filas;
    }

    public async Task<int> EliminarAsync(string email)
    {
        email = ValidarEmail(email);
        var filas = await _repositorio.EliminarAsync(email);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un usuario con email = {email}");
        }
        return filas;
    }

    public async Task<(int Codigo, string Mensaje)> VerificarContrasenaAsync(string email, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(contrasena))
        {
            throw new ArgumentException("El usuario y la contraseña son obligatorios.");
        }
        var resultado = await _repositorio.VerificarContrasenaAsync(email.Trim(), contrasena);
        return resultado switch
        {
            null => (404, "Usuario no encontrado."),
            true => (200, "Contraseña válida."),
            false => (401, "Contraseña incorrecta."),
        };
    }
}
