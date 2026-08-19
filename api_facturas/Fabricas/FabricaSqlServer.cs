// ============================================================
// FabricaSqlServer — la fábrica del PRIMER motor (v4).
//
// Entrega los 11 repositorios en dialecto SQL Server, cada uno con
// la cadena de conexión que recibió al construirse. Construir un
// repositorio NO abre conexiones (eso pasa en cada método, cuando
// llega una petición) — por eso la fábrica se puede probar sin BD.
// ============================================================

using ApiFacturas.Repositorios;

namespace ApiFacturas.Fabricas;

public class FabricaSqlServer : IFabricaRepositorios
{
    private readonly string _cadenaConexion;

    public FabricaSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public IRepositorioProducto CrearRepositorioProducto() => new RepositorioProductoSqlServer(_cadenaConexion);
    public IRepositorioPersona CrearRepositorioPersona() => new RepositorioPersonaSqlServer(_cadenaConexion);
    public IRepositorioFactura CrearRepositorioFactura() => new RepositorioFacturaSqlServer(_cadenaConexion);
    public IRepositorioEmpresa CrearRepositorioEmpresa() => new RepositorioEmpresaSqlServer(_cadenaConexion);
    public IRepositorioCliente CrearRepositorioCliente() => new RepositorioClienteSqlServer(_cadenaConexion);
    public IRepositorioVendedor CrearRepositorioVendedor() => new RepositorioVendedorSqlServer(_cadenaConexion);
    public IRepositorioUsuario CrearRepositorioUsuario() => new RepositorioUsuarioSqlServer(_cadenaConexion);
    public IRepositorioRol CrearRepositorioRol() => new RepositorioRolSqlServer(_cadenaConexion);
    public IRepositorioRuta CrearRepositorioRuta() => new RepositorioRutaSqlServer(_cadenaConexion);
    public IRepositorioRolUsuario CrearRepositorioRolUsuario() => new RepositorioRolUsuarioSqlServer(_cadenaConexion);
    public IRepositorioRutaRol CrearRepositorioRutaRol() => new RepositorioRutaRolSqlServer(_cadenaConexion);
}
