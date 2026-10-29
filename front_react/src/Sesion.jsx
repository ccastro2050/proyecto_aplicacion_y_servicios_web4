import { createContext, useContext, useEffect, useState } from 'react'
import { api, leerSesion, guardarSesion, borrarSesion } from './api.js'

// ============================================================
// Sesion.jsx — quien esta adentro, y que puede abrir.
//
// El equivalente del `session` de Flask o del circuito de Blazor. La
// diferencia es donde vive: aqui, en el navegador. No hay servidor propio
// donde guardarla.
//
// Y OJO con lo que esto NO es: la lista de permisos sirve para DIBUJAR el
// menu, no para proteger nada. Quien escriba la direccion a mano recibe un
// 403 de la API, que es quien de verdad protege. Esconder un boton no es
// seguridad: es cortesia.
// ============================================================

const ContextoSesion = createContext(null)

export function ProveedorSesion({ children }) {
  const [sesion, setSesion] = useState(() => leerSesion())
  const [permisos, setPermisos] = useState([])
  const [cargando, setCargando] = useState(false)

  // Los permisos se piden EN SEGUIDA del login, con el token recien recibido.
  useEffect(() => {
    if (!sesion?.token) { setPermisos([]); return }
    let vivo = true
    api.misPermisos()
      .then((r) => { if (vivo) setPermisos(r?.datos || []) })
      .catch(() => { if (vivo) setPermisos([]) })
    return () => { vivo = false }
  }, [sesion?.token])

  async function entrar(email, contrasena) {
    setCargando(true)
    try {
      const r = await api.iniciarSesion(email, contrasena)
      const nueva = { email: r.email, token: r.token, roles: r.roles || [], expira: r.expira }
      guardarSesion(nueva)
      setSesion(nueva)
      return null                       // sin error
    } catch (error) {
      return error.message              // el mensaje que dio la API
    } finally {
      setCargando(false)
    }
  }

  function salir() {
    borrarSesion()
    setSesion(null)
    setPermisos([])
  }

  const puede = (permiso) => !permiso || permisos.includes(permiso)

  return (
    <ContextoSesion.Provider value={{ sesion, permisos, puede, entrar, salir, cargando }}>
      {children}
    </ContextoSesion.Provider>
  )
}

export function useSesion() {
  const valor = useContext(ContextoSesion)
  if (!valor) throw new Error('useSesion fuera del ProveedorSesion')
  return valor
}
