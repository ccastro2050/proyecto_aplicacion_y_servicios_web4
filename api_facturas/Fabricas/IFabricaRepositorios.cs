// ============================================================
// IFabricaRepositorios — el contrato de la FÁBRICA (v5).
//
// El patrón fábrica abstracta: quien implementa esta interfaz
// decide el motor de las CATORCE rebanadas a la vez. El ensamblador
// (Program.cs) elige UNA fábrica al arrancar y le pide todo; nadie
// más en el sistema vuelve a pensar en motores.
//
// Los 14 métodos "aburridos" SON la lección: la fábrica promete la
// familia COMPLETA de repositorios, no repositorios sueltos.
//
// POR QUÉ ESO IMPORTA, y es lo que un «14 métodos repetidos» esconde:
// sin la fábrica, Program.cs tendría CATORCE decisiones de motor, y
// nada impediría que la catorceava se quedara en SQL Server mientras
// las otras trece pasaron a PostgreSQL. Un sistema mitad en un motor
// y mitad en otro — que compila y arranca. Aquí se escoge UNA VEZ.
//
// Y agregar una entidad obliga a los DOS motores a soportarla: el
// compilador no deja fábricas incompletas.
// ============================================================

using ApiFacturas.Repositorios;

namespace ApiFacturas.Fabricas;

public interface IFabricaRepositorios
{
    IRepositorioProducto CrearRepositorioProducto();
    IRepositorioPersona CrearRepositorioPersona();
    IRepositorioFactura CrearRepositorioFactura();
    IRepositorioEmpresa CrearRepositorioEmpresa();
    IRepositorioCliente CrearRepositorioCliente();
    IRepositorioVendedor CrearRepositorioVendedor();
    IRepositorioUsuario CrearRepositorioUsuario();
    IRepositorioRol CrearRepositorioRol();
    IRepositorioRuta CrearRepositorioRuta();
    IRepositorioRolUsuario CrearRepositorioRolUsuario();
    IRepositorioRutaRol CrearRepositorioRutaRol();

    /// <summary>El recurso maestro-detalle sobre la tabla puente (v2):
    /// el usuario Y sus roles en una sola operacion.</summary>
    IRepositorioUsuarioConRoles CrearRepositorioUsuarioConRoles();

    /// <summary>El control de acceso (v3). Tambien pasa por aqui, y es
    /// importante: con el motor en postgres, un repositorio de acceso que
    /// siguiera hablando con SQL Server no se notaria —los dos motores
    /// tienen los mismos datos— hasta que dejaran de tenerlos.</summary>
    IRepositorioAcceso CrearRepositorioAcceso();

    /// <summary>Las diez consultas multitabla de la v4. Tambien por la
    /// fabrica: un tablero que leyera de otro motor que el resto del sistema
    /// mostraria numeros que no corresponden a lo que el usuario acaba de
    /// hacer — y nadie lo notaria mientras los dos motores tengan los
    /// mismos datos.</summary>
    IRepositorioConsultas CrearRepositorioConsultas();
}
