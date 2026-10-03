// ============================================================
// FabricaPostgres — la fábrica del SEGUNDO motor (v4).
//
// La gemela de FabricaSqlServer, dialecto PostgreSQL. Compárelas
// lado a lado: misma forma, otra familia — eso ES el patrón.
// La v5 (MariaDB) será la tercera gemela: una clase y un case.
// ============================================================

using ApiFacturas.Repositorios;

namespace ApiFacturas.Fabricas;

public class FabricaPostgres : IFabricaRepositorios
{
    private readonly string _cadenaConexion;

    public FabricaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public IRepositorioProducto CrearRepositorioProducto() => new RepositorioProductoPostgres(_cadenaConexion);
    public IRepositorioPersona CrearRepositorioPersona() => new RepositorioPersonaPostgres(_cadenaConexion);
    public IRepositorioFactura CrearRepositorioFactura() => new RepositorioFacturaPostgres(_cadenaConexion);
    public IRepositorioEmpresa CrearRepositorioEmpresa() => new RepositorioEmpresaPostgres(_cadenaConexion);
    public IRepositorioCliente CrearRepositorioCliente() => new RepositorioClientePostgres(_cadenaConexion);
    public IRepositorioVendedor CrearRepositorioVendedor() => new RepositorioVendedorPostgres(_cadenaConexion);
    public IRepositorioUsuario CrearRepositorioUsuario() => new RepositorioUsuarioPostgres(_cadenaConexion);
    public IRepositorioRol CrearRepositorioRol() => new RepositorioRolPostgres(_cadenaConexion);
    public IRepositorioRuta CrearRepositorioRuta() => new RepositorioRutaPostgres(_cadenaConexion);
    public IRepositorioRolUsuario CrearRepositorioRolUsuario() => new RepositorioRolUsuarioPostgres(_cadenaConexion);
    public IRepositorioRutaRol CrearRepositorioRutaRol() => new RepositorioRutaRolPostgres(_cadenaConexion);

    // v2 — el recurso maestro-detalle sobre la tabla puente.
    public IRepositorioUsuarioConRoles CrearRepositorioUsuarioConRoles() => new RepositorioUsuarioConRolesPostgres(_cadenaConexion);

    // v3 — el control de acceso.
    public IRepositorioAcceso CrearRepositorioAcceso() => new RepositorioAccesoPostgres(_cadenaConexion);

    // v4 — las diez consultas del tablero.
    public IRepositorioConsultas CrearRepositorioConsultas() => new RepositorioConsultasPostgres(_cadenaConexion);
}
