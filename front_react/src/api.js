// ============================================================
// api.js — LO UNICO que habla con la API.
//
// Es el espejo de `cliente_api.py` del front de Flask y del `ServicioApi` del
// de Blazor: toda peticion pasa por aqui. Ningun componente hace `fetch` por
// su cuenta, y eso no es orden por el orden — es lo que permite que el token,
// los errores y la direccion de la API esten escritos UNA vez.
//
// LA DIFERENCIA CON LOS OTROS DOS FRONTS, que es la razon de ser de este:
// aqui el `fetch` lo ejecuta EL NAVEGADOR de la persona, no un servidor. Por
// eso la API necesita CORS, y por eso el token viaja en el navegador.
// ============================================================

// De donde sale la direccion de la API:
//
//   import.meta.env.VITE_URL_API   la pone Vite al COMPILAR, leyendola del
//                                  entorno o del archivo .env. No se lee en
//                                  tiempo de ejecucion: queda incrustada en
//                                  el JavaScript compilado.
//
// Y de ahi una consecuencia que conviene tener clara: cambiar de API NO es
// cambiar una variable del hospedaje, como en Flask o en Blazor. Es VOLVER A
// COMPILAR. El precio de que el front sean archivos estaticos.
export const URL_API =
  import.meta.env.VITE_URL_API || 'http://localhost:8035'

// ------------------------------------------------------------
// LA SESION: donde vive el token
//
// En `sessionStorage`, no en `localStorage`: se borra al cerrar la pestana.
// Ninguno de los dos es ideal -cualquier script de la pagina puede leerlos-,
// y esa es una diferencia real con los otros dos fronts, donde el token vive
// en el servidor o en una cookie firmada.
//
// Se dice aqui porque es el tipo de cosa que un front en el navegador NO
// puede esconder, y vale entenderlo antes que descubrirlo.
// ------------------------------------------------------------
const CLAVE_SESION = 'facturas.sesion'

export function leerSesion() {
  try {
    const crudo = sessionStorage.getItem(CLAVE_SESION)
    return crudo ? JSON.parse(crudo) : null
  } catch {
    return null
  }
}

export function guardarSesion(sesion) {
  sessionStorage.setItem(CLAVE_SESION, JSON.stringify(sesion))
}

export function borrarSesion() {
  sessionStorage.removeItem(CLAVE_SESION)
}

// ------------------------------------------------------------
// La peticion, con el token puesto y los errores traducidos
// ------------------------------------------------------------
async function pedir(ruta, opciones = {}) {
  const sesion = leerSesion()
  const cabeceras = { ...(opciones.headers || {}) }

  if (opciones.body) cabeceras['Content-Type'] = 'application/json'
  // La palabra «Bearer» la pone el cliente, no Swagger: aqui no hay Swagger.
  if (sesion?.token) cabeceras['Authorization'] = `Bearer ${sesion.token}`

  let respuesta
  try {
    respuesta = await fetch(`${URL_API}${ruta}`, { ...opciones, headers: cabeceras })
  } catch (error) {
    // Un `fetch` que falla asi -sin respuesta- casi nunca es «la API esta
    // caida»: es el navegador bloqueando por CORS, o la API dormida. El
    // mensaje del navegador no lo dice, asi que lo decimos nosotros.
    throw new Error(
      'No se pudo hablar con la API. Puede estar dormida -espere unos ' +
      'segundos y reintente- o estar rechazando este origen por CORS. ' +
      `Direccion: ${URL_API}`
    )
  }

  if (respuesta.status === 204) return null

  let cuerpo = null
  try { cuerpo = await respuesta.json() } catch { /* sin cuerpo */ }

  if (!respuesta.ok) {
    const error = new Error(cuerpo?.mensaje || `La API respondio ${respuesta.status}.`)
    error.estado = respuesta.status
    error.errores = cuerpo?.errores
    throw error
  }
  return cuerpo
}

// ------------------------------------------------------------
// Las operaciones, con el nombre del contrato
// ------------------------------------------------------------
export const api = {
  diagnostico: () => pedir('/'),

  iniciarSesion: (email, contrasena) =>
    pedir('/api/sesion', {
      method: 'POST',
      body: JSON.stringify({ email, contrasena }),
    }),

  // El sobre del contrato: { tabla, limite, total, datos }
  listar: (endpoint, limite = 1000) => pedir(`${endpoint}?limite=${limite}`),
  consultar: (endpoint, id) => pedir(`${endpoint}/${encodeURIComponent(id)}`),
  crear: (endpoint, cuerpo) =>
    pedir(endpoint, { method: 'POST', body: JSON.stringify(cuerpo) }),
  reemplazar: (endpoint, id, cuerpo) =>
    pedir(`${endpoint}/${encodeURIComponent(id)}`, { method: 'PUT', body: JSON.stringify(cuerpo) }),
  modificar: (endpoint, id, cuerpo) =>
    pedir(`${endpoint}/${encodeURIComponent(id)}`, { method: 'PATCH', body: JSON.stringify(cuerpo) }),
  borrar: (endpoint, id) =>
    pedir(`${endpoint}/${encodeURIComponent(id)}`, { method: 'DELETE' }),

  misPermisos: () => pedir('/api/permisos/mios'),
}
