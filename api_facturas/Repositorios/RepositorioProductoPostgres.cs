// ============================================================
// RepositorioProductoPostgres — la capa de DATOS de la v1.
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
//
// Única clase del sistema que habla SQL y que conoce la conexión.
// Cumple el contrato IRepositorioProducto.
//
// Usa ADO.NET "crudo" (NpgsqlConnection + NpgsqlCommand) a propósito:
// el SQL queda VISIBLE y parametrizado — sin ORM que lo esconda.
// Reglas de la constitución que se cumplen aquí:
// - SQL SIEMPRE con parámetros @nombre (nunca concatenar valores).
// - Todo asíncrono (async/await).
// ============================================================

using ApiFacturas.Modelos;
using Npgsql;   // el proveedor oficial de PostgreSQL para .NET

namespace ApiFacturas.Repositorios;

public class RepositorioProductoPostgres : IRepositorioProducto
{
    // La cadena de conexión llega POR CONSTRUCTOR (este archivo no
    // sabe de appsettings ni de variables de entorno — eso es del
    // ensamblador). readonly = se asigna una vez.
    private readonly string _cadenaConexion;

    public RepositorioProductoPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    // ------------------------------------------------------------
    // Ayudantes privados
    // ------------------------------------------------------------

    /// <summary>Abre una conexión nueva. El "await using" del que llama
    /// garantiza que se cierre sola al terminar (aunque haya error).</summary>
    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    /// <summary>Convierte la fila actual del lector en un objeto del
    /// MODELO. Los índices (0,1,2,3) son el orden de las columnas del
    /// SELECT; GetDecimal/GetInt32 leen ya con el tipo correcto.</summary>
    private static Producto ArmarProducto(NpgsqlDataReader lector)
    {
        return new Producto
        {
            Codigo = lector.GetString(0),
            Nombre = lector.GetString(1),
            Stock = lector.GetInt32(2),
            Valorunitario = lector.GetDecimal(3),
        };
    }

    // ------------------------------------------------------------
    // Los 5 métodos del contrato
    // ------------------------------------------------------------

    public async Task<List<Producto>> ObtenerTodosAsync(int limite)
    {
        // El SQL con PARÁMETROS (@limite): el valor viaja por aparte y
        // el motor jamás lo confunde con SQL — eso evita la inyección.
        // "LIMIT @limite" es el TOP del dialecto PostgreSQL (y va al final).
        const string sql = @"SELECT codigo, nombre, stock, valorunitario
                             FROM producto ORDER BY codigo LIMIT @limite";

        // "await using" = úsalo y ciérralo solo al salir del bloque:
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        // Enlazar el valor al parámetro:
        comando.Parameters.AddWithValue("@limite", limite);

        // ExecuteReader devuelve un LECTOR que recorre las filas:
        await using var lector = await comando.ExecuteReaderAsync();
        var productos = new List<Producto>();
        // ReadAsync avanza a la siguiente fila (false cuando se acaban):
        while (await lector.ReadAsync())
        {
            productos.Add(ArmarProducto(lector));
        }
        return productos;
    }

    public async Task<Producto?> ObtenerPorCodigoAsync(string codigo)
    {
        const string sql = @"SELECT codigo, nombre, stock, valorunitario
                             FROM producto WHERE codigo = @codigo";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);

        await using var lector = await comando.ExecuteReaderAsync();
        // ¿Hay fila? → el modelo. ¿No hay? → null (el contrato):
        if (await lector.ReadAsync())
        {
            return ArmarProducto(lector);
        }
        return null;
    }

    public async Task CrearAsync(Producto producto)
    {
        const string sql = @"INSERT INTO producto (codigo, nombre, stock, valorunitario)
                             VALUES (@codigo, @nombre, @stock, @valorunitario)";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        // Los valores salen del OBJETO del modelo (ya tipados y limpios):
        comando.Parameters.AddWithValue("@codigo", producto.Codigo);
        comando.Parameters.AddWithValue("@nombre", producto.Nombre);
        comando.Parameters.AddWithValue("@stock", producto.Stock);
        comando.Parameters.AddWithValue("@valorunitario", producto.Valorunitario);

        // ExecuteNonQuery = ejecutar algo que NO devuelve filas (INSERT):
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        // SET dinámico SOLO con las columnas que llegaron (PUT manda las
        // 3, PATCH un subconjunto). Los NOMBRES de columna vienen del
        // controlador, que los sacó de las PETICIONES (lista blanca) —
        // nunca del cliente — por eso es seguro armarlos en el texto;
        // los VALORES sí van siempre como parámetros.
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys)
        {
            // Cada columna genera su pareja "columna = @columna":
            asignaciones.Add($"{columna} = @{columna}");
        }
        // string.Join une la lista con comas: "nombre = @nombre, stock = @stock".
        // El parámetro de la clave se llama distinto (@codigo_clave) para
        // no chocar con un posible campo del SET:
        var sql = $"UPDATE producto SET {string.Join(", ", asignaciones)} " +
                  "WHERE codigo = @codigo_clave";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos)
        {
            comando.Parameters.AddWithValue($"@{columna}", valor);
        }
        comando.Parameters.AddWithValue("@codigo_clave", codigo);

        // En UPDATE/DELETE, ExecuteNonQuery devuelve las FILAS AFECTADAS.
        // Nota didáctica: PostgreSQL también cuenta las filas que CUMPLIERON
        // el WHERE (aunque el valor nuevo sea igual al viejo) — por eso un
        // PATCH con el mismo valor reporta 1 fila, igual que en SQL Server.
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        const string sql = "DELETE FROM producto WHERE codigo = @codigo";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);
        return await comando.ExecuteNonQueryAsync();
    }
}
