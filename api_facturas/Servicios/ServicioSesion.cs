// ============================================================
// ServicioSesion — verifica las credenciales y arma el token.
//
// Dos decisiones viven aquí, y las dos son del dominio:
//
//   1. UN SOLO ERROR PARA LOS DOS CASOS. Si el correo no existe y si la
//      contraseña está mal, la respuesta es la misma. Decir «ese correo no
//      existe» le confirma a un desconocido CUÁLES SÍ existen — y con una
//      lista de correos válidos, probar contraseñas vale la pena.
//
//      La versión anterior tenía un endpoint que SÍ los distinguía (404 vs
//      401). Aquí no, y es el criterio 2.
//
//   2. EL TOKEN LLEVA EL CORREO Y LOS ROLES, Y NADA MÁS. No lleva permisos
//      —se consultan al usar— ni datos personales: el contenido de un JWT se
//      lee sin ninguna clave, pegándolo en una página web. Está FIRMADO, no
//      cifrado.
// ============================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;
using Microsoft.IdentityModel.Tokens;

namespace ApiFacturas.Servicios;

public class ServicioSesion : IServicioSesion
{
    private readonly IRepositorioUsuario _usuarios;
    private readonly IRepositorioRolUsuario _rolesDeUsuario;
    private readonly IRepositorioRol _roles;
    private readonly ConfiguracionJwt _jwt;

    public ServicioSesion(
        IRepositorioUsuario usuarios,
        IRepositorioRolUsuario rolesDeUsuario,
        IRepositorioRol roles,
        ConfiguracionJwt jwt)
    {
        _usuarios = usuarios;
        _rolesDeUsuario = rolesDeUsuario;
        _roles = roles;
        _jwt = jwt;
    }

    public async Task<Sesion?> IniciarAsync(string email, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
        {
            return null;
        }

        // El repositorio compara el HASH, nunca la cadena. Devuelve:
        //   null  -> el correo no existe
        //   false -> existe y la contraseña no coincide
        //   true  -> es
        var resultado = await _usuarios.VerificarContrasenaAsync(email.Trim(), contrasena);

        // LOS DOS CASOS MALOS SE COLAPSAN EN UNO. Quien llama no puede
        // distinguirlos, y eso es lo que se quiere.
        if (resultado != true)
        {
            return null;
        }

        var roles = await NombresDeRolesAsync(email.Trim());
        var expira = DateTime.UtcNow.AddMinutes(
            _jwt.DuracionMinutos > 0 ? _jwt.DuracionMinutos : 60);

        return new Sesion
        {
            Token = ArmarToken(email.Trim(), roles, expira),
            Email = email.Trim(),
            Roles = roles,
            Expira = expira,
        };
    }

    /// <summary>Los NOMBRES de los roles, no sus ids: el menú de la interfaz le
    /// habla a una persona.</summary>
    private async Task<List<string>> NombresDeRolesAsync(string email)
    {
        var asignaciones = await _rolesDeUsuario.ObtenerPorUsuarioAsync(email);
        var todos = await _roles.ObtenerTodosAsync(1000);
        return asignaciones
            .Select(a => todos.FirstOrDefault(r => r.Id == a.Fkidrol)?.Nombre)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!)
            .ToList();
    }

    private string ArmarToken(string email, List<string> roles, DateTime expira)
    {
        // Los `claims` son el CONTENIDO del token. Se lee sin clave: aquí no va
        // nada privado.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, email),
            new(ClaimTypes.Name, email),
        };

        // Los roles van como `role` para que ASP.NET los entienda, por si
        // algún día se usa [Authorize(Roles = "...")]. El permiso FINO no sale
        // de aquí: sale de verificar_acceso_ruta, en cada operación.
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var firma = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expira,
            signingCredentials: firma);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>Los nombres de los `claims` registrados que se usan aquí. Se
/// escriben una vez para no repetir la cadena suelta por el código.</summary>
internal static class JwtRegisteredClaimNames
{
    public const string Sub = "sub";
}
