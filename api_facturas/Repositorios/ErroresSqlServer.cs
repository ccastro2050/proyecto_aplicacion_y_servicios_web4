// ============================================================
// ErroresSqlServer — el unico sitio que traduce los numeros del motor.
//
// SQL Server no lanza «ya existe» ni «no existe»: lanza un NUMERO de error.
// Los tres que importan aqui:
//
//   547    violacion de restriccion — FOREIGN KEY o CHECK. La fila a la que
//          apunta la clave foranea no existe, o alguien quiere borrar una fila
//          de la que otra depende
//   2627   violacion de PRIMARY KEY o UNIQUE — una clave repetida
//   2601   indice unico duplicado — lo mismo, por otra via
//
// LOS TRES SON 409, Y NO 422. Y conviene decir por que, porque la tentacion es
// el 422: el dato que llego tiene la FORMA correcta —«P099» es un texto de la
// longitud permitida—. Lo que se rompe es el ESTADO de la base: esa fila no
// esta, o ya esta. El 422 se reserva para lo que la peticion puede rechazar
// sin consultar nada.
//
// Y FIJESE EN QUE ESTOS NUMEROS NO SE PARECEN A LOS DE PostgreSQL, que usa
// SQLSTATE -23503 y 23505-. Dicen lo mismo y no se parecen en nada. Por eso la
// traduccion vive AQUI, en la capa que sabe de motor: arriba de aqui nadie
// conoce SqlException, y eso es lo que permite que manana el motor sea otro sin
// tocar el servicio.
// ============================================================

using ApiFacturas.Excepciones;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public static class ErroresSqlServer
{
    /// <summary>Ejecuta la operacion y traduce los errores de integridad del
    /// motor a excepciones del dominio. Lo que no reconoce, lo deja subir tal
    /// cual: un 500 con el mensaje del motor es mejor que un 409 inventado.</summary>
    public static async Task<T> TraducirAsync<T>(Func<Task<T>> operacion)
    {
        try
        {
            return await operacion();
        }
        catch (SqlException e) when (e.Number == 547)
        {
            // El mensaje del motor viaja completo: trae el NOMBRE de la
            // restriccion -«FK_cliente_persona»-, que es la pista util.
            throw new ConflictoExcepcion(
                "La operacion rompe una relacion de la base de datos: " + e.Message);
        }
        catch (SqlException e) when (e.Number == 2627 || e.Number == 2601)
        {
            throw new ConflictoExcepcion("Ese registro ya existe: " + e.Message);
        }
    }
}
